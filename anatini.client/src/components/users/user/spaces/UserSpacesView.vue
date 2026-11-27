<script setup lang="ts">
  import type { Space, StatusActions } from '@/common/types';
  import { onMounted } from 'vue';
  import { apiFetchAuthenticated } from '@/common/apiFetch';
  import { ContentBody } from '@/common/utils';

  const props = defineProps<{
    dataUserId: string,
    dataUserHandle: string,
    dataSpaces: Space[] | null,
  }>();

  const emit = defineEmits<{
    'update-spaces': [newSpaces: Space[]],
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
      <li v-for="space in dataSpaces" :key="'space' + space.id">
        <article>
          <header>
            <h3>
              <RouterLink :to="{ name: 'Space', params: { spaceId: space.handle } }">{{ space.name }}</RouterLink>
            </h3>
          </header>

          <ContentBody v-if="space.about !== null" :html="space.about" />
        </article>
      </li>
    </ul>

    <p v-else>You do not have any spaces</p>
  </section>
</template>
