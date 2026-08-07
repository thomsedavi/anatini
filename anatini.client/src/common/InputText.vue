<script setup lang="ts">
  import type { InputAutoCompleteAttribute, InputTypeHTMLAttribute } from 'vue';

  const model = defineModel<string | number>();

  defineProps<{
    label?: string,
    name: string,
    id: string,
    placeholder?: string,
    pattern?: string,
    error?: string,
    controls?: string,
    maxlength?: number,
    help?: string,
    autocomplete?: InputAutoCompleteAttribute,
    type?: InputTypeHTMLAttribute,
    required?: boolean,
    disabled?: boolean,
    hidden?: boolean,
    readonly?: boolean,
    min?: number | string,
    max?: number | string,
    step?: number | string,
    helpclass?: string,
    input?: (payload: InputEvent) => void,
    change?: (payload: Event) => void,
  }>();
</script>

<template>
  <label v-if="label" :for="`input-${id}`" :hidden="hidden ?? undefined">{{ label }}</label>
  <input
    :type="type ?? 'text'"
    :id="`input-${id}`"
    v-model="model"
    :name="name"
    :maxlength="maxlength ?? undefined"
    :aria-describedby="help ? `help-${id}` : undefined"
    :aria-invalid="error ? true : undefined"
    :aria-errormessage="error ? `error-${id}` : undefined"
    :aria-controls="controls ?? undefined"
    :autocomplete="autocomplete ?? 'on'"
    :required="required ? true : undefined"
    :hidden="hidden ?? undefined"
    :disabled="disabled ?? undefined"
    :readonly="readonly ?? undefined"
    :pattern="pattern ?? undefined"
    :min="min ?? undefined"
    :max="max ?? undefined"
    :placeholder="placeholder ?? undefined"
    :step="step ?? undefined"
    @input="input"
    @change="change"
    :aria-required="required ? true : undefined" />
  <small v-if="help" :id="`help-${id}`" :hidden="hidden ?? undefined" :class="helpclass ?? undefined">{{ help }}</small>
  <small v-if="error" :id="`error-${id}`" role="alert" :hidden="hidden ?? undefined">{{ error }}</small>
</template>
