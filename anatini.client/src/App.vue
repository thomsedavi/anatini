<script setup lang="ts">
  import { useRouter } from 'vue-router';
  import { store } from './store.ts';

  const router = useRouter();
</script>

<template>
  <h1>Anatini!</h1>
  <nav>
    <RouterLink to="/">Go to Home</RouterLink>
    <RouterLink to="/about">Go to About</RouterLink>
    <RouterLink to="/users/david/posts/post-1">Post 1</RouterLink>
    <RouterLink to="/users/david/posts/post-2">Post 2</RouterLink>
    <RouterLink to="/users/david">David</RouterLink>
    <RouterLink v-if="!store.isLoggedIn" to="/signup">Signup</RouterLink>
    <RouterLink v-if="!store.isLoggedIn" to="/login">Login</RouterLink>
    <button v-if="store.isLoggedIn" @click="store.logOut(router)">Log Out</button>
  </nav>
  <main>
    <RouterView />
  </main>
</template>
