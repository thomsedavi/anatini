<script lang="ts">
export default {
  methods: {
    goToAbout() {
      this.$router.push('/about')
    },
  },
}
</script>

<template>
  <h2>HomeView</h2>
  <button @click="goToAbout">Go to About</button>
  <br />
  <br />
  <svg viewBox="0 0 100 100" xmlns="http://www.w3.org/2000/svg" width="16em">
    <rect width="100" height="100" fill="#fed" />
    <rect x="10" y="10" width="80" height="80" fill="#123" />
    <rect x="20" y="20" width="60" height="60" fill="#567" />
    <rect x="30" y="30" width="40" height="40" fill="#721" />
    <rect x="40" y="40" width="20" height="20" fill="#883" />
  </svg>
</template>
