<script setup lang="ts">
  defineProps<{
    busy: boolean,
    text: string,
    busyText: string,
    disabled: boolean,
  }>();
</script>

<template>
  <button type="submit" :aria-disabled="busy || disabled ? true : undefined">{{ busy ? busyText : text }}</button>
</template>
