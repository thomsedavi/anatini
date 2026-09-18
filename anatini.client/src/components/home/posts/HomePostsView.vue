<script setup lang="ts">
  import { apiFetch } from '@/common/apiFetch';
  import { buttonAction, getPostHtml } from '@/common/html';
  import { store } from '@/common/store';
  import type { Post, SearchParameter, StatusActions } from '@/common/types';
  import { handleClick } from '@/common/utils';
  import { onMounted, ref } from 'vue';
  import { useRouter } from 'vue-router';
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
</script>

<template>
  <section id="panel-posts" role="tabpanel" aria-labelledby="tab-posts">
    <header>
      <h2>Posts</h2>
      <RouterLink v-if="store.isAuthenticated" :to="{ name: 'UserPostCreate', params: { userId: store.userHandle } }">+ Create Post</RouterLink>
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
          <button type="reset">Clear Filters</button>
        </form>
      </details>
    </search>

    <ul role="list" v-if="dataPosts !== null">
      <li v-for="post in dataPosts" :key="'post' + post.id">
        <article v-html="getPostHtml(post)" @click.prevent="(mouseEvent) => handleClick(mouseEvent, router, (label, pressed) => buttonAction(label, pressed, post))">
        </article>
      </li>
    </ul>

    <p v-else>There are not any posts</p>

    <footer>
      <button type="button" aria-controls="panel-posts" v-if="dataPosts !== null && hasMore" @click="() => getMorePosts()">More</button>
    </footer>
  </section>
</template>
