<script setup lang="ts">
  import { apiFetch } from '@/common/apiFetch';
  import { buttonAction, getWorkHtml } from '@/common/html';
  import { store } from '@/common/store';
  import type { StatusActions, Work } from '@/common/types';
  import { handleClick } from '@/common/utils';
  import { onMounted } from 'vue';
  import { useRouter } from 'vue-router';

  const router = useRouter();

  const props = defineProps<{
    dataWorks: Work[] | null,
  }>();

  const emit = defineEmits<{
    'update-works': [newPosts: Work[]],
  }>();

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
</script>

<template>
  <section id="panel-works" role="tabpanel" aria-labelledby="tab-works">
    <header>
      <h2>Posts</h2>
      <RouterLink v-if="store.isAuthenticated" :to="{ name: 'UserWorkCreate', params: { userId: store.userHandle } }">+ Create Work</RouterLink>
    </header>

    <ul role="list" v-if="dataWorks !== null">
      <li v-for="work in dataWorks" :key="'work' + work.id">
        <article v-html="getWorkHtml(work)" @click.prevent="(mouseEvent) => handleClick(mouseEvent, router, (label, pressed) => buttonAction(label, pressed, work))">
        </article>
      </li>
    </ul>

    <p v-else>There are not any works</p>
  </section>
</template>
