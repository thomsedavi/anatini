<script setup lang="ts">
  import type { PostEdit, StatusActions } from '@/types';
  import { apiFetchAuthenticated } from '../common/apiFetch';

  const props = defineProps<{
    channelId: string,
    postId: string,
    post: PostEdit,
    pageStatus: string,
    eTag: string | null,
  }>();

  const emit = defineEmits<{
    'update-etag': [eTag: string | null],
    'update-page-status': [newPageStatus: string],
    'update-post-status': [newPostStatus: 'Draft' | 'Published'],
  }>();

  function setStatus(status: 'Draft' | 'Published'): void {
    if (props.eTag === null) {
      return;
    }

    emit('update-page-status', status === 'Draft' ? 'Unpublishing...' : 'Publishing...');

    const body = new FormData();
      
    body.append('status', status);

    const init = { method: "PATCH", headers: { "If-Match": props.eTag }, body: body };

    const statusActions: StatusActions = {
        204: (response?: Response) => {
          emit('update-etag', response?.headers.get("ETag") ?? null);
          emit('update-post-status', status);
          emit('update-page-status', 'Ready');
        }
    }

    apiFetchAuthenticated(`channels/${props.channelId}/posts/${props.postId}`, statusActions, init);
  }
</script>

<template>
  <section id="panel-status" role="tabpanel" aria-labelledby="tab-status">
    <header>
      <h2>Status</h2>
    </header>

    <p>This article is currently {{ post.status.toLowerCase() }}.</p>

    <p v-if="post.status === 'Published'">Republish to update with any changes.</p>

    <menu>
      <li>
        <button type="button" @click="() => setStatus('Published')" :aria-disabled="pageStatus !== 'Ready' ? true : undefined">{{ post.status === 'Published' ? 'Republish' : 'Publish' }}</button>
      </li>
      <li>
        <button type="button" @click="() => setStatus('Draft')" v-if="post.status !== 'Draft'" :aria-disabled="pageStatus !== 'Ready' ? true : undefined">Unpublish</button>
      </li>
    </menu>
  </section>
</template>
