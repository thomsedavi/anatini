<script setup lang="ts">
  import { ref, watch } from 'vue';
  import { useRoute, useRouter } from 'vue-router';
  import { apiFetch } from '@/common/apiFetch';
  import { contentButtonAction, getWorkHtml } from '@/common/html';
  import type { APIResponse, StatusActions, Work } from '@/common/types';
  import { getContentStatus, getWorkAddress, handleClick, parseParams, type Params } from '@/common/utils';

  const route = useRoute();
  const router = useRouter();

  defineProps<{
    dataSpaceId: string,
    dataSpaceHandle: string,
    dataSpaceName: string,
  }>();

  const work = ref<APIResponse<Work>>({ fetching: true });

  watch([() => route.params.spaceId, () => route.params.workId], (params: Params) => fetchWork(parseParams(params)), { immediate: true });

  async function fetchWork(params: string[]) {
    const input = `spaces/${params[0]}/works/${params[1]}`;

    const statusActions: StatusActions = {
      200: (response?: Response) => {
        response?.json()
          .then((value: Work) => {
            work.value = { data: value };
          })
          .catch(() => {
            work.value = { error: { status: 'unknown', heading: 'Unknown Error', body: 'There was a problem fetching this work' }};
          });
      },
      401: () => {
        work.value = { error: { status: '401', heading: '401 Unauthorised', body: 'You do not have access to this work' }};
      },
      403: () => {
        work.value = { error: { status: '403', heading: '403 Forbidden', body: 'You do not have access to this work' }};
      },
      404: () => {
        work.value = { error: { status: '404', heading: '404 Not Found', body: 'Work not found' }};
      },
      500: () => {
        work.value = { error: { status: '500', heading: '500 Internal Server Error', body: 'There was a problem fetching this work' }};
      }
    }

    apiFetch({ input, statusActions });
  }
</script>

<template>
  <section id="panel-works" role="tabpanel" aria-labelledby="tab-works" tabindex="0">
    <nav aria-label="Breadcrumb">
      <ol role="list">
        <li>
          <RouterLink :to="{ name: 'SpaceWorks', params: { spaceId: dataSpaceHandle } }"><span aria-hidden="true">&larr;</span> Back to all works by {{ dataSpaceName }}</RouterLink>
        </li>
      </ol>
    </nav>

    <article v-if="work.data !== undefined" :data-status="getContentStatus(work.data.status, work.data.isScheduled)" v-html="getWorkHtml(work.data, false, 'h2', false)" @click.prevent="(mouseEvent) => handleClick(mouseEvent, router, (label, pressed) => contentButtonAction(label, pressed, work.data!, getWorkAddress(work.data!, 'id')))">
    </article>

    <article v-if="work.error !== undefined" :data-status="work.error.status">
      <h2>{{ work.error.heading }}</h2>
      <p>{{ work.error.body }}</p>
    </article>
  </section>
</template>
