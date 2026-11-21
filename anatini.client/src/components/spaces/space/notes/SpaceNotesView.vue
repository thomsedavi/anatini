<script setup lang="ts">
  import { onMounted, ref } from 'vue';
  import { useRouter } from 'vue-router';
  import { apiFetch } from '@/common/apiFetch';
  import { defaultFilters, getSearchParameters, type Filters } from '@/common/filters';
  import FilterRadios from '@/common/FilterRadios.vue';
  import { contentButtonAction, getNoteHtml } from '@/common/html';
  import { store } from '@/common/store';
  import type { Note, SearchParameter, StatusActions } from '@/common/types';
  import { getNoteAddress, handleClick } from '@/common/utils';

  const router = useRouter();

  const props = defineProps<{
    dataSpaceId: string,
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
      const input = `spaces/${props.dataSpaceId}/notes`;

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
    const input = `spaces/${props.dataSpaceId}/notes`;

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
</script>

<template>
  <section id="panel-notes" role="tabpanel" aria-labelledby="tab-notes">
    <header>
      <h2>Notes</h2>
      <RouterLink :to="{ name: 'SpaceNoteCreate' }">+ Create Note</RouterLink>
    </header>

    <search role="search" v-if="store.isAuthenticated">
      <details>
        <summary>Filter Options</summary>

        <form @submit.prevent="getNotes" :action="`/api/spaces/${dataSpaceId}/notes`" method="GET" novalidate>
          <FilterRadios v-model="filters" content-type="Note" />

          <button type="submit">Apply Filters</button>
          <button type="reset">Clear Filters</button>
        </form>
      </details>
    </search>

    <ul role="list" v-if="dataNotes !== null">
      <li v-for="note in dataNotes" :key="'note' + note.id">
        <article :data-status="note.status.toLowerCase()" v-html="getNoteHtml(note, false)" @click.prevent="(mouseEvent) => handleClick(mouseEvent, router, (label, pressed) => contentButtonAction(label, pressed, note, getNoteAddress(note, 'id')))">
        </article>
      </li>
    </ul>

    <p v-else>You do not have any notes</p>
  </section>
</template>
