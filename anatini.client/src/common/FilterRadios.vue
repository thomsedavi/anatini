<script setup lang="ts">
  import type { Filters } from '@/common/filters';
  import RadioFieldset from '@/common/RadioFieldset.vue';

  const model = defineModel<Filters>({ required: true });

  defineProps<{
    contentType: string,
  }>();
</script>

<template>
  <details>
    <summary>Starred {{contentType}}s</summary>

    <RadioFieldset
      v-model="model.starred"
      :radios="[
        { name: 'starred', value: 'all', id: 'starredAll', label: 'No filter' },
        { name: 'starred', value: 'only', id: 'starredOnly', label: 'Show only starred' },
        { name: 'starred', value: 'hide', id: 'starredHide', label: 'Hide starred' }
      ]"
      :legend="`Starred ${contentType} Options`" />
  </details>

  <details>
    <summary>Bookmarked {{contentType}}s</summary>

    <RadioFieldset
      v-model="model.bookmarked"
      :radios="[
        { name: 'bookmarked', value: 'all', id: 'bookmarkedAll', label: 'No filter' },
        { name: 'bookmarked', value: 'only', id: 'bookmarkedOnly', label: 'Show only bookmarked' },
        { name: 'bookmarked', value: 'hide', id: 'bookmarkedHide', label: 'Hide bookmarked' }
      ]"
      :legend="`Bookmarked ${contentType} Options`" />
  </details>

  <details>
    <summary>Dismissed {{contentType}}s</summary>

    <RadioFieldset
      v-model="model.dismissed"
      :radios="[
        { name: 'dismissed', value: 'all', id: 'dismissedAll', label: 'No filter' },
        { name: 'dismissed', value: 'show', id: 'dismissedShow', label: 'Show dismissed' },
        { name: 'dismissed', value: 'only', id: 'dismissedOnly', label: 'Show only dismissed' }
      ]"
      :legend="`Dismissed ${contentType} Options`" />
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
