<script setup lang="ts">
  import { nextTick, onMounted, ref } from 'vue';
  import type { EventOccurrence, Note, Tab, Work } from '@/common/types';
  import TabButton from '@/common/TabButton.vue';
  import { getTabIndex } from '@/common/utils';
  import { useRoute, useRouter } from 'vue-router';

  const route = useRoute();
  const router = useRouter();

  const tabIndex = ref<number>(-1);
  const notes = ref<Note[] | null>(null);
  const works = ref<Work[] | null>(null);
  const eventOccurrenceGroups = ref<{ date: string, eventOccurrences: EventOccurrence[] }[] | null>(null);

  const tabs: Tab[] = [
    { id: 'notes', text: 'Notes', name: 'HomeNotes' },
    { id: 'works', text: 'Works', name: 'HomeWorks' },
    { id: 'events', text: 'Events', name: 'HomeEvents' },
  ];

  const tabRefs = ref<HTMLButtonElement[]>([]);

  onMounted(() => {
    tabIndex.value = tabs.findIndex(tab => tab.name === route.name);
  });

  function handleKeyDown(event: KeyboardEvent, index: number): void {
    event.preventDefault();
    
    const newIndex = getTabIndex(event.key, index, tabs.length);

    if (newIndex !== undefined) {
      const tab = tabs[newIndex];

      if (tab !== undefined) {
        tabIndex.value = newIndex;

        router.push({ name: tab.name });
      
        nextTick(() => {
          tabRefs.value[newIndex]?.focus();
        });
      }
    }
  }

  function handleClickTab(index: number): void {
    const tab = tabs[index];

    if (tab !== undefined) {
      tabIndex.value = index;

      router.push({ name: tab.name });
      
      nextTick(() => {
        tabRefs.value[index]?.focus();
      });
    }
  }

  function handleUpdateNotes(newNotes: Note[]): void {
    notes.value = newNotes;
  }

  function handleUpdateWorks(newWorks: Work[]): void {
    works.value = newWorks;
  }

  function handleUpdateEventOccurrenceGroups(newEventOccurrenceGroups: { date: string, eventOccurrences: EventOccurrence[] }[]): void {
    eventOccurrenceGroups.value = newEventOccurrenceGroups;
  }
</script>

<template>
  <main role="main" id="main" tabindex="-1">
    <header class="visually-hidden">
      <h1>Note Feed</h1>
    </header>

    <ul role="tablist" aria-label="Content Views">
      <TabButton v-for="(tab, index) in tabs"
        :key="tab.id"
        :selected="tabIndex === index"
        @click="() => handleClickTab(index)"
        @keydown="(event: KeyboardEvent) => handleKeyDown(event, index)"
        :text="tab.text"
        :id="tab.id"
        :add-button-ref="(el: HTMLButtonElement) => { tabRefs.push(el); }" />
    </ul>

    <RouterView v-slot="{ Component }">
      <component
        :is="Component"
        :data-notes="notes"
        :data-works="works"
        :data-event-occurrence-groups="eventOccurrenceGroups"
        @update-notes="handleUpdateNotes"
        @update-works="handleUpdateWorks"
        @update-event-occurrence-groups="handleUpdateEventOccurrenceGroups"
      />
    </RouterView>
  </main>
</template>
