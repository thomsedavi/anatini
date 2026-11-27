<script setup lang="ts">
  import { ref, watch } from 'vue';
  import { useRoute } from 'vue-router';
  import { parseParams, type Params } from '@/common/utils';
  import { apiFetchAll, apiFetchAuthenticated } from '@/common/apiFetch';
  import type { APIResponse, EventOccurrence, Request, StatusActions, User } from '@/common/types';

  const route = useRoute();

  const userResponse = ref<APIResponse<User>>({ fetching: true });
  const eventOccurrenceResponse = ref<APIResponse<EventOccurrence>>({ fetching: true });

  watch([() => route.params.userId, () => route.params.eventId, () => route.params.occurrenceId], (params: Params) => fetchEventOccurrence(parseParams(params)), { immediate: true });

  async function fetchEventOccurrence(params: string[]) {
    const userRequest: Request = {
      input: `users/${params[0]}`,
      statusActions: {
        200: (response?: Response) => {
          response?.json()
            .then((value: User) => {
              userResponse.value = { data: value };
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
              eventOccurrenceResponse.value = { data: value };
            });
        }
      }
    };

    apiFetchAll([userRequest, userEventOccurrenceRequest]);
  }

  function toggleBookmark(): void {
    if (eventOccurrenceResponse.value.data?.isBookmarked === true) {
      const input = `users/${route.params.userId}/events/${route.params.eventId}/occurrences/${route.params.occurrenceId}/bookmark`;

      const statusActions: StatusActions = {
        204: () => {
          if (eventOccurrenceResponse.value.data !== undefined) eventOccurrenceResponse.value.data.isBookmarked = false;
        }
      }

      const init: RequestInit = { method: "DELETE" };

      apiFetchAuthenticated({ input, statusActions, init });
    } else if (eventOccurrenceResponse.value.data?.isBookmarked === false) {
      const input = `users/${route.params.userId}/events/${route.params.eventId}/occurrence/${route.params.occurrenceId}/bookmark`;

      const statusActions: StatusActions = {
        201: () => {
          if (eventOccurrenceResponse.value.data !== undefined) eventOccurrenceResponse.value.data.isBookmarked = true;
        }
      }

      const init: RequestInit = { method: "POST" };

      apiFetchAuthenticated({ input, statusActions, init });
    }
  }
</script>

<template>
  <main role="main" id="main" tabindex="-1">
    <article :aria-busy="eventOccurrenceResponse.fetching === true" aria-labelledby="heading-main">
      <template v-if="eventOccurrenceResponse.data !== undefined">
        <header>
          <h1>{{ eventOccurrenceResponse.data.title }}</h1>
        </header>

        <section v-if="eventOccurrenceResponse.data.body !== null" aria-label="About event occurrence" v-html="eventOccurrenceResponse.data.body">
        </section>

        <footer>
          <nav>

          </nav>
          <menu role="toolbar">
            <li>
              <button type="button" :aria-pressed="eventOccurrenceResponse.data.isBookmarked ?? false" @click="toggleBookmark">Bookmark</button>
            </li>
          </menu>
        </footer>
      </template>
    </article>
  </main>
</template>
