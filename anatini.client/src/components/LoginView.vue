<script setup lang="ts">
  import { ref, useTemplateRef, onMounted } from 'vue';
  import { useRoute, useRouter } from 'vue-router';
  import { store } from '../store.ts';
  import { reportValidity, validateInputs } from './common/validity.ts';

  const router = useRouter();
  const route = useRoute();

  const emailAddressInput = useTemplateRef<HTMLInputElement>('email-address');
  const passwordInput = useTemplateRef<HTMLInputElement>('password');
  const isFetching = ref<boolean>(false);

  onMounted(() => {
    emailAddressInput.value!.focus()
  });

  async function login(e: Event) {
    e.preventDefault();

    if (!validateInputs([
      {element: emailAddressInput.value, error: 'Please enter an email address.'},
      {element: passwordInput.value, error: 'Please enter a password.'},
    ]))
      return;

    isFetching.value = true;

    const body: Record<string, string> = {
      emailAddress: emailAddressInput.value!.value.trim(),
      password: passwordInput.value!.value.trim(),
    };

    fetch("api/authentication/login", {
      method: "POST",
      headers: {
        "Content-Type": "application/x-www-form-urlencoded",
      },
      body: new URLSearchParams(body),
    }).then((response: Response) => {
      if (response.ok) {
        store.isLoggedIn = true;

        let path = '/';

        if (typeof route.query.redirect === 'string') {
          path = route.query.redirect;
        }

        router.replace({ path: path });
      } else if (response.status === 401) {
        passwordInput.value!.setCustomValidity("Incorrect password");

        reportValidity([passwordInput.value]);
      } else {
        console.log("Unknown Error");
      }
    }).finally(() => {
      isFetching.value = false;
    });
  }
</script>

<template>
  <h2>LoginView</h2>
  <form id="login" @submit="login" action="???" method="post">
    <p>
      <label for="emailAddress">Email Address</label>
      <input id="emailAddress" type="email" name="emailAddress" ref="email-address" @input="event => emailAddressInput?.setCustomValidity('')">
    </p>

    <p>
      <label for="password">Password</label>
      <input id="password" type="password" name="password" ref="password" @input="event => passwordInput?.setCustomValidity('')">
    </p>

    <p>
      <input type="submit" value="Submit" :disabled="isFetching">
    </p>
  </form>
</template>
