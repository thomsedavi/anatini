<script setup lang="ts">
  const model = defineModel<string>();

  defineProps<{
    label: string,
    name: string,
    options: { text: string, value: string }[],
    id: string,
    help?: string,
    disabled?: boolean,
  }>();
</script>

<template>
  <label :for="`input-${id}`">{{ label }}</label>
  <select
    :name="name"
    :id="`input-${id}`"
    v-model="model"
    :disabled="disabled ?? undefined"
    :aria-describedby="help ? `help-${id}` : undefined">
    <option v-for="option in options" :value="option.value" :key="`${id}-${option.value}`">
      {{ option.text }}
    </option>
  </select>
  <small v-if="help" :id="`help-${id}`">{{ help }}</small>
</template>