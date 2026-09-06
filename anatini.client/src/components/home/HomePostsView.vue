<script setup lang="ts">
  import type { Post, SearchParameter, StatusActions } from '@/common/types';
  import { formatLong } from '@/common/dateUtils';
  import { onMounted, ref } from 'vue';
  import { apiFetch, apiFetchAuthenticated } from '@/common/apiFetch';
  import { useRouter } from 'vue-router';
  import { handleClick } from '@/common/utils';
  import { store } from '@/common/store';
  import RadioFieldset from '@/common/RadioFieldset.vue';

  const router = useRouter();

  const props = defineProps<{
    dataPosts: Post[] | null,
  }>();

  const emit = defineEmits<{
    'update-posts': [newPosts: Post[]],
  }>();

  const bookmarkFilter = ref<string>('all');
  const starredFilter = ref<string>('all');
  const dismissedFilter = ref<string>('all');
  const followedFilter = ref<string>('all');
  const baseSearchParams = ref<SearchParameter[]>([]);
  const hasMore = ref<boolean>(true);

  onMounted(() => {
    if (props.dataPosts === null) {
      const input = 'posts';

      const statusActions: StatusActions = {
        200: (response?: Response) => {
          response?.json()
            .then((value: Post[]) => {
              emit('update-posts', value);
            });
        }
      }

      apiFetch({ input, statusActions });
    }
  });

  function getHeader(post: Post): string {
    if (post.userHeader !== null) {
      let header = `<header><h3><a href='/users/${post.userHeader.handle}' rel='author'>`;

      if (post.userHeader.iconImage !== null) {
        header += `<img src='${post.userHeader.iconImage.uri}' alt='' aria-hidden='true' />`;
      }
      
      header += `<span>${post.userHeader.name}</span></a></h3><time datetime='${post.publishedAtNz}'>${formatLong(post.publishedAtNz)}</time></header>`;

      return header;
    } else if (post.spaceHeader !== null) {
      let header = `<header><h3><a href='/spaces/${post.spaceHeader.handle}' rel='author'>`;

      if (post.spaceHeader.iconImage !== null) {
        header += `<img src='${post.spaceHeader.iconImage.uri}' alt='' aria-hidden='true' />`;
      }
      
      header += `<span>${post.spaceHeader.name}</span></a></h3><time datetime='${post.publishedAtNz}'>${formatLong(post.publishedAtNz)}</time></header>`;

      return header;
    }

    return '';
  }
  
  function getPosts() {
    const input = 'posts';

    const statusActions: StatusActions = {
      200: (response?: Response) => {
        response?.json()
          .then((value: Post[]) => {
            if (value.length < 10) {
              hasMore.value = false;
            }

            emit('update-posts', value);
          });
      }
    }

    baseSearchParams.value = [];

    if (bookmarkFilter.value !== 'all') {
      baseSearchParams.value.push({ key: 'bookmarked', value: bookmarkFilter.value });
    }

    if (starredFilter.value !== 'all') {
      baseSearchParams.value.push({ key: 'starred', value: starredFilter.value });
    }

    if (dismissedFilter.value !== 'all') {
      baseSearchParams.value.push({ key: 'dismissed', value: dismissedFilter.value });
    }

    if (followedFilter.value !== 'all') {
      baseSearchParams.value.push({ key: 'followed', value: followedFilter.value });
    }

    const searchParameters = baseSearchParams.value;

    apiFetch({ input, statusActions, searchParameters });
  }

  function getMorePosts() {
    if (props.dataPosts !== null) {
      const input = 'posts';

      const statusActions: StatusActions = {
        200: (response?: Response) => {
          response?.json()
            .then((value: Post[]) => {
              if (value.length < 2) {
                hasMore.value = false;
              }

              emit('update-posts', [...(props.dataPosts ?? []), ...value]);
            });
        }
      }

      const lastPost = props.dataPosts[props.dataPosts.length - 1];

      const searchParameters = [...baseSearchParams.value];

      searchParameters.push({ key: 'lastPublishedAtNz', value: lastPost.publishedAtNz });
      searchParameters.push({ key: 'lastPostId', value: lastPost.id });

      apiFetch({ input, statusActions, searchParameters });
    }
  }

  function buttonAction(label: string, pressed: string | null, post: Post): void {
    const action = label.toLowerCase();

    if (pressed === 'true') {
      const statusActions: StatusActions = {
        204: () => {
          if (action === "bookmark") {
            post.hasBookmarked = null;
          } else if (action === "dismiss") {
            post.hasDismissed = null;
          } else if (action === "star") {
            post.hasStarred = null;
          }
        }
      }

      const init: RequestInit = { method: "DELETE" };

      if (post.spaceHeader !== null) {
        apiFetchAuthenticated({ input: `spaces/${post.spaceHeader.handle}/posts/${post.handle}/${action}`, statusActions, init });
      } else if (post.userHeader !== null) {
        apiFetchAuthenticated({ input: `users/${post.userHeader.handle}/posts/${post.handle}/${action}`, statusActions, init });
      }
    } else {
      const statusActions: StatusActions = {
        201: () => {
          if (action === "bookmark") {
            post.hasBookmarked = true;
          } else if (action === "dismiss") {
            post.hasDismissed = true;
          } else if (action === "star") {
            post.hasStarred = true;
          }
        }
      }

      const init: RequestInit = { method: "POST" };

      if (post.spaceHeader !== null) {
        apiFetchAuthenticated({ input: `spaces/${post.spaceHeader.handle}/posts/${post.handle}/${action}`, statusActions, init });
      } else if (post.userHeader !== null) {
        apiFetchAuthenticated({ input: `users/${post.userHeader.handle}/posts/${post.handle}/${action}`, statusActions, init });
      }
    }
  }

  function postHtml(post: Post): string {
    return `
      ${getHeader(post)}
      ${post.article.substring(9, post.article.length - 10)}
      ${store.isAuthenticated ? `<footer>
        <menu>
          <li>
            <button type='button' aria-label='Dismiss' aria-pressed='${post.hasDismissed ? 'true' : 'false'}'>${post.hasDismissed ? '<svg width="1em" height="1em" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true" focusable="false"><path d="M17.94 17.94A10.07 10.07 0 0 1 12 20c-7 0-11-8-11-8a18.45 18.45 0 0 1 5.06-5.94M9.9 4.24A9.12 9.12 0 0 1 12 4c7 0 11 8 11 8a18.5 18.5 0 0 1-2.16 3.19m-6.72-1.07a3 3 0 1 1-4.24-4.24" /><line x1="1" y1="1" x2="23" y2="23" /></svg>' : '<svg width="1em" height="1em" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true" focusable="false"><path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z" /><circle cx="12" cy="12" r="3" /></svg>'}</button>
          </li>
          <li>
            <button type='button' aria-label='Star' aria-pressed='${post.hasStarred ? 'true' : 'false'}'>${post.hasStarred ? '<svg width="1em" height="1em" viewBox="0 0 24 24" fill="currentColor" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true" focusable="false"><polygon points="12 2 15.09 8.26 22 9.27 17 14.14 18.18 21.02 12 17.77 5.82 21.02 7 14.14 2 9.27 8.91 8.26 12 2"/></svg>' : '<svg width="1em" height="1em" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true" focusable="false"><polygon points="12 2 15.09 8.26 22 9.27 17 14.14 18.18 21.02 12 17.77 5.82 21.02 7 14.14 2 9.27 8.91 8.26 12 2"/></svg>'}</button>
          </li>
          <li>
            <button type='button' aria-label='Bookmark' aria-pressed='${post.hasBookmarked ? 'true' : 'false'}'>${post.hasBookmarked ? '<svg width="1em" height="1em" viewBox="0 0 24 24" fill="currentColor" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true" focusable="false"><path d="M19 21l-7-5-7 5V5a2 2 0 0 1 2-2h10a2 2 0 0 1 2 2z" /></svg>' : '<svg width="1em" height="1em" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true" focusable="false"><path d="M19 21l-7-5-7 5V5a2 2 0 0 1 2-2h10a2 2 0 0 1 2 2z" /></svg>'}</button>
          </li>
        </menu>
      </footer>` : ''}
    `;
  }
