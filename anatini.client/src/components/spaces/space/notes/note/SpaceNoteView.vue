<script setup lang="ts">
  import { ref, watch } from 'vue';
  import { useRoute } from 'vue-router';
  import { apiFetch } from '@/common/apiFetch';
  import ContentArticle from '@/common/ContentArticle.vue';
  import type { APIResponse, Note, StatusActions } from '@/common/types';
  import { parseParams, updateContent, type Params } from '@/common/utils';

  const route = useRoute();

  defineProps<{
    dataSpaceId: string,
    dataSpaceHandle: string,
    dataSpaceName: string,
  }>();

  const noteResponse = ref<APIResponse<Note>>({ fetching: true });

  watch([() => route.params.spaceId, () => route.params.noteId], (params: Params) => fetchNote(parseParams(params)), { immediate: true });

  async function fetchNote(params: string[]) {
    const input = `spaces/${params[0]}/notes/${params[1]}`;

    const statusActions: StatusActions = {
      200: (response?: Response) => {
        response?.json()
          .then((value: Note) => {
            noteResponse.value = { data: value };
          })
          .catch(() => {
            noteResponse.value = { error: { status: 'unknown', heading: 'Unknown Error', body: 'There was a problem fetching this note' }};
          });
      },
      401: () => {
        noteResponse.value = { error: { status: '401', heading: '401 Unauthorised', body: 'You do not have access to this note' }};
      },
      403: () => {
        noteResponse.value = { error: { status: '403', heading: '403 Forbidden', body: 'You do not have access to this note' }};
      },
      404: () => {
        noteResponse.value = { error: { status: '404', heading: '404 Not Found', body: 'Note not found' }};
      },
      500: () => {
        noteResponse.value = { error: { status: '500', heading: '500 Internal Server Error', body: 'There was a problem fetching this note' }};
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
          <RouterLink :to="{ name: 'SpaceNotes', params: { spaceId: dataSpaceHandle } }"><span aria-hidden="true">&larr;</span> Back to all notes by {{ dataSpaceName }}</RouterLink>
        </li>
      </ol>
    </nav>

    <ContentArticle v-if="noteResponse.data !== undefined" :content="noteResponse.data" content-type="Note" @update-content="(newNote) => updateContent(noteResponse.data!, newNote)" />

    <article v-if="noteResponse.error !== undefined" :data-status="noteResponse.error.status">
      <h2>{{ noteResponse.error.heading }}</h2>
      <p>{{ noteResponse.error.body }}</p>
    </article>
  </section></template>
