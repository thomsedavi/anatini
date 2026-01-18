<script setup lang="ts">
  const model = defineModel<string>();

  defineProps({
    label: { type: String, required: true },
    name: { type: String, required: true },
    id: { type: String, required: true },
    error: { type: String, required: false },
    maxlength: { type: Number, required: false },
    help: { type: String, required: false },
    autocomplete: { type: String, required: false },
    type: { type: String, required: false },
    required: { type: Boolean, required: false }
  });
</script>

<template>
  <div>
    <label :for="`input-${id}`">{{ label }}</label>
    <span>
      <input
        :type="type ?? 'text'"
        :id="`input-${id}`"
        v-model="model"
        :name="name"
        :maxlength="maxlength ?? undefined"
        :aria-describedby="help ? `help-${id}` : undefined"
        :aria-invalid="error ? true : undefined"
        :aria-errormessage="error ? `error-${id}` : undefined"
        :autocomplete="autocomplete ?? 'on'"
        :required="required ? true : undefined"
        :aria-required="required ? true : undefined" />
      <small v-if="help" :id="`help-${id}`">{{ help }}</small>
      <small v-if="error" :id="`error-${id}`" role="alert">{{ error }}</small>
    </span>
  </div>
</template>
