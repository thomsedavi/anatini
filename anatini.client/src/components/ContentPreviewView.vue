<script setup lang="ts">
  import { ref, watch } from 'vue';
  import { useRoute } from 'vue-router';
  import { apiFetchAuthenticated } from './common/apiFetch';
  import type { Content, ContentElement, ErrorMessage, StatusActions } from '@/types';
import { tidy } from './common/utils';

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

  function replaceAsterisks(text: string, replacementTags: {asteriskCount: number, openingTags: string, closingTags: string}[]): string {
    if (replacementTags.length === 0) {
      return text;
    }

    const replacementTag = replacementTags[0];

    const regExp = new RegExp(`${'\\\*'.repeat(replacementTag.asteriskCount)}(?!\\\*)`, 'g');

    let execArrays = [...text.matchAll(regExp)];

    if (execArrays.length % 2 === 1) {
      execArrays = execArrays.slice(0, execArrays.length - 1);
    }

    const cleanedLineSegments: string[] = [text.substring(0, execArrays[0]?.index ?? text.length)];

    execArrays.forEach((execArray, index) => {
      cleanedLineSegments.push(text.substring(execArray.index + replacementTag.asteriskCount, execArrays[index + 1]?.index ?? text.length));
    });

    let result: string = '';

    cleanedLineSegments.forEach((lineSegment, index) => {
      if (index % 2 === 0) {
        result += lineSegment;
      } else {
        result += `${replacementTag.openingTags}${lineSegment.trim()}${replacementTag.closingTags}`;
      }
    });

    return replaceAsterisks(result, replacementTags.slice(1));
  }

  function formatParagraph(elementContent: string): string {
    let result = '';

    const lines = elementContent.split('\n');

    const paragraphs: string[][] = [];

    let paragraph: string[] = [];

    lines.forEach(line => {
      const cleanedLine = tidy(line);

      if (cleanedLine.length > 0) {
        const replacementTags = [
          { asteriskCount: 3, openingTags: '<em><strong>', closingTags: '</strong></em>' },
          { asteriskCount: 2, openingTags: '<strong>', closingTags: '</strong>' },
          { asteriskCount: 1, openingTags: '<em>', closingTags: '</em>' }
        ];

        paragraph.push(replaceAsterisks(cleanedLine, replacementTags));
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
