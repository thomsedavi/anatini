<script setup lang="ts">
  import { onMounted, ref } from 'vue';
  import { useRouter } from 'vue-router';
  import { apiFetch } from '@/common/apiFetch';
  import { defaultFilters, getSearchParameters, type Filters } from '@/common/filters';
  import FilterRadios from '@/common/FilterRadios.vue';
  import { buttonAction, getPostHtml } from '@/common/html';
  import { store } from '@/common/store';
  import type { Post, SearchParameter, StatusActions } from '@/common/types';
  import { getPostAddress, handleClick } from '@/common/utils';

  const router = useRouter();

  const props = defineProps<{
    dataSpaceId: string,
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
      const input = `spaces/${props.dataSpaceId}/posts`;

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
    const input = `spaces/${props.dataSpaceId}/posts`;

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
</script>

<template>
  <section id="panel-posts" role="tabpanel" aria-labelledby="tab-posts">
    <header>
      <h2>Posts</h2>
      <RouterLink :to="{ name: 'SpacePostCreate' }">+ Create Post</RouterLink>
    </header>

    <search role="search" v-if="store.isAuthenticated">
      <details>
        <summary>Filter Options</summary>

        <form @submit.prevent="getPosts" :action="`/api/spaces/${dataSpaceId}/posts`" method="GET" novalidate>
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

    <p v-else>You do not have any posts</p>
  </section>
</template>
