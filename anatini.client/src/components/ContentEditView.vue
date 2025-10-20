<script setup lang="ts">
  import { ref, watch } from 'vue';
  import { useRoute } from 'vue-router';
  import { apiFetch } from './common/apiFetch';
  import type { ContentEdit, ContentElement } from '@/types';

  const route = useRoute();

  const loading = ref<boolean>(false);
  const content = ref<ContentEdit | null>(null);
  const error = ref<string | null>(null);
  const eTag = ref<string | null>(null);

  watch([() => route.params.channelId, () => route.params.contentId], fetchContent, { immediate: true });

  async function fetchContent(array: (() => string | string[])[]) {
    error.value = content.value = null;
    loading.value = true;

    const onfulfilled = (value: ContentEdit) => {
      content.value = value;
    };

    const onfinally = () => {
      loading.value = false;
    };

    const handleResponse = (response: Response) => {
      eTag.value = response.headers.get("ETag");
    }

    const statusActions = {
      401: () => {
        error.value = 'Unauthorised';
      }
    };

    await apiFetch(`channels/${array[0]}/contents/${array[1]}/edit`, onfulfilled, onfinally, undefined, statusActions, handleResponse);
  }

  async function contentElement(tag: string, insertAfter: number) {
    if (eTag.value == null || content.value == null) {
      return;
    }

    const onfulfilled = (value: ContentElement) => {
      const elements = content.value!.draftVersion.elements ?? [];

      elements.push(value);

      content.value!.draftVersion.elements = elements;
    };

    const onfinally = () => {
      loading.value = false;
    };

    const handleResponse = (response: Response) => {
      eTag.value = response.headers.get("ETag");
    }

    const body: Record<string, string> = {
      insertAfter: insertAfter.toString(),
      tag: tag,
      content: tag
    };

    const init = { method: "POST", headers: { "Content-Type": "application/x-www-form-urlencoded", "If-Match": eTag.value }, body: new URLSearchParams(body) };

    apiFetch(`channels/${content.value.channelId}/contents/${content.value.id}/elements`, onfulfilled, onfinally, init, undefined, handleResponse);
  }
</script>

<template>
  <h2>ContetnView</h2>
  <p>Current route path: {{ $route.fullPath }}</p>
  <p v-if="loading">Loading...</p>
  <p v-if="error">{{ error }}</p>
  <template v-if="content">
    <h1>{{ content.draftVersion.name }}</h1>
    <button @click="() => contentElement('h1', 0)">Add Header 1</button>
    <button @click="() => contentElement('h2', 0)">Add Header 2</button>
    <button @click="() => contentElement('h3', 0)">Add Header 3</button>
    <button @click="() => contentElement('h4', 0)">Add Header 4</button>
    <button @click="() => contentElement('h5', 0)">Add Header 5</button>
    <button @click="() => contentElement('h6', 0)">Add Header 6</button>
    <button @click="() => contentElement('p', 0)">Add Paragraph</button>
    <template v-if="content.draftVersion.elements">
      <template v-for="element in content.draftVersion.elements.sort((a, b) => a.index > b.index ? 1 : -1)" :key="'element' + element.index">
        <h1 v-if="element.tag == 'h1'">{{ element.content ?? "(unknown)" }}</h1>
        <h2 v-if="element.tag == 'h2'">{{ element.content ?? "(unknown)" }}</h2>
        <h3 v-if="element.tag == 'h3'">{{ element.content ?? "(unknown)" }}</h3>
        <h4 v-if="element.tag == 'h4'">{{ element.content ?? "(unknown)" }}</h4>
        <h5 v-if="element.tag == 'h5'">{{ element.content ?? "(unknown)" }}</h5>
        <h6 v-if="element.tag == 'h6'">{{ element.content ?? "(unknown)" }}</h6>
        <p v-if="element.tag == 'p'">{{ element.content ?? "(unknown)" }}</p>
        <button @click="() => contentElement('h1', element.index)">Add Header 1</button>
        <button @click="() => contentElement('h2', element.index)">Add Header 2</button>
        <button @click="() => contentElement('h3', element.index)">Add Header 3</button>
        <button @click="() => contentElement('h4', element.index)">Add Header 4</button>
        <button @click="() => contentElement('h5', element.index)">Add Header 5</button>
        <button @click="() => contentElement('h6', element.index)">Add Header 6</button>
        <button @click="() => contentElement('p', element.index)">Add Paragraph</button>
      </template>
    </template>
  </template>
</template>
