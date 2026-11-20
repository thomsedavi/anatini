<script setup lang="ts">
  import type { NoteEdit, StatusActions, Tab, APIResponse } from '@/common/types';
  import { nextTick, onMounted, ref, watch } from 'vue';
  import { useRoute, useRouter } from 'vue-router';
  import { getTabIndex, parseFromString, parseParams, type Params } from '@/common/utils';
  import { apiFetchAuthenticated } from '@/common/apiFetch';
  import TabButton from '@/common/TabButton.vue';

  const headingMainRef = ref<HTMLElement | null>(null);

  const route = useRoute();
  const router = useRouter();

  const note = ref<APIResponse<NoteEdit>>({ fetching: true });
  const article = ref<Node | null>(null);
  const eTag = ref<string | null>(null);
  const tabIndex = ref<number>(0);
  const pageStatus = ref<string>('Loading note...');

  const tabRefs = ref<HTMLButtonElement[]>([]);

  const tabs: Tab[] = [
    { id: 'article', text: 'Article', name: 'NoteEditArticle' },
    { id: 'details', text: 'Details', name: 'NoteEditDetails' },
    { id: 'status', text: 'Status', name: 'NoteEditStatus' }
  ];

  onMounted(() => {
    tabIndex.value = tabs.findIndex(tab => tab.name === route.name);
  });

  watch([() => route.params.spaceId, () => route.params.noteId], (params: Params) => fetchNote(parseParams(params)), { immediate: true });

  async function fetchNote(params: string[]) {
    const input = `spaces/${params[0]}/notes/${params[1]}/edit`;

    const statusActions: StatusActions = {
      200: (response?: Response) => {
        response?.json()
          .then((value: NoteEdit) => {
            note.value = { data: value };
            pageStatus.value = 'Ready';

            eTag.value = response.headers.get("ETag");

            const node = parseFromString(value.version.body);

            if (node?.nodeName === 'ARTICLE') {
              article.value = node;
            }

            nextTick(() => {
              headingMainRef.value?.focus();
            });
          })
          .catch(() => {
            note.value = { error: { status: 500, heading: 'Unknown Error', body: 'There was a problem fetching your note, please reload the page' }};
          });
      }
    }

    apiFetchAuthenticated({ input, statusActions });
  }

  function getHeading(): string {
    if (note.value.fetching === true) {
      return 'Fetching...';
    } else if (note.value.error !== undefined) {
      return note.value.error.heading;
    } else if (note.value.data !== undefined) {
      return 'Note Edit';
    } else {
      return 'Unknown Error';
    }
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

  function handleClick(index: number): void {
    const tab = tabs[index];

    if (tab !== undefined) {
      tabIndex.value = index;

      router.push({ name: tab.name });
      
      nextTick(() => {
        tabRefs.value[index]?.focus();
      });
    }
  }

  function handleUpdateTitle(newTitle: string): void {
    if (note.value.data !== undefined) note.value.data.version.title = newTitle;
  }

  function handleUpdateDateNZ(newDateNZ: string): void {
    if (note.value.data !== undefined) note.value.data.version.dateNZ = newDateNZ;
  }

  function handleUpdateNoteStatus(newNoteStatus: 'Draft' | 'Published'): void {
    if (note.value.data !== undefined) note.value.data.status = newNoteStatus;
  }

  function handleUpdateETag(newETag: string | null): void {
    eTag.value = newETag;
  }

  function handleUpdateArticle(newArticle: Node | null): void {
    article.value = newArticle;
  }
</script>

<template>
  <main role="main" id="main" tabindex="-1" :aria-busy="note === null">
    <header>
      <h1 ref="headingMainRef" tabindex="-1">{{ getHeading() }}</h1>
    </header>

    <section v-if="note.fetching === true">                
      <progress max="100">Fetching note...</progress>
    </section>

    <section v-if="note.error !== undefined">
      <p>
        {{ note.error.body }}
      </p>
    </section>

    <template v-if="note.data !== undefined">
      <ul role="tablist" aria-label="Note Settings">
        <TabButton v-for="(tab, index) in tabs"
          :key="tab.id"
          :selected="tabIndex === index"
          @click="() => handleClick(index)"
          @keydown="(event: KeyboardEvent) => handleKeyDown(event, index)"
          :text="tab.text"
          :id="tab.id"
          :add-button-ref="(el: HTMLButtonElement) => { tabRefs.push(el); }" />
      </ul>

      <RouterView v-slot="{ Component }">
        <component
          :is="Component"  
          :data-article="article"
          :data-note="note"
          :data-space-id="note.data.spaceId"
          :data-note-id="note.data.id"
          :data-page-status="pageStatus"
          :data-title="note.data.version.title"
          :data-date-nz="note.data.version.dateNZ"
          :data-e-tag="eTag"
          @update-etag="handleUpdateETag"
          @update-article="handleUpdateArticle"
          @update-title="handleUpdateTitle"
          @update-date-nz="handleUpdateDateNZ"
          @update-note-status="handleUpdateNoteStatus"
        />
      </RouterView>
    </template>

    <p role="status" class="visually-hidden">{{ pageStatus }}</p>
  </main>
</template>
