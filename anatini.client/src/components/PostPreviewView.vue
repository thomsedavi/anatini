<script setup lang="ts">
  import { ref, watch } from 'vue';
  import { useRoute } from 'vue-router';
  import { apiFetchAuthenticated } from './common/apiFetch';
  import type { Post, StatusActions, APIResponse } from '@/types';

  const route = useRoute();

  const post = ref<APIResponse<Post>>({ fetching: true });

  watch([() => route.params.spaceId, () => route.params.postId], fetchPost, { immediate: true });

  async function fetchPost(array: (() => string | string[])[]) {
    post.value = { fetching: true };

    const input = `spaces/${array[0]}/posts/${array[1]}/preview`;

    const statusActions: StatusActions = {
      200: (response?: Response) => {
        response?.json()
          .then((value: Post) => {
            post.value = { data: value};
          })
          .catch(() => { post.value = { error: { heading: 'Unknown Error', body: 'There was a problem fetching your post, please reload the page' }}});
      },
      404: () => {
        post.value = { error: { heading: '404 Not Found', body: 'Post not found' }};
      },
      500: () => {
        post.value = { error: { heading: 'Unknown Error', body: 'There was a problem fetching this post, please reload the page' }};
      }
    };

    await apiFetchAuthenticated({ input, statusActions });
  }

  function getMainHtml(): string {
    if (post.value.fetching === true) {
      return '<p>Loading...</p>';
    } else if (post.value.error !== undefined) {
      return `<h1>${ post.value.error.heading }</h1>`;
    //} else if (post.value.data !== undefined) {
    //  return getPosts(post.value.data.version.article);
    } else {
      return '<h1>Unknown Error</h1>';
    }
  }
</script>

<template>
  <main id="main" tabindex="-1" v-html="getMainHtml()"></main>
</template>
