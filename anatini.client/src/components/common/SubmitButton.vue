<script setup lang="ts">
  defineProps({
    busy: { type: Boolean, required: true },
    text: { type: String, required: true },
    busyText: { type: String, required: true },
    disabled: { type: Boolean, required: true }
  });
</script>

<template>
  <button type="submit" :aria-disabled="busy || disabled ? true : undefined">{{ busy ? busyText : text }}</button>
</template>
