<script setup lang="ts">
  import type { SelectOption } from '@/types';

  const model = defineModel<string>();

  defineProps<{
    label: string,
    name: string,
    options: SelectOption[],
    id: string,
    error?: string,
    help?: string,
    disabled?: boolean,
    hidden?: boolean,
  }>();
</script>

<template>
  <label :for="`input-${id}`" :hidden="hidden ?? undefined">{{ label }}</label>
  <select
    :name="name"
    :id="`input-${id}`"
    v-model="model"
    :disabled="disabled ?? undefined"
    :aria-invalid="error ? true : undefined"
    :aria-errormessage="error ? `error-${id}` : undefined"
    :hidden="hidden ?? undefined"
    :aria-describedby="help ? `help-${id}` : undefined">
    <option v-for="option in options" :value="option.value" :key="`${id}-${option.value}`" :disabled="option.disabled ?? undefined">
      {{ option.text }}
    </option>
  </select>
  <small v-if="help" :id="`help-${id}`" :hidden="hidden ?? undefined">{{ help }}</small>
  <small v-if="error" :id="`error-${id}`" role="alert" :hidden="hidden ?? undefined">{{ error }}</small>
</template>