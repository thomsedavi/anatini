<script setup lang="ts">
  import { nextTick, ref } from 'vue';
  import type { InputError, Status, StatusActions, Visibility } from '@/types';
  import { tidy } from './common/utils';
  import InputText from './common/InputText.vue';
  import SubmitButton from './common/SubmitButton.vue';
  import { apiFetch } from './common/apiFetch';
  import { store } from '@/store';
  import { useRouter } from 'vue-router';
  import VisibilitySelect from './common/VisibilitySelect.vue';
  import InputCheckbox from './common/InputCheckbox.vue';

  const router = useRouter();

  const props = defineProps<{
    email?: string,
  }>();

  const emit = defineEmits<{
    goBack: [];
  }>();
  
  const inputErrors = ref<InputError[]>([]);
  const inputEmail = ref<string>(props.email ?? '');
  const inputName = ref<string>('');
  const inputConfirmationCode = ref<string>('');
  const inputHandle = ref<string>('');
  const inputVisibility = ref<Visibility>('Public');
  const inputPassword = ref<string>('');
  const inputIsPersistent = ref<boolean>(false);
  const errorSectionRef = ref<HTMLElement | null>(null);
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

    const tidiedEmail = tidy(inputEmail.value);
    const tidiedName = tidy(inputName.value);
    const tidiedHandle = tidy(inputHandle.value);
    const tidiedPassword = tidy(inputPassword.value);
    const tidiedConfirmationCode = tidy(inputConfirmationCode.value);

    if (tidiedEmail === '') {
      inputErrors.value.push({id: 'email', message: 'Email is required'});
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

    if (tidiedConfirmationCode === '') {
      inputErrors.value.push({id: 'confirmationCode', message: 'Confirmation Code is required'});
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
        store.isAuthenticated = true;

        router.replace({ path: '/account' });     
      },
      404: () => {
        status.value = 'error';

        inputErrors.value.push({id: 'confirmationCode', message: 'Confirmation code does not match, please go back and resend email'});

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

    body.append('email', tidiedEmail);
    body.append('name', tidiedName);
    body.append('handle', tidiedHandle);
    body.append('password', tidiedPassword);
    body.append('confirmationCode', tidiedConfirmationCode);
    body.append('visibility', inputVisibility.value);

    if (inputIsPersistent.value === true) {
      body.append('isPersistent', 'true');
    }

    const init = { method: "POST", body: body };

    apiFetch('authentication/sign-up', statusActions, init);
  }
</script>

<template>
  <main id="main" tabindex="-1">
    <header>
      <h1>Sign Up</h1>
    </header>

    <section id="errors" v-if="inputErrors.length > 0" ref="errorSectionRef" tabindex="-1" aria-live="assertive" aria-labelledby="heading-errors">
      <h2 id="heading-errors">There was a problem signing up</h2>
      <ul role="list">
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
          v-model="inputEmail"
          label="Email"
          name="email"
          id="email"
          :error="getError('email')"
          autocomplete="email"
          :readonly="email !== undefined"
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
          v-model="inputConfirmationCode"
          label="Confirmation Code"
          name="confirmationCode"
          id="confirmationCode"
          autocomplete="one-time-code"
          :error="getError('confirmationCode')"
          :required="true" />

        <VisibilitySelect v-model="inputVisibility" />

        <InputCheckbox
          v-model="inputIsPersistent"
          label="Remember Me"
          name="is-persistent"
          id="is-persistent"
          help="Remember you" />
      </fieldset>

      <SubmitButton
        :busy="status === 'pending'"
        text="Complete Sign Up"
        busy-text="Completing Sign Up..." />

      <button type="button" @click="emit('goBack')">Go Back</button>
    </form>
  </main>
</template>
