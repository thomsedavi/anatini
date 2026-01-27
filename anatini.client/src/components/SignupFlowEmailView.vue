<script setup lang="ts">
  import { nextTick, ref } from 'vue';
  import InputText from './common/InputText.vue';
  import SubmitButton from './common/SubmitButton.vue';
  import { tidy } from './common/utils';
  import type { InputError, Status, StatusActions } from '@/types';
  import { apiFetch } from './common/apiFetch';

  const emit = defineEmits<{
    submitEmail: [email: string | null];
  }>();

  const inputErrors = ref<InputError[]>([]);
  const inputEmailAddress = ref<string>('');
  const errorSectionRef = ref<HTMLElement | null>(null);
  const status = ref<Status>('idle');

  function getError(id: string): string | undefined {
    return inputErrors.value.find(inputError => inputError.id === id)?.message;
  }

  async function email(event: Event) {
    if ((event as SubmitEvent).submitter?.ariaDisabled === 'true')
    {
      return;
    }

    inputErrors.value = [];

    const tidiedEmailAddress = tidy(inputEmailAddress.value);

    if (tidiedEmailAddress === '') {
      inputErrors.value.push({id: 'emailAddress', message: 'Email address is required'});
    }

    if (inputErrors.value.length > 0) {
      nextTick(() => {
        errorSectionRef.value?.focus();
      });

      return;
    }

    status.value = 'pending';

    const statusActions: StatusActions = {
      204: () => {
        emit('submitEmail', tidiedEmailAddress);
      },
      500: () => {
        status.value = 'error';

        inputErrors.value.push({id: 'email', message: 'There was an error signing you up, please try again'});

        nextTick(() => {
          errorSectionRef.value?.focus();
        });
      }
    }

    const body = new FormData();

    body.append('emailAddress', tidiedEmailAddress);

    const init = { method: "POST", body: body };

    apiFetch('authentication/email', statusActions, init);
  }
</script>

<template>
  <main id="main" tabindex="-1">
    <header>
      <h1>Sign Up</h1>
    </header>

    <section id="errors" v-if="inputErrors.length > 0" ref="errorSectionRef" tabindex="-1" aria-live="assertive" aria-labelledby="heading-errors">
      <h2 id="heading-errors">There was a problem signup up</h2>
      <ul>
        <li v-for="error in inputErrors" :key="'error' + error.id">
          <a :href="'#input-' + error.id">{{ error.message }}</a>
        </li>
      </ul>
    </section>

    <form @submit.prevent="email" action="/api/authentication/email" method="POST">
      <fieldset>
        <legend class="visuallyhidden">Email Address</legend>

        <InputText
          type="email"
          v-model="inputEmailAddress"
          label="Email"
          name="emailAddress"
          id="emailAddress"
          :error="getError('emailAddress')"
          autocomplete="email"
          :required="true" />
      </fieldset>

      <SubmitButton
        :busy="status === 'pending'"
        :disabled="tidy(inputEmailAddress) === ''"
        text="Sign Up"
        busy-text="Signing Up..." />

      <button type="button" @click="emit('submitEmail', null)">I Have A Code</button>
    </form>

    <p role="status" class="visuallyhidden" aria-live="polite">{{ status === 'pending' ? 'Busy...' : undefined }}</p>
  </main>
</template>
