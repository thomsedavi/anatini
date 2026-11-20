<script setup lang="ts">
  import { onMounted, ref } from 'vue';
  import { useRouter } from 'vue-router';
  import { apiFetch } from '@/common/apiFetch';
  import { defaultFilters, getSearchParameters, type Filters } from '@/common/filters';
  import FilterRadios from '@/common/FilterRadios.vue';
  import { buttonAction, getNoteHtml } from '@/common/html';
  import { store } from '@/common/store';
  import type { Note, SearchParameter, StatusActions } from '@/common/types';
  import { getNoteAddress, handleClick } from '@/common/utils';

  const router = useRouter();

  const props = defineProps<{
    dataNotes: Note[] | null,
  }>();

  const emit = defineEmits<{
    'update-notes': [newNotes: Note[]],
  }>();

  const filters = ref<Filters>({ ...defaultFilters });
  const currentSearchParameters = ref<SearchParameter[]>([]);
  const hasMore = ref<boolean>(true);

  onMounted(() => {
    if (props.dataNotes === null) {
      const input = 'notes';

      const statusActions: StatusActions = {
        200: (response?: Response) => {
          response?.json()
            .then((value: Note[]) => {
              emit('update-notes', value);
            });
        }
      }

      apiFetch({ input, statusActions });
    }
  });

  function getNotes() {
    const input = 'notes';

    const statusActions: StatusActions = {
      200: (response?: Response) => {
        response?.json()
          .then((value: Note[]) => {
            if (value.length < 10) {
              hasMore.value = false;
            }

            emit('update-notes', value);
          });
      }
    }

    currentSearchParameters.value = getSearchParameters(filters.value);

    apiFetch({ input, statusActions, searchParameters: currentSearchParameters.value });
  }

  function getMoreNotes() {
    const lastNote = props.dataNotes?.[props.dataNotes.length - 1];

    if (lastNote !== undefined) {
      const input = 'notes';

      const statusActions: StatusActions = {
        200: (response?: Response) => {
          response?.json()
            .then((value: Note[]) => {
              if (value.length < 2) {
                hasMore.value = false;
              }

              emit('update-notes', [...(props.dataNotes ?? []), ...value]);
            });
        }
      }

      const searchParameters = [...currentSearchParameters.value];

      searchParameters.push({ key: 'lastPublishedAtNz', value: lastNote.publishedAtNz });
      searchParameters.push({ key: 'lastNoteId', value: lastNote.id });

      apiFetch({ input, statusActions, searchParameters });
    }
  }
</script>

<template>
  <section id="panel-notes" role="tabpanel" aria-labelledby="tab-notes">
    <header>
      <h2>Notes</h2>

      <template v-if="store.isAuthenticated">
        <nav v-if="(store.spaces ?? []).length > 0" aria-label="Note creation options">
          <button popovertarget="create-note-menu">
            + Create Note
          </button>
          <ul popover="auto" id="create-note-menu">
            <li>
              <RouterLink :to="{ name: 'UserNoteCreate', params: { userId: store.userHandle } }">Personal Note</RouterLink>
            </li>
            <li v-for="space in store.spaces ?? []" :key="'space' + space.id">
              <RouterLink :to="{ name: 'SpaceNoteCreate', params: { spaceId: space.handle } }">{{space.name}} Note</RouterLink>
            </li>
          </ul>
        </nav>
        <RouterLink v-else :to="{ name: 'UserNoteCreate', params: { userId: store.userHandle } }">+ Create Note</RouterLink>
      </template>
    </header>

    <search role="search" v-if="store.isAuthenticated">
      <details>
        <summary>Filter Options</summary>

        <form @submit.prevent="getNotes" action="/api/notes" method="GET" novalidate>
          <FilterRadios v-model="filters" content-type="Note" />

          <button type="submit">Apply Filters</button>
          <button type="reset">Clear Filters</button>
        </form>
      </details>
    </search>

    <ul role="list" v-if="dataNotes !== null">
      <li v-for="note in dataNotes" :key="'note' + note.id">
        <article v-html="getNoteHtml(note, true)" @click.prevent="(mouseEvent) => handleClick(mouseEvent, router, (label, pressed) => buttonAction(label, pressed, note, getNoteAddress(note, 'id')))">
        </article>
      </li>
    </ul>

    <p v-else>There are not any notes</p>

    <footer>
      <button type="button" aria-controls="panel-notes" v-if="dataNotes !== null && hasMore" @click="() => getMoreNotes()">More</button>
    </footer>
  </section>
</template>
