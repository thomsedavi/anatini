<script setup lang="ts">
  import type { Note, StatusActions } from '@/types';
  import { formatLong, formatUTC } from '../common/dateUtils';
  import { onMounted } from 'vue';
  import { apiFetchAuthenticated } from '../common/apiFetch';
  import { useRouter } from 'vue-router';
  import { handleClick } from '../common/utils';

  const router = useRouter();

  const props = defineProps<{
    notes: Note[] | null,
  }>();

  const emit = defineEmits<{
    'update-notes': [newNotes: Note[]],
  }>();

  onMounted(() => {
    if (props.notes === null) {
      const statusActions: StatusActions = {
        200: (response?: Response) => {
          response?.json()
            .then((value: Note[]) => {
              emit('update-notes', value);
            });
        }
      }

      apiFetchAuthenticated(`account/notes`, statusActions);
    }
  });
</script>

<template>
  <section id="panel-notes" role="tabpanel" aria-labelledby="tab-notes">
    <header>
      <h2>Notes</h2>
      <RouterLink :to="{ name: 'AccountNoteCreate' }">+ Create Note</RouterLink>
    </header>

    <ul role="list" v-if="notes !== null">
      <li v-for="note in notes" :key="'note' + note.id">
        <article v-html="`${note.article.substring(9, note.article.length - 10)}<footer><time datetime='${formatUTC(note.publishedAtUtc)}'>${formatLong(note.publishedAtUtc)}</time><menu><li><a href='/account/notes/${note.handle ?? note.id}/edit'>Edit</a></li></menu></footer>`" @click.prevent="(mouseEvent) => handleClick(mouseEvent, router)">
        </article>
      </li>
    </ul>

    <p v-else>You do not have any notes</p>
  </section>
</template>
