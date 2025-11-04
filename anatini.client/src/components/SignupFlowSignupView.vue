<script setup lang="ts">
  import { ref, useTemplateRef } from 'vue';
  import { useRouter } from 'vue-router'
  import { reportValidity, validateInputs } from './common/validity';
  import { store } from '@/store.ts';
  import type { IsAuthenticated } from '@/types';

  const { emailAddress, verificationFailed } = defineProps<{
    emailAddress?: string;
    verificationFailed: boolean;
  }>();

  const emit = defineEmits<{
    goBack: [];
    failVerification: [];
  }>();
  
  const router = useRouter();

  const emailAddressInput = useTemplateRef<HTMLInputElement>('email-address');
  const nameInput = useTemplateRef<HTMLInputElement>('name');
  const slugInput = useTemplateRef<HTMLInputElement>('slug');
  const passwordInput = useTemplateRef<HTMLInputElement>('password');
  const verificationCodeInput = useTemplateRef<HTMLInputElement>('verification-code');
  const isFetching = ref<boolean>(false);

  async function signup(event: Event) {
    event.preventDefault();

    if (!validateInputs([
      { element: emailAddressInput.value, error: 'Please enter an email address.' },
      { element: nameInput.value, error: 'Please enter a name.' },
      { element: slugInput.value, error: 'Please enter a slug.' },
      { element: passwordInput.value, error: 'Please enter a password.' },
      { element: verificationCodeInput.value, error: 'Please enter a verification code.' },
    ]))
      return;

    isFetching.value = true;

    const body: Record<string, string> = {
      emailAddress: emailAddressInput.value!.value.trim(),
      name: nameInput.value!.value.trim(),
      slug: slugInput.value!.value.trim(),
      password: passwordInput.value!.value,
      verificationCode: verificationCodeInput.value!.value.trim(),
    };

    fetch("api/authentication/signup", {
      method: "POST",
      headers: {
        "Content-Type": "application/x-www-form-urlencoded",
      },
      body: new URLSearchParams(body),
    }).then((response: Response) => {
      if (response.ok) {
        response.json()
          .then((value: IsAuthenticated) => {
            store.isLoggedIn = value.isAuthenticated;
            store.expiresUtc = value.expiresUtc;

            router.replace({ path: '/account' });
          })
          .catch(() => {
            store.isLoggedIn = false;
          });
      } else if (response.status === 404) {
        verificationCodeInput.value!.setCustomValidity("Verification code does not match, please go back and resend email.");

        reportValidity([verificationCodeInput.value]);

        emit('failVerification');
      } else if (response.status === 409) {
        slugInput.value!.setCustomValidity("Slug already in use!");

        reportValidity([slugInput.value]);
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
    <h2>SignupView</h2>
    <form id="signup" @submit="signup" action="/api/authentication/signup" method="post">
      <p>
        <label for="emailAddress">Email Address</label>
        <input id="emailAddress" type="email" name="emailAddress" ref="email-address" :value="emailAddress" :disabled="emailAddress !== undefined" @input="() => emailAddressInput?.setCustomValidity('')">
      </p>

      <p>
        <label for="name">Name</label>
        <input id="name" type="text" name="name" maxlength="64" ref="name" @input="() => nameInput?.setCustomValidity('')">
      </p>

      <p>
        <label for="slug">Slug</label>
        <input id="slug" type="text" name="slug" maxlength="64" ref="slug" @input="() => slugInput?.setCustomValidity('')">
      </p>

      <p>
        <label for="password">Password</label>
        <input id="password" type="password" name="password" ref="password" @input="() => passwordInput?.setCustomValidity('')">
      </p>

      <p>
        <label for="verificationCode">Verification Code</label>
        <input id="verificationCode" type="text" name="verificationCode" ref="verification-code" @input="() => verificationCodeInput?.setCustomValidity('')">
      </p>

      <p>
        <input type="submit" value="Submit" :disabled="isFetching || verificationFailed">
      </p>
    </form>
    <button v-if="verificationFailed" @click="emit('goBack')">Go Back</button>
  </main>
</template>
