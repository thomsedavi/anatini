<script setup lang="ts">
  import { useTemplateRef, onMounted } from 'vue';
  import { useRouter } from 'vue-router'
  import { store } from '../store.ts'

  type OkResponseJson = {
    bearer: string;
  }

  const router = useRouter();

  const nameInput = useTemplateRef<HTMLInputElement>('name');
  const emailInput = useTemplateRef<HTMLInputElement>('email');
  const passwordInput = useTemplateRef<HTMLInputElement>('password');
  const inviteCodeInput = useTemplateRef<HTMLInputElement>('invite-code');

  onMounted(() => {
    nameInput.value!.focus()
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
    // only one input gets reported at a time,
    // chain them this way so they cascade from top to bottom
    !nameInput.value!.reportValidity() || !emailInput.value!.reportValidity() || !passwordInput.value!.reportValidity() || !inviteCodeInput.value!.reportValidity();
  }

  async function signup(e: Event) {
    e.preventDefault();

    let validationPassed = true;

    !validateInput(nameInput.value, 'Please enter a name.') && (validationPassed = false);
    !validateInput(emailInput.value, 'Please enter an email.') && (validationPassed = false);
    !validateInput(passwordInput.value, 'Please enter a password.') && (validationPassed = false);
    !validateInput(inviteCodeInput.value, 'Please enter an invite code.') && (validationPassed = false);

    if (!validationPassed) {
      reportValidity();
      return;
    }

    const body: Record<string, string> = {
      name: nameInput.value!.value.trim(),
      email: emailInput.value!.value.trim(),
      password: passwordInput.value!.value,
      inviteCode: inviteCodeInput.value!.value.trim(),
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
        inviteCodeInput.value!.setCustomValidity("Invite code not found");

        reportValidity();
      } else if (response.status === 409) {
        emailInput.value!.setCustomValidity("Email in use");

        reportValidity();
      } else {
        console.log("Unknown Error");
      }
    });
  }
</script>

<template>
  <h2>SignupView</h2>
  <form id="signup" @submit="signup" action="???" method="post">
    <p>
      <label for="name">Name</label>
      <input id="name" type="text" name="name" maxlength="64" ref="name" @input="event => nameInput?.setCustomValidity('')">
    </p>

    <p>
      <label for="email">Email</label>
      <input id="email" type="email" name="email" ref="email" @input="event => emailInput?.setCustomValidity('')">
    </p>

    <p>
      <label for="password">Password</label>
      <input id="password" type="password" name="password" ref="password" @input="event => passwordInput?.setCustomValidity('')">
    </p>

    <p>
      <label for="inviteCode">Invite Code</label>
      <input id="inviteCode" type="text" name="inviteCode" ref="invite-code" @input="event => inviteCodeInput?.setCustomValidity('')">
    </p>

    <p>
      <input type="submit" value="Submit">
    </p>
  </form>
</template>
