<script setup lang="ts">
  import { onMounted, ref } from 'vue';
  import { apiFetch } from '@/common/apiFetch';
  import ContentArticle from '@/common/ContentArticle.vue';
  import { defaultFilters, getSearchParameters, type Filters } from '@/common/filters';
  import FilterRadios from '@/common/FilterRadios.vue';
  import { store } from '@/common/store';
  import type { SearchParameter, StatusActions, Work } from '@/common/types';
  import { updateContent } from '@/common/utils';

  const props = defineProps<{
    dataUserId: string,
    dataWorks: Work[] | null,
  }>();

  const emit = defineEmits<{
    'update-works': [newWorks: Work[]],
  }>();

  const filters = ref<Filters>({ ...defaultFilters });
  const currentSearchParameters = ref<SearchParameter[]>([]);
  const hasMore = ref<boolean>(true);

  onMounted(() => {
    if (props.dataWorks === null) {
      const input = `users/${props.dataUserId}/works`;

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
    const input = `users/${props.dataUserId}/works`;

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

    currentSearchParameters.value = getSearchParameters(filters.value);

    apiFetch({ input, statusActions, searchParameters: currentSearchParameters.value });
  }
</script>

<template>
  <section id="panel-works" role="tabpanel" aria-labelledby="tab-works">
    <header>
      <h2>Works</h2>
      <RouterLink :to="{ name: 'UserWorkCreate' }">+ Create Work</RouterLink>
    </header>

    <search role="search" v-if="store.account.isAuthenticated">
      <details>
        <summary>Filter Options</summary>

        <form @submit.prevent="getWorks" :action="`/api/users/${dataUserId}/works`" method="GET" novalidate>
          <FilterRadios v-model="filters" content-type="Work" />

          <button type="submit">Apply Filters</button>
          <button type="reset">Clear Filters</button>
        </form>
      </details>
    </search>

    <ul role="list" v-if="dataWorks !== null">
      <li v-for="(work, index) in dataWorks" :key="'work' + work.id">
        <ContentArticle :content="work" content-type="Work" :index="index" :is-main="false" @update-content="(newWork) => updateContent(work, newWork)" />
      </li>
    </ul>

    <p v-else>You do not have any works</p>
  </section>
</template>
