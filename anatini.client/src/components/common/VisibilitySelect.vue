<script setup lang="ts">
  import type { Visibility } from '@/types';
  import { ref } from 'vue';

  const model = defineModel<Visibility>();

  const visibilityOptions = ref([
    { text: 'Public', value: 'Public' },
    { text: 'Protected', value: 'Protected' },
    { text: 'Private', value: 'Private' }
  ]);
</script>

<template>
  <label for="input-visibility">Visibility Level</label>
  <select name="visibility" id="input-visibility" v-model="model" aria-describedby="help-visibility">
    <option v-for="option in visibilityOptions" :value="option.value" :key="'visibility' + option.value">
      {{ option.text }}
    </option>
  </select>
  <small id="help-visibility">Publicly visible, protected to only be visible to trusted users, or private to only be visible to privately trusted users, I need to reword this to explain it better</small>
</template>
