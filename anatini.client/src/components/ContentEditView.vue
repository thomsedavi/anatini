<script setup lang="ts">
  import type { ContentEdit, ErrorMessage, StatusActions } from '@/types';
  import { nextTick, ref, watch } from 'vue';
  import { useRoute } from 'vue-router';
  import { markdownToHtml, paragraphToHTML, paragraphToMarkdown, parseFromString, parseSource, serializeToString, type Source } from './common/utils';
  import { apiFetchAuthenticated } from './common/apiFetch';
  import InputText from './common/InputText.vue';

  const headingMainRef = ref<HTMLElement | null>(null);

  const route = useRoute();

  const content = ref<ContentEdit | ErrorMessage | null>(null);
  const article = ref<Node | null>(null);
  const editText = ref<string | null>(null);
  const editIndex = ref<number | null>(null);
  const eTag = ref<string | null>(null);

  watch([() => route.params.channelId, () => route.params.contentId], (source: Source) => fetchContent(parseSource(source)), { immediate: true });

  async function fetchContent(params: string[]) {
    const statusActions: StatusActions = {
      200: (response?: Response) => {
        response?.json()
          .then((value: ContentEdit) => {
            content.value = value;
            eTag.value = response.headers.get("ETag");

            if (content.value.version.article !== null) {
              const node = parseFromString(content.value.version.article);

              if (node?.nodeName === 'ARTICLE') {
                article.value = node;
              }
            }

            nextTick(() => {
              headingMainRef.value?.focus();
            });
          })
          .catch(() => { content.value = { error: true, heading: 'Unknown Error', body: 'There was a problem fetching your content, please reload the page' }});
      }
    }

    apiFetchAuthenticated(`channels/${params[0]}/contents/${params[1]}/edit`, statusActions);
  }

  function getHeading(): string {
    if (content.value === null) {
      return 'Fetching...';
    } if ('error' in content.value) {
      return content.value.heading;
    } else {
      return 'Content Edit';
    }
  }

  function setEdit(text: string, index: number): void {
    editText.value = text;
    editIndex.value = index;
  }

  function saveParagraph(): void {
    if (eTag.value === null || content.value === null || 'error' in content.value || editText.value === null || article.value == null) {
      return;
    }

    const node = parseFromString(markdownToHtml(`<p>${editText.value}</p>`));

    if (node !== null) {
      article.value.childNodes.forEach((value, index) => {
        if (index === editIndex.value) {
          value.replaceWith(node);
        }
      });

      const body = new FormData();
      
      body.append('article', serializeToString(article.value));

      const init = { method: "PATCH", headers: { "If-Match": eTag.value }, body: body };

      const statusActions: StatusActions = {
        204: (response?: Response) => {
          eTag.value = response?.headers.get("ETag") ?? null;

          editText.value = null;
          editIndex.value = null;
        }
      }

      apiFetchAuthenticated(`channels/${content.value.channelId}/contents/${content.value.id}`, statusActions, init);
    }
  }
</script>

<template>
  <main id="main" tabindex="-1" :aria-busy="content === null">
    <header>
      <h1 ref="headingMainRef" tabindex="-1">{{ getHeading() }}</h1>
    </header>

    <section v-if="content === null">                
      <progress max="100">Fetching content...</progress>
    </section>

    <section v-else-if="'error' in content">
      <p>
        {{ content.body }}
      </p>
    </section>

    <template v-else>
      <template v-if="article !== null">
        <section v-for="(child, index) in article.childNodes" :key="'child' + index">
          <template v-if="child.nodeName === 'P'">
            <template v-if="editText === null || editIndex !== index">
              <p v-html="paragraphToHTML(child as Element)"></p>
              <button @click="setEdit(paragraphToMarkdown(child as Element), index)" :aria-disabled="editText !== null ? true : undefined">Edit</button>
            </template>
            <template v-else>
              <InputText
                v-model="editText"
                name="p"
                id="p" />
              <button @click="saveParagraph">Save</button>
            </template>
          </template>
          <template v-else>
            <p>Unknown Element</p>
          </template>
        </section>
      </template>
    </template>
  </main>
</template>
