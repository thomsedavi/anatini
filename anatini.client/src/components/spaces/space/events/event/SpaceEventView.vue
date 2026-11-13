<script setup lang="ts">
  import { watch } from 'vue';
  import { useRoute } from 'vue-router';
  import { parseParams, type Params } from '@/common/utils';

  const route = useRoute();

  watch([() => route.params.spaceId, () => route.params.eventId], (params: Params) => fetchEvent(parseParams(params)), { immediate: true });

  async function fetchEvent(params: string[]) {
    console.log(params);
  }
</script>

<template>
  <h1>Space Event</h1>
</template>
