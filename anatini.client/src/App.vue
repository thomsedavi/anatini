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
            store.isAuthenticated = value.isAuthenticated;
            store.isTrusted = value.isTrusted;
          })
          .catch(() => {
            store.isAuthenticated = false;
          });
      } else {
        store.isAuthenticated = false;
      }
    }).catch(() => {
      store.isAuthenticated = false;
    }).finally(() => {
      isFetching.value = false;
    });
  });

  async function signOut() {
    fetch("/api/authentication/sign-out", {
      method: "POST",
    }).then((response: Response) => {
      if (response.ok) {
        store.isAuthenticated = false;
      } else {
        store.isAuthenticated = false;
      }
    }).catch(() => {
      store.isAuthenticated = false;
    }).finally(() => {
      router.replace({ path: '/' });
    });
  }
</script>

<template>
  <template v-if="store.isAuthenticated !== null">
    <a href="#main" id="skip">Skip to main content</a>

    <header>
      <RouterLink to="/"><strong>ANATINI</strong></RouterLink>

      <nav aria-label="Main">
        <ul>
          <li><RouterLink to="/about">ABOUT</RouterLink></li>
          <template v-if="store.isAuthenticated === null || !store.isAuthenticated">
            <li><RouterLink to="/sign-up">SIGN UP</RouterLink></li>
            <li><RouterLink to="/sign-in">SIGN IN</RouterLink></li>
          </template>
          <template v-else>
            <li><RouterLink to="/account">ACCOUNT</RouterLink></li>
            <li><a href="/" @click.prevent="signOut">SIGN OUT</a></li>
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
