<script setup lang="ts">
  const model = defineModel<string>();

  defineProps<{
    label?: string,
    name: string,
    id: string,
    error?: string,
    maxlength?: number,
    help?: string,
    autocomplete?: string,
    type?: string,
    required?: boolean,
    disabled?: boolean,
    readonly?: boolean,
  }>();
</script>

<template>
  <label v-if="label" :for="`input-${id}`">{{ label }}</label>
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
    :disabled="disabled ?? undefined"
    :readonly="readonly ?? undefined"
    :aria-required="required ? true : undefined" />
  <small v-if="help" :id="`help-${id}`">{{ help }}</small>
  <small v-if="error" :id="`error-${id}`" role="alert">{{ error }}</small>
</template>
