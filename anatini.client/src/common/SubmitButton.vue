<script setup lang="ts">
  defineProps<{
    busy: boolean,
    text: string,
    busyText: string,
  }>();
</script>

<template>
  <button type="submit" :aria-disabled="busy ? true : undefined">{{ busy ? busyText : text }}</button>
</template>
