<script setup lang="ts">
  import { ref, useTemplateRef } from 'vue';

  const emit = defineEmits<{
    submitInviteCode: [email?: string];
  }>();

  const emailInput = useTemplateRef<HTMLInputElement>('email');
  const inviteCodeInput = useTemplateRef<HTMLInputElement>('invite-code');
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
    const inputs: HTMLInputElement[] = [emailInput.value!, inviteCodeInput.value!];

    for (let i = 0; i < inputs.length; i++) {
      if (!inputs[i].reportValidity()) {
        break;
      }
    }
  }

  async function inviteCode(event: Event) {
    event.preventDefault();

    let validationPassed = true;

    if (!validateInput(emailInput.value!, 'Please enter an email.'))
      validationPassed = false;
    if (!validateInput(inviteCodeInput.value!, 'Please enter an invite code.'))
      validationPassed = false;

    if (!validationPassed) {
      reportValidity();
      return;
    }

    isFetching.value = true;

    const body: Record<string, string> = {
      email: emailInput.value!.value.trim(),
      inviteCode: inviteCodeInput.value!.value.trim(),
    };

    fetch("api/authentication/inviteCode", {
      method: "POST",
      headers: {
        "Content-Type": "application/x-www-form-urlencoded",
      },
      body: new URLSearchParams(body),
    }).then((response: Response) => {
      if (response.ok) {
        emit('submitInviteCode', emailInput.value!.value);
      } else if (response.status === 404) {
        inviteCodeInput.value!.setCustomValidity("Invite code not found.");

        reportValidity();
      } else {
        console.log("Unknown Error");
      }
    }).finally(() => {
      isFetching.value = false;
    });
  }
</script>

<template>
  <h2>SignupFlowInviteCodeView</h2>
  <form id="inviteCode" @submit="inviteCode" action="api/authentication/inviteCode" method="post">
    <p>
      <label for="email">Email</label>
      <input id="email" type="email" name="email" ref="email" @input="() => emailInput?.setCustomValidity('')">
    </p>

    <p>
      <label for="inviteCode">Invite Code</label>
      <input id="inviteCode" type="text" name="inviteCode" ref="invite-code" @input="() => inviteCodeInput?.setCustomValidity('')">
    </p>

    <p>
      <input type="submit" value="Submit" :disabled="isFetching">
    </p>
  </form>
  <button v-on:click="emit('submitInviteCode')">I have an email verification code already</button>
</template>
