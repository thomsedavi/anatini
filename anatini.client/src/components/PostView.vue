<script setup lang="ts">
  import { ref, watch } from 'vue'
  import { useRoute } from 'vue-router'

  const route = useRoute();

  const loading = ref<boolean>(false)
  const post = ref<string | null>(null)
  const error = ref<string | null>(null)

  watch([() => route.params.userHandle, () => route.params.postHandle], fetchPost, { immediate: true })

  async function fetchPost(array: (() => string | string[])[]) {
    error.value = post.value = null
    loading.value = true

    fetch(`/api/users/${array[0]}/posts/${array[1]}`, { method: "GET" })
      .then(value => {
        if (value.ok) {
          value.json()
            .then(value => {
              post.value = value.post;
            })
            .catch(() => {
              error.value = 'Unknown Error';
            });
        } else if (value.status === 401) {
          error.value = 'unauthorised';
        } else {
          error.value = 'internal server error';
        }
      })
      .catch(() => {
        error.value = 'Unknown Error';
      }).
      finally(() => {
        loading.value = false
      });
  }
</script>

<template>
  <h2>PostView</h2>
  <p v-if="loading">Loading...</p>
  <p v-if="error">{{ error }}</p>
  <p v-if="post">{{ post }}</p>
</template>
