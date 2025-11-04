<script setup lang="ts">
  import { ref, watch } from 'vue';
  import { useRoute } from 'vue-router';
  import { apiFetch } from './common/apiFetch';
  import type { Content } from '@/types';

  const route = useRoute();

  const loading = ref<boolean>(false);
  const content = ref<Content | null>(null);

  watch([() => route.params.channelId, () => route.params.contentId], fetchContent, { immediate: true });

  async function fetchContent(array: (() => string | string[])[]) {
    loading.value = true;

    const onfulfilled = (value: Content) => {
      content.value = value;
    };

    const onfinally = () => {
      loading.value = false;
    };

    await apiFetch(`channels/${array[0]}/contents/${array[1]}/preview`, onfulfilled, onfinally);
  }
</script>

<template>
  <main>
    <p v-if="loading">Loading...</p>
    <template v-else-if="content">
      <template v-if="content.version.elements">
        <template v-for="element in content.version.elements.sort((a, b) => a.index > b.index ? 1 : -1)" :key="'element' + element.index">
          <template v-if="element.content !== null">
            <h1 v-if="element.tag == 'h1'">{{ element.content }}</h1>
            <h2 v-else-if="element.tag == 'h2'">{{ element.content }}</h2>
            <h3 v-else-if="element.tag == 'h3'">{{ element.content }}</h3>
            <h4 v-else-if="element.tag == 'h4'">{{ element.content }}</h4>
            <h5 v-else-if="element.tag == 'h5'">{{ element.content }}</h5>
            <h6 v-else-if="element.tag == 'h6'">{{ element.content }}</h6>
            <p v-else-if="element.tag == 'p'">{{ element.content }}</p>
          </template>
        </template>
      </template>
    </template>
  </main>
</template>