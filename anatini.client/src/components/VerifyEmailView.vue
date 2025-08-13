<script setup lang="ts">
  import { useTemplateRef, onMounted } from 'vue';
  import { useRouter } from 'vue-router'
  import { store } from '../store.ts'

  type ResponseErrors = {
    verificationCode?: string[];
  }

  type OkResponseJson = {
    bearer: string;
  }

  type BadRequestResponseJson = {
    errors: ResponseErrors;
  };

  const router = useRouter();

  const verificationCodeInput = useTemplateRef<HTMLInputElement>('verification-code');

  onMounted(() => {
    verificationCodeInput.value?.focus()
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
    verificationCodeInput.value?.reportValidity();
  }

  async function verifyEmail(e: Event) {
    e.preventDefault();

    let validationPassed = true;

    !validateInput(verificationCodeInput.value, 'Please enter a verification code.') && (validationPassed = false);

    if (!validationPassed) {
      reportValidity();
      return;
    }

    const body = {
      verificationCode: verificationCodeInput.value?.value.trim(),
    };

    fetch("api/authentication/verifyEmail", {
      method: "POST",
      headers: {
        "Content-Type": "application/x-www-form-urlencoded",
        Authorization: `Bearer ${localStorage.getItem("jwtToken")}`,
      },
      body: new URLSearchParams(body),
    }).then((response: Response) => {
      if (response.ok) {
        response.json()
          .then((json: OkResponseJson) => {
            store.logIn(json.bearer);

            router.replace({ path: '/' });
          })
          .catch(() => {
            console.log('Unknown Error');
          });

      } else if (response.status === 401) {
        console.log("Redirect?");
      } else if (response.status === 400) {
        response.json()
          .then((json: BadRequestResponseJson) => {
            if (json.errors) {
              json.errors.verificationCode && verificationCodeInput.value?.setCustomValidity(json.errors.verificationCode.join(';'));

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
  <h2>VerifyEmailView</h2>
  <form id="verifyEmail" @submit="verifyEmail" action="???" method="post">
    <p>
      <label for="verificationCode">Verification Code</label>
      <input id="verificationCode" type="text" name="verificationCode" ref="verification-code" @input="event => verificationCodeInput?.setCustomValidity('')">
    </p>

    <p>
      <input type="submit" value="Submit">
    </p>
  </form>
</template>
