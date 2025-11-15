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
  <main>
    <h2>Anatini</h2>
    <button type="button" @click="goToAbout">Go to About</button>
  </main>
</template>
