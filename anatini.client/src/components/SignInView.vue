<script setup lang="ts">
  import type { InputError, Status, StatusActions } from '@/types';
  import InputText from './common/InputText.vue';
  import { nextTick, ref } from 'vue';
  import { tidy } from './common/utils';
  import { apiFetch } from './common/apiFetch';
  import { store } from '@/store';
  import { useRoute, useRouter } from 'vue-router';
  import SubmitButton from './common/SubmitButton.vue';

  const router = useRouter();
  const route = useRoute();

  const inputEmail = ref<string>('');
  const inputPassword = ref<string>('');
  const inputErrors = ref<InputError[]>([]);
  const status = ref<Status>('idle');
  const errorSectionRef = ref<HTMLElement | null>(null);

  function getError(id: string): string | undefined {
    return inputErrors.value.find(inputError => inputError.id === id)?.message;
  }

  async function signIn(event: Event) {
    if ((event as SubmitEvent).submitter?.ariaDisabled === 'true')
    {
      return;
    }

    inputErrors.value = [];

    const tidiedEmail = tidy(inputEmail.value);

    if (tidiedEmail === '') {
      inputErrors.value.push({id: 'email', message: 'Email is required'});
    }

    if (inputPassword.value === '') {
      inputErrors.value.push({id: 'password', message: 'Password is required'});
    }

    if (inputErrors.value.length > 0) {
      nextTick(() => {
        errorSectionRef.value?.focus();
      });

      return;
    }

    status.value = 'pending';

    const statusActions: StatusActions = {
      200: () => {
        status.value = 'success';

        store.isAuthenticated = true;

        let path = '/';

        if (typeof route.query.redirect === 'string') {
          path = route.query.redirect;
        }

        router.replace({ path: path });
      },
      404: () => {
        status.value = 'error';

        inputErrors.value.push({id: 'password', message: 'Unable to match email and password'});

        nextTick(() => {
          errorSectionRef.value?.focus();
        });
      },
      500: () => {
        status.value = 'error';

        inputErrors.value.push({id: 'password', message: 'There was an error logging you in, please try again'});

        nextTick(() => {
          errorSectionRef.value?.focus();
        });
      }
    }

    const body = new FormData();

    body.append('email', tidiedEmail);
    body.append('password', inputPassword.value);

    const init = { method: "POST", body: body };

    apiFetch('authentication/sign-in', statusActions, init);
  }
</script>

<template>
  <main id="main" tabindex="-1">
    <header>
      <h1>Log In</h1>
    </header>

    <section id="errors" v-if="inputErrors.length > 0" ref="errorSectionRef" tabindex="-1" aria-live="assertive" aria-labelledby="heading-errors">
      <h2 id="heading-errors">There was a problem Logging In</h2>
      <ul>
        <li v-for="error in inputErrors" :key="'error' + error.id">
          <a :href="'#input-' + error.id">{{ error.message }}</a>
        </li>
      </ul>
    </section>

    <form @submit.prevent="signIn" action="/api/authentication/sign-in" method="POST" novalidate :aria-busy="status === 'pending' ? true : undefined">
      <fieldset>
        <legend>Credentials</legend>

        <InputText
          type="email"
          v-model="inputEmail"
          label="Email"
          name="email"
          id="email"
          :error="getError('email')"
          autocomplete="email" />

        <InputText
          type="password"
          v-model="inputPassword"
          label="Password"
          name="password"
          id="password"
          :error="getError('password')"
          autocomplete="current-password" />
      </fieldset>

      <SubmitButton
        :busy="status === 'pending'"
        :disabled="tidy(inputEmail) === '' || inputPassword === ''"
        text="Log In"
        busy-text="Logging In..." />
    </form>

    <p role="status" class="visuallyhidden" aria-live="polite">{{ status === 'pending' ? 'Busy...' : undefined }}</p>
  </main>
</template>
