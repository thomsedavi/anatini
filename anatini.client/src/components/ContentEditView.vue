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
  const input = ref<string>('');
  const textarea = ref<string>('');

  watch([() => route.params.channelId, () => route.params.contentId], watchCallback, { immediate: true });

  async function watchCallback(array: (() => string | string[])[]) {
    content.value = null;

    const channelId = typeof array[0] === 'string' ? array[0] : array[0]();
    const contentId = typeof array[1] === 'string' ? array[1] : array[1]();

    await fetchContent(channelId, contentId);
  }

  async function fetchContent(channelId: string | string[], contentId: string | string[]) {
    error.value = null;
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

    await apiFetch(`channels/${channelId}/contents/${contentId}/edit`, onfulfilled, onfinally, undefined, statusActions, handleResponse);
  }

  async function editElement(index: number) {
    if (eTag.value === null || content.value === null) {
      return;
    }

    const onfulfilled = async (value: string) => {
      console.log(value);
    };

    const onfinally = () => {
      loading.value = false;
    };

    const handleResponse = (response: Response) => {
      eTag.value = response.headers.get("ETag");
    }

    const body = new FormData();

    body.append('index', index.toString());
    body.append('content', ' this  ');

    const init = { method: "PUT", headers: { "If-Match": eTag.value }, body: body };

    apiFetch(`channels/${content.value.channelId}/contents/${content.value.id}/elements`, onfulfilled, onfinally, init, undefined, handleResponse);
  }

  async function publish() {
    if (eTag.value === null || content.value === null) {
      return;
    }

    const onfulfilled = async (value: string) => {
      console.log(value);
    };

    const onfinally = () => {
      loading.value = false;
    };

    const handleResponse = (response: Response) => {
      eTag.value = response.headers.get("ETag");
    }

    const body = new FormData();

    body.append('status', 'Published');

    const init = { method: "PATCH", headers: { "If-Match": eTag.value }, body: body };

    apiFetch(`channels/${content.value.channelId}/contents/${content.value.id}`, onfulfilled, onfinally, init, undefined, handleResponse);
  }

  async function updateName() {
    if (eTag.value === null || content.value === null) {
      return;
    }

    const onfulfilled = async (value: string) => {
      console.log(value);
    };

    const onfinally = () => {
      loading.value = false;
    };

    const handleResponse = (response: Response) => {
      eTag.value = response.headers.get("ETag");
    }

    const body = new FormData();

    body.append('name', input.value);

    const init = { method: "PATCH", headers: { "If-Match": eTag.value }, body: body };

    apiFetch(`channels/${content.value.channelId}/contents/${content.value.id}`, onfulfilled, onfinally, init, undefined, handleResponse);
  }

  async function updateDate() {
    if (eTag.value === null || content.value === null) {
      return;
    }

    const onfulfilled = async (value: string) => {
      console.log(value);
    };

    const onfinally = () => {
      loading.value = false;
    };

    const handleResponse = (response: Response) => {
      eTag.value = response.headers.get("ETag");
    }

    const body = new FormData();

    body.append('dateNZ', input.value);

    const init = { method: "PATCH", headers: { "If-Match": eTag.value }, body: body };

    apiFetch(`channels/${content.value.channelId}/contents/${content.value.id}`, onfulfilled, onfinally, init, undefined, handleResponse);
  }

  async function contentElement(tag: string, element: string, insertAfter: number) {
    if (eTag.value === null || content.value === null) {
      return;
    }

    const onfulfilled = async (value: ContentElement) => {
      if (content.value?.version.elements?.some(element => (element.index === value.index + 1) || (element.index === value.index - 1))) {
        // reload the content because it will have been respaced
        await fetchContent(route.params.channelId, route.params.contentId);
      } else {
        const elements = content.value!.version.elements ?? [];

        elements.push(value);

        content.value!.version.elements = elements;
      }
    };

    const onfinally = () => {
      loading.value = false;
    };

    const handleResponse = (response: Response) => {
      eTag.value = response.headers.get("ETag");
    }

    const body = new FormData();

    body.append('insertAfter', insertAfter.toString());
    body.append('tag', tag);
    body.append('content', element);

    const init = { method: "POST", headers: { "If-Match": eTag.value }, body: body };

    apiFetch(`channels/${content.value.channelId}/contents/${content.value.id}/elements`, onfulfilled, onfinally, init, undefined, handleResponse);
  }
</script>

<template>
  <main>
    <h2>ContentEditView</h2>
    <p>Current route path: {{ $route.fullPath }}</p>
    <p v-if="loading">Loading...</p>
    <p v-if="error">{{ error }}</p>
    <template v-if="content">
      <h1>{{ content.version.name }}</h1>
      <p>{{ content.version.dateNZ }}</p>
      <p>{{ new Date(content.version.dateNZ).getFullYear() }}</p>
      <p>{{ new Date(content.version.dateNZ).getMonth() + 1 }}</p>
      <p>{{ new Date(content.version.dateNZ).getDate() }}</p>
      <input v-model="input" placeholder="add a single line" />
      <textarea v-model="textarea" placeholder="add multiple line"></textarea>
      <br>
      <br>
      <button type="button" @click="() => updateDate()">Update Date</button>
      <button type="button" @click="() => updateName()">Update Name</button>
      <button type="button" @click="() => publish()">Publish</button>
      <br>
      <br>
      <button type="button" @click="() => contentElement('h1', input, 0)">Add Header 1</button>
      <button type="button" @click="() => contentElement('h2', input, 0)">Add Header 2</button>
      <button type="button" @click="() => contentElement('h3', input, 0)">Add Header 3</button>
      <button type="button" @click="() => contentElement('h4', input, 0)">Add Header 4</button>
      <button type="button" @click="() => contentElement('h5', input, 0)">Add Header 5</button>
      <button type="button" @click="() => contentElement('h6', input, 0)">Add Header 6</button>
      <button type="button" @click="() => contentElement('p', textarea, 0)">Add Paragraph</button>
      <template v-if="content.version.elements">
        <template v-for="element in content.version.elements.sort((a, b) => a.index > b.index ? 1 : -1)" :key="'element' + element.index">
          <h1 v-if="element.tag === 'h1'">{{ element.content ?? "(unknown)" }}</h1>
          <h2 v-if="element.tag === 'h2'">{{ element.content ?? "(unknown)" }}</h2>
          <h3 v-if="element.tag === 'h3'">{{ element.content ?? "(unknown)" }}</h3>
          <h4 v-if="element.tag === 'h4'">{{ element.content ?? "(unknown)" }}</h4>
          <h5 v-if="element.tag === 'h5'">{{ element.content ?? "(unknown)" }}</h5>
          <h6 v-if="element.tag === 'h6'">{{ element.content ?? "(unknown)" }}</h6>
          <p v-if="element.tag === 'p'">{{ element.content ?? "(unknown)" }}<button type="button" @click="() => editElement(element.index)">Edit</button></p>
          <button type="button" @click="() => contentElement('h1', input, element.index)">Add Header 1</button>
          <button type="button" @click="() => contentElement('h2', input, element.index)">Add Header 2</button>
          <button type="button" @click="() => contentElement('h3', input, element.index)">Add Header 3</button>
          <button type="button" @click="() => contentElement('h4', input, element.index)">Add Header 4</button>
          <button type="button" @click="() => contentElement('h5', input, element.index)">Add Header 5</button>
          <button type="button" @click="() => contentElement('h6', input, element.index)">Add Header 6</button>
          <button type="button" @click="() => contentElement('p', textarea, element.index)">Add Paragraph</button>
        </template>
      </template>
    </template>
  </main>
</template>
