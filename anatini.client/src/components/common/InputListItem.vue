<script setup lang="ts">
  const model = defineModel<string>();

  const props = defineProps({
    label: { type: String, required: true },
    name: { type: String, required: true },
    id: { type: String, required: true },
    error: { type: String, required: false },
    maxlength: {type: Number, required: true }
  });
</script>

<template>
  <li>
      <label :for="`${id}-input`">{{ props.label }}</label>
      <input
      type="text"
      :id="`${id}-input`"
      v-model="model"
      :name="name"
      :maxlength="maxlength"
      :aria-describedby="`${id}-help`"
      :aria-invalid="error ? true : undefined"
      :aria-errormessage="error ? `${id}-error` : undefined"
      :autocomplete="id"
      required />
      <small :id="`${id}-help`">Channel Slug</small>
      <small v-if="error" :id="`${id}-error`" role="alert">{{ error ?? 'Unknown Error' }}</small>
   </li>
</template>
