<script setup lang="ts">
  import { ref, watch } from 'vue';
  import { useRoute } from 'vue-router';
  import { apiFetchAuthenticated } from './common/apiFetch';
  import type { Post, PostElement, ErrorMessage, StatusActions } from '@/types';
  import { formatParagraph } from './common/utils';

  const route = useRoute();

  const post = ref<Post | ErrorMessage | null>(null);

  watch([() => route.params.channelId, () => route.params.postId], fetchPost, { immediate: true });

  async function fetchPost(array: (() => string | string[])[]) {
    post.value = null;

    const statusActions: StatusActions = {
      200: (response?: Response) => {
        response?.json()
          .then((value: Post) => {
            post.value = value;
          })
          .catch(() => { post.value = { error: true, heading: 'Unknown Error', body: 'There was a problem fetching your post, please reload the page' }});
      },
      404: () => {
        post.value = { error: true, heading: '404 Not Found', body: 'Post not found' };
      },
      500: () => {
        post.value = { error: true, heading: 'Unknown Error', body: 'There was a problem fetching this post, please reload the page' };
      }
    };

    await apiFetchAuthenticated(`channels/${array[0]}/posts/${array[1]}/preview`, statusActions);
  }

  function sanitizeElementContent(elementContent: string): string {
    return elementContent
      .replace(/</g, '&lt;')
      .replace(/>/g, '&gt;');
  }

  function getPosts(elements: PostElement[]): string {
    let result = '';

    elements.sort((a, b) => a.index - b.index).forEach(element => {
      if (element.post !== null)
      {
        const elementContent = sanitizeElementContent(element.post);

        if (element.tag === 'p') {
          result += formatParagraph(elementContent);
        }
        else {
          result += `<${element.tag}>${elementContent}</${element.tag}>`;
        }
      }
    });

    return result;
  }

  function getMainHtml(): string {
    if (post.value === null) {
      return '<p>Loading...</p>';
    } else if ('heading' in post.value) {
      return `<h1>${ post.value.heading }</h1>`;
    } else if (post.value.version.elements) {
      return getPosts(post.value.version.elements);
    } else {
      return '<h1>Unknown Error</h1>';
    }
  }
</script>

<template>
  <main id="main" tabindex="-1" v-html="getMainHtml()"></main>
</template>
