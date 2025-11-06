<script setup lang="ts">
  import { ref, watch } from 'vue';
  import { useRoute } from 'vue-router';
  import { apiFetch } from './common/apiFetch';
  import type { Content, ContentElement } from '@/types';

  const route = useRoute();

  const loading = ref<boolean>(false);
  const content = ref<Content | null>(null);

  watch([() => route.params.channelId, () => route.params.contentId], fetchContent, { immediate: true });

  async function fetchContent(array: (() => string | string[])[]) {
    loading.value = true;

    const onfulfilled = (value: Content) => {
      content.value = value;
    };

    const onfinally = () => {
      loading.value = false;
    };

    await apiFetch(`channels/${array[0]}/contents/${array[1]}/preview`, onfulfilled, onfinally);
  }

  function sanitizeElementContent(elementContent: string): string {
    return elementContent
      .replace(/</g, '&lt;')
      .replace(/>/g, '&gt;');
  }

  function formatParagraph(elementContent: string): string {
    let result = '';

    const lines = elementContent.split('\n');

    const paragraphs: string[][] = [];

    let paragraph: string[] = [];

    lines.forEach(line => {
      const cleanedLine = line.replace(/\s+/g, ' ').trim();

      if (cleanedLine.length > 0) {
        paragraph.push(cleanedLine);
      } else {
        if (paragraph.length > 0) {
          paragraphs.push(paragraph);
        }

        paragraph = [];
      }
    });

    paragraphs.push(paragraph);

    paragraphs.forEach(paragraphLines => {
      result += '<p>';

      paragraphLines.forEach((paragraphLine, index) => {
        result += paragraphLine;

        if (index !== paragraphLines.length - 1) {
          result += '<br>';
        }
      })

      result += '</p>';
    });

    return result;
  }

  function getContents(elements: ContentElement[]): string {
    let result = '';

    elements.sort((a, b) => a.index - b.index).forEach(element => {
      if (element.content !== null)
      {
        const elementContent = sanitizeElementContent(element.content);

        if (element.tag == 'p') {
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
    if (loading.value) {
      return '<p>Loading...</p>';
    } else if (content.value?.version.elements !== undefined) {
      return getContents(content.value.version.elements);
    } else {
      return '<b>Error</b>';
    }
  }
</script>

<template>
  <main v-html="getMainHtml()"></main>
</template>