<script setup lang="ts">
  import { onMounted } from 'vue';
  import { apiFetchAuthenticated } from '@/common/apiFetch';
  import ContentArticle from '@/common/ContentArticle.vue';
  import type { Note, StatusActions } from '@/common/types';
  import { updateContent } from '@/common/utils';

  const props = defineProps<{
    dataSpaceId: string,
    dataNotes: Note[] | null,
  }>();

  const emit = defineEmits<{
    'update-notes': [newNotes: Note[]],
  }>();

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

      apiFetchAuthenticated({ input, statusActions });
    }
  });
</script>

<template>
  <section id="panel-notes" role="tabpanel" aria-labelledby="tab-notes">
    <header>
      <h2>Notes</h2>
      <RouterLink :to="{ name: 'SpaceEditNoteCreate' }">+ Create Note</RouterLink>
    </header>

    <ul role="list" v-if="dataNotes !== null">
      <li v-for="(note, index) in dataNotes" :key="'note' + note.id">
        <ContentArticle :content="note" content-type="Note" :index="index" :is-main="false" @update-content="(newNote) => updateContent(note, newNote)" />
      </li>
    </ul>

    <p v-else>You do not have any notes</p>
  </section>
</template>
