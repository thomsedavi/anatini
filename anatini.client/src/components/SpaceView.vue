<script setup lang="ts">
  import type { Space } from '@/types';
  import { ref, watch } from 'vue';
  import { useRoute } from 'vue-router';
  
  const route = useRoute();

  const loading = ref<boolean>(false);
  const error = ref<string | null>(null);
  const space = ref<Space | null>(null);

  watch([() => route.params.spaceId], fetchSpace, { immediate: true });

  async function fetchSpace(array: (() => string | string[])[]) {
    error.value = space.value = null
    loading.value = true

    fetch(`/api/spaces/${array[0]}`, { method: "GET" })
      .then((value: Response) => {
        if (value.ok) {
          value.json()
            .then((value: Space) => {
              space.value = value;
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
  <main id="main" tabindex="-1">
    <h2>SpaceView</h2>
    <h3 v-if="space">{{ space.name }}</h3>
  </main>
</template>
