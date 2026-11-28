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
    <summary>Liked {{contentType}}s</summary>

    <RadioFieldset
      v-model="model.liked"
      :radios="[
        { name: 'liked', value: 'all', id: 'likedAll', label: 'No filter' },
        { name: 'liked', value: 'only', id: 'likedOnly', label: 'Show only liked' },
        { name: 'liked', value: 'hide', id: 'likedHide', label: 'Hide liked' }
      ]"
      :legend="`Liked ${contentType} Options`" />
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
    <summary>Hidden {{contentType}}s</summary>

    <RadioFieldset
      v-model="model.hidden"
      :radios="[
        { name: 'hidden', value: 'all', id: 'hiddenAll', label: 'No filter' },
        { name: 'hidden', value: 'show', id: 'hiddenShow', label: 'Show hidden' },
        { name: 'hidden', value: 'only', id: 'hiddenOnly', label: 'Show only hidden' }
      ]"
      :legend="`Hidden ${contentType} Options`" />
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
