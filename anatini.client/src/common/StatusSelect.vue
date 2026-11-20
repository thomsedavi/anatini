<script setup lang="ts">
  import { ref } from 'vue';
  import InputSelect from '@/common/InputSelect.vue';
  import type { ContentStatus } from '@/common/types';

  const model = defineModel<ContentStatus>({ required: true });

  const props = defineProps<{
    canArchive: boolean,
  }>();

  const statusOptions = ref([
    { text: 'Draft', value: 'Draft' },
    { text: 'Published', value: 'Published' },
    { text: 'Archived', value: 'Archived', active: props.canArchive }
  ]);
</script>

<template>
  <InputSelect
    v-model="model"
    label="Status"
    id="Status"
    name="Status"
    :options="statusOptions"
    help="Draft or Published" />
</template>
