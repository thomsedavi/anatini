<script setup lang="ts">
  import type { APIResponse, Space, StatusActions } from '@/common/types';
  import { ref, watch } from 'vue';
  import { useRoute } from 'vue-router';
  import { apiFetch } from '@/common/apiFetch';
  import { parseSource, type Source } from '@/common/utils';
  
  const route = useRoute();

  const space = ref<APIResponse<Space>>({ fetching: true });

  watch([() => route.params.spaceId], (source: Source) => fetchSpace(parseSource(source)), { immediate: true });

  async function fetchSpace(params: string[]) {
    const input = `spaces/${params[0]}`;

    const statusActions: StatusActions = {
      200: (response?: Response) => {
        response?.json()
          .then((value: Space) => {
            space.value = { data: value };
          })
          .catch(() => {
            space.value = { error: { heading: 'Unknown Error', body: 'There was a problem fetching your space, please reload the page' }};
          });
      },
      404: () => {
        space.value = { error: { heading: '404 Not Found', body: 'Space not found' }};
      },
      500: () => {
        space.value = { error: { heading: 'Unknown Error', body: 'There was a problem fetching your space, please reload the page' }};
      }
    }

    apiFetch({ input, statusActions });
  }
</script>

<template>
  <main id="main" tabindex="-1">
    <h2>SpaceView</h2>
    <h3 v-if="space.data">{{ space.data.name }}</h3>
  </main>
</template>
