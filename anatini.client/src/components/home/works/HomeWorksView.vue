<script setup lang="ts">
  import { apiFetch } from '@/common/apiFetch';
  import { buttonAction, getWorkHtml } from '@/common/html';
  import { store } from '@/common/store';
  import type { Filters, StatusActions, Work } from '@/common/types';
  import { handleClick } from '@/common/utils';
  import { onMounted, ref } from 'vue';
  import { useRouter } from 'vue-router';
  import FilterRadios from '@/common/FilterRadios.vue';

  const router = useRouter();

  const props = defineProps<{
    dataWorks: Work[] | null,
  }>();

  const emit = defineEmits<{
    'update-works': [newPosts: Work[]],
  }>();

  const filters = ref<Filters>({ bookmarked: 'all', starred: 'all', dismissed: 'all', followed: 'all', baseSearchParams: [] });
  const hasMore = ref<boolean>(true);

  onMounted(() => {
    if (props.dataWorks === null) {
      const input = 'works';

      const statusActions: StatusActions = {
        200: (response?: Response) => {
          response?.json()
            .then((value: Work[]) => {
              emit('update-works', value);
            });
        }
      }

      apiFetch({ input, statusActions });
    }
  });

  
  function getWorks() {
    const input = 'works';

    const statusActions: StatusActions = {
      200: (response?: Response) => {
        response?.json()
          .then((value: Work[]) => {
            if (value.length < 10) {
              hasMore.value = false;
            }

            emit('update-works', value);
          });
      }
    }

    filters.value.baseSearchParams = [];

    if (filters.value.bookmarked !== 'all') {
      filters.value.baseSearchParams.push({ key: 'bookmarked', value: filters.value.bookmarked });
    }

    if (filters.value.starred !== 'all') {
      filters.value.baseSearchParams.push({ key: 'starred', value: filters.value.starred });
    }

    if (filters.value.dismissed !== 'all') {
      filters.value.baseSearchParams.push({ key: 'dismissed', value: filters.value.dismissed });
    }

    if (filters.value.followed !== 'all') {
      filters.value.baseSearchParams.push({ key: 'followed', value: filters.value.followed });
    }

    const searchParameters = filters.value.baseSearchParams;

    apiFetch({ input, statusActions, searchParameters });
  }

  function getMoreWorks() {
    if (props.dataWorks !== null) {
      const input = 'works';

      const statusActions: StatusActions = {
        200: (response?: Response) => {
          response?.json()
            .then((value: Work[]) => {
              if (value.length < 2) {
                hasMore.value = false;
              }

              emit('update-works', [...(props.dataWorks ?? []), ...value]);
            });
        }
      }

      const lastWork = props.dataWorks[props.dataWorks.length - 1];

      const searchParameters = [...filters.value.baseSearchParams];

      searchParameters.push({ key: 'lastName', value: lastWork.name });
      searchParameters.push({ key: 'lastWorkId', value: lastWork.id });

      apiFetch({ input, statusActions, searchParameters });
    }
  }
</script>

<template>
  <section id="panel-works" role="tabpanel" aria-labelledby="tab-works">
    <header>
      <h2>Posts</h2>
      <RouterLink v-if="store.isAuthenticated" :to="{ name: 'UserWorkCreate', params: { userId: store.userHandle } }">+ Create Work</RouterLink>
    </header>

    <search v-if="store.isAuthenticated">
      <details>
        <summary>Filter Options</summary>

        <form @submit.prevent="getWorks" action="/api/posts" method="GET" novalidate>
          <FilterRadios v-model="filters" />

          <button type="submit">Apply Filters</button>
          <button type="reset">Clear Filters</button>
        </form>
      </details>
    </search>

    <ul role="list" v-if="dataWorks !== null">
      <li v-for="work in dataWorks" :key="'work' + work.id">
        <article v-html="getWorkHtml(work)" @click.prevent="(mouseEvent) => handleClick(mouseEvent, router, (label, pressed) => buttonAction(label, pressed, work))">
        </article>
      </li>
    </ul>

    <p v-else>There are not any works</p>

    <footer>
      <button type="button" aria-controls="panel-works" v-if="dataWorks !== null && hasMore" @click="() => getMoreWorks()">More</button>
    </footer>
  </section>
</template>
