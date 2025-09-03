<script setup lang="ts">
  import { ref, useTemplateRef } from 'vue';
  import { reportValidity, validateInputs } from './common/validity';

  const emit = defineEmits<{
    submitInviteCode: [email?: string];
  }>();

  const emailInput = useTemplateRef<HTMLInputElement>('email');
  const inviteCodeInput = useTemplateRef<HTMLInputElement>('invite-code');
  const isFetching = ref<boolean>(false);

  async function inviteCode(event: Event) {
    event.preventDefault();

    if (!validateInputs([
      {element: emailInput.value, error: 'Please enter an email.'},
      {element: inviteCodeInput.value, error: 'Please enter an invite code.'},
    ]))
      return;

    isFetching.value = true;

    const body: Record<string, string> = {
      email: emailInput.value!.value.trim(),
      inviteCode: inviteCodeInput.value!.value.trim(),
    };

    fetch("api/authentication/email", {
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

        reportValidity([inviteCodeInput.value]);
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
  <form id="inviteCode" @submit="inviteCode" action="api/authentication/email" method="post">
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
  <button @click="emit('submitInviteCode')">I have an email verification code already</button>
</template>
