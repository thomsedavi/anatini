<script setup lang="ts">
  import { apiFetch } from '@/common/apiFetch';
  import { buttonAction, getPostHtml } from '@/common/html';
  import { store } from '@/common/store';
  import { defaultFilters, getSearchParameters, type Filters } from '@/common/filters';
  import type { Post, SearchParameter, StatusActions } from '@/common/types';
  import { getPostAddress, handleClick } from '@/common/utils';
  import { onMounted, ref } from 'vue';
  import { useRouter } from 'vue-router';
  import FilterRadios from '@/common/FilterRadios.vue';

  const router = useRouter();

  const props = defineProps<{
    dataPosts: Post[] | null,
  }>();

  const emit = defineEmits<{
    'update-posts': [newPosts: Post[]],
  }>();

  const filters = ref<Filters>({ ...defaultFilters });
  const currentSearchParameters = ref<SearchParameter[]>([]);
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

    currentSearchParameters.value = getSearchParameters(filters.value);

    apiFetch({ input, statusActions, searchParameters: currentSearchParameters.value });
  }

  function getMorePosts() {
    const lastPost = props.dataPosts?.[props.dataPosts.length - 1];

    if (lastPost !== undefined) {
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

      const searchParameters = [...currentSearchParameters.value];

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

      <template v-if="store.isAuthenticated">
        <nav v-if="(store.spaces ?? []).length > 0" aria-label="Post creation options">
          <button popovertarget="create-post-menu">
            + Create Post
          </button>
          <ul popover="auto" id="create-post-menu">
            <li>
              <RouterLink :to="{ name: 'UserPostCreate', params: { userId: store.userHandle } }">Personal Post</RouterLink>
            </li>
            <li v-for="space in store.spaces ?? []" :key="'space' + space.id">
              <RouterLink :to="{ name: 'SpacePostCreate', params: { spaceId: space.handle } }">{{space.name}} Post</RouterLink>
            </li>
          </ul>
        </nav>
        <RouterLink v-else :to="{ name: 'UserPostCreate', params: { userId: store.userHandle } }">+ Create Post</RouterLink>
      </template>
    </header>

    <search role="search" v-if="store.isAuthenticated">
      <details>
        <summary>Filter Options</summary>

        <form @submit.prevent="getPosts" action="/api/posts" method="GET" novalidate>
          <FilterRadios v-model="filters" />

          <button type="submit">Apply Filters</button>
          <button type="reset">Clear Filters</button>
        </form>
      </details>
    </search>

    <ul role="list" v-if="dataPosts !== null">
      <li v-for="post in dataPosts" :key="'post' + post.id">
        <article v-html="getPostHtml(post)" @click.prevent="(mouseEvent) => handleClick(mouseEvent, router, (label, pressed) => buttonAction(label, pressed, post, getPostAddress(post, 'id')))">
        </article>
      </li>
    </ul>

    <p v-else>There are not any posts</p>

    <footer>
      <button type="button" aria-controls="panel-posts" v-if="dataPosts !== null && hasMore" @click="() => getMorePosts()">More</button>
    </footer>
  </section>
</template>
