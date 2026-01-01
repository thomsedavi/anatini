<script setup lang="ts">
  import type { UserEdit } from '@/types';
  import { onMounted, ref } from 'vue';
  import { useRouter } from 'vue-router';
  import { apiFetchAuthenticated } from './common/apiFetch';

  const router = useRouter();
  const fetchingUser = ref<boolean>(true);
  const user = ref<UserEdit | null>(null);

  onMounted(() => {
    const onfulfilled = async (value: UserEdit) => {
      user.value = value;
    };

    const onfinally = () => {
      fetchingUser.value = false
    };

    const statusActions = {
      401: () => {
        router.replace({ path: '/login', query: { redirect: '/account' } });
      }
    };

    apiFetchAuthenticated('account', onfulfilled, onfinally, undefined, statusActions);
  });

  function getHeading(): string {
    if (fetchingUser.value) {
      return 'Fetching...';
    } if (user.value) {
      return 'Account Settings';
    } else {
      return 'Unknown Error';
    }
  }
</script>

<template>
  <main>
    <article :aria-busy="fetchingUser" aria-live="polite" aria-labelledby="heading">
      <header>
        <h1 id="heading">{{ getHeading() }}</h1>
      </header>
    </article>
  </main>
</template>
