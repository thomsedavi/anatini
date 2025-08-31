<script setup lang="ts">
  import { ref, onMounted } from 'vue';
  import { store } from '../store.ts';

  type User = {
    name: string;
    emails: {
      email: string;
      verified: boolean;
    }[];
  }

  const user = ref<User | null>(null);
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
        response.json()
          .then((value: User) => {
            user.value = value;
          })
          .catch(() => {
            console.log('Unknown Error');
          });
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
  <template v-if="user">
    <h3>Name</h3>
    <p>{{ user.name }}</p>
    <h3>Emails</h3>
    <ul>
      <li v-for="(email, index) in user.emails" :key="'email' + index">
        {{ email.email }}: {{ email.verified ? "Verified" : "Not Verified" }}
      </li>
    </ul>
  </template>
</template>
