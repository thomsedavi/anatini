<script setup lang="ts">
  import { ref, onMounted } from 'vue';
  import { store } from '../store.ts';

  const name = ref<string | null>(null);
  const error = ref<string | null>(null);
  const isFetching = ref<boolean>(false);

  onMounted(() => {
    isFetching.value = true;

    fetch("api/users/settings", {
      method: "GET",
      headers: {
        Authorization: `Bearer ${store.jwtToken}`
      },
    }).then((response: Response) => {
      if (response.ok) {
        console.log('Unknown Success');
      } else {
        console.log("Unknown Error");
      }
    }).finally(() => {
      isFetching.value = false;
    });;
  });
</script>

<template>
  <h2>SettingsView</h2>
  <p v-if="isFetching">Loading...</p>
  <p v-if="error">{{ error }}</p>
  <p v-if="name">{{ name }}</p>
</template>
