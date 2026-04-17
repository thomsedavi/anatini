<script setup lang="ts">
  import { formatArticle, formatParagraph, tidy } from './utils';

  const model = defineModel<string>();

  defineProps<{
    label: string,
    name: string,
    id: string,
    error?: string,
    maxLength?: number,
    help?: string,
    isArticle?: boolean,
  }>();
</script>

<template>
  <label :for="`input-${id}`">{{ label }}</label>
  <textarea
    :id="`input-${id}`"
    v-model="model"
    :name="name"
    :maxlength="maxLength ?? undefined"
    :aria-invalid="error ? true : undefined"
    :aria-errormessage="error ? `error-${id}` : undefined"
    :aria-describedby="`${help ? `help-${id}` : undefined} ${maxLength ? `counter-${id}` : undefined}`"
    :aria-controls="isArticle ? `preview-${id}` : undefined"></textarea>
  <small v-if="help" :id="`help-${id}`">{{ help }}</small>
  <small v-if="error" :id="`error-${id}`" role="alert">{{ error }}</small>
  <output v-if="maxLength"
    :id="`counter-${id}`"
    :aria-live="maxLength - tidy(model ?? '').length < 20 ? 'assertive' : 'polite'"
    aria-atomic="true">
    Characters remaining: {{ maxLength - (isArticle ? formatArticle(model ?? '') : tidy(model ?? '')).length }}
  </output>
  <section v-if="isArticle" aria-label="Preview">
    <output :id="`preview-${id}`">
      <article v-html="tidy(model ?? '') !== '' ? formatParagraph(model ?? '') : '<p><em>Preview goes here</em></p>'"></article>
    </output>
  </section>
</template>
