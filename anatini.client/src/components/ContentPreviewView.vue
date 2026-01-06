<script setup lang="ts">
  import { ref, watch } from 'vue';
  import { useRoute } from 'vue-router';
  import { apiFetchAuthenticated } from './common/apiFetch';
  import type { Content, ContentElement, ErrorMessage, StatusActions } from '@/types';
  import { formatParagraph } from './common/utils';

  const route = useRoute();

  const content = ref<Content | ErrorMessage | null>(null);

  watch([() => route.params.channelId, () => route.params.contentId], fetchContent, { immediate: true });

  async function fetchContent(array: (() => string | string[])[]) {
    content.value = null;

    const statusActions: StatusActions = {
      200: (response?: Response) => {
        response?.json()
          .then((value: Content) => {
            content.value = value;
          })
          .catch(() => { content.value = { error: true, heading: 'Unknown Error', body: 'There was a problem fetching your content, please reload the page' }});
      },
      404: () => {
        content.value = { error: true, heading: '404 Not Found', body: 'This content cannot be found' };
      },
      500: () => {
        content.value = { error: true, heading: 'Unknown Error', body: 'There was a problem fetching this content, please reload the page' };
      }
    };

    await apiFetchAuthenticated(`channels/${array[0]}/contents/${array[1]}/preview`, statusActions);
  }

  function sanitizeElementContent(elementContent: string): string {
    return elementContent
      .replace(/</g, '&lt;')
      .replace(/>/g, '&gt;');
  }

  function getContents(elements: ContentElement[]): string {
    let result = '';

    elements.sort((a, b) => a.index - b.index).forEach(element => {
      if (element.content !== null)
      {
        const elementContent = sanitizeElementContent(element.content);

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
    if (content.value === null) {
      return '<p>Loading...</p>';
    } else if ('heading' in content.value) {
      return `<h1>${ content.value.heading }</h1>`;
    } else if (content.value.version.elements) {
      return getContents(content.value.version.elements);
    } else {
      return '<h1>Unknown Error</h1>';
    }
  }
</script>

<template>
  <main v-html="getMainHtml()"></main>
</template>