</script>

<template>
  <section id="panel-posts" role="tabpanel" aria-labelledby="tab-posts">
    <header>
      <h2>Posts</h2>
    </header>

    <search v-if="store.isAuthenticated">
      <details>
        <summary>Filter Options</summary>
        
        <form @submit.prevent="getPosts" action="/api/posts" method="GET" novalidate>
          <details>
            <summary>Starred Posts</summary>

            <RadioFieldset
              v-model="starredFilter"
              :radios="[
                { name: 'starred', value: 'all', id: 'starredAll', label: 'No filter' },
                { name: 'starred', value: 'only', id: 'starredOnly', label: 'Show only starred' },
                { name: 'starred', value: 'hide', id: 'starredHide', label: 'Hide starred' }
              ]"
              legend="Starred Posts Options" />
          </details>

          <details>
            <summary>Bookmarked Posts</summary>

            <RadioFieldset
              v-model="bookmarkFilter"
              :radios="[
                { name: 'bookmarked', value: 'all', id: 'bookmarkedAll', label: 'No filter' },
                { name: 'bookmarked', value: 'only', id: 'bookmarkedOnly', label: 'Show only bookmarked' },
                { name: 'bookmarked', value: 'hide', id: 'bookmarkedHide', label: 'Hide bookmarked' }
              ]"
              legend="Bookmarked Posts Options" />
          </details>

          <details>
            <summary>Dismissed Posts</summary>

            <RadioFieldset
              v-model="dismissedFilter"
              :radios="[
                { name: 'dismissed', value: 'all', id: 'dismissedAll', label: 'No filter' },
                { name: 'dismissed', value: 'only', id: 'dismissedOnly', label: 'Show only dismissed' },
                { name: 'dismissed', value: 'hide', id: 'dismissedHide', label: 'Hide dismissed' }
              ]"
              legend="Dismissed Posts Options" />
          </details>

          <details>
            <summary>Followed Users</summary>

            <RadioFieldset
              v-model="followedFilter"
              :radios="[
                { name: 'followed', value: 'all', id: 'followedAll', label: 'No filter' },
                { name: 'followed', value: 'only', id: 'followedOnly', label: 'Show only followed' },
                { name: 'followed', value: 'hide', id: 'followedHide', label: 'Hide followed' }
              ]"
              legend="Followed Users Options" />
          </details>

          <button type="submit">Apply Filters</button>
        </form>
      </details>
    </search>

    <p role="status" aria-live="polite" class="visuallyhidden">
      Showing {{ dataPosts?.length }} post{{ dataPosts?.length == 1 ? '' : 's' }}. TODO, move this status element to parent.
    </p>

    <ul role="list" v-if="dataPosts !== null">
      <li v-for="post in dataPosts" :key="'post' + post.id">
        <article v-html="postHtml(post)" @click.prevent="(mouseEvent) => handleClick(mouseEvent, router, (label, pressed) => buttonAction(label, pressed, post))">
        </article>
      </li>
    </ul>

    <p v-else>We do not have any posts.</p>

    <footer>
      <button type="button" aria-controls="panel-posts" v-if="dataPosts !== null && hasMore" @click="() => getMorePosts()">More</button>
    </footer>
  </section>
</template>
