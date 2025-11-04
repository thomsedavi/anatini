<script setup lang="ts">
  import { ref, watch } from 'vue';
  import { useRoute } from 'vue-router';

  const route = useRoute();

  const loading = ref<boolean>(false);
  const content = ref<string | null>(null);
  const error = ref<string | null>(null);

  watch([() => route.params.channelId, () => route.params.contentId], fetchContent, { immediate: true });

  async function fetchContent(array: (() => string | string[])[]) {
    error.value = content.value = null
    loading.value = true

    fetch(`/api/channels/${array[0]}/contents/${array[1]}`, { method: "GET" })
      .then((value: Response) => {
        if (value.ok) {
          value.json()
            .then(value => {
              content.value = value.content;
            })
            .catch(() => {
              error.value = 'Unknown Error';
            });
        } else if (value.status === 401) {
          error.value = 'Unauthorised';
        } else {
          error.value = 'Unkown Error';
        }
      })
      .catch(() => {
        error.value = 'Unknown Error';
      }).
      finally(() => {
        loading.value = false
      });
  }
</script>

<template>
  <main>
    <h2>ContentView</h2>
    <p>Current route path: {{ $route.fullPath }}</p>
    <p v-if="loading">Loading...</p>
    <p v-if="error">{{ error }}</p>
    <p v-if="content">{{ content }}</p>
  </main>
</template>
