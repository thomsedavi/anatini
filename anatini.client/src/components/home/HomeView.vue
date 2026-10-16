<script setup lang="ts">
  import { nextTick, onMounted, ref } from 'vue';
  import type { EventOccurrence, Post, Tab, Work } from '@/common/types';
  import TabButton from '@/common/TabButton.vue';
  import { getTabIndex } from '@/common/utils';
  import { useRoute, useRouter } from 'vue-router';

  const route = useRoute();
  const router = useRouter();

  const tabIndex = ref<number>(-1);
  const posts = ref<Post[] | null>(null);
  const works = ref<Work[] | null>(null);
  const eventOccurrences = ref<EventOccurrence[] | null>(null);

  const tabs: Tab[] = [
    { id: 'posts', text: 'Posts', name: 'HomePosts' },
    { id: 'works', text: 'Works', name: 'HomeWorks' },
    { id: 'events', text: 'Events', name: 'HomeEvents' },
  ];

  const tabRefs = ref<HTMLButtonElement[]>([]);

  onMounted(() => {
    tabIndex.value = tabs.findIndex(tab => tab.name === route.name);
  });

  function handleKeyDown(event: KeyboardEvent, index: number): void {
    event.preventDefault();
    
    const newIndex = getTabIndex(event.key, index, tabs.length);

    if (newIndex !== undefined) {
      const tab = tabs[newIndex];

      if (tab !== undefined) {
        tabIndex.value = newIndex;

        router.push({ name: tab.name });
      
        nextTick(() => {
          tabRefs.value[newIndex]?.focus();
        });
      }
    }
  }

  function handleClick(index: number): void {
    const tab = tabs[index];

    if (tab !== undefined) {
      tabIndex.value = index;

      router.push({ name: tab.name });
      
      nextTick(() => {
        tabRefs.value[index]?.focus();
      });
    }
  }

  function handleUpdatePosts(newPosts: Post[]): void {
    posts.value = newPosts;
  }

  function handleUpdateWorks(newWorks: Work[]): void {
    works.value = newWorks;
  }

  function handleUpdateEventOccurrences(newEventOccurrences: EventOccurrence[]): void {
    eventOccurrences.value = newEventOccurrences;
  }
</script>

<template>
  <main id="main" tabindex="-1">
    <header class="visuallyhidden">
      <h1>Post Feed</h1>
    </header>

    <ul role="tablist" aria-label="Content Views">
      <TabButton v-for="(tab, index) in tabs"
        :key="tab.id"
        :selected="tabIndex === index"
        @click="() => handleClick(index)"
        @keydown="(event: KeyboardEvent) => handleKeyDown(event, index)"
        :text="tab.text"
        :id="tab.id"
        :add-button-ref="(el: HTMLButtonElement) => { tabRefs.push(el); }" />
    </ul>

    <RouterView v-slot="{ Component }">
      <component
        :is="Component"
        :data-posts="posts"
        :data-works="works"
        :data-event-occurrences="eventOccurrences"
        @update-posts="handleUpdatePosts"
        @update-works="handleUpdateWorks"
        @update-event-occurrences="handleUpdateEventOccurrences"
      />
    </RouterView>
  </main>
</template>
