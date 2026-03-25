<script setup lang="ts">
  import { ref } from 'vue';
  import SignUpFlowEmailView from './SignUpFlowEmailView.vue';
  import SignUpFlowSignupView from './SignUpFlowSignupView.vue';

  const page = ref<'email' | 'signup'>('email');
  const email = ref<string | undefined>(undefined);
  const confirmationFailed = ref<boolean>(false);

  function submitEmail(resultEmail: string | undefined) {
    confirmationFailed.value = false;
    email.value = resultEmail;
    page.value = 'signup';
  }

  function goBack() {
    page.value = 'email';
  }
</script>

<template>
  <SignUpFlowEmailView v-if="page === 'email'" @submit-email="submitEmail" />
  <SignUpFlowSignupView v-else-if="page === 'signup'" :email="email" :confirmationFailed="confirmationFailed" @go-back="goBack" @fail-confirmation="confirmationFailed = true" />
</template>
