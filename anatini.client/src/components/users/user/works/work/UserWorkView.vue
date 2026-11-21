<script setup lang="ts">
  import { ref, watch } from 'vue';
  import { useRoute, useRouter } from 'vue-router';
  import { apiFetch } from '@/common/apiFetch';
  import { contentButtonAction, getWorkHtml } from '@/common/html';
  import type { APIResponse, StatusActions, Work } from '@/common/types';
  import { getWorkAddress, handleClick, parseParams, type Params } from '@/common/utils';

  const route = useRoute();
  const router = useRouter();

  defineProps<{
    dataUserId: string,
    dataUserHandle: string,
    dataUserName: string,
  }>();

  const work = ref<APIResponse<Work>>({ fetching: true });

  watch([() => route.params.userId, () => route.params.workId], (params: Params) => fetchWork(parseParams(params)), { immediate: true });

  async function fetchWork(params: string[]) {
    const input = `users/${params[0]}/works/${params[1]}`;

    const statusActions: StatusActions = {
      200: (response?: Response) => {
        response?.json()
          .then((value: Work) => {
            work.value = { data: value };
          })
          .catch(() => {
            work.value = { error: { status: 'unknown', heading: 'Unknown Error', body: 'There was a problem fetching your work, please reload the page' }};
          });
      },
      404: () => {
        work.value = { error: { status: '404', heading: '404 Not Found', body: 'Work not found' }};
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

    <article v-if="work.data !== undefined" :data-status="work.data.status.toLowerCase()" v-html="getWorkHtml(work.data, false, 'h2', false)" @click.prevent="(mouseEvent) => handleClick(mouseEvent, router, (label, pressed) => contentButtonAction(label, pressed, work.data!, getWorkAddress(work.data!, 'id')))">
    </article>
  </section>
</template>
