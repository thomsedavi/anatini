<script setup lang="ts">
  import { apiFetchAuthenticated } from '@/common/apiFetch';
  import type { StatusActions, Work } from '@/common/types';
  import { onMounted } from 'vue';

  const props = defineProps<{
    userId: string,
    userHandle: string,
    works: Work[] | null,
  }>();

  const emit = defineEmits<{
    'update-works': [newWorks: Work[]],
  }>();

  onMounted(() => {
    if (props.works === null) {
      const input = `users/${props.userId}/works`;

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
      <RouterLink :to="{ name: 'UserWebsiteCreate' }">+ Create Website</RouterLink>
    </header>

    <ul role="list" v-if="works !== null">
      <li v-for="work in works" :key="'work' + work.id">
        <article>
          <h1>{{ work.name }}</h1>
        </article>
      </li>
    </ul>

    <p v-else>You do not have any posts</p>
  </section>
</template>
