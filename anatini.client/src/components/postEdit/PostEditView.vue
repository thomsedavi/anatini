<script setup lang="ts">
  import type { PostEdit, StatusActions, Tab, APIResponse } from '@/types';
  import { nextTick, onMounted, ref, watch } from 'vue';
  import { useRoute, useRouter } from 'vue-router';
  import { getTabIndex, parseFromString, parseSource, type Source } from '../common/utils';
  import { apiFetchAuthenticated } from '../common/apiFetch';
  import TabButton from '../common/TabButton.vue';

  const headingMainRef = ref<HTMLElement | null>(null);

  const route = useRoute();
  const router = useRouter();

  const post = ref<APIResponse<PostEdit>>({ fetching: true });
  const article = ref<Node | null>(null);
  const eTag = ref<string | null>(null);
  const tabIndex = ref<number>(0);
  const pageStatus = ref<string>('Loading post...');

  const tabRefs = ref<HTMLButtonElement[]>([]);

  const tabs: Tab[] = [
    { id: 'article', text: 'Article', name: 'PostEditArticle' },
    { id: 'details', text: 'Details', name: 'PostEditDetails' },
    { id: 'status', text: 'Status', name: 'PostEditStatus' }
  ];

  onMounted(() => {
    tabIndex.value = tabs.findIndex(tab => tab.name === route.name);
  });

  watch([() => route.params.spaceId, () => route.params.postId], (source: Source) => fetchPost(parseSource(source)), { immediate: true });

  async function fetchPost(params: string[]) {
    const input = `spaces/${params[0]}/posts/${params[1]}/edit`;

    const statusActions: StatusActions = {
      200: (response?: Response) => {
        response?.json()
          .then((value: PostEdit) => {
            post.value = { data: value };
            pageStatus.value = 'Ready';

            eTag.value = response.headers.get("ETag");

            const node = parseFromString(value.version.article);

            if (node?.nodeName === 'ARTICLE') {
              article.value = node;
            }

            nextTick(() => {
              headingMainRef.value?.focus();
            });
          })
          .catch(() => { post.value = { error: { heading: 'Unknown Error', body: 'There was a problem fetching your post, please reload the page' }}});
      }
    }

    apiFetchAuthenticated({ input, statusActions });
  }

  function getHeading(): string {
    if (post.value.fetching === true) {
      return 'Fetching...';
    } else if (post.value.error !== undefined) {
      return post.value.error.heading;
    } else if (post.value.data !== undefined) {
      return 'Post Edit';
    } else {
      return 'Unknown Error';
    }
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

  function handleUpdateName(newName: string): void {
    if (post.value.data !== undefined) post.value.data.version.name = newName;
  }

  function handleUpdateDateNZ(newDateNZ: string): void {
    if (post.value.data !== undefined) post.value.data.version.dateNZ = newDateNZ;
  }

  function handleUpdatePostStatus(newPostStatus: 'Draft' | 'Published'): void {
    if (post.value.data !== undefined) post.value.data.status = newPostStatus;
  }

  function handleUpdateETag(newETag: string | null): void {
    eTag.value = newETag;
  }

  function handleUpdateArticle(newArticle: Node | null): void {
    article.value = newArticle;
  }
</script>

<template>
  <main id="main" tabindex="-1" :aria-busy="post === null">
    <header>
      <h1 ref="headingMainRef" tabindex="-1">{{ getHeading() }}</h1>
    </header>

    <section v-if="post.fetching === true">                
      <progress max="100">Fetching post...</progress>
    </section>

    <section v-if="post.error !== undefined">
      <p>
        {{ post.error.body }}
      </p>
    </section>

    <template v-if="post.data !== undefined">
      <ul role="tablist" aria-label="Post Settings">
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
          :article="article"
          :post="post"
          :spaceId="post.data.spaceId"
          :postId="post.data.id"
          :pageStatus="pageStatus"
          :name="post.data.version.name"
          :dateNZ="post.data.version.dateNZ"
          :eTag="eTag"
          @update-etag="handleUpdateETag"
          @update-article="handleUpdateArticle"
          @update-name="handleUpdateName"
          @update-date-nz="handleUpdateDateNZ"
          @update-post-status="handleUpdatePostStatus"
        />
      </RouterView>
    </template>

    <p role="status" class="visuallyhidden">{{ pageStatus }}</p>
  </main>
</template>
