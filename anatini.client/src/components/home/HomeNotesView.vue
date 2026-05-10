<script setup lang="ts">
  import type { Note, StatusActions } from '@/types';
  import { formatLong, formatUTC } from '../common/dateUtils';
  import { onMounted } from 'vue';
  import { apiFetch } from '../common/apiFetch';
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

      apiFetch('notes', statusActions);
    }
  });

  function getHeader(note: Note): string {
    if (note.userHeader !== null) {
      let header = `<header><h3><a href='/users/${note.userHeader.handle}' rel='author'>`;

      if (note.userHeader.iconImage !== null) {
        header += `<img src='${note.userHeader.iconImage.uri}' alt='' aria-hidden='true' />`;
      }
      
      header += `<span>${note.userHeader.name}</span></a></h3><time datetime='${formatUTC(note.publishedAtUtc)}'>${formatLong(note.publishedAtUtc)}</time></header>`;

      return header;
    } else if (note.channelHeader !== null) {
      let header = `<header><h3><a href='/channels/${note.channelHeader.handle}' rel='author'>`;

      if (note.channelHeader.iconImage !== null) {
        header += `<img src='${note.channelHeader.iconImage.uri}' alt='' aria-hidden='true' />`;
      }
      
      header += `<span>${note.channelHeader.name}</span></a></h3><time datetime='${formatUTC(note.publishedAtUtc)}'>${formatLong(note.publishedAtUtc)}</time></header>`;

      return header;
    }

    return '';
  }
</script>

<template>
  <section id="panel-notes" role="tabpanel" aria-labelledby="tab-notes">
    <header>
      <h2>Notes</h2>
    </header>

    <ul role="list" v-if="notes !== null">
      <li v-for="note in notes" :key="'note' + note.id">
        <article v-html="`${getHeader(note)}${note.article.substring(9, note.article.length - 10)}<footer><menu><li><button type='button' aria-label='Like'><svg xmlns='http://www.w3.org/2000/svg' width='24' height='24' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2' stroke-linecap='round' stroke-linejoin='round' aria-hidden='true'><path d='M20.84 4.61a5.5 5.5 0 0 0-7.78 0L12 5.67l-1.06-1.06a5.5 5.5 0 0 0-7.78 7.78l1.06 1.06L12 21.23l8.78-8.78 1.06-1.06a5.5 5.5 0 0 0 0-7.78z'></path></svg></button></li><li><button type='button' aria-label='Bookmark'>Bookmark</button></li></menu></footer>`" @click.prevent="(mouseEvent) => handleClick(mouseEvent, router)">
        </article>
      </li>
    </ul>

    <p v-else>We do not have any notes</p>
  </section>
</template>
