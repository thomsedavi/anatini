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
    dataUserId: string,
    dataUserHandle: string,
    dataUserName: string,
  }>();

  const workResponse = ref<APIResponse<Work>>({ fetching: true });

  watch([() => route.params.userId, () => route.params.workId], (params: Params) => fetchWork(parseParams(params)), { immediate: true });

  async function fetchWork(params: string[]) {
    const input = `users/${params[0]}/works/${params[1]}`;

    const statusActions: StatusActions = {
      200: (response?: Response) => {
        response?.json()
          .then((value: Work) => {
            workResponse.value = { data: value };
          })
          .catch(() => {
            workResponse.value = { error: { status: 'unknown', heading: 'Unknown Error', body: 'There was a problem fetching this work' }};
          });
      },
      401: () => {
        workResponse.value = { error: { status: '401', heading: '401 Unauthorised', body: 'You do not have access to this work' }};
      },
      403: () => {
        workResponse.value = { error: { status: '403', heading: '403 Forbidden', body: 'You do not have access to this work' }};
      },
      404: () => {
        workResponse.value = { error: { status: '404', heading: '404 Not Found', body: 'Work not found' }};
      },
      500: () => {
        workResponse.value = { error: { status: '500', heading: '500 Internal Server Error', body: 'There was a problem fetching this work' }};
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
          <RouterLink :to="{ name: 'UserWorks', params: { userId: dataUserHandle } }"><span aria-hidden="true">&larr;</span> Back to all works by {{ dataUserName }}</RouterLink>
        </li>
      </ol>
    </nav>

    <article v-if="workResponse.data !== undefined" :data-status="getContentStatus(workResponse.data.status, workResponse.data.isScheduled)" v-html="getWorkHtml(workResponse.data, false, 'h2', false)" @click.prevent="(mouseEvent) => handleClick(mouseEvent, router, (label, entityType, pressed) => contentButtonAction(label, pressed, workResponse.data!, getWorkAddress(workResponse.data!, entityType, 'id')))">
    </article>

    <article v-if="workResponse.error !== undefined" :data-status="workResponse.error.status">
      <h2>{{ workResponse.error.heading }}</h2>
      <p>{{ workResponse.error.body }}</p>
    </article>
  </section>
</template>
