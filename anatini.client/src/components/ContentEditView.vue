<script setup lang="ts">
  import { ref, watch } from 'vue';
  import { useRoute } from 'vue-router';
  import { apiFetch } from './common/apiFetch';
  import type { ContentEdit } from '@/types';

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

  async function contentElement(tag: string) {
    if (eTag.value == null || content.value == null) {
      return;
    }

    const onfulfilled = (value: unknown) => {
      console.log(value);
    };

    const onfinally = () => {
      loading.value = false;
    };

    const body: Record<string, string> = {
      insertAfter: "256",
      tag: tag
    };

    const init = { method: "POST", headers: { "Content-Type": "application/x-www-form-urlencoded", "If-Match": eTag.value }, body: new URLSearchParams(body) };

    apiFetch(`channels/${content.value.channelId}/contents/${content.value.id}/elements`, onfulfilled, onfinally, init);
  }
</script>

<template>
  <h2>ContetnView</h2>
  <p>Current route path: {{ $route.fullPath }}</p>
  <p v-if="loading">Loading...</p>
  <p v-if="error">{{ error }}</p>
  <template v-if="content">
    <h1>{{ content.name }}</h1>
    <button @click="() => contentElement('h1')">Add Header 1</button>
    <button @click="() => contentElement('h2')">Add Header 2</button>
    <button @click="() => contentElement('h3')">Add Header 3</button>
    <button @click="() => contentElement('h4')">Add Header 4</button>
    <button @click="() => contentElement('h5')">Add Header 5</button>
    <button @click="() => contentElement('h6')">Add Header 6</button>
    <button @click="() => contentElement('p')">Add Paragraph</button>
  </template>
</template>
