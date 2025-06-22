<script setup lang="ts">
  import { ref } from 'vue';

  const name = ref<string>('');
  const email = ref<string>('');
  const password = ref<string>('');
</script>

<template>
  <form id="app" action="/api/authentication/signup" method="post">
    <p>
      <label for="name">Name</label>
      <input id="name" v-model="name" type="text" name="name">
    </p>

    <p>
      <label for="email">Email</label>
      <input id="email" v-model="email" type="email" name="email">
    </p>

    <p>
      <label for="password">Password</label>
      <input id="password" v-model="password" type="password" name="password">
    </p>

    <p>
      <input type="submit" value="Submit">
    </p>
  </form>
</template>

<style scoped>
header {
  line-height: 1.5;
}

.logo {
  display: block;
  margin: 0 auto 2rem;
}

@media (min-width: 1024px) {
  header {
    display: flex;
    place-items: center;
    padding-right: calc(var(--section-gap) / 2);
  }

  .logo {
    margin: 0 2rem 0 0;
  }

  header .wrapper {
    display: flex;
    place-items: flex-start;
    flex-wrap: wrap;
  }
}
</style>
