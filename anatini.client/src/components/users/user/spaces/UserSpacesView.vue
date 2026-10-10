<script setup lang="ts">
  import type { Space, StatusActions } from '@/common/types';
  import { onMounted } from 'vue';
  import { apiFetchAuthenticated } from '@/common/apiFetch';
  import { useRouter } from 'vue-router';
  import { handleClick } from '@/common/utils';
  import { getSpaceHtml } from '@/common/html';

  const router = useRouter();

  const props = defineProps<{
    dataUserId: string,
    dataUserHandle: string,
    dataSpaces: Space[] | null,
  }>();

  const emit = defineEmits<{
    'update-spaces': [newPosts: Space[]],
  }>();

  onMounted(() => {
    if (props.dataSpaces === null) {
      const input = `users/${props.dataUserId}/spaces`;

      const statusActions: StatusActions = {
        200: (response?: Response) => {
          response?.json()
            .then((value: Space[]) => {
              emit('update-spaces', value);
            });
        }
      }

      apiFetchAuthenticated({ input, statusActions });
    }
  });
</script>

<template>
  <section id="panel-spaces" role="tabpanel" aria-labelledby="tab-spaces">
    <header>
      <h2>Spaces</h2>
      <RouterLink :to="{ name: 'UserSpaceCreate' }">+ Create Space</RouterLink>
    </header>

    <ul role="list" v-if="dataSpaces !== null">
      <li v-for="space in dataSpaces" :key="'post' + space.id">
        <article v-html="getSpaceHtml(space)" @click.prevent="(mouseEvent) => handleClick(mouseEvent, router)">
        </article>
      </li>
    </ul>

    <p v-else>You do not have any posts</p>
  </section>
</template>
