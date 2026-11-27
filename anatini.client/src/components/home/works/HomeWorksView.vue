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

    currentSearchParameters.value = getSearchParameters(filters.value);

    apiFetch({ input, statusActions, searchParameters: currentSearchParameters.value });
  }

  function getMoreWorks() {
    const lastWork = props.dataWorks?.[props.dataWorks.length - 1];

    if (lastWork !== undefined) {
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

      const searchParameters = [...currentSearchParameters.value];

      searchParameters.push({ key: 'lastTitle', value: lastWork.title });
      searchParameters.push({ key: 'lastWorkId', value: lastWork.id });

      apiFetch({ input, statusActions, searchParameters });
    }
  }
</script>

<template>
  <section id="panel-works" role="tabpanel" aria-labelledby="tab-works">
    <header>
      <h2>Work</h2>

      <template v-if="store.account.isAuthenticated">
        <nav v-if="(store.account.spaces ?? []).length > 0" aria-label="Work creation options">
          <button popovertarget="create-work-menu">
            + Create Work
          </button>
          <ul popover="auto" id="create-work-menu">
            <li>
              <RouterLink :to="{ name: 'UserWorkCreate', params: { userId: store.account.userHandle } }">Personal Work</RouterLink>
            </li>
            <li v-for="space in store.account.spaces ?? []" :key="'space' + space.id">
              <RouterLink :to="{ name: 'SpaceWorkCreate', params: { spaceId: space.handle } }">{{space.name}} Work</RouterLink>
            </li>
          </ul>
        </nav>
        <RouterLink v-else :to="{ name: 'UserWorkCreate', params: { userId: store.account.userHandle } }">+ Create Work</RouterLink>
      </template>
    </header>

    <search role="search" v-if="store.account.isAuthenticated">
      <details>
        <summary>Filter Options</summary>

        <form @submit.prevent="getWorks" action="/api/works" method="GET" novalidate>
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

    <p v-else>There are not any works</p>

    <footer>
      <button type="button" aria-controls="panel-works" v-if="dataWorks !== null && hasMore" @click="() => getMoreWorks()">More</button>
    </footer>
  </section>
</template>
