<script setup lang="ts">
  import type { ErrorMessage, Note, StatusActions } from '@/types';
  import { nextTick, ref, watch } from 'vue';
  import { parseSource, type Source } from './common/utils';
  import { useRoute } from 'vue-router';
  import { apiFetch } from './common/apiFetch';

  const route = useRoute();

  const note = ref<Note | ErrorMessage | null>(null);

  watch([() => route.params.channelId, () => route.params.noteId], (source: Source) => fetchNote(parseSource(source)), { immediate: true });

  async function fetchNote(params: string[]) {
    const statusActions: StatusActions = {
      200: (response?: Response) => {
        response?.json()
          .then((value: Note) => {
            note.value = value;

            nextTick(() => {
              document.querySelector('h1')?.focus();
            });
          })
          .catch(() => { note.value = { error: true, heading: 'Unknown Error', body: 'There was a problem fetching your note, please reload the page' }});
      },
      404: () => {
        note.value = { error: true, heading: '404 Not Found', body: 'Note not found' };
      }
    }

    apiFetch(`channels/${params[0]}/notes/${params[1]}`, statusActions);
  }

  function getMainHtml(): string {
    if (note.value === null) {
      return '<p>Loading...</p>';
    } else if ('heading' in note.value) {
      return `<h1>${ note.value.heading }</h1>`;
    } else if ('article' in note.value) {
      return note.value.article;
    } else {
      return '<h1>Unknown Error</h1>';
    }
  }
</script>

<template>
  <main id="main" tabindex="-1" :aria-busy="note === null" v-html="getMainHtml()"></main>
</template>
