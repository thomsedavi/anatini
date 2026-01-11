<script setup lang="ts">
  import type { InputError, IsAuthenticated, Status, StatusActions } from '@/types';
  import InputText from './common/InputText.vue';
  import { nextTick, ref } from 'vue';
  import { tidy } from './common/utils';
  import { apiFetch } from './common/apiFetch';
  import { store } from '@/store';
  import { useRoute, useRouter } from 'vue-router';

  const router = useRouter();
  const route = useRoute();

  const inputEmailAddress = ref<string>('');
  const inputPassword = ref<string>('');
  const inputErrors = ref<InputError[]>([]);
  const status = ref<Status>('idle');
  const errorSectionRef = ref<HTMLElement | null>(null);

  function getError(id: string): string | undefined {
    return inputErrors.value.find(inputError => inputError.id === id)?.message;
  }

  async function login() {
    inputErrors.value = [];

    const tidiedEmailAddress = tidy(inputEmailAddress.value);

    if (tidiedEmailAddress === '') {
      inputErrors.value.push({id: 'emailAddress', message: 'Email address is required'});
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
      200: (response?: Response) => {
        status.value = 'success';

        response?.json()
          .then((value: IsAuthenticated) => {
            store.isLoggedIn = value.isAuthenticated;
            store.expiresUtc = value.expiresUtc;

            let path = '/';

            if (typeof route.query.redirect === 'string') {
              path = route.query.redirect;
            }

            router.replace({ path: path });
          })
          .catch(() => {
            store.isLoggedIn = false;
          });
      },
      404: () => {
        status.value = 'error';

        inputErrors.value.push({id: 'password', message: 'Unable to match email address and password'});

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

    body.append('emailAddress', tidiedEmailAddress);
    body.append('password', inputPassword.value);

    const init = { method: "POST", body: body };

    apiFetch('authentication/login', statusActions, undefined, init);
  }
</script>

<template>
  <main>
    <section aria-labelledby="heading-main">
      <header>
        <h1 id="heading-main">Login</h1>
      </header>

      <section v-if="inputErrors.length > 0" ref="errorSectionRef" tabindex="-1" aria-live="assertive" aria-labelledby="heading-errors">
        <h2 id="heading-errors">There was a problem Logging In</h2>
        <ul>
          <li v-for="error in inputErrors" :key="'error' + error.id">
            <a :href="'#input-' + error.id">{{ error.message }}</a>
          </li>
        </ul>
      </section>

      <form @submit.prevent="login" action="/api/authentication/login" method="POST" novalidate>
        <fieldset>
          <legend>Login</legend>

          <InputText
            v-model="inputEmailAddress"
            label="Email"
            name="emailAddress"
            id="emailAddress"
            :error="getError('emailAddress')"
            autocomplete="email" />

          <br>

          <InputText
            v-model="inputPassword"
            label="Password"
            name="password"
            id="password"
            :error="getError('password')"
            autocomplete="current-password" />
        </fieldset>

        <footer>
          <button type="submit" :disabled="status === 'pending' || tidy(inputEmailAddress) === '' || inputPassword === ''">{{status === 'pending' ? 'Logging In...' : 'Log in' }}</button>
        </footer>
      </form>
    </section>
  </main>
</template>
