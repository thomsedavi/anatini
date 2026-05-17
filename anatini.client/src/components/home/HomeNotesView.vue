<script setup lang="ts">
  import type { Note, SearchParameter, StatusActions } from '@/types';
  import { formatLong, formatUTC } from '../common/dateUtils';
  import { onMounted, ref } from 'vue';
  import { apiFetch, apiFetchAuthenticated } from '../common/apiFetch';
  import { useRouter } from 'vue-router';
  import { handleClick } from '../common/utils';
  import { store } from '@/store';

  const router = useRouter();

  const props = defineProps<{
    notes: Note[] | null,
  }>();

  const emit = defineEmits<{
    'update-notes': [newNotes: Note[]],
  }>();

  const bookmarkFilter = ref<string>('All');
  const starredFilter = ref<string>('All');
  const seenFilter = ref<string>('All');

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

    if (bookmarkFilter.value !== 'All') {
      searchParams.push({ key: 'bookmarked', value: bookmarkFilter.value });
    }

    if (starredFilter.value !== 'All') {
      searchParams.push({ key: 'starred', value: starredFilter.value });
    }

    if (seenFilter.value !== 'All') {
      searchParams.push({ key: 'seen', value: seenFilter.value });
    }

    apiFetch('notes', statusActions, undefined, searchParams);
  }

  function buttonAction(label: string, note: Note): void {
    const lowerLabel = label.toLowerCase();

    if (lowerLabel.startsWith('un')) {
      const action = label.substring(2);

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
          if (lowerLabel === "bookmark") {
            note.hasBookmarked = true;
          } else if (lowerLabel === "seen") {
            note.hasSeen = true;
          } else if (lowerLabel === "star") {
            note.hasStarred = true;
          }
        }
      }

      const init: RequestInit = { method: "POST" };

      if (note.channelHeader !== null) {
        apiFetchAuthenticated(`channels/${note.channelHeader.handle}/notes/${note.handle}/${lowerLabel}`, statusActions, init);
      } else if (note.userHeader !== null) {
        apiFetchAuthenticated(`users/${note.userHeader.handle}/notes/${note.handle}/${lowerLabel}`, statusActions, init);
      }
    }
  }

  function noteHtml(note: Note): string {
    return `
      ${getHeader(note)}${note.article.substring(9, note.article.length - 10)}
      <footer>
        <menu>
          <li>
            ${note.hasSeen ?? false ? `<button type='button' aria-label='Unseen'>Unseen</button>` : `<button type='button' aria-label='Seen'>Seen</button>`}
          </li>
          <li>
            ${note.hasStarred ?? false ? `<button type='button' aria-label='Unstar'>Unstar</button>` : `<button type='button' aria-label='Star'>Star</button>`}
          </li>
          <li>
            ${note.hasBookmarked ?? false ? `<button type='button' aria-label='Unbookmark'>Unbookmark</button>` : `<button type='button' aria-label='Bookmark'>Bookmark</button>`}
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
        <summary>Filter options</summary>
        
        <form @submit.prevent="getNotes" action="/api/notes" method="GET" novalidate>
          <fieldset>
            <legend>Starred Posts</legend>

            <ul role="list">
              <li>
                <input type="radio" id="starred-all" name="starred" value="All" v-model="starredFilter" />
                <label for="starred-all">Do not filter by starred posts</label>
              </li>
              <li>
                <input type="radio" id="starred-only" name="starred" value="Only" v-model="starredFilter" />
                <label for="starred-only">Show only my starred posts</label>
              </li>
              <li>
                <input type="radio" id="starred-hide" name="starred" value="Hide" v-model="starredFilter" />
                <label for="starred-hide">Hide my starred posts</label>
              </li>
            </ul>
          </fieldset>

          <fieldset>
            <legend>Bookmarked Posts</legend>

            <ul role="list">
              <li>
                <input type="radio" id="bookmarked-all" name="bookmark" value="All" v-model="bookmarkFilter" />
                <label for="bookmarked-all">Do not filter by bookmarked posts</label>
              </li>
              <li>
                <input type="radio" id="bookmarked-only" name="bookmark" value="Only" v-model="bookmarkFilter" />
                <label for="bookmarked-only">Show only my bookmarked posts</label>
              </li>
              <li>
                <input type="radio" id="bookmarked-hide" name="bookmark" value="Hide" v-model="bookmarkFilter" />
                <label for="bookmarked-hide">Hide my bookmarked posts</label>
              </li>
            </ul>
          </fieldset>

          <fieldset>
            <legend>Seen Posts</legend>

            <ul role="list">
              <li>
                <input type="radio" id="seen-all" name="seen" value="All" v-model="seenFilter" />
                <label for="seen-all">Do not filter by seen posts</label>
              </li>
              <li>
                <input type="radio" id="seen-only" name="seen" value="Only" v-model="seenFilter" />
                <label for="seen-only">Show only my seen posts</label>
              </li>
              <li>
                <input type="radio" id="seen-hide" name="seen" value="Hide" v-model="seenFilter" />
                <label for="seen-hide">Hide my seen posts</label>
              </li>
            </ul>
          </fieldset>

          <button type="submit">Apply Filters</button>
        </form>
      </details>
    </search>

    <p role="status" aria-live="polite" class="visuallyhidden">
      Showing {{ notes?.length }} note{{ notes?.length == 1 ? '' : 's' }}. TODO, move this status element to parent.
    </p>

    <ul role="list" v-if="notes !== null">
      <li v-for="note in notes" :key="'note' + note.id">
        <article v-html="noteHtml(note)" @click.prevent="(mouseEvent) => handleClick(mouseEvent, router, (label) => buttonAction(label, note))">
        </article>
      </li>
    </ul>

    <p v-else>We do not have any notes.</p>
  </section>
</template>
