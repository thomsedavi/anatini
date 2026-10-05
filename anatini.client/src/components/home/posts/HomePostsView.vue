<script setup lang="ts">
  import { apiFetch } from '@/common/apiFetch';
  import { buttonAction, getPostHtml } from '@/common/html';
  import { store } from '@/common/store';
  import type { Post, StatusActions } from '@/common/types';
  import { handleClick } from '@/common/utils';
  import { onMounted, ref } from 'vue';
  import { useRouter } from 'vue-router';
  import FilterRadios from '@/common/FilterRadios.vue';
  import { Filters } from '@/common/classes';

  const router = useRouter();

  const props = defineProps<{
    dataPosts: Post[] | null,
  }>();

  const emit = defineEmits<{
    'update-posts': [newPosts: Post[]],
  }>();

  const filters = ref<Filters>(new Filters());
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

    const searchParameters = filters.value.getSearchParameters();

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

      const searchParameters = [...filters.value.currentSearchParameters];

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
          <FilterRadios v-model="filters" />

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
