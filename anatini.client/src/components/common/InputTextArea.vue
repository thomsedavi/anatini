<script setup lang="ts">
  import { tidy } from './utils';

  const model = defineModel<string>();

  defineProps<{
    label: string,
    name: string,
    id: string,
    error?: string,
    maxlength?: number,
    help?: string,
  }>();
</script>

<template>
  <label :for="`input-${id}`">{{ label }}</label>
  <textarea
    :id="`input-${id}`"
    v-model="model"
    :name="name"
    :maxlength="maxlength ?? undefined"
    :aria-invalid="error ? true : undefined"
    :aria-errormessage="error ? `error-${id}` : undefined"
    :aria-describedby="`${help ? `help-${id}` : undefined} ${maxlength ? `counter-${id}` : undefined}`"></textarea>
  <small v-if="help" :id="`help-${id}`">{{ help }}</small>
  <small v-if="error" :id="`error-${id}`" role="alert">{{ error }}</small>
  <output v-if="maxlength"
    :id="`counter-${id}`"
    :aria-live="256 - tidy(model ?? '').length < 20 ? 'assertive' : 'polite'"
    aria-atomic="true">
    Characters remaining: {{ 256 - tidy(model ?? '').length }}
  </output>
</template>
