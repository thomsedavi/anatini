<script setup lang="ts">
  import { computed } from 'vue';
  import { useRoute, useRouter } from 'vue-router';

  const router = useRouter();
  const route = useRoute();

  const search = computed({
    get() {
      return route.query.search ?? ''
    },
    set(search) {
      router.replace({ query: { search } })
    }
  });
</script>

<template>
  <main id="main" tabindex="-1">
    <h2>AboutView</h2>
    <label>
      Search: <input v-model.trim="search" maxlength="20">
    </label>
  </main>
</template>
