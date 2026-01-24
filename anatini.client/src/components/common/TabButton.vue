<script setup lang="ts">
  const emit = defineEmits<{
    (e: 'click'): void;
    (e: 'keydown', event: KeyboardEvent): void;
  }>();

  defineProps({
    selected: { type: Boolean, required: true },
    text: { type: String, required: true },
    id: { type: String, required: true },
    addButtonRef: { type: Function, required: true},
  });
</script>

<template>
  <li role="presentation">
    <button 
      :ref="(ref) => addButtonRef(ref)"
      :id="`tab-${id}`" 
      role="tab" 
      :aria-selected="selected"
      :aria-controls="`panel-${id}`" 
      type="button"
      @click="emit('click')"
      @keydown="(payload) => emit('keydown', payload)"
      :tabindex="selected ? undefined : -1">
      {{ text }}
    </button>
  </li>
</template>
