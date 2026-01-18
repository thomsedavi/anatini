<script setup lang="ts">
  import { ref, onMounted, onBeforeUnmount } from 'vue';
  import { useRouter } from 'vue-router';
  import { store } from './store.ts';
  import type { IsAuthenticated } from '@/types';

  const router = useRouter();

  const isCollapsed = ref<boolean>(true);
  const isFetching = ref<boolean>(false);
  const isMobile = ref<boolean>(false);

  function onResize() {
    isMobile.value = window.innerWidth <= 600;

    if (!isCollapsed.value && window.innerWidth >= 601) {
      isCollapsed.value = true;
    }
  }

  onBeforeUnmount(() => {
    window.removeEventListener('resize', onResize);
  });

  onMounted(() => {
    window.addEventListener('resize', onResize);

    isMobile.value = window.innerWidth <= 600;
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
      isCollapsed.value = true;
    });
  }
</script>

<template>
  <template v-if="store.isLoggedIn !== null">
    <nav v-if="isMobile">
      <template v-if="isCollapsed">
        <RouterLink to="/">Home</RouterLink>
        <svg viewBox="0 0 100 100" xmlns="http://www.w3.org/2000/svg" width="3.16rem" height="3.16rem" @click="isCollapsed = false">
          <rect width="100" height="100" />
          <path d="M 32.5 32.5 L 67.5 32.5" />
          <path d="M 32.5 50 L 67.5 50" />
          <path d="M 32.5 67.7 L 67.5 67.5" />
        </svg>
      </template>
      <template v-else>
        <RouterLink to="/" @click="isCollapsed = true">Home</RouterLink>
        <RouterLink to="/about" @click="isCollapsed = true">Go to About</RouterLink>
        <template v-if="!store.isLoggedIn">
          <RouterLink to="/signup" @click="isCollapsed = true">Signup</RouterLink>
          <RouterLink to="/login" @click="isCollapsed = true">Login</RouterLink>
        </template>
        <template v-else>
          <RouterLink to="/account" @click="isCollapsed = true">Account</RouterLink>
          <a href="/" @click.prevent="logout">Log Out</a>
        </template>
        <svg viewBox="0 0 100 100" xmlns="http://www.w3.org/2000/svg" width="3.16rem" height="3.16rem" @click="isCollapsed = true">
        <rect width="100" height="100" />
        <path d="M 32.5 32.5 L 67.5 67.5" />
        <path d="M 32.5 67.7 L 67.5 32.5" />
        </svg>
      </template>
    </nav>
    <nav v-else>
      <RouterLink to="/">Home</RouterLink>
      <RouterLink to="/about">Go to About</RouterLink>
      <template v-if="!store.isLoggedIn">
        <RouterLink to="/signup">Signup</RouterLink>
        <RouterLink to="/login">Login</RouterLink>
      </template>
      <template v-else>
        <RouterLink to="/account">Account</RouterLink>
        <a href="/" @click.prevent="logout">Log Out</a>
      </template>
    </nav>
    <RouterView />
  </template>
  <template v-else>
    <p>...</p>
  </template>
</template>
