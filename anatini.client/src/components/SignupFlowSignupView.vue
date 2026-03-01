<script setup lang="ts">
  import { nextTick, ref, useTemplateRef } from 'vue';
  import type { InputError, IsAuthenticated, Status, StatusActions } from '@/types';
  import { tidy } from './common/utils';
  import InputText from './common/InputText.vue';
  import SubmitButton from './common/SubmitButton.vue';
  import { apiFetch } from './common/apiFetch';
  import { store } from '@/store';
  import { useRouter } from 'vue-router';

  const router = useRouter();

  const props = defineProps<{
    emailAddress?: string,
    verificationFailed: boolean,
  }>();

  const emit = defineEmits<{
    goBack: [];
  }>();
  
  const inputErrors = ref<InputError[]>([]);
  const inputEmailAddress = ref<string>(props.emailAddress ?? '');
  const inputName = ref<string>('');
  const inputVerificationCode = ref<string>('');
  const inputHandle = ref<string>('');
  const inputPassword = ref<string>('');
  const errorSectionRef = ref<HTMLElement | null>(null);
  const protectedInput = useTemplateRef<HTMLInputElement>('protected');
  const status = ref<Status>('idle');

  function getError(id: string): string | undefined {
    return inputErrors.value.find(inputError => inputError.id === id)?.message;
  }

  async function signup(event: Event) {
    if ((event as SubmitEvent).submitter?.ariaDisabled === 'true')
    {
      return;
    }

    inputErrors.value = [];

    const tidiedEmailAddress = tidy(inputEmailAddress.value);
    const tidiedName = tidy(inputName.value);
    const tidiedHandle = tidy(inputHandle.value);
    const tidiedPassword = tidy(inputPassword.value);
    const tidiedVerificationCode = tidy(inputVerificationCode.value);

    if (tidiedEmailAddress === '') {
      inputErrors.value.push({id: 'emailAddress', message: 'Email address is required'});
    }

    if (tidiedName === '') {
      inputErrors.value.push({id: 'name', message: 'Name is required'});
    }

    if (tidiedHandle === '') {
      inputErrors.value.push({id: 'handle', message: 'Handle is required'});
    }

    if (tidiedPassword === '') {
      inputErrors.value.push({id: 'password', message: 'Password is required'});
    }

    if (tidiedPassword === '') {
      inputErrors.value.push({id: 'password', message: 'Password is required'});
    }

    if (tidiedVerificationCode === '') {
      inputErrors.value.push({id: 'verificationCode', message: 'Verification Code is required'});
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
        response?.json()
          .then((value: IsAuthenticated) => {
            store.isLoggedIn = value.isAuthenticated;
            store.expiresUtc = value.expiresUtc;

            router.replace({ path: '/account' });     
          });
      },
      404: () => {
        status.value = 'error';

        inputErrors.value.push({id: 'verificationCode', message: 'Verification code does not match, please go back and resend email'});

        nextTick(() => {
          errorSectionRef.value?.focus();
        });
      },
      409: () => {
        status.value = 'error';

        inputErrors.value.push({id: 'handle', message: 'Handle already in use'});

        nextTick(() => {
          errorSectionRef.value?.focus();
        });
      }
    }

    const body = new FormData();

    body.append('emailAddress', tidiedEmailAddress);
    body.append('name', tidiedName);
    body.append('handle', tidiedHandle);
    body.append('password', tidiedPassword);
    body.append('verificationCode', tidiedVerificationCode);

    if (protectedInput.value!.checked) {
      body.append('protected', 'true');
    }

    const init = { method: "POST", body: body };

    apiFetch('authentication/signup', statusActions, init);
  }
</script>

<template>
  <main id="main" tabindex="-1">
    <header>
      <h1>Sign Up</h1>
    </header>

    <section id="errors" v-if="inputErrors.length > 0" ref="errorSectionRef" tabindex="-1" aria-live="assertive" aria-labelledby="heading-errors">
      <h2 id="heading-errors">There was a problem signing up</h2>
      <ul>
        <li v-for="error in inputErrors" :key="'error' + error.id">
          <a :href="'#input-' + error.id">{{ error.message }}</a>
        </li>
      </ul>
    </section>

    <form @submit.prevent="signup" action="/api/authentication/signup" method="POST">
      <fieldset>
        <legend class="visuallyhidden">Complete Signup</legend>

        <InputText
          type="email"
          v-model="inputEmailAddress"
          label="Email"
          name="emailAddress"
          id="emailAddress"
          :error="getError('emailAddress')"
          autocomplete="email"
          :readonly="emailAddress !== null"
          :required="true" />

        <InputText
          v-model="inputName"
          label="Name"
          name="name"
          id="name"
          autocomplete="name"
          :error="getError('name')"
          :required="true" />

        <InputText
          v-model="inputHandle"
          label="Handle"
          name="handle"
          id="handle"
          autocomplete="username"
          :error="getError('handle')"
          :required="true"
          help="lower case characters, number, and hyphens" />

        <InputText
          v-model="inputPassword"
          label="Password"
          name="password"
          id="password"
          type="password"
          autocomplete="new-password"
          :error="getError('password')"
          :required="true" />

        <InputText
          v-model="inputVerificationCode"
          label="Verification Code"
          name="verificationCode"
          id="verificationCode"
          autocomplete="one-time-code"
          :error="getError('verificationCode')"
          :required="true" />

        <input id="protected" type="checkbox" name="protected" ref="protected" />
        <label for="protected" aria-describedby="help-protected">Protected</label>
        <small id="help-protected">your account will only be visible to other verified users</small>
      </fieldset>

      <SubmitButton
        :busy="status === 'pending'"
        :disabled="tidy(inputName) === '' || tidy(inputHandle) === '' || tidy(inputPassword) === '' || tidy(inputVerificationCode) === '' || tidy(inputEmailAddress) === '' || verificationFailed"
        text="Complete Sign Up"
        busy-text="Completing Sign Up..." />

      <button type="button" @click="emit('goBack')">Go Back</button>
    </form>
  </main>
</template>
