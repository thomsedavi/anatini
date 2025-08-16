<script setup lang="ts">
  import { ref, useTemplateRef, onMounted } from 'vue';
  import { useRoute, useRouter } from 'vue-router'
  import { store } from '../store.ts'

  type OkResponseJson = {
    bearer: string;
  }

  const router = useRouter();
  const route = useRoute();

  const emailInput = useTemplateRef<HTMLInputElement>('email');
  const passwordInput = useTemplateRef<HTMLInputElement>('password');
  const isFetching = ref<boolean>(false);

  onMounted(() => {
    emailInput.value!.focus()
  });

  function validateInput(input: HTMLInputElement | null, error: string): boolean {
    if (!input?.value.trim()) {
      input?.setCustomValidity(error);
      return false;
    } else {
      input?.setCustomValidity('');
      return true;
    }
  }

  function reportValidity(): void {
    // only one input gets reported at a time,
    // chain them this way so they cascade from top to bottom
    !emailInput.value!.reportValidity() || !passwordInput.value!.reportValidity();
  }

  async function login(e: Event) {
    e.preventDefault();

    let validationPassed = true;

    !validateInput(emailInput.value, 'Please enter an email.') && (validationPassed = false);
    !validateInput(passwordInput.value, 'Please enter a password.') && (validationPassed = false);

    if (!validationPassed) {
      reportValidity();
      return;
    }

    isFetching.value = true;

    const body: Record<string, string> = {
      email: emailInput.value!.value.trim(),
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
        response.json()
          .then((json: OkResponseJson) => {
            store.logIn(json.bearer);

            let path = '/';

            if (typeof route.query.redirect === 'string') {
              path = route.query.redirect;
            }

            router.replace({ path: path });
          })
          .catch(() => {
            console.log('Unknown Error');
          });
      } else if (response.status === 401) {
        passwordInput.value!.setCustomValidity("Incorrect password");

        reportValidity();
      } else if (response.status === 404) {
        emailInput.value!.setCustomValidity("Email not found");

        reportValidity();
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
      <label for="email">Email</label>
      <input id="email" type="email" name="email" ref="email" @input="event => emailInput?.setCustomValidity('')">
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
