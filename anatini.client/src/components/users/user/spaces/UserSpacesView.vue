<script setup lang="ts">
  import { apiFetchAuthenticated } from '@/common/apiFetch';
  import { onMounted } from 'vue';
  import type { Space, StatusActions } from '@/common/types';

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
  </section>
</template>
