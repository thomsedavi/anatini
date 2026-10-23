<script setup lang="ts">
  import { onMounted } from 'vue';
  import { apiFetch } from '@/common/apiFetch';
  import type { EventOccurrence, StatusActions } from '@/common/types';
  import { formatDate, getDayOfWeek } from '@/common/dateUtils';
  import { getEventOccurrenceHtml } from '@/common/html';

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
    </header>

    <template v-if="dataEventOccurrenceGroups">
      <section v-for="eventOccurrenceGroup in dataEventOccurrenceGroups" :key="'eventOccurrenceGroup' + eventOccurrenceGroup.date" :aria-labelledby="eventOccurrenceGroup.date">
        <h3 :id="eventOccurrenceGroup.date">
          <time :datetime="eventOccurrenceGroup.date">{{ getDayOfWeek(eventOccurrenceGroup.date) }}, {{ formatDate(eventOccurrenceGroup.date) }}</time>
        </h3>

        <ul :aria-label="`Events starting ${formatDate(eventOccurrenceGroup.date)}`">
          <li v-for="eventOccurrence in eventOccurrenceGroup.eventOccurrences" :key="'eventOccurrence' + eventOccurrence.id">
            <article v-html="getEventOccurrenceHtml(eventOccurrence)">
            </article>
          </li>
        </ul>
      </section>
    </template>
  </section>
</template>
