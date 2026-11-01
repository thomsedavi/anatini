<script setup lang="ts">
  import { onMounted } from 'vue';
  import { useRouter } from 'vue-router';
  import { apiFetchAuthenticated } from '@/common/apiFetch';
  import { getWorkHtml } from '@/common/html';
  import type { StatusActions, Work } from '@/common/types';
  import { handleClick } from '@/common/utils';

  const router = useRouter();

  const props = defineProps<{
    dataUserId: string,
    dataWorks: Work[] | null,
  }>();

  const emit = defineEmits<{
    'update-works': [newWorks: Work[]],
  }>();

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

      apiFetchAuthenticated({ input, statusActions });
    }
  });
</script>

<template>
  <section id="panel-works" role="tabpanel" aria-labelledby="tab-works">
    <header>
      <h2>Works</h2>
      <RouterLink :to="{ name: 'UserWorkCreate' }">+ Create Work</RouterLink>
    </header>

    <ul role="list" v-if="dataWorks !== null">
      <li v-for="work in dataWorks" :key="'work' + work.id">
        <article v-html="getWorkHtml(work, false)" @click.prevent="(mouseEvent) => handleClick(mouseEvent, router)">
        </article>
      </li>
    </ul>

    <p v-else>You do not have any works</p>
  </section>
</template>
