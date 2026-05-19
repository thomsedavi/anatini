<script setup lang="ts">
  import type { Note, SearchParameter, StatusActions } from '@/types';
  import { formatLong, formatUTC } from '../common/dateUtils';
  import { onMounted, ref } from 'vue';
  import { apiFetch, apiFetchAuthenticated } from '../common/apiFetch';
  import { useRouter } from 'vue-router';
  import { handleClick } from '../common/utils';
  import { store } from '@/store';
  import RadioFieldset from '../common/RadioFieldset.vue';

  const router = useRouter();

  const props = defineProps<{
    notes: Note[] | null,
  }>();

  const emit = defineEmits<{
    'update-notes': [newNotes: Note[]],
  }>();

  const bookmarkFilter = ref<string>('all');
  const starredFilter = ref<string>('all');
  const seenFilter = ref<string>('all');

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
  
  function getNotes() {
    const statusActions: StatusActions = {
      200: (response?: Response) => {
        response?.json()
          .then((value: Note[]) => {
            emit('update-notes', value);
          });
      }
    }

    const searchParams: SearchParameter[] = [];

    if (bookmarkFilter.value !== 'all') {
      searchParams.push({ key: 'bookmarked', value: bookmarkFilter.value });
    }

    if (starredFilter.value !== 'all') {
      searchParams.push({ key: 'starred', value: starredFilter.value });
    }

    if (seenFilter.value !== 'all') {
      searchParams.push({ key: 'seen', value: seenFilter.value });
    }

    apiFetch('notes', statusActions, undefined, searchParams);
  }

  function buttonAction(label: string, pressed: string | null, note: Note): void {
    const action = label.toLowerCase();

    if (pressed === 'true') {
      const statusActions: StatusActions = {
        204: () => {
          if (action === "bookmark") {
            note.hasBookmarked = null;
          } else if (action === "seen") {
            note.hasSeen = null;
          } else if (action === "star") {
            note.hasStarred = null;
          }
        }
      }

      const init: RequestInit = { method: "DELETE" };

      if (note.channelHeader !== null) {
        apiFetchAuthenticated(`channels/${note.channelHeader.handle}/notes/${note.handle}/${action}`, statusActions, init);
      } else if (note.userHeader !== null) {
        apiFetchAuthenticated(`users/${note.userHeader.handle}/notes/${note.handle}/${action}`, statusActions, init);
      }
    } else {
      const statusActions: StatusActions = {
        201: () => {
          if (action === "bookmark") {
            note.hasBookmarked = true;
          } else if (action === "seen") {
            note.hasSeen = true;
          } else if (action === "star") {
            note.hasStarred = true;
          }
        }
      }

      const init: RequestInit = { method: "POST" };

      if (note.channelHeader !== null) {
        apiFetchAuthenticated(`channels/${note.channelHeader.handle}/notes/${note.handle}/${action}`, statusActions, init);
      } else if (note.userHeader !== null) {
        apiFetchAuthenticated(`users/${note.userHeader.handle}/notes/${note.handle}/${action}`, statusActions, init);
      }
    }
  }

  function noteHtml(note: Note): string {
    return `
      ${getHeader(note)}${note.article.substring(9, note.article.length - 10)}
      <footer>
        <menu>
          <li>
            <button type='button' aria-label='Seen' aria-pressed='${note.hasSeen ? 'true' : 'false'}'>${note.hasSeen ? '<svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true" focusable="false"><path d="M17.94 17.94A10.07 10.07 0 0 1 12 20c-7 0-11-8-11-8a18.45 18.45 0 0 1 5.06-5.94M9.9 4.24A9.12 9.12 0 0 1 12 4c7 0 11 8 11 8a18.5 18.5 0 0 1-2.16 3.19m-6.72-1.07a3 3 0 1 1-4.24-4.24" /><line x1="1" y1="1" x2="23" y2="23" /></svg>' : '<svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true" focusable="false"><path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z" /><circle cx="12" cy="12" r="3" /></svg>'}</button>
          </li>
          <li>
            <button type='button' aria-label='Star' aria-pressed='${note.hasStarred ? 'true' : 'false'}'>${note.hasStarred ? '<svg width="24" height="24" viewBox="0 0 24 24" fill="currentColor" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true" focusable="false"><polygon points="12 2 15.09 8.26 22 9.27 17 14.14 18.18 21.02 12 17.77 5.82 21.02 7 14.14 2 9.27 8.91 8.26 12 2"/></svg>' : '<svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true" focusable="false"><polygon points="12 2 15.09 8.26 22 9.27 17 14.14 18.18 21.02 12 17.77 5.82 21.02 7 14.14 2 9.27 8.91 8.26 12 2"/></svg>'}</button>
          </li>
          <li>
            <button type='button' aria-label='Bookmark' aria-pressed='${note.hasBookmarked ? 'true' : 'false'}'>${note.hasBookmarked ? '<svg width="24" height="24" viewBox="0 0 24 24" fill="currentColor" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true" focusable="false"><path d="M19 21l-7-5-7 5V5a2 2 0 0 1 2-2h10a2 2 0 0 1 2 2z" /></svg>' : '<svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true" focusable="false"><path d="M19 21l-7-5-7 5V5a2 2 0 0 1 2-2h10a2 2 0 0 1 2 2z" /></svg>'}</button>
          </li>
        </menu>
      </footer>
    `;
  }
