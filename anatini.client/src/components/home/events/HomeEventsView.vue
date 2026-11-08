<script setup lang="ts">
  import { onMounted } from 'vue';
  import { useRouter } from 'vue-router';
  import { apiFetch } from '@/common/apiFetch';
  import { formatDate, getDayOfWeek } from '@/common/dateUtils';
  import { buttonAction, getEventOccurrenceHtml } from '@/common/html';
  import { store } from '@/common/store';
  import type { EventOccurrence, StatusActions } from '@/common/types';
  import { getEventOccurrenceAddress, handleClick } from '@/common/utils';

  const router = useRouter();

  const props = defineProps<{
    dataEventOccurrenceGroups: { date: string, eventOccurrences: EventOccurrence[] }[] | null,
  }>();

  const emit = defineEmits<{
    'update-event-occurrence-groups': [newEvents: { date: string, eventOccurrences: EventOccurrence[] }[]],
  }>();

  function addEventOccurrences(eventOccurrences: EventOccurrence[]): { date: string, eventOccurrences: EventOccurrence[] }[] {
    const eventOccurrenceGroups: { date: string, eventOccurrences: EventOccurrence[] }[] = [...props.dataEventOccurrenceGroups ?? []];

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
      const input = 'event-occurrences';

      const statusActions: StatusActions = {
        200: (response?: Response) => {
          response?.json()
            .then((value: EventOccurrence[]) => {
              emit('update-event-occurrence-groups', addEventOccurrences(value));
            });
        }
      }

      apiFetch({ input, statusActions });
    }
  });
</script>

<template>
  <section id="panel-events" role="tabpanel" aria-labelledby="tab-events">
    <header>
      <h2>Events</h2>

      <template v-if="store.isAuthenticated">
        <nav v-if="(store.spaces ?? []).length > 0" aria-label="Event creation options">
          <button popovertarget="create-event-menu">
            + Create Event
          </button>
          <ul popover="auto" id="create-event-menu">
            <li>
              <RouterLink :to="{ name: 'UserEventCreate', params: { userId: store.userHandle } }">Personal Event</RouterLink>
            </li>
            <li v-for="space in store.spaces ?? []" :key="'space' + space.id">
              <RouterLink :to="{ name: 'SpaceEventCreate', params: { spaceId: space.handle } }">{{space.name}} Event</RouterLink>
            </li>
          </ul>
        </nav>
        <RouterLink v-else :to="{ name: 'UserEventCreate', params: { userId: store.userHandle } }">+ Create Event</RouterLink>
      </template>
    </header>

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
