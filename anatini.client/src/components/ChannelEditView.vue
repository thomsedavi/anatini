<script setup lang="ts">
  import { ref, watch, useTemplateRef } from 'vue';
  import { useRoute } from 'vue-router';
  import { reportValidity, validateInputs } from './common/validity';
  import type { ChannelEdit } from '@/types';

  const route = useRoute();

  const loading = ref<boolean>(false);
  const error = ref<string | null>(null);
  const channel = ref<ChannelEdit | null>(null);
  const isCreatingPost = ref<boolean>(false);
  const postSlugInput = useTemplateRef<HTMLInputElement>('post-slug');
  const postNameInput = useTemplateRef<HTMLInputElement>('post-name');

  watch([() => route.params.channelSlug], fetchChannel, { immediate: true });

  async function fetchChannel(array: (() => string | string[])[]) {
    error.value = channel.value = null
    loading.value = true

    fetch(`/api/channels/${array[0]}/edit`, { method: "GET" })
      .then((value: Response) => {
        if (value.ok) {
          value.json()
            .then((value: ChannelEdit) => {
              channel.value = value;
            })
            .catch(() => {
              error.value = 'Unknown Error';
            });
        } else if (value.status === 401) {
          error.value = 'Unauthorised';
        } else {
          error.value = 'Unkown Error';
        }
      })
      .catch(() => {
        error.value = 'Unknown Error';
      }).
      finally(() => {
        loading.value = false
      });
  }

  async function createPost(event: Event) {
    event.preventDefault();

    if (channel.value === null)
      return;

    if (!validateInputs([
      { element: postNameInput.value, error: 'Please enter a post name.' },
      { element: postSlugInput.value, error: 'Please enter a post slug.' },
    ]))
      return;

    isCreatingPost.value = true;

    const body: Record<string, string> = {
      name: postNameInput.value!.value.trim(),
      slug: postSlugInput.value!.value.trim(),
    };

    fetch(`/api/channels/${route.params.channelSlug}/posts`, {
      method: "POST",
      headers: {
        "Content-Type": "application/x-www-form-urlencoded",
      },
      body: new URLSearchParams(body),
    }).then((value: Response) => {
        if (value.ok) {
          value.json()
            .then((value: ChannelEdit) => {
              channel.value = value;
            })
            .catch(() => {
              error.value = 'Unknown Error';
            });
        } else if (value.status === 409) {
          postSlugInput.value!.setCustomValidity("Slug already in use!");
          reportValidity([postSlugInput.value]);
        } else {
          console.log("Unknown Error");
        }
      }).finally(() => {
        isCreatingPost.value = false;
      });
  }
</script>

<template>
  <h2>ChannelView</h2>
  <template v-if="channel">
    <h3>{{ channel.name }}</h3>
    <form id="createPost" @submit="createPost" action="???" method="post">
      <p>
        <label for="postName">Post Name</label>
        <input id="postName" type="text" name="postName" maxlength="64" ref="post-name" @input="event => postNameInput?.setCustomValidity('')">
      </p>

      <p>
        <label for="postSlug">Post Slug</label>
        <input id="postSlug" type="text" name="postSlug" maxlength="64" ref="post-slug" @input="event => postSlugInput?.setCustomValidity('')">
      </p>

      <p>
        <input type="submit" value="Submit" :disabled="isCreatingPost">
      </p>
    </form>
    <template v-if="channel.topDraftPosts?.length">
      <div v-for="(post, index) in channel.topDraftPosts" :key="'topDraftPost' + index">
        <RouterLink :to="{ name: 'PostEdit', params: { channelSlug: route.params.channelSlug, postSlug: post.defaultSlug }}">{{ post.name }}</RouterLink>
      </div>
    </template>
  </template>
</template>