</script>

<template>
  <section id="panel-notes" role="tabpanel" aria-labelledby="tab-notes">
    <header>
      <h2>Notes</h2>
    </header>

    <search v-if="store.isAuthenticated">
      <details>
        <summary>Filter Options</summary>
        
        <form @submit.prevent="getNotes" action="/api/notes" method="GET" novalidate>
          <details>
            <summary>Starred Notes</summary>

            <RadioFieldset
              v-model="starredFilter"
              :radios="[
                { name: 'starred', value: 'all', id: 'starredAll', label: 'No filter' },
                { name: 'starred', value: 'only', id: 'starredOnly', label: 'Show only starred' },
                { name: 'starred', value: 'hide', id: 'starredHide', label: 'Hide starred' }
              ]"
              legend="Starred Notes Options" />
          </details>

          <details>
            <summary>Bookmarked Notes</summary>

            <RadioFieldset
              v-model="bookmarkFilter"
              :radios="[
                { name: 'bookmarked', value: 'all', id: 'bookmarkedAll', label: 'No filter' },
                { name: 'bookmarked', value: 'only', id: 'bookmarkedOnly', label: 'Show only bookmarked' },
                { name: 'bookmarked', value: 'hide', id: 'bookmarkedHide', label: 'Hide bookmarked' }
              ]"
              legend="Bookmarked Notes Options" />
          </details>

          <details>
            <summary>Seen Notes</summary>

            <RadioFieldset
              v-model="seenFilter"
              :radios="[
                { name: 'seen', value: 'all', id: 'seenAll', label: 'No filter' },
                { name: 'seen', value: 'only', id: 'seenOnly', label: 'Show only seen' },
                { name: 'seen', value: 'hide', id: 'seenHide', label: 'Hide seen' }
              ]"
              legend="Seen Notes Options" />
          </details>
          
          <button type="submit">Apply Filters</button>
        </form>
      </details>
    </search>

    <p role="status" aria-live="polite" class="visuallyhidden">
      Showing {{ notes?.length }} note{{ notes?.length == 1 ? '' : 's' }}. TODO, move this status element to parent.
    </p>

    <ul role="list" v-if="notes !== null">
      <li v-for="note in notes" :key="'note' + note.id">
        <article v-html="noteHtml(note)" @click.prevent="(mouseEvent) => handleClick(mouseEvent, router, (label, pressed) => buttonAction(label, pressed, note))">
        </article>
      </li>
    </ul>

    <p v-else>We do not have any notes.</p>
  </section>
</template>
