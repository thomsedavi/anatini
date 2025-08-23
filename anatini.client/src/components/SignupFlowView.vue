<script setup lang="ts">
  import { ref } from 'vue';
  import SignupFlowInviteCodeView from './SignupFlowInviteCodeView.vue'
  import SignupFlowSignupView from './SignupFlowSignupView.vue'

  const page = ref<'inviteCode' | 'signup'>('inviteCode');
  const email = ref<string | undefined>();
  const verificationFailed = ref<boolean>(false);

  function submitInviteCode(resultEmail?: string) {
    verificationFailed.value = false;
    email.value = resultEmail;
    page.value = 'signup';
  }

  function goBack() {
    page.value = 'inviteCode';
  }
</script>

<template>
  <SignupFlowInviteCodeView v-if="page === 'inviteCode'" @submit-invite-code="submitInviteCode" />
  <SignupFlowSignupView v-if="page === 'signup'" :email="email" :verificationFailed="verificationFailed" @go-back="goBack" @fail-verification="verificationFailed = true" />
</template>
