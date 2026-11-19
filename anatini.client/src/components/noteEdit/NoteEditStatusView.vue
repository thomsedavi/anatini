<script setup lang="ts">
  import type { NoteEdit, StatusActions } from '@/common/types';
  import { apiFetchAuthenticated } from '@/common/apiFetch';

  const props = defineProps<{
    dataSpaceId: string,
    dataNoteId: string,
    dataNote: NoteEdit,
    dataPageStatus: string,
    dataETag: string | null,
  }>();

  const emit = defineEmits<{
    'update-etag': [eTag: string | null],
    'update-page-status': [newPageStatus: string],
    'update-note-status': [newNoteStatus: 'Draft' | 'Published'],
  }>();

  function setStatus(status: 'Draft' | 'Published'): void {
    if (props.dataETag === null) {
      return;
    }

    emit('update-page-status', status === 'Draft' ? 'Unpublishing...' : 'Publishing...');

    const body = new FormData();
      
    body.append('status', status);

    const init = { method: "PATCH", headers: { "If-Match": props.dataETag }, body: body };

    const input = `spaces/${props.dataSpaceId}/notes/${props.dataNoteId}`;

    const statusActions: StatusActions = {
        204: (response?: Response) => {
          emit('update-etag', response?.headers.get("ETag") ?? null);
          emit('update-note-status', status);
          emit('update-page-status', 'Ready');
        }
    }

    apiFetchAuthenticated({ input, statusActions, init });
  }
</script>

<template>
  <section id="panel-status" role="tabpanel" aria-labelledby="tab-status">
    <header>
      <h2>Status</h2>
    </header>

    <p>This article is currently {{ dataNote.status.toLowerCase() }}.</p>

    <p v-if="dataNote.status === 'Published'">Republish to update with any changes.</p>

    <menu role="toolbar">
      <li>
        <button type="button" @click="() => setStatus('Published')" :aria-disabled="dataPageStatus !== 'Ready' ? true : undefined">{{ dataNote.status === 'Published' ? 'Republish' : 'Publish' }}</button>
      </li>
      <li>
        <button type="button" @click="() => setStatus('Draft')" v-if="dataNote.status !== 'Draft'" :aria-disabled="dataPageStatus !== 'Ready' ? true : undefined">Unpublish</button>
      </li>
    </menu>
  </section>
</template>
