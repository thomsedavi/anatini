<script setup lang="ts">
  import { ref, watch } from 'vue';
  import { useRoute } from 'vue-router';

  type User = {
    name: string;
  };

  const route = useRoute();

  const loading = ref<boolean>(false);
  const error = ref<string | null>(null);
  const user = ref<User | null>(null);

  watch([() => route.params.userSlug], fetchUser, { immediate: true });

  async function fetchUser(array: (() => string | string[])[]) {
    error.value = user.value = null
    loading.value = true

    fetch(`/api/users/${array[0]}`, { method: "GET" })
      .then((value: Response) => {
        if (value.ok) {
          value.json()
            .then((value: User) => {
              user.value = value;
            })
            .catch(() => {
              error.value = 'Unknown Error';
            });
        } else if (value.status === 401) {
          error.value = 'Unauthorised';
        } else {
          error.value = 'Unkown Error';
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
  <h2>UserView</h2>
  <h3 v-if="user">{{ user.name }}</h3>
</template>
