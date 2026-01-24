<script setup lang="ts">
  import { ref, onMounted } from 'vue';
  import { useRouter } from 'vue-router';
  import { store } from './store.ts';
  import type { IsAuthenticated } from '@/types';

  const router = useRouter();

  const isFetching = ref<boolean>(false);

  onMounted(() => {
    isFetching.value = true;

    fetch("/api/authentication/is-authenticated", {
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
    fetch("/api/authentication/logout", {
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
  <template v-if="store.isLoggedIn !== null">
    <a href="#main" class="skip">Skip to main content</a>

    <header role="banner">
      <RouterLink to="/"><strong>ANATINI</strong></RouterLink>

      <nav aria-label="Main">
        <ul>
          <li><RouterLink to="/about">ABOUT</RouterLink></li>
          <template v-if="!store.isLoggedIn">
            <li><RouterLink to="/signup">SIGNUP</RouterLink></li>
            <li><RouterLink to="/login">LOGIN</RouterLink></li>
          </template>
          <template v-else>
            <li><RouterLink to="/account">ACCOUNT</RouterLink></li>
            <li><a href="/" @click.prevent="logout">LOG OUT</a></li>
          </template>
        </ul>
      </nav>
    </header>

    <RouterView />
  </template>
  <template v-else>
    <p>...</p>
  </template>
</template>
