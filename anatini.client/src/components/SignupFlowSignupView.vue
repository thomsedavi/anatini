<script setup lang="ts">
  import { ref, useTemplateRef } from 'vue';
  import { useRouter } from 'vue-router'
  import { store } from '../store.ts'

  const { email, verificationFailed } = defineProps<{
    email?: string;
    verificationFailed: boolean;
  }>();

  const emit = defineEmits<{
    goBack: [];
    failVerification: [];
  }>();
  
  type OkResponseJson = {
    bearer: string;
  }

  const router = useRouter();

  const emailInput = useTemplateRef<HTMLInputElement>('email');
  const nameInput = useTemplateRef<HTMLInputElement>('name');
  const passwordInput = useTemplateRef<HTMLInputElement>('password');
  const verificationCodeInput = useTemplateRef<HTMLInputElement>('verification-code');
  const isFetching = ref<boolean>(false);

  function validateInput(input: HTMLInputElement, error: string): boolean {
    if (!input.value.trim()) {
      input.setCustomValidity(error);
      return false;
    } else {
      input.setCustomValidity('');
      return true;
    }
  }

  function reportValidity(): void {
    const inputs: HTMLInputElement[] = [emailInput.value!, nameInput.value!, passwordInput.value!, verificationCodeInput.value!];

    for (let i = 0; i < inputs.length; i++) {
      if (!inputs[i].reportValidity()) {
        break;
      }
    }
  }

  async function signup(event: Event) {
    event.preventDefault();

    let validationPassed = true;

    if (!validateInput(emailInput.value!, 'Please enter an email.'))
      validationPassed = false;
    if (!validateInput(nameInput.value!, 'Please enter a name.'))
      validationPassed = false;
    if (!validateInput(passwordInput.value!, 'Please enter a password.'))
      validationPassed = false;
    if (!validateInput(verificationCodeInput.value!, 'Please enter a verification code.'))
      validationPassed = false;

    if (!validationPassed) {
      reportValidity();
      return;
    }

    isFetching.value = true;

    const body: Record<string, string> = {
      email: emailInput.value!.value.trim(),
      name: nameInput.value!.value.trim(),
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
          .then((json: OkResponseJson) => {
            store.logIn(json.bearer);

            router.replace({ path: '/settings' });
          })
          .catch(() => {
            console.log('Unknown Error');
          });
      } else if (response.status === 404) {
        verificationCodeInput.value!.setCustomValidity("Verification code does not match, please go back and resend email.");

        reportValidity();

        emit('failVerification');
      } else {
        console.log("Unknown Error");
      }
    }).finally(() => {
      isFetching.value = false;
    });
  }
</script>

<template>
  <h2>SignupView</h2>
  <form id="signup" @submit="signup" action="api/authentication/signup" method="post">
    <p>
      <label for="email">Email</label>
      <input id="email" type="email" name="email" ref="email" :value="email" :disabled="email !== undefined" @input="() => emailInput?.setCustomValidity('')">
    </p>

    <p>
      <label for="name">Name</label>
      <input id="name" type="text" name="name" maxlength="64" ref="name" @input="() => nameInput?.setCustomValidity('')">
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
  <button v-if="verificationFailed" v-on:click="emit('goBack')">Go Back</button>
</template>