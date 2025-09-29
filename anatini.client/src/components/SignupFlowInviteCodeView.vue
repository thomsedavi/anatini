<script setup lang="ts">
  import { ref, useTemplateRef } from 'vue';
  import { reportValidity, validateInputs } from './common/validity';

  const emit = defineEmits<{
    submitInviteCode: [email?: string];
  }>();

  const emailAddressInput = useTemplateRef<HTMLInputElement>('email-address');
  const inviteCodeInput = useTemplateRef<HTMLInputElement>('invite-code');
  const isFetching = ref<boolean>(false);

  async function inviteCode(event: Event) {
    event.preventDefault();

    if (!validateInputs([
      {element: emailAddressInput.value, error: 'Please enter an email address.'},
      {element: inviteCodeInput.value, error: 'Please enter an invite code.'},
    ]))
      return;

    isFetching.value = true;

    const body: Record<string, string> = {
      emailAddress: emailAddressInput.value!.value.trim(),
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
        emit('submitInviteCode', emailAddressInput.value!.value);
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
      <label for="emailAddress">Email Address</label>
      <input id="emailAddress" type="email" name="emailAddress" ref="email-address" @input="() => emailAddressInput?.setCustomValidity('')">
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
