<script setup lang="ts">
  import type { Post, ErrorMessage, StatusActions } from '@/types';
  import { nextTick, ref, watch } from 'vue';
  import { parseSource, type Source } from './common/utils';
  import { useRoute } from 'vue-router';
  import { apiFetch } from './common/apiFetch';

  const route = useRoute();

  const post = ref<Post | ErrorMessage | null>(null);

  watch([() => route.params.channelId, () => route.params.postId], (source: Source) => fetchPost(parseSource(source)), { immediate: true });

  async function fetchPost(params: string[]) {
    const statusActions: StatusActions = {
      200: (response?: Response) => {
        response?.json()
          .then((value: Post) => {
            post.value = value;

            nextTick(() => {
              document.querySelector('h1')?.focus();
            });
          })
          .catch(() => { post.value = { error: true, heading: 'Unknown Error', body: 'There was a problem fetching your post, please reload the page' }});
      },
      404: () => {
        post.value = { error: true, heading: '404 Not Found', body: 'Post not found' };
      }
    }

    apiFetch(`channels/${params[0]}/posts/${params[1]}`, statusActions);
  }

  function getMainHtml(): string {
    if (post.value === null) {
      return '<p>Loading...</p>';
    } else if ('heading' in post.value) {
      return `<h1>${ post.value.heading }</h1>`;
    } else if ('version' in post.value) {
      return post.value.version.article;
    } else {
      return '<h1>Unknown Error</h1>';
    }
  }
</script>

<template>
  <main id="main" tabindex="-1" :aria-busy="post === null" v-html="getMainHtml()"></main>
</template>
