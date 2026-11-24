<script setup lang="ts">
  import { ref, watch } from 'vue';
  import { useRoute, useRouter } from 'vue-router';
  import { apiFetch } from '@/common/apiFetch';
  import { contentButtonAction, getNoteHtml } from '@/common/html';
  import type { APIResponse, Note, StatusActions } from '@/common/types';
  import { getContentStatus, getNoteAddress, handleClick, parseParams, type Params } from '@/common/utils';

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
            note.value = { error: { status: 'unknown', heading: 'Unknown Error', body: 'There was a problem fetching this note' }};
          });
      },
      401: () => {
        note.value = { error: { status: '401', heading: '401 Unauthorised', body: 'You do not have access to this note' }};
      },
      403: () => {
        note.value = { error: { status: '403', heading: '403 Forbidden', body: 'You do not have access to this note' }};
      },
      404: () => {
        note.value = { error: { status: '404', heading: '404 Not Found', body: 'Note not found' }};
      },
      500: () => {
        note.value = { error: { status: '500', heading: '500 Internal Server Error', body: 'There was a problem fetching this note' }};
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
          <RouterLink :to="{ name: 'UserNotes', params: { userId: dataUserHandle } }"><span aria-hidden="true">&larr;</span> Back to all notes by {{ dataUserName }}</RouterLink>
        </li>
      </ol>
    </nav>

    <article v-if="note.data !== undefined" :data-status="getContentStatus(note.data.status, note.data.isScheduled)" v-html="getNoteHtml(note.data, false)" @click.prevent="(mouseEvent) => handleClick(mouseEvent, router, (label, entityType, pressed) => contentButtonAction(label, pressed, note.data!, getNoteAddress(note.data!, entityType, 'id')))">
    </article>

    <article v-if="note.error !== undefined" :data-status="note.error.status">
      <h2>{{ note.error.heading }}</h2>
      <p>{{ note.error.body }}</p>
    </article>
  </section>
</template>
