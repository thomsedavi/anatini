<script setup lang="ts">
  import { ref } from 'vue';
  import { useRouter } from 'vue-router'
  import { store } from '../store.ts'

  const router = useRouter();

  const name = ref<string>('');
  const email = ref<string>('');
  const password = ref<string>('');
  const inviteCode = ref<string>('');

  async function signup(e: Event) {
    e.preventDefault();

    var body = {
      name: name.value,
      email: email.value,
      password: password.value,
      inviteCode: inviteCode.value
    };

    fetch("api/authentication/signup", {
      method: "POST",
      headers: {
        "Content-Type": "application/x-www-form-urlencoded",
      },
      body: new URLSearchParams(body),
    }).then((value: Response) => {
      if (value.ok) {
        value.json()
          .then(value => {
            store.logIn(value.bearer);

            router.replace({ path: '/' });
          })
          .catch(() => {
            console.log('Unknown Error');
          });
      } else if (value.status === 400) {
        console.log("Bad Request");
      } else {
        console.log("Unknown Error");
      }
    });
  }

</script>

<template>
  <h2>SignupView</h2>
  <form id="signup" @submit="signup" action="???" method="post">
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
      <label for="inviteCode">Invite Code</label>
      <input id="inviteCode" v-model="inviteCode" type="text" name="inviteCode">
    </p>

    <p>
      <input type="submit" value="Submit">
    </p>
  </form>
</template>
