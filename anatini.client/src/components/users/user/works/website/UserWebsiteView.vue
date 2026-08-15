<script setup lang="ts">
  import { apiFetch } from '@/common/apiFetch';
  import type { APIResponse, StatusActions, Website } from '@/common/types';
  import { parseSource, type Source } from '@/common/utils';
  import { nextTick, ref, watch } from 'vue';
  import { useRoute } from 'vue-router';

  const route = useRoute();

  const website = ref<APIResponse<Website>>({ fetching: true });

  watch([() => route.params.userId, () => route.params.websiteId], (source: Source) => fetchWebsite(parseSource(source)), { immediate: true });

  async function fetchWebsite(params: string[]) {
    const input = `users/${params[0]}/websites/${params[1]}`;

    const statusActions: StatusActions = {
      200: (response?: Response) => {
        response?.json()
          .then((value: Website) => {
            website.value = { data: value };

            nextTick(() => {
              document.querySelector('h1')?.focus();
            });
          })
          .catch(() => {
            website.value = { error: { heading: 'Unknown Error', body: 'There was a problem fetching your website, please reload the page' }};
          });
      },
      404: () => {
        website.value = { error: { heading: '404 Not Found', body: 'Website not found' }};
      }
    }

    apiFetch({ input, statusActions });
  }

  function getMainHtml(): string {
    if (website.value.fetching === true) {
      return '<p>Loading...</p>';
    } else if (website.value.error !== undefined) {
      return `<h1>${ website.value.error.body }</h1>`;
    } else if (website.value.data !== undefined) {
      return '<article><p>text</p></article>';
    } else {
      return '<h1>Unknown Error</h1>';
    }
  }
</script>

<template>
  <main id="main" tabindex="-1" :aria-busy="website.fetching === true" v-html="getMainHtml()"></main>
</template>
