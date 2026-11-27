<script setup lang="ts">
  import { nextTick, ref, watch } from 'vue';
  import { useRoute, useRouter } from 'vue-router';
  import { apiFetch } from '@/common/apiFetch';
  import TabButton from '@/common/TabButton.vue';
  import type { APIResponse, EventOccurrence, InputError, Note, Space, Status, StatusActions, Tab, Work } from '@/common/types';
  import { getTabIndex, parseParams, type Params } from '@/common/utils';
  
  const route = useRoute();
  const router = useRouter();

  const spaceResponse = ref<APIResponse<Space>>({ fetching: true });
  const errorSectionRef = ref<HTMLElement | null>(null);
  const inputErrors = ref<InputError[]>([]);
  const status = ref<Status>('idle');
  const tabIndex = ref<number>(-1);
  const notes = ref<Note[] | null>(null);
  const works = ref<Work[] | null>(null);
  const eventOccurrenceGroups = ref<{ date: string, eventOccurrences: EventOccurrence[] }[] | null>(null);

  const tabs: Tab[] = [
    { id: 'notes', text: 'Notes', name: 'SpaceNotes', childNames: ['SpaceNote', 'SpaceNoteCreate', 'SpaceNoteEdit'] },
    { id: 'works', text: 'Works', name: 'SpaceWorks', childNames: ['SpaceWork', 'SpaceWorkCreate'] },
    { id: 'events', text: 'Events', name: 'SpaceEvents', childNames: ['SpaceEvent', 'SpaceEventCreate'] },
  ];

  const tabRefs = ref<HTMLButtonElement[]>([]);

  watch([() => route.params.spaceId], (params: Params) => fetchSpace(parseParams(params)), { immediate: true });

  async function fetchSpace(params: string[]) {
    tabIndex.value = tabs.findIndex(tab => tab.name === route.name || tab.childNames?.includes(route.name));

    spaceResponse.value = { fetching: true };

    const input = `spaces/${params[0]}`;

    const statusActions: StatusActions = {
      200: (response?: Response) => {
        response?.json()
          .then((value: Space) => {
            spaceResponse.value = {
              data: { ...value, about: value.about?.replace(/\r\n/g, "\n") ?? null }
            };
          })
          .catch(() => {
            spaceResponse.value = { error: { status: 'unknown', heading: 'Unknown Error', body: 'There was a problem fetching this space, please reload the page' }};
          });
      },
      404: () => {
        spaceResponse.value = { error: { status: '404', heading: '404 Not Found', body: 'Space not found' }};
      },
      500: () => {
        spaceResponse.value = { error: { status: '500', heading: '500 Internal Server Error', body: 'There was a problem fetching this space, please reload the page' }};
      }
    };

    apiFetch({ input, statusActions });
  }

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

  function handleUpdateErrors(newInputErrors: InputError[]): void {
    inputErrors.value = newInputErrors;

    if (newInputErrors.length > 0) {
      nextTick(() => {
        errorSectionRef.value?.focus();
      });
    }
  }
</script>

<template>
  <main role="main" id="main" tabindex="-1">
    <template v-if="spaceResponse.data !== undefined">
      <ul role="tablist" aria-label="Space Content">
        <TabButton v-for="(tab, index) in tabs"
          :key="tab.id"
          :selected="tabIndex === index"
          @click="() => handleClickTab(index)"
          @keydown="(event: KeyboardEvent) => handleKeyDown(event, index)"
          :text="spaceResponse.data.settings[`${tab.id}-name`] ?? tab.text"
          :id="tab.id"
          :add-button-ref="(el: HTMLButtonElement) => { tabRefs.push(el); }" />
      </ul>

      <RouterView v-slot="{ Component }">
        <component
          :is="Component"
          :data-status="status"
          :data-input-errors="inputErrors"
          :data-notes="notes"
          :data-works="works"
          :data-event-occurrence-groups="eventOccurrenceGroups"
          :data-space-id="spaceResponse.data.id"
          :data-space-handle="spaceResponse.data.handle"
          :data-space-name="spaceResponse.data.name"
          @update-notes="handleUpdateNotes"
          @update-works="handleUpdateWorks"
          @update-event-occurrence-groups="handleUpdateEventOccurrenceGroups"
          @update-errors="handleUpdateErrors"
        />
      </RouterView>
    </template>
  </main>
</template>
