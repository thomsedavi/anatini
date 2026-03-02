<script setup lang="ts">
  import { nextTick, onMounted, ref } from 'vue';
  import { apiFetch } from '../common/apiFetch';
  import type { StatusActions, Tab } from '@/types';
  import TabButton from '../common/TabButton.vue';
  import { getTabIndex } from '../common/utils';
  import { useRoute, useRouter } from 'vue-router';

  const route = useRoute();
  const router = useRouter();

  const tabIndex = ref<number>(0);
  const attributePosts = ref<{ name: string, postChannelHandle: string, postHandle: string, dateNZ: string }[] | null>(null);

  const tabs: Tab[] = [
    { id: 'posts', text: 'Posts', name: 'HomePosts' },
    { id: 'calendar', text: 'Calendar', name: 'HomeCalendar' }
  ];

  const tabRefs = ref<HTMLButtonElement[]>([]);

  onMounted(() => {
    const statusActions: StatusActions = {
      200: (response?: Response) => {
        tabIndex.value = tabs.findIndex(tab => tab.name === route.name);

        response?.json()
          .then((value: { attributePosts: { name: string, postChannelHandle: string, postHandle: string, dateNZ: string }[] }) => {
            attributePosts.value = value.attributePosts
          })
          .catch(() => { console.log('error') });
      }
    }

    apiFetch('posts?week=2026-W09', statusActions);
  });

  function handleKeyDown(event: KeyboardEvent, index: number): void {
    const newIndex = getTabIndex(event.key, index, tabs.length);

    if (newIndex === undefined) {
      return;
    }

    event.preventDefault();
    tabIndex.value = newIndex;

    router.push({ name: tabs[newIndex].name });
    
    nextTick(() => {
      tabRefs.value[newIndex].focus();
    })
  }

  function handleClick(index: number): void {
    tabIndex.value = index;

    router.push({ name: tabs[index].name });
    
    nextTick(() => {
      tabRefs.value[index].focus();
    })
  }
</script>

<template>
  <main id="main" tabindex="-1">
    <header class="visuallyhidden">
      <h1>Post Feed</h1>
    </header>

    <ul role="tablist" aria-label="Settings Options">
      <TabButton v-for="(tab, index) in tabs"
        :key="tab.id"
        :selected="tabIndex === index"
        @click="() => handleClick(index)"
        @keydown="event => handleKeyDown(event, index)"
        :text="tab.text"
        :id="tab.id"
        :add-button-ref="(el: HTMLButtonElement) => { tabRefs.push(el); }" />
    </ul>

    <RouterView v-slot="{ Component }">
      <component
        :is="Component"    
        :attribute-posts="attributePosts"
      />
    </RouterView>
  </main>
</template>
