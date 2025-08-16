<script setup lang="ts">
  import { useTemplateRef, onMounted } from 'vue';
  import { useRoute, useRouter } from 'vue-router'
  import { store } from '../store.ts'

  type ResponseErrors = {
    email?: string[];
    password?: string[];
  }

  type OkResponseJson = {
    bearer: string;
  }

  type BadRequestResponseJson = {
    errors: ResponseErrors;
  };

  const router = useRouter();
  const route = useRoute();

  const emailInput = useTemplateRef<HTMLInputElement>('email');
  const passwordInput = useTemplateRef<HTMLInputElement>('password');

  onMounted(() => {
    emailInput.value?.focus()
  })

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
    !emailInput.value?.reportValidity() || !passwordInput.value?.reportValidity();
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

    const body = {
      email: emailInput.value?.value.trim(),
      password: passwordInput.value?.value.trim(),
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

            router.replace({ path: route.query.redirect ?? '/' });
          })
          .catch(() => {
            console.log('Unknown Error');
          });
      } else if (response.status === 400) {
        response.json()
          .then((json: BadRequestResponseJson) => {
            if (json.errors) {
              json.errors.email && emailInput.value?.setCustomValidity(json.errors.email.join(';'));
              json.errors.password && passwordInput.value?.setCustomValidity(json.errors.password.join(';'));

              reportValidity();
            } else {
              console.log("Unknown Error");
            }
          }
        );
      } else {
        console.log("Unknown Error");
      }
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
      <input type="submit" value="Submit">
    </p>
  </form>
</template>
