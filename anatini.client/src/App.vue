<script setup lang="ts">
  import { ref } from 'vue';

  const name = ref<string>('');
  const email = ref<string>('');
  const password = ref<string>('');
  const loggedIn = ref<boolean>(!!localStorage.getItem('jwtToken'));

  function logout() {
    localStorage.removeItem('jwtToken');
    loggedIn.value = false;
  }

  async function doLoggedInThing() {
    const response = await fetch("api/controller/action/1", {
      method: "GET",
      headers: { Authorization: `Bearer ${localStorage.getItem("jwtToken") }`}
    });
  }

  async function signup(e: Event) {
    e.preventDefault();

    const response = await fetch("api/authentication/signup", {
      method: "POST",
      headers: {
        "Content-Type": "application/x-www-form-urlencoded",
      },
      body: new URLSearchParams({ name: name.value, email: email.value, password: password.value }),
    });

    const json = await response.json();

    localStorage.setItem("jwtToken", json.bearer);
    loggedIn.value = true;
  }
</script>

<template>
  <form v-if="!loggedIn" id="signup" @submit="signup" action="???" method="post">
    <p>
      <label for="name">Name</label>
      <input id="name" v-model.trim="name" type="text" name="name">
    </p>

    <p>
      <label for="email">Email</label>
      <input id="email" v-model.trim="email" type="email" name="email">
    </p>

    <p>
      <label for="password">Password</label>
      <input id="password" v-model="password" type="password" name="password">
    </p>

    <p>
      <input type="submit" value="Submit">
    </p>
  </form>
  <button v-if="loggedIn" @click="logout">Logout</button>
  <button v-if="loggedIn" @click="doLoggedInThing">Do Logged In Thing</button>
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
