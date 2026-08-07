<script setup lang="ts">
  const model = defineModel<boolean>();

  defineProps<{
    label: string,
    name?: string,
    id: string,
    help?: string,
    controls?: string,
    disabled?: boolean,
    hidden?: boolean,
    expanded?: boolean,
  }>();
</script>

<template>
  <input
    type="checkbox"
    :id="`input-${id}`"
    :name="name ?? undefined"
    v-model="model"
    :disabled="disabled ?? undefined"
    :hidden="hidden ?? undefined"
    :aria-controls="controls ?? undefined"
    :aria-expanded="expanded ?? undefined"
    :aria-describedby="help ? `help-${id}` : undefined" />
  <label :for="`input-${id}`" :hidden="hidden ?? undefined">{{ label }}</label>
  <small v-if="help" :id="`help-${id}`" :hidden="hidden ?? undefined">{{ help }}</small>
</template>