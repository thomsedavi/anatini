<script setup lang="ts">
  import { apiFetchAuthenticated } from '@/common/apiFetch';
  import { getWorkHtml } from '@/common/html';
  import type { StatusActions, Work } from '@/common/types';
  import { onMounted } from 'vue';

  const props = defineProps<{
    dataSpaceId: string,
    dataSpaceHandle: string,
    dataWorks: Work[] | null,
  }>();

  const emit = defineEmits<{
    'update-works': [newWorks: Work[]],
  }>();

  onMounted(() => {
    if (props.dataWorks === null) {
      const input = `spaces/${props.dataSpaceId}/works`;

      const statusActions: StatusActions = {
        200: (response?: Response) => {
          response?.json()
            .then((value: Work[]) => {
              emit('update-works', value);
            });
        }
      }

      apiFetchAuthenticated({ input, statusActions });
    }
  });
</script>

<template>
  <section id="panel-works" role="tabpanel" aria-labelledby="tab-works">
    <header>
      <h2>Works</h2>
      <RouterLink :to="{ name: 'SpaceWorkCreate' }">+ Create Work</RouterLink>
    </header>

    <ul role="list" v-if="dataWorks !== null">
      <li v-for="work in dataWorks" :key="'work' + work.id">
        <article v-html="getWorkHtml(work)">
        </article>
      </li>
    </ul>

    <p v-else>You do not have any posts</p>
  </section>
</template>
