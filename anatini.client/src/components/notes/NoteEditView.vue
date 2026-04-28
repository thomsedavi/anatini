<script setup lang="ts">
  import { ref, watch } from 'vue';
  import { useRoute } from 'vue-router';
  import { parseSource, type Source } from '../common/utils';
  import type { ErrorMessage, NoteEdit, StatusActions } from '@/types';
  import { apiFetchAuthenticated } from '../common/apiFetch';

  const route = useRoute();

  const note = ref<NoteEdit | ErrorMessage | null>(null);

  watch([() => route.params.channelId, () => route.params.noteId], (source: Source) => fetchNote(parseSource(source)), { immediate: true });

  async function fetchNote(params: string[]) {
    const statusActions: StatusActions = {
      200: (response?: Response) => {
        response?.json()
          .then((value: NoteEdit) => {
            note.value = value;
          })
          .catch(() => { note.value = { error: true, heading: 'Unknown Error', body: 'There was a problem fetching your account, please reload the page' }});
      },
      404: () => {
        note.value = { error: true, heading: '404 Not Found', body: 'Channel not found' };
      },
      500: () => {
        note.value = { error: true, heading: 'Unknown Error', body: 'There was a problem fetching your account, please reload the page' };
      }
    };

    apiFetchAuthenticated(`channels/${params[0]}/notes/${params[1]}/edit`, statusActions);
  };
</script>

<template>
  <main id="main" tabindex="-1">

  </main>
</template>
