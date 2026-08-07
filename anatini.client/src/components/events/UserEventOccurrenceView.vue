<script setup lang="ts">
  import { ref, watch } from 'vue';
  import { useRoute } from 'vue-router';
  import { parseSource, type Source } from '@/common/utils';
  import { apiFetchAll, apiFetchAuthenticated } from '@/common/apiFetch';
  import type { APIResponse, EventOccurrence, Request, StatusActions, User } from '@/common/types';

  const route = useRoute();

  const user = ref<APIResponse<User>>({ fetching: true });
  const eventOccurrence = ref<APIResponse<EventOccurrence>>({ fetching: true });

  watch([() => route.params.userId, () => route.params.eventId, () => route.params.occurrenceId], (source: Source) => fetchEventOccurrence(parseSource(source)), { immediate: true });

  async function fetchEventOccurrence(params: string[]) {
    const userRequest: Request = {
      input: `users/${params[0]}`,
      statusActions: {
        200: (response?: Response) => {
          response?.json()
            .then((value: User) => {
              user.value = { data: value };
            });
        }
      }
    };

    const userEventOccurrenceRequest: Request = {
      input: `users/${params[0]}/events/${params[1]}/occurrence/${params[2]}`,
      statusActions: {
        200: (response?: Response) => {
          response?.json()
            .then((value: EventOccurrence) => {
              eventOccurrence.value = { data: value };
            });
        }
      }
    };

    apiFetchAll([userRequest, userEventOccurrenceRequest]);
  }

  function toggleBookmark(): void {
    if (eventOccurrence.value.data?.hasBookmarked === true) {
      const input = `users/${route.params.userId}/events/${route.params.eventId}/instances/${route.params.occurrenceId}/bookmark`;

      const statusActions: StatusActions = {
        204: () => {
          if (eventOccurrence.value.data !== undefined) eventOccurrence.value.data.hasBookmarked = false;
        }
      }

      const init: RequestInit = { method: "DELETE" };

      apiFetchAuthenticated({ input, statusActions, init });
    } else if (eventOccurrence.value.data?.hasBookmarked === false) {
      const input = `users/${route.params.userId}/events/${route.params.eventId}/instances/${route.params.occurrenceId}/bookmark`;

      const statusActions: StatusActions = {
        201: () => {
          if (eventOccurrence.value.data !== undefined) eventOccurrence.value.data.hasBookmarked = true;
        }
      }

      const init: RequestInit = { method: "POST" };

      apiFetchAuthenticated({ input, statusActions, init });
    }
  }
</script>

<template>
  <main id="main" tabindex="-1">
    <article :aria-busy="eventOccurrence.fetching === true" aria-labelledby="heading-main">
      <template v-if="eventOccurrence.data !== undefined">
        <header>
          <h1>{{ eventOccurrence.data.name }}</h1>
        </header>

        <section v-if="eventOccurrence.data.article !== null" aria-label="About event occurrence" v-html="eventOccurrence.data.article">
        </section>

        <footer>
          <nav>

          </nav>
          <menu>
            <li>
              <button type="button" :aria-pressed="eventOccurrence.data.hasBookmarked ?? false" @click="toggleBookmark">Bookmark</button>
            </li>
          </menu>
        </footer>
      </template>
    </article>
  </main>
</template>
