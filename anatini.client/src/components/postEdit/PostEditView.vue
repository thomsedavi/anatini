<script setup lang="ts">
  import type { PostEdit, ErrorMessage, StatusActions, Tab } from '@/types';
  import { nextTick, ref, watch } from 'vue';
  import { useRoute, useRouter } from 'vue-router';
  import { getTabIndex, parseFromString, parseSource, type Source } from '../common/utils';
  import { apiFetchAuthenticated } from '../common/apiFetch';
  import TabButton from '../common/TabButton.vue';

  const headingMainRef = ref<HTMLElement | null>(null);

  const route = useRoute();
  const router = useRouter();

  const post = ref<PostEdit | ErrorMessage | null>(null);
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

  watch([() => route.params.channelId, () => route.params.postId], (source: Source) => fetchPost(parseSource(source)), { immediate: true });

  async function fetchPost(params: string[]) {
    const statusActions: StatusActions = {
      200: (response?: Response) => {
        response?.json()
          .then((value: PostEdit) => {
            post.value = value;
            pageStatus.value = 'Ready';

            eTag.value = response.headers.get("ETag");

            const node = parseFromString(post.value.version.article);

            if (node?.nodeName === 'ARTICLE') {
              article.value = node;
            }

            nextTick(() => {
              headingMainRef.value?.focus();
            });
          })
          .catch(() => { post.value = { error: true, heading: 'Unknown Error', body: 'There was a problem fetching your post, please reload the page' }});
      }
    }

    apiFetchAuthenticated(`channels/${params[0]}/posts/${params[1]}/edit`, statusActions);
  }

  function getHeading(): string {
    if (post.value === null) {
      return 'Fetching...';
    } if ('error' in post.value) {
      return post.value.heading;
    } else {
      return 'Post Edit';
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
    if (post.value !== null && 'version' in post.value) {
     post.value.version.name = newName;
    }
  }

  function handleUpdateDateNZ(newDateNZ: string): void {
    if (post.value !== null && 'version' in post.value) {
     post.value.version.dateNZ = newDateNZ;
    }
  }

  function handleUpdatePostStatus(newPostStatus: 'Draft' | 'Published'): void {
    if (post.value !== null && 'version' in post.value) {
     post.value.status = newPostStatus;
    }
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

    <section v-if="post === null">                
      <progress max="100">Fetching post...</progress>
    </section>

    <section v-else-if="'error' in post">
      <p>
        {{ post.body }}
      </p>
    </section>

    <template v-else>
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
          :channelId="post.channelId"
          :postId="post.id"
          :pageStatus="pageStatus"
          :name="post.version.name"
          :dateNZ="post.version.dateNZ"
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
