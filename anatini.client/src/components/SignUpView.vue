<script setup lang="ts">
  import { ref } from 'vue';
  import SignUpEmailView from './signUp/SignUpEmailView.vue';
  import SignUpSignupView from './signUp/SignUpSignupView.vue';

  const page = ref<'email' | 'signup'>('email');
  const email = ref<string | undefined>(undefined);

  function submitEmail(resultEmail: string | undefined) {
    email.value = resultEmail;
    page.value = 'signup';
  }

  function goBack() {
    page.value = 'email';
  }
</script>

<template>
  <SignUpEmailView v-if="page === 'email'" @submit-email="submitEmail" />
  <SignUpSignupView v-else-if="page === 'signup'" :dataEmail="email" @go-back="goBack" />
</template>
