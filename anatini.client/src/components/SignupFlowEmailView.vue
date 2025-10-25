<script setup lang="ts">
  import { ref, useTemplateRef } from 'vue';
  import { validateInputs } from './common/validity';

  const emit = defineEmits<{
    submitEmail: [email?: string];
  }>();

  const emailAddressInput = useTemplateRef<HTMLInputElement>('email-address');
  const isFetching = ref<boolean>(false);

  async function email(event: Event) {
    event.preventDefault();

    if (!validateInputs([
      {element: emailAddressInput.value, error: 'Please enter an email address.'},
    ]))
      return;

    isFetching.value = true;

    const body: Record<string, string> = {
      emailAddress: emailAddressInput.value!.value.trim(),
    };

    fetch("/api/authentication/email", {
      method: "POST",
      headers: {
        "Content-Type": "application/x-www-form-urlencoded",
      },
      body: new URLSearchParams(body),
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
  <h2>SignupFlowEmailView</h2>
  <form id="email" @submit="email" action="/api/authentication/email" method="post">
    <p>
      <label for="emailAddress">Email Address</label>
      <input id="emailAddress" type="email" name="emailAddress" ref="email-address" @input="() => emailAddressInput?.setCustomValidity('')">
    </p>

    <p>
      <input type="submit" value="Submit" :disabled="isFetching">
    </p>
  </form>
  <button @click="emit('submitEmail')">I have an email verification code already</button>
</template>
