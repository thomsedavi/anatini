<script setup>
  import { ref } from 'vue'
  import { useRoute, onBeforeRouteUpdate } from 'vue-router'

  const route = useRoute();

  const userHandle = ref($route.params.userHandle);
  const postHandle = ref($route.params.postHandle);

  onBeforeRouteUpdate(async (to, from) => {
    userHandle.value = to.params.userHandle;
    postHandle.value = to.params.postHandle;
  })
</script>

<template>
  <h2>PostView</h2>
  <p>User {{ userHandle }}</p>
  <p>Post {{ postHandle }}</p>
</template>
