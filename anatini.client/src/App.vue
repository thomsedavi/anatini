<script setup lang="ts">
  import { ref, onMounted } from 'vue';
  import { useRouter } from 'vue-router';
  import { store } from './store.ts';
  import type { IsAuthenticated } from '@/types';

  const router = useRouter();
  const isFetching = ref<boolean>(false);

  onMounted(() => {
    isFetching.value = true;

    fetch("api/authentication/is-authenticated", {
      method: "GET",
    }).then((response: Response) => {
      if (response.ok) {
        response.json()
          .then((value: IsAuthenticated) => {
            store.isLoggedIn = value.isAuthenticated;
            store.expiresUtc = value.expiresUtc;
          })
          .catch(() => {
            store.isLoggedIn = false;
          });
      } else {
        store.isLoggedIn = false;
      }
    }).catch(() => {
      store.isLoggedIn = false;
    }).finally(() => {
      isFetching.value = false;
    });
  });

  async function logout() {
    fetch("api/authentication/logout", {
      method: "POST",
    }).then((response: Response) => {
      if (response.ok) {
        store.isLoggedIn = false;
      } else {
        store.isLoggedIn = false;
      }
    }).catch(() => {
      store.isLoggedIn = false;
    }).finally(() => {
      router.replace({ path: '/' });
    });
  }
</script>

<template>
  <template v-if="store.isLoggedIn !== undefined">
    <h1>Anatini! Session expires ({{ store.expiresUtc }}) now ({{ new Date().toISOString() }})</h1>
    <nav>
      <RouterLink to="/">Go to Home</RouterLink>
      <RouterLink to="/about">Go to About</RouterLink>
      <RouterLink to="/channels/david/posts/post-1">Post 1</RouterLink>
      <RouterLink to="/channels/david/posts/post-2">Post 2</RouterLink>
      <RouterLink to="/users/david">David</RouterLink>
      <RouterLink v-if="!store.isLoggedIn" to="/signup">Signup</RouterLink>
      <RouterLink v-if="!store.isLoggedIn" to="/login">Login</RouterLink>
      <RouterLink v-if="store.isLoggedIn" to="/account">Account</RouterLink>
      <button v-if="store.isLoggedIn" @click="logout">Log Out</button>
    </nav>
    <main>
      <RouterView />
    </main>
  </template>
  <template v-else>
    <p>...</p>
  </template>
</template>
