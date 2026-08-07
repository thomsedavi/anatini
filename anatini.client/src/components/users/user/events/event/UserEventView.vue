<script setup lang="ts">
  import { watch } from 'vue';
  import { useRoute } from 'vue-router';
  import { parseSource, type Source } from '@/common/utils';
  import { apiFetchAll } from '@/common/apiFetch';
  import type { Event, EventOccurrence, Request } from '@/common/types';

  const route = useRoute();

  watch([() => route.params.userId, () => route.params.eventId], (source: Source) => fetchEvent(parseSource(source)), { immediate: true });

  async function fetchEvent(params: string[]) {
    const userEventRequest: Request = {
      input: `users/${params[0]}/events/${params[1]}`,
      statusActions: {
        200: (response?: Response) => {
          response?.json()
            .then((value: Event) => {
              console.log(value.name);
            });
        }
      }
    };

    const userEventOccurrencesRequest: Request = {
      input: `users/${params[0]}/events/${params[1]}/occurrences`,
      statusActions: {
        200: (response?: Response) => {
          response?.json()
            .then((value: EventOccurrence[]) => {
              console.log(value);
            });
        }
      }
    };

    apiFetchAll([userEventRequest, userEventOccurrencesRequest]);
  }
</script>

<template>
  <h1>User Event</h1>
</template>
