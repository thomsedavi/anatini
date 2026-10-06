<script setup lang="ts">
  import type { Filters } from '@/common/filters';
  import RadioFieldset from '@/common/RadioFieldset.vue';

  const model = defineModel<Filters>({ required: true });
</script>

<template>
  <details>
    <summary>Starred Posts</summary>

    <RadioFieldset
      v-model="model.starred"
      :radios="[
        { name: 'starred', value: 'all', id: 'starredAll', label: 'No filter' },
        { name: 'starred', value: 'only', id: 'starredOnly', label: 'Show only starred' },
        { name: 'starred', value: 'hide', id: 'starredHide', label: 'Hide starred' }
      ]"
      legend="Starred Posts Options" />
  </details>

  <details>
    <summary>Bookmarked Posts</summary>

    <RadioFieldset
      v-model="model.bookmarked"
      :radios="[
        { name: 'bookmarked', value: 'all', id: 'bookmarkedAll', label: 'No filter' },
        { name: 'bookmarked', value: 'only', id: 'bookmarkedOnly', label: 'Show only bookmarked' },
        { name: 'bookmarked', value: 'hide', id: 'bookmarkedHide', label: 'Hide bookmarked' }
      ]"
      legend="Bookmarked Posts Options" />
  </details>

  <details>
    <summary>Dismissed Posts</summary>

    <RadioFieldset
      v-model="model.dismissed"
      :radios="[
        { name: 'dismissed', value: 'all', id: 'dismissedAll', label: 'No filter' },
        { name: 'dismissed', value: 'only', id: 'dismissedOnly', label: 'Show only dismissed' },
        { name: 'dismissed', value: 'hide', id: 'dismissedHide', label: 'Hide dismissed' }
      ]"
      legend="Dismissed Posts Options" />
  </details>

  <details>
    <summary>Followed Users</summary>

    <RadioFieldset
      v-model="model.followed"
      :radios="[
        { name: 'followed', value: 'all', id: 'followedAll', label: 'No filter' },
        { name: 'followed', value: 'only', id: 'followedOnly', label: 'Show only followed' },
        { name: 'followed', value: 'hide', id: 'followedHide', label: 'Hide followed' }
      ]"
      legend="Followed Users Options" />
  </details>
</template>
