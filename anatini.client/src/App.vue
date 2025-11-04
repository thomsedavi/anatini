<script setup lang="ts">
  import { ref, onMounted, onBeforeUnmount } from 'vue';
  import { useRouter } from 'vue-router';
  import { store } from './store.ts';
  import type { IsAuthenticated } from '@/types';

  const router = useRouter();

  const isCollapsed = ref<boolean>(true);
  const isFetching = ref<boolean>(false);

  function onResize() {
    if (window.innerWidth >= 601) {
      isCollapsed.value = false;
    } else if (window.innerWidth <= 600) {
      isCollapsed.value = true;
    }
  }

  onBeforeUnmount(() => {
    window.removeEventListener('resize', onResize);
  });

  onMounted(() => {
    window.addEventListener('resize', onResize);

    isCollapsed.value = window.innerWidth <= 600;
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

  function toggleIsCollapsed() {
    isCollapsed.value = !isCollapsed.value;
  }
</script>

<template>
  <template v-if="store.isLoggedIn !== undefined">
    <nav>
      <template v-if="isCollapsed">
        <RouterLink to="/">Home</RouterLink>
      </template>
      <template v-else>
        <RouterLink to="/">Home</RouterLink>
        <RouterLink to="/about">Go to About</RouterLink>
        <template v-if="!store.isLoggedIn">
          <RouterLink to="/signup">Signup</RouterLink>
          <RouterLink to="/login">Login</RouterLink>
        </template>
        <template v-else>
          <RouterLink to="/account">Account</RouterLink>
          <a @click="logout">Log Out</a>
        </template>
      </template>
      <svg viewBox="0 0 100 100" xmlns="http://www.w3.org/2000/svg" width="40" height="40" @click="toggleIsCollapsed">
        <rect width="100" height="100" />
        <g>
          <rect x="10" y="20" width="80" height="15" rx="10" />
          <rect x="10" y="60" width="80" height="15" rx="10" />
        </g>
      </svg>
    </nav>
    <RouterView />
    <footer>
      Footer
    </footer>
  </template>
  <template v-else>
    <p>...</p>
  </template>
</template>
