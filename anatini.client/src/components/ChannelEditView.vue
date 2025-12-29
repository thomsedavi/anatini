<script setup lang="ts">
  import { ref, watch, useTemplateRef } from 'vue';
  import { useRoute } from 'vue-router';
  import { reportValidity, validateInputs } from './common/validity';
  import type { ChannelEdit } from '@/types';
  import { apiFetch } from './common/apiFetch';

  const route = useRoute();

  const loading = ref<boolean>(false);
  const error = ref<string | null>(null);
  const channel = ref<ChannelEdit | null>(null);
  const isCreatingContent = ref<boolean>(false);
  const contentSlugInput = useTemplateRef<HTMLInputElement>('content-slug');
  const contentNameInput = useTemplateRef<HTMLInputElement>('content-name');

  watch([() => route.params.channelId], fetchChannel, { immediate: true });

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

  async function createContent(event: Event) {
    event.preventDefault();

    if (channel.value === null)
      return;

    if (!validateInputs([
      { element: contentNameInput.value, error: 'Please enter content name.' },
      { element: contentSlugInput.value, error: 'Please enter content slug.' },
    ]))
    {
      return;
    }

    isCreatingContent.value = true;

    const body = new FormData();

    body.append('name', contentNameInput.value!.value.trim());
    body.append('slug', contentSlugInput.value!.value.trim());

    const onfulfilled = (value: ChannelEdit) => {
      channel.value = value;
    };

    const onfinally = () => {
      isCreatingContent.value = false;
    };

    const init = { method: "POST", body: body };

    const statusActions = {
      409: () => {
        contentSlugInput.value!.setCustomValidity("Slug already in use!");
        reportValidity([contentSlugInput.value]);
      }
    };

    apiFetch(`channels/${route.params.channelId}/contents`, onfulfilled, onfinally, init, statusActions);
  }
</script>

<template>
  <main>
    <h2>ChannelEditView</h2>
    <template v-if="channel">
      <h3>{{ channel.name }}</h3>
      <form id="createContent" @submit="createContent" action="???" method="content">
        <fieldset>
          <legend>Create Content</legend>

          <label for="contentName">Content Name</label>
          <input id="contentName" type="text" name="contentName" maxlength="64" ref="content-name" @input="event => contentNameInput?.setCustomValidity('')">

          <hr>

          <label for="contentSlug">Content Slug</label>
          <input id="contentSlug" type="text" name="contentSlug" maxlength="64" ref="content-slug" @input="event => contentSlugInput?.setCustomValidity('')">

          <hr>

          <button type="submit" :disabled="isCreatingContent">Submit</button>
        </fieldset>
      </form>
      <template v-if="channel.topDraftContents?.length">
        <div v-for="(content, index) in channel.topDraftContents" :key="'topDraftContent' + index">
          <RouterLink :to="{ name: 'ContentEdit', params: { channelId: route.params.channelId, contentId: content.defaultSlug }}">{{ content.name }}: Edit</RouterLink>
          <RouterLink :to="{ name: 'ContentPreview', params: { channelId: route.params.channelId, contentId: content.defaultSlug }}">{{ content.name }}: Preview</RouterLink>
        </div>
      </template>
    </template>
  </main>
</template>
