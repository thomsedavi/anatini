<script setup lang="ts">
  import type { APIResponse, SpaceEdit, InputError, Note, Status, StatusActions, Tab } from '@/types';
  import { nextTick, onMounted, ref, watch } from 'vue';
  import { apiFetchAuthenticated } from '../common/apiFetch.ts';
  import { useRoute, useRouter } from 'vue-router';
  import { getTabIndex, parseSource, type Source } from '../common/utils.ts';
  import TabButton from '../common/TabButton.vue';

  const route = useRoute();
  const router = useRouter();

  const space = ref<APIResponse<SpaceEdit>>({ fetching: true });
  const notes = ref<Note[] | null>(null);
  const tabIndex = ref<number>(-1);
  const inputName = ref<string>('');
  const inputErrors = ref<InputError[]>([]);
  const status = ref<Status>('idle');
  const errorSectionRef = ref<HTMLElement | null>(null);

  const tabs: Tab[] = [
    { id: 'posts', text: 'Posts', name: 'SpaceEditPosts' },
    { id: 'notes', text: 'Notes', name: 'SpaceEditNotes', childNames: ['SpaceEditNoteCreate', 'SpaceEditNoteEdit'] },
    { id: 'public', text: 'Display', name: 'SpaceEditDisplay' },
  ];

  const tabRefs = ref<HTMLButtonElement[]>([]);

  onMounted(() => {
    tabIndex.value = tabs.findIndex(tab => tab.name === route.name || tab.childNames?.includes(route.name));
  });

  watch([() => route.params.spaceId], (source: Source) => fetchSpace(parseSource(source)), { immediate: true });

  async function fetchSpace(params: string[]) {
    const statusActions: StatusActions = {
      200: (response?: Response) => {
        response?.json()
          .then((value: SpaceEdit) => {
            space.value = { data: value };
            inputName.value = value.name;
          })
          .catch(() => { space.value = { error: { heading: 'Unknown Error', body: 'There was a problem fetching your account, please reload the page' }}});
      },
      401: () => {
        router.replace({ path: '/sign-in', query: { redirect: `/spaces/${params[0]}/posts/create` } });
      },
      403: () => {
        space.value = { error: { heading: 'Unknown Error', body: 'No access to space' }};
      },
      404: () => {
        space.value = { error: { heading: '404 Not Found', body: 'Space not found' }};
      },
      500: () => {
        space.value = { error: { heading: 'Unknown Error', body: 'There was a problem fetching your account, please reload the page' }};
      }
    };

    apiFetchAuthenticated(`spaces/${params[0]}/edit`, statusActions);
  };

  function getHeading(): string {
    if (space.value.fetching === true) {
      return 'Fetching...';
    } else if (space.value.error !== undefined) {
      return space.value.error.heading;
    } else if (space.value.data !== undefined) {
      return 'Space Settings';
    } else {
      return 'Unknown Error';
    }
  }

  function handleKeyDown(event: KeyboardEvent, index: number): void {
    const newIndex = getTabIndex(event.key, index, tabs.length);

    if (newIndex === undefined) {
      return;
    }

    event.preventDefault();
    tabIndex.value = newIndex;

    router.push({ name: tabs[newIndex].name });
    
    nextTick(() => {
      tabRefs.value[newIndex].focus();
    })
  }

  function handleClick(index: number): void {
    tabIndex.value = index;

    router.push({ name: tabs[index].name });
    
    nextTick(() => {
      tabRefs.value[index].focus();
    })
  }

  function handleUpdateName(newName: string): void {
    if (space.value.data !== undefined) {
     space.value.data.name = newName;
    }
  }

  function handleUpdateNotes(newNotes: Note[]): void {
    notes.value = newNotes;
  }

  function handleUpdateStatus(newStatus: Status): void {
    status.value = newStatus;
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
  <main id="main" tabindex="-1" :aria-busy="space === null">
    <header>
      <h1 id="heading-main">{{ getHeading() }}</h1>
    </header>

    <section v-if="space === null">
      <progress max="100">Fetching account...</progress>
    </section>

    <section v-if="space.error !== undefined">
      <p>
        {{ space.error.body }}
      </p>
    </section>

    <template v-if="space.data !== undefined">
      <section id="errors" v-if="inputErrors.length > 0" ref="errorSectionRef" tabindex="-1" aria-live="assertive" aria-labelledby="heading-errors">
        <h2 id="heading-errors">There was a problem updating your account</h2>
        <ul role="list">
          <li v-for="error in inputErrors" :key="'error' + error.id">
            <a :href="'#input-' + error.id">{{ error.message }}</a>
          </li>
        </ul>
      </section>

      <ul role="tablist" aria-label="Settings Options">
        <TabButton v-for="(tab, index) in tabs"
          :key="tab.id"
          :selected="tabIndex === index"
          @click="() => handleClick(index)"
          @keydown="event => handleKeyDown(event, index)"
          :text="tab.text"
          :id="tab.id"
          :add-button-ref="(el: HTMLButtonElement) => { tabRefs.push(el); }" />
      </ul>

      <RouterView v-slot="{ Component }">
        <component
          :is="Component"
          :spaceId="space.data.id"
          :notes="notes"
          :name="space.data.name"
          :icon-image="space.data.iconImage"
          :status="status"
          :inputErrors="inputErrors"
          @update-name="handleUpdateName"
          @update-status="handleUpdateStatus"
          @update-notes="handleUpdateNotes"
          @update-errors="handleUpdateErrors"
        />
      </RouterView>
    </template>

    <p role="status" class="visuallyhidden" aria-live="polite">{{ status === 'success' ? 'Created!' : undefined }}</p>
  </main>
</template>
