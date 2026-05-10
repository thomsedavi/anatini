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
      let header = `<header><address><a href='/users/${note.userHeader.handle}' rel='author'>`;

      if (note.userHeader.iconImage !== null) {
        header += `<img src='${note.userHeader.iconImage.uri}' alt='${note.userHeader.iconImage.altText ?? 'User icon'}' aria-hidden='true' />`;
      }
      
      header += `<span>${note.userHeader.name}</span></a></address></header>`;

      return header;
    } else if (note.channelHeader !== null) {
      let header = `<header><address><a href='/channels/${note.channelHeader.handle}' rel='author'>`;

      if (note.channelHeader.iconImage !== null) {
        header += `<img src='${note.channelHeader.iconImage.uri}' alt='${note.channelHeader.iconImage.altText ?? 'User icon'}' aria-hidden='true' />`;
      }
      
      header += `<span>${note.channelHeader.name}</span></a></address></header>`;

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
        <article v-html="`${getHeader(note)}${note.article.substring(9, note.article.length - 10)}<footer><time datetime='${formatUTC(note.publishedAtUtc)}'>${formatLong(note.publishedAtUtc)}</time></footer>`" @click.prevent="(mouseEvent) => handleClick(mouseEvent, router)">
        </article>
      </li>
    </ul>

    <p v-else>We do not have any notes</p>
  </section>
</template>
