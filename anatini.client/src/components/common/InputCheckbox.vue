<script setup lang="ts">
  const model = defineModel<boolean>();

  defineProps<{
    label: string,
    name?: string,
    id: string,
    help?: string,
    controls?: string,
    expanded?: boolean,
  }>();
</script>

<template>
  <input
    type="checkbox"
    :id="`input-${id}`"
    :name="name ?? undefined"
    v-model="model"
    :aria-controls="controls ?? undefined"
    :aria-expanded="expanded ?? undefined"
    :aria-describedby="help ? `help-${id}` : undefined" />
  <label :for="`input-${id}`">{{ label }}</label>
  <small v-if="help" :id="`help-${id}`">{{ help }}</small>
</template>