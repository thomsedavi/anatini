<script setup lang="ts">
  import type { ContentEdit, ErrorMessage, StatusActions } from '@/types';
  import { nextTick, ref, watch } from 'vue';
  import { useRoute } from 'vue-router';
  import { parseSource, type Source } from './common/utils';
  import { apiFetchAuthenticated } from './common/apiFetch';
  const headingMainRef = ref<HTMLElement | null>(null);

  const route = useRoute();

  const content = ref<ContentEdit | ErrorMessage | null>(null);

  watch([() => route.params.channelId, () => route.params.contentId], (source: Source) => fetchContent(parseSource(source)), { immediate: true });

  async function fetchContent(params: string[]) {
    const statusActions: StatusActions = {
      200: (response?: Response) => {
        response?.json()
          .then((value: ContentEdit) => {
            content.value = value;

            nextTick(() => {
              headingMainRef.value?.focus();
            });
          })
          .catch(() => { content.value = { error: true, heading: 'Unknown Error', body: 'There was a problem fetching your content, please reload the page' }});
      }
    }

    apiFetchAuthenticated(`channels/${params[0]}/contents/${params[1]}/edit`, statusActions);
  }

  function getHeading(): string {
    if (content.value === null) {
      return 'Fetching...';
    } if ('error' in content.value) {
      return content.value.heading;
    } else {
      return 'Content Edit';
    }
  }
</script>

<template>
  <main id="main" tabindex="-1" :aria-busy="content === null">
    <header>
      <h1 ref="headingMainRef" tabindex="-1">{{ getHeading() }}</h1>
    </header>

    <section v-if="content === null">                
      <progress max="100">Fetching content...</progress>
    </section>

    <section v-else-if="'error' in content">
      <p>
        {{ content.body }}
      </p>
    </section>

    <template v-else>
      <p>Edit: {{ content.version.article }}</p>
    </template>
  </main>
</template>
