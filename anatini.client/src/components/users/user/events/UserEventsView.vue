<script setup lang="ts">
  import { onMounted, ref } from 'vue';
  import { useRouter } from 'vue-router';
  import { apiFetch } from '@/common/apiFetch';
  import { formatDate, getDayOfWeek } from '@/common/dateUtils';
  import { defaultFilters, getSearchParameters, type Filters } from '@/common/filters';
  import FilterRadios from '@/common/FilterRadios.vue';
  import { buttonAction, getEventOccurrenceHtml } from '@/common/html';
  import { store } from '@/common/store';
  import type { EventOccurrence, SearchParameter, StatusActions } from '@/common/types';
  import { getEventOccurrenceAddress, handleClick } from '@/common/utils';

  const router = useRouter();

  const props = defineProps<{
    dataUserId: string,
    dataEventOccurrenceGroups: { date: string, eventOccurrences: EventOccurrence[] }[] | null,
  }>();

  const emit = defineEmits<{
    'update-event-occurrence-groups': [newEvents: { date: string, eventOccurrences: EventOccurrence[] }[]],
  }>();

  const filters = ref<Filters>({ ...defaultFilters });
  const currentSearchParameters = ref<SearchParameter[]>([]);
  const hasMore = ref<boolean>(true);

  function addEventOccurrences(existingEventOccurrences: { date: string, eventOccurrences: EventOccurrence[] }[], eventOccurrences: EventOccurrence[]): { date: string, eventOccurrences: EventOccurrence[] }[] {
    const eventOccurrenceGroups: { date: string, eventOccurrences: EventOccurrence[] }[] = [...existingEventOccurrences];

    eventOccurrences.forEach(eventOccurrence => {
      const eventOccurrenceDate = eventOccurrence.startsAtNz.slice(0, 10);

      const eventOccurrenceGroup = eventOccurrenceGroups.filter(a => a.date === eventOccurrenceDate)[0];

      if (eventOccurrenceGroup === undefined) {
        eventOccurrenceGroups.push({ date: eventOccurrenceDate, eventOccurrences: [eventOccurrence] });
      } else {
        eventOccurrenceGroup.eventOccurrences.push(eventOccurrence);
      }
    });

    return eventOccurrenceGroups;
  }

  onMounted(() => {
    if (props.dataEventOccurrenceGroups === null) {
      const input = `users/${props.dataUserId}/event-occurrences`;

      const statusActions: StatusActions = {
        200: (response?: Response) => {
          response?.json()
            .then((value: EventOccurrence[]) => {
              emit('update-event-occurrence-groups', addEventOccurrences(props.dataEventOccurrenceGroups ?? [], value));
            });
        }
      }

      apiFetch({ input, statusActions });
    }
  });

  function getEventOccurrences() {
    const input = `users/${props.dataUserId}/event-occurrences`;

    const statusActions: StatusActions = {
      200: (response?: Response) => {
        response?.json()
          .then((value: EventOccurrence[]) => {
            if (value.length < 10) {
              hasMore.value = false;
            }

            emit('update-event-occurrence-groups', addEventOccurrences([], value));
          });
      }
    }

    currentSearchParameters.value = getSearchParameters(filters.value);

    apiFetch({ input, statusActions, searchParameters: currentSearchParameters.value });
  }
</script>

<template>
  <section id="panel-events" role="tabpanel" aria-labelledby="tab-events">
    <header>
      <h2>Events</h2>
      <RouterLink :to="{ name: 'UserEventCreate' }">+ Create Event</RouterLink>
    </header>

    <search role="search" v-if="store.isAuthenticated">
      <details>
        <summary>Filter Options</summary>

        <form @submit.prevent="getEventOccurrences" :action="`/api/users/${dataUserId}/event-occurrences`" method="GET" novalidate>
          <FilterRadios v-model="filters" />

          <button type="submit">Apply Filters</button>
          <button type="reset">Clear Filters</button>
        </form>
      </details>
    </search>

    <template v-if="dataEventOccurrenceGroups">
      <section v-for="eventOccurrenceGroup in dataEventOccurrenceGroups" :key="'eventOccurrenceGroup' + eventOccurrenceGroup.date" :aria-labelledby="eventOccurrenceGroup.date">
        <h3 :id="eventOccurrenceGroup.date">
          <time :datetime="eventOccurrenceGroup.date">{{ getDayOfWeek(eventOccurrenceGroup.date) }}, {{ formatDate(eventOccurrenceGroup.date) }}</time>
        </h3>

        <ul :aria-label="`Events starting ${formatDate(eventOccurrenceGroup.date)}`">
          <li v-for="eventOccurrence in eventOccurrenceGroup.eventOccurrences" :key="'eventOccurrence' + eventOccurrence.id">
            <article v-html="getEventOccurrenceHtml(eventOccurrence)" @click.prevent="(mouseEvent) => handleClick(mouseEvent, router, (label, pressed) => buttonAction(label, pressed, eventOccurrence, getEventOccurrenceAddress(eventOccurrence, 'id')))">
            </article>
          </li>
        </ul>
      </section>
    </template>
  </section>
</template>
