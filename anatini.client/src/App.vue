<script setup lang="ts">

</script>

<template>
  <h1>Hello App!</h1>
  <nav>
    <RouterLink to="/">Go to Home</RouterLink>
    <RouterLink to="/about">Go to About</RouterLink>
    <RouterLink to="/users/david/posts/post-1">Post 1</RouterLink>
    <RouterLink to="/users/david/posts/post-2">Post 2</RouterLink>
    <RouterLink to="/users/david">David</RouterLink>
    <RouterLink to="/signup">Signup</RouterLink>
  </nav>
  <main>
    <RouterView />
  </main>
</template>
