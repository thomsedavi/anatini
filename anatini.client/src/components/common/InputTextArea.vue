<script setup lang="ts">
  import { tidy } from './utils';

  const model = defineModel<string>();

  defineProps({
    label: { type: String, required: true },
    name: { type: String, required: true },
    id: { type: String, required: true },
    error: { type: String, required: false },
    maxlength: { type: Number, required: false },
    help: { type: String, required: false },
  });
</script>

<template>
  <div>
    <label :for="`input-${id}`">{{ label }}</label>
    <span>
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
    </span>
  </div>
</template>
