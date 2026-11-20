<script setup lang="ts">
  import { ref, watch } from 'vue';
  import { useRoute, useRouter } from 'vue-router';
  import { apiFetch } from '@/common/apiFetch';
  import { buttonAction, getNoteHtml } from '@/common/html';
  import type { APIResponse, Note, StatusActions } from '@/common/types';
  import { getNoteAddress, handleClick, parseParams, type Params } from '@/common/utils';

  const route = useRoute();
  const router = useRouter();

  defineProps<{
    dataUserId: string,
    dataUserHandle: string,
    dataUserName: string,
  }>();

  const note = ref<APIResponse<Note>>({ fetching: true });

  watch([() => route.params.userId, () => route.params.noteId], (params: Params) => fetchNote(parseParams(params)), { immediate: true });

  async function fetchNote(params: string[]) {
    const input = `users/${params[0]}/notes/${params[1]}`;

    const statusActions: StatusActions = {
      200: (response?: Response) => {
        response?.json()
          .then((value: Note) => {
            note.value = { data: value };
          })
          .catch(() => {
            note.value = { error: { heading: 'Unknown Error', body: 'There was a problem fetching your note, please reload the page' }};
          });
      },
      404: () => {
        note.value = { error: { heading: '404 Not Found', body: 'Note not found' }};
      }
    }

    apiFetch({ input, statusActions });
  }
</script>

<template>
  <section id="panel-notes" role="tabpanel" aria-labelledby="tab-notes" tabindex="0">
    <nav aria-label="Breadcrumb">
      <ol role="list">
        <li>
          <RouterLink :to="{ name: 'UserNotes', params: { userId: dataUserHandle } }"><span aria-hidden="true">&larr;</span> Back to all works by {{ dataUserName }}</RouterLink>
        </li>
      </ol>
    </nav>

    <article v-if="note.data !== undefined" v-html="getNoteHtml(note.data, false)" @click.prevent="(mouseEvent) => handleClick(mouseEvent, router, (label, pressed) => buttonAction(label, pressed, note.data!, getNoteAddress(note.data!, 'id')))">
    </article>
  </section>
</template>
