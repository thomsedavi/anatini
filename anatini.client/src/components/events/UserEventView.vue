<script setup lang="ts">
  import { watch } from 'vue';
  import { useRoute } from 'vue-router';
  import { parseSource, type Source } from '../common/utils';
  import { apiFetchAll } from '../common/apiFetch';
  import type { Request } from '@/types';

  const route = useRoute();

  watch([() => route.params.userId, () => route.params.eventId], (source: Source) => fetchEvent(parseSource(source)), { immediate: true });

  async function fetchEvent(params: string[]) {
    const eventRequest: Request = {
      input: `users/${params[0]}/events/${params[1]}`,
      statusActions: {
        200: (response?: Response) => {
          console.log('response', response);
        }
      }
    };

    const eventInstancesRequest: Request = {
      input: `users/${params[0]}/events/${params[1]}/occurrences`,
      statusActions: {
        200: (response?: Response) => {
          console.log('response', response);
        }
      }
    };

    apiFetchAll([eventRequest, eventInstancesRequest]);
  }
</script>

<template>
  <h1>Test</h1>
</template>
