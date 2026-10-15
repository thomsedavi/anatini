<script setup lang="ts">
  import { onMounted } from 'vue';
  import { apiFetch } from '@/common/apiFetch';
  import type { EventOccurrence, StatusActions } from '@/common/types';

  const props = defineProps<{
    dataEventOccurrences: EventOccurrence[] | null,
  }>();

  const emit = defineEmits<{
    'update-event-occurrences': [newPosts: EventOccurrence[]],
  }>();

  onMounted(() => {
    if (props.dataEventOccurrences === null) {
      const input = 'event-occurrences';

      const statusActions: StatusActions = {
        200: (response?: Response) => {
          response?.json()
            .then((value: EventOccurrence[]) => {
              emit('update-event-occurrences', value);
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
  </section>
</template>
