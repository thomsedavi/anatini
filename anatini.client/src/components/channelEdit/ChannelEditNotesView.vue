<script setup lang="ts">
  import type { Note } from '@/types';
  import { formatLong, formatUTC } from '../common/dateUtils';

  defineProps<{
    notes: Note[] | null,
    hasNotesContinuationToken: boolean,
  }>();

  const emit = defineEmits<{
    'get-more-notes': [],
  }>();
</script>

<template>
  <section id="panel-notes" role="tabpanel" aria-labelledby="tab-notes">
    <header>
      <h2>Notes</h2>
      <RouterLink :to="{ name: 'NoteCreate' }">+ Create Note</RouterLink>
    </header>

    <section aria-labelledby="section-your-notes">
      <header>
        <h3 id="section-your-notes">Your Notes</h3>
      </header>

      <ul role="list" v-if="notes !== null">
        <li v-for="note in notes" :key="'note' + note.id">
          <article v-html="`${note.article.substring(9, note.article.length - 10)}<footer><time datetime='${formatUTC(note.publishedAtUtc)}'>${formatLong(note.publishedAtUtc)}</time></footer>`">
          </article>
        </li>
      </ul>

      <p v-else>You do not have any notes</p>

      <button v-if="hasNotesContinuationToken" @click="emit('get-more-notes')">Continuation Token</button>
    </section>
  </section>
</template>
