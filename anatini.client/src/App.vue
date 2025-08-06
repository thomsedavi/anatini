<script setup lang="ts">
  import { store } from './store.ts'
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
    <button v-if="store.isLoggedIn" @click="store.logOut">Log Out</button>
  </nav>
  <main>
    <RouterView />
  </main>
</template>
