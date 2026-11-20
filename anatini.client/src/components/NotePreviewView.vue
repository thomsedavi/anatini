<script setup lang="ts">
  import { ref, watch } from 'vue';
  import { useRoute } from 'vue-router';
  import { apiFetchAuthenticated } from '@/common/apiFetch';
  import type { Note, StatusActions, APIResponse } from '@/common/types';
  import { parseParams, type Params } from '@/common/utils';

  const route = useRoute();

  const note = ref<APIResponse<Note>>({ fetching: true });

  watch([() => route.params.spaceId, () => route.params.noteId], (params: Params) => fetchNote(parseParams(params)), { immediate: true });

  async function fetchNote(params: string[]) {
    note.value = { fetching: true };

    const input = `spaces/${params[0]}/notes/${params[1]}/preview`;

    const statusActions: StatusActions = {
      200: (response?: Response) => {
        response?.json()
          .then((value: Note) => {
            note.value = { data: value};
          })
          .catch(() => {
            note.value = { error: { status: 500, heading: 'Unknown Error', body: 'There was a problem fetching your note, please reload the page' }};
          });
      },
      404: () => {
        note.value = { error: { status: 404, heading: '404 Not Found', body: 'Note not found' }};
      },
      500: () => {
        note.value = { error: { status: 500, heading: 'Unknown Error', body: 'There was a problem fetching this note, please reload the page' }};
      }
    };

    await apiFetchAuthenticated({ input, statusActions });
  }

  function getMainHtml(): string {
    if (note.value.fetching === true) {
      return '<p>Loading...</p>';
    } else if (note.value.error !== undefined) {
      return `<h1>${ note.value.error.heading }</h1>`;
    //} else if (note.value.data !== undefined) {
    //  return getNotes(note.value.data.version.article);
    } else {
      return '<h1>Unknown Error</h1>';
    }
  }
</script>

<template>
  <main role="main" id="main" tabindex="-1" v-html="getMainHtml()"></main>
</template>
