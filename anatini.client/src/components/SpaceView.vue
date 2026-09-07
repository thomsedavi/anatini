<script setup lang="ts">
  import type { APIResponse, InputError, Post, Space, Status, StatusActions, Tab, Work } from '@/common/types';
  import { nextTick, ref, watch } from 'vue';
  import { useRoute, useRouter } from 'vue-router';
  import { apiFetch } from '@/common/apiFetch';
  import { getTabIndex, parseSource, type Source } from '@/common/utils';
  import TabButton from '@/common/TabButton.vue';
  
  const route = useRoute();
  const router = useRouter();

  const space = ref<APIResponse<Space>>({ fetching: true });
  const errorSectionRef = ref<HTMLElement | null>(null);
  const inputErrors = ref<InputError[]>([]);
  const status = ref<Status>('idle');
  const tabIndex = ref<number>(-1);
  const posts = ref<Post[] | null>(null);
  const works = ref<Work[] | null>(null);

  const tabs: Tab[] = [
    { id: 'posts', text: 'Posts', name: 'SpacePosts', childNames: ['SpacePost', 'SpacePostCreate', 'SpacePostEdit'] },
    { id: 'works', text: 'Works', name: 'SpaceWorks', childNames: ['SpaceWork', 'SpaceWorkCreate'] },
    { id: 'events', text: 'Events', name: 'SpaceEvents', childNames: ['SpaceEventCreate'] },
  ];

  const tabRefs = ref<HTMLButtonElement[]>([]);

  watch([() => route.params.spaceId], (source: Source) => fetchSpace(parseSource(source)), { immediate: true });

  async function fetchSpace(params: string[]) {
    tabIndex.value = tabs.findIndex(tab => tab.name === route.name || tab.childNames?.includes(route.name));

    space.value = { fetching: true };

    const input = `spaces/${params[0]}`;

    const statusActions: StatusActions = {
      200: (response?: Response) => {
        response?.json()
          .then((value: Space) => {
            space.value = {
              data: { ...value, about: value.about?.replace(/\r\n/g, "\n") ?? null }
            };
          })
          .catch(() => {
            space.value = { error: { heading: 'Unknown Error', body: 'There was a problem fetching this space, please reload the page' }};
          });
      },
      404: () => {
        space.value = { error: { heading: '404 Not Found', body: 'Space not found' }};
      },
      500: () => {
        space.value = { error: { heading: 'Unknown Error', body: 'There was a problem fetching this space, please reload the page' }};
      }
    };

    apiFetch({ input, statusActions });
  }

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

  function handleUpdatePosts(newPosts: Post[]): void {
    posts.value = newPosts;
  }

  function handleUpdateWorks(newWorks: Work[]): void {
    works.value = newWorks;
  }

  function handleUpdateErrors(newInputErrors: InputError[]): void {
    inputErrors.value = newInputErrors;

    if (newInputErrors.length > 0) {
      nextTick(() => {
        errorSectionRef.value?.focus();
      });
    }
  }
</script>

<template>
  <main id="main" tabindex="-1">
    <template v-if="space.data !== undefined">
      <ul role="tablist" aria-label="Space Content">
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
          :data-status="status"
          :data-input-errors="inputErrors"
          :data-posts="posts"
          :data-works="works"
          :data-space-id="space.data.id"
          :data-space-handle="space.data.handle"
          :data-space-name="space.data.name"
          @update-posts="handleUpdatePosts"
          @update-works="handleUpdateWorks"
          @update-errors="handleUpdateErrors"
        />
      </RouterView>
    </template>
  </main>
</template>
