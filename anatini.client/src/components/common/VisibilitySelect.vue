<script setup lang="ts">
  import type { Visibility } from '@/types';
  import { ref } from 'vue';
  import InputSelect from './InputSelect.vue';

  const model = defineModel<Visibility>();

  const visibilityOptions = ref([
    { text: 'Public', value: 'Public' },
    { text: 'Protected', value: 'Protected' },
    { text: 'Private', value: 'Private' }
  ]);
</script>

<template>
  <InputSelect
    v-model="model"
    label="Visibility Level"
    id="visibility"
    name="visibility"
    :options="visibilityOptions"
    help="Publicly visible, protected to only be visible to trusted users, or private to only be visible to privately trusted users, I need to reword this to explain it better" />
</template>
