<script setup lang="ts">
  import type { User } from '@/types';
  import { ref, watch } from 'vue';
  import { useRoute } from 'vue-router';
  import { apiFetch } from './common/apiFetch';

  const route = useRoute();

  const loading = ref<boolean>(true);
  const error = ref<string | null>(null);
  const user = ref<User | null>(null);

  watch([() => route.params.userId], fetchUser, { immediate: true });

  async function fetchUser(array: (() => string | string[])[]) {
    error.value = user.value = null;
    loading.value = true;

    const onfulfilled = async (value: User) => {
      user.value = value;
    };

    const onfinally = () => {
      loading.value = false;
    };

    apiFetch(`users/${array[0]}`, onfulfilled, onfinally);
  }

  async function trust() {
    if (user.value === null) {
      return;
    }

    const onfulfilled = async (value: string) => {
      console.log(value);
    };

    const onfinally = () => {
      loading.value = false;
    };

    const body = new FormData();

    body.append('type', 'Trusts');

    const init = { method: "POST", body: body };

    apiFetch(`users/${user.value.id}/relationships`, onfulfilled, onfinally, init);
  }
</script>

<template>
  <main v-if="user">
    <h1>{{ user.name }}</h1>
    <button type="button" @click="() => trust()">Trust</button>
  </main>
  <main v-else>
    <h1>Loading...</h1>
  </main>
</template>
