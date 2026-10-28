<script setup lang="ts">
  import type { APIResponse, Post, StatusActions } from '@/common/types';
  import { nextTick, ref, watch } from 'vue';
  import { parseParams, type Params } from '@/common/utils';
  import { useRoute } from 'vue-router';
  import { apiFetch } from '@/common/apiFetch';

  const route = useRoute();

  const post = ref<APIResponse<Post>>({ fetching: true });

  watch([() => route.params.spaceId, () => route.params.postId], (params: Params) => fetchPost(parseParams(params)), { immediate: true });

  async function fetchPost(params: string[]) {
    const input = `spaces/${params[0]}/posts/${params[1]}`;

    const statusActions: StatusActions = {
      200: (response?: Response) => {
        response?.json()
          .then((value: Post) => {
            post.value = { data: value };

            nextTick(() => {
              document.querySelector('h1')?.focus();
            });
          })
          .catch(() => {
            post.value = { error: { heading: 'Unknown Error', body: 'There was a problem fetching your post, please reload the page' }};
          });
      },
      404: () => {
        post.value = { error: { heading: '404 Not Found', body: 'Post not found' }};
      }
    }

    apiFetch({ input, statusActions });
  }

  function getMainHtml(): string {
    if (post.value.fetching === true) {
      return '<p>Loading...</p>';
    } else if (post.value.error !== undefined) {
      return `<h1>${ post.value.error.body }</h1>`;
    } else if (post.value.data !== undefined) {
      return `<article>${post.value.data.body}</article>`;
    } else {
      return '<h1>Unknown Error</h1>';
    }
  }
</script>

<template>
  <main role="main" id="main" tabindex="-1" :aria-busy="post.fetching === true" v-html="getMainHtml()"></main>
</template>
