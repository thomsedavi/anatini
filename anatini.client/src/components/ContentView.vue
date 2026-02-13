<script setup lang="ts">
  import type { Content, ErrorMessage, StatusActions } from '@/types';
  import { nextTick, ref, watch } from 'vue';
  import { parseSource, type Source } from './common/utils';
  import { useRoute } from 'vue-router';
  import { apiFetch } from './common/apiFetch';

  const route = useRoute();

  const content = ref<Content | ErrorMessage | null>(null);

  watch([() => route.params.channelId, () => route.params.contentId], (source: Source) => fetchContent(parseSource(source)), { immediate: true });

  async function fetchContent(params: string[]) {
    const statusActions: StatusActions = {
      200: (response?: Response) => {
        response?.json()
          .then((value: Content) => {
            content.value = value;

            nextTick(() => {
              document.querySelector('h1')?.focus();
            });
          })
          .catch(() => { content.value = { error: true, heading: 'Unknown Error', body: 'There was a problem fetching your content, please reload the page' }});
      },
      404: () => {
        content.value = { error: true, heading: '404 Not Found', body: 'Content not found' };
      }
    }

    apiFetch(`channels/${params[0]}/contents/${params[1]}`, statusActions);
  }

  function getMainHtml(): string {
    if (content.value === null) {
      return '<p>Loading...</p>';
    } else if ('heading' in content.value) {
      return `<h1>${ content.value.heading }</h1>`;
    } else if ('version' in content.value && content.value.version.article !== null) {
      return content.value.version.article;
    } else {
      return '<h1>Unknown Error</h1>';
    }
  }
</script>

<template>
  <main id="main" tabindex="-1" :aria-busy="content === null" v-html="getMainHtml()"></main>
</template>
