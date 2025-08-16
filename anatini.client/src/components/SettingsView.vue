<script setup lang="ts">
  import { ref, onMounted } from 'vue';

  const loading = ref<boolean>(false);
  const name = ref<string | null>(null);
  const error = ref<string | null>(null);

  onMounted(() => {
    console.log('fetch now');
  });
</script>

<template>
  <h2>SettingsView</h2>
  <p v-if="loading">Loading...</p>
  <p v-if="error">{{ error }}</p>
  <p v-if="name">{{ name }}</p>
</template>
