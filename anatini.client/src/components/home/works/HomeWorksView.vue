<script setup lang="ts">
  import { apiFetch } from '@/common/apiFetch';
  import { buttonAction, getWorkHtml } from '@/common/html';
  import { store } from '@/common/store';
  import type { SearchParameter, StatusActions, Work } from '@/common/types';
  import { handleClick } from '@/common/utils';
  import { onMounted, ref } from 'vue';
  import { useRouter } from 'vue-router';
  import RadioFieldset from '@/common/RadioFieldset.vue';

  const router = useRouter();

  const props = defineProps<{
    dataWorks: Work[] | null,
  }>();

  const emit = defineEmits<{
    'update-works': [newPosts: Work[]],
  }>();

  const bookmarkFilter = ref<string>('all');
  const starredFilter = ref<string>('all');
  const dismissedFilter = ref<string>('all');
  const followedFilter = ref<string>('all');
  const baseSearchParams = ref<SearchParameter[]>([]);
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

      const searchParameters = [...baseSearchParams.value];

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
