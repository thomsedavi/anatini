<script setup lang="ts">
  import { apiFetch } from '@/common/apiFetch';
  import { buttonAction, getPostHtml } from '@/common/html';
  import { store } from '@/common/store';
  import type { Post, StatusActions } from '@/common/types';
  import { handleClick } from '@/common/utils';
  import { onMounted } from 'vue';
  import { useRouter } from 'vue-router';

  const router = useRouter();

  const props = defineProps<{
    dataPosts: Post[] | null,
  }>();

  const emit = defineEmits<{
    'update-posts': [newPosts: Post[]],
  }>();

  onMounted(() => {
    if (props.dataPosts === null) {
      const input = 'posts';

      const statusActions: StatusActions = {
        200: (response?: Response) => {
          response?.json()
            .then((value: Post[]) => {
              emit('update-posts', value);
            });
        }
      }

      apiFetch({ input, statusActions });
    }
  });
</script>

<template>
  <section id="panel-posts" role="tabpanel" aria-labelledby="tab-posts">
    <header>
      <h2>Posts</h2>
      <RouterLink :to="{ name: 'UserPostCreate', params: { userId: store.userHandle } }">+ Create Post</RouterLink>
    </header>

    <ul role="list" v-if="dataPosts !== null">
      <li v-for="post in dataPosts" :key="'post' + post.id">
        <article v-html="getPostHtml(post)" @click.prevent="(mouseEvent) => handleClick(mouseEvent, router, (label, pressed) => buttonAction(label, pressed, post))">
        </article>
      </li>
    </ul>

    <p v-else>There are not any posts</p>
  </section>
</template>
