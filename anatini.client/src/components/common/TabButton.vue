<script setup lang="ts">
  const emit = defineEmits<{
    (e: 'click'): void;
    (e: 'keydown', event: KeyboardEvent): void;
  }>();

  defineProps<{
    selected: boolean,
    text: string,
    id: string,
    addButtonRef: (ref: HTMLButtonElement) => void,
  }>();
</script>

<template>
  <li role="presentation">
    <button 
      :ref="(ref) => addButtonRef(ref as HTMLButtonElement)"
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
