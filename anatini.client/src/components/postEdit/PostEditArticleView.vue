<script setup lang="ts">
  import { ref } from 'vue';
  import InputText from '../common/InputText.vue';
  import { markdownToHtml, paragraphToHTML, paragraphToMarkdown, parseFromString, serializeToString } from '../common/utils';
  import { apiFetchAuthenticated } from '../common/apiFetch';
  import type { StatusActions } from '@/types';

  const props = defineProps<{
    article: Node | null,
    channelId: string,
    postId: string,
    eTag: string | null,
  }>();

  const emit = defineEmits<{
    'update-etag': [eTag: string | null],
    'update-article': [article: Node | null],
  }>();

  const editText = ref<string | null>(null);
  const editIndex = ref<number | null>(null);
  const addIndex = ref<number | null>(null);
  const addText = ref<string | null>(null);

  function saveParagraph(): void {
    if (props.eTag === null || editText.value === null || props.article === null) {
      return;
    }

    const article = props.article;
    const node = parseFromString(markdownToHtml(`<p>${editText.value}</p>`));

    if (node !== null) {
      article.childNodes.forEach((value, index) => {
        if (index === editIndex.value) {
          value.replaceWith(node);
        }
      });

      const body = new FormData();
      
      body.append('article', serializeToString(article));

      const init = { method: "PATCH", headers: { "If-Match": props.eTag }, body: body };

      const statusActions: StatusActions = {
        204: (response?: Response) => {
          emit('update-article', article);
          emit('update-etag', response?.headers.get("ETag") ?? null);

          editText.value = null;
          editIndex.value = null;
        }
      }

      apiFetchAuthenticated(`channels/${props.channelId}/posts/${props.postId}`, statusActions, init);
    }
  }

  function addParagraph(): void {
    if (props.eTag === null || addText.value === null || props.article == null) {
      return;
    }

    const article = props.article;
    const node = parseFromString(markdownToHtml(`<p>${addText.value}</p>`));

    if (node !== null) {
      article.childNodes.forEach((value, index) => {
        if (index === addIndex.value) {
          value.after(node);
        }
      });

      const body = new FormData();
      
      body.append('article', serializeToString(article));

      const init = { method: "PATCH", headers: { "If-Match": props.eTag }, body: body };

      const statusActions: StatusActions = {
        204: (response?: Response) => {
          emit('update-article', article);
          emit('update-etag', response?.headers.get("ETag") ?? null);

          addText.value = null;
          addIndex.value = null;
        }
      }

      apiFetchAuthenticated(`channels/${props.channelId}/posts/${props.postId}`, statusActions, init);
    }
  }
  
  function setEdit(text: string, index: number): void {
    editText.value = text;
    editIndex.value = index;
  }

  function cancelSave(): void {
    editText.value = null;
    editIndex.value = null;
  }
</script>

<template>
  <section id="panel-article" role="tabpanel" aria-labelledby="tab-article">
    <header>
      <h2>Article</h2>
    </header>

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
            <button @click="cancelSave">Cancel</button>
          </template>
        </template>
        <template v-else>
          <p>Unknown Element</p>
        </template>
        <button @click="addIndex = index; addText = ''" v-if="addIndex === null">Add Paragraph</button>
        <template v-if="index === addIndex && addText !== null">
          <InputText
            v-model="addText"
            name = "p"
            id="p" />
          <button @click="addParagraph">Save</button>
          <button @click="addIndex = null">Cancel</button>
        </template>
      </section>
    </template>
  </section>
</template>
