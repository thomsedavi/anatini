<script setup lang="ts">
  import { watch } from 'vue';
  import { useRoute } from 'vue-router';
  import { parseSource, type Source } from '../common/utils';
  import { apiFetchAll } from '../common/apiFetch';
  import type { Event, Request, User } from '@/types';

  const route = useRoute();

  watch([() => route.params.userId, () => route.params.eventId, () => route.params.occurrenceId], (source: Source) => fetchEventOccurrence(parseSource(source)), { immediate: true });

  async function fetchEventOccurrence(params: string[]) {
    const userRequest: Request = {
      input: `users/${params[0]}`,
      statusActions: {
        200: (response?: Response) => {
          response?.json()
            .then((value: User) => {
              console.log(value.name);
            });
        }
      }
    };

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

    const userEventOccurrenceRequest: Request = {
      input: `users/${params[0]}/events/${params[1]}/occurrence/${params[2]}`,
      statusActions: {
        200: (response?: Response) => {
          console.log(response);
        }
      }
    };

    apiFetchAll([userRequest, userEventRequest, userEventOccurrenceRequest]);
  }
</script>

<template>
  <h1>User Event Occurrence</h1>
</template>
