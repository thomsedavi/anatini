<script setup lang="ts">
  const model = defineModel<string>();

  defineProps({
    label: { type: String, required: true },
    name: { type: String, required: true },
    id: { type: String, required: true },
    error: { type: String, required: false },
    maxlength: { type: Number, required: true },
    help: { type: String, required: true },
    autocomplete: { type: String, required: false }
  });
</script>

<template>
  <label :for="`input-${id}`">{{ label }}</label>
  <span>
    <input
      type="text"
      :id="`input-${id}`"
      v-model="model"
      :name="name"
      :maxlength="maxlength"
      :aria-describedby="`help-${id}`"
      :aria-invalid="error ? true : undefined"
      :aria-errormessage="error ? `error-${id}` : undefined"
      :autocomplete="autocomplete ?? 'on'"
      required />
    <small :id="`help-${id}`">{{ help }}</small>
    <small v-if="error" :id="`error-${id}`" role="alert">{{ error ?? 'Unknown Error' }}</small>
  </span>
</template>
