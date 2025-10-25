<script setup lang="ts">
  import { ref } from 'vue';
  import SignupFlowEmailView from './SignupFlowEmailView.vue'
  import SignupFlowSignupView from './SignupFlowSignupView.vue'

  const page = ref<'email' | 'signup'>('email');
  const emailAddress = ref<string | undefined>();
  const verificationFailed = ref<boolean>(false);

  function submitEmail(resultEmailAddress?: string) {
    verificationFailed.value = false;
    emailAddress.value = resultEmailAddress;
    page.value = 'signup';
  }

  function goBack() {
    page.value = 'email';
  }
</script>

<template>
  <SignupFlowEmailView v-if="page === 'email'" @submit-email="submitEmail" />
  <SignupFlowSignupView v-if="page === 'signup'" :emailAddress="emailAddress" :verificationFailed="verificationFailed" @go-back="goBack" @fail-verification="verificationFailed = true" />
</template>
