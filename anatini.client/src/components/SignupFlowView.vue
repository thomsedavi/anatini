<script setup lang="ts">
  import { ref } from 'vue';
  import SignupFlowInviteCodeView from './SignupFlowInviteCodeView.vue'
  import SignupFlowSignupView from './SignupFlowSignupView.vue'

  const page = ref<'inviteCode' | 'signup'>('inviteCode');
  const emailAddress = ref<string | undefined>();
  const verificationFailed = ref<boolean>(false);

  function submitInviteCode(resultEmailAddress?: string) {
    verificationFailed.value = false;
    emailAddress.value = resultEmailAddress;
    page.value = 'signup';
  }

  function goBack() {
    page.value = 'inviteCode';
  }
</script>

<template>
  <SignupFlowInviteCodeView v-if="page === 'inviteCode'" @submit-invite-code="submitInviteCode" />
  <SignupFlowSignupView v-if="page === 'signup'" :emailAddress="emailAddress" :verificationFailed="verificationFailed" @go-back="goBack" @fail-verification="verificationFailed = true" />
</template>
