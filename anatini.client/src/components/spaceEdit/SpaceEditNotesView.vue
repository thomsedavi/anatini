<script setup lang="ts">
  import type { Note, StatusActions } from '@/common/types';
  import { formatDateTime } from '@/common/dateUtils';
  import { onMounted } from 'vue';
  import { apiFetchAuthenticated } from '@/common/apiFetch';
  import { handleClick } from '@/common/utils';
  import { useRouter } from 'vue-router';

  const router = useRouter();

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
      <li v-for="note in dataNotes" :key="'note' + note.id">
        <article :data-status="note.status.toLowerCase()" v-html="`${note.body}<footer><time datetime='${note.publishedAtNz}'>${formatDateTime(note.publishedAtNz)}</time><menu role='toolbar'><li><a href='/spaces/${dataSpaceId}/edit/notes/${note.handle ?? note.id}/edit'>Edit</a></li></menu></footer>`" @click.prevent="(mouseEvent) => handleClick(mouseEvent, router)">
        </article>
      </li>
    </ul>

    <p v-else>You do not have any notes</p>
  </section>
</template>
