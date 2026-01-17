<script setup lang="ts">
  import { ref, useTemplateRef } from 'vue';
  import { validateInputs } from './common/validity';

  const emit = defineEmits<{
    submitEmail: [email: string | null];
  }>();

  const emailAddressInput = useTemplateRef<HTMLInputElement>('email-address');
  const isFetching = ref<boolean>(false);

  async function email() {
    if (!validateInputs([
      {element: emailAddressInput.value, error: 'Please enter an email address.'},
    ]))
      return;

    isFetching.value = true;

    const body = new FormData();

    body.append('emailAddress', emailAddressInput.value!.value.trim());

    fetch("/api/authentication/email", {
      method: "POST",
      body: body,
    }).then((response: Response) => {
      if (response.ok) {
        emit('submitEmail', emailAddressInput.value!.value);
      } else {
        console.log("Unknown Error");
      }
    }).finally(() => {
      isFetching.value = false;
    });
  }
</script>

<template>
  <main>
    <h2>Sign Up</h2>
    <form @submit.prevent="email" action="/api/authentication/email" method="POST">
      <fieldset>
        <legend>Email Address</legend>

        <label for="emailAddress">Email Address</label>
        <input id="emailAddress" type="email" name="emailAddress" ref="email-address" @input="() => emailAddressInput?.setCustomValidity('')">
        <hr>

        <button type="submit" :aria-disabled="isFetching">Submit</button>
      </fieldset>
    </form>
    <button type="button" @click="emit('submitEmail', null)">I Have A Code</button>
  </main>
</template>
