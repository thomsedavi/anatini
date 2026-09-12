<script setup lang="ts">
  import type { Post, StatusActions } from '@/common/types';
  import { formatLong } from '@/common/dateUtils';
  import { onMounted } from 'vue';
  import { apiFetchAuthenticated } from '@/common/apiFetch';
  import { useRouter } from 'vue-router';
  import { handleClick } from '@/common/utils';

  const router = useRouter();

  const props = defineProps<{
    dataUserId: string,
    dataUserHandle: string,
    dataPosts: Post[] | null,
  }>();

  const emit = defineEmits<{
    'update-posts': [newPosts: Post[]],
  }>();

  onMounted(() => {
    if (props.dataPosts === null) {
      const input = `users/${props.dataUserId}/posts`;

      const statusActions: StatusActions = {
        200: (response?: Response) => {
          response?.json()
            .then((value: Post[]) => {
              emit('update-posts', value);
            });
        }
      }

      apiFetchAuthenticated({ input, statusActions });
    }
  });

  function getHeader(post: Post): string {
      return `<header><time datetime='${post.publishedAtNz}'>${formatLong(post.publishedAtNz)}</time></header>`;
  }

  function postHtml(post: Post): string {
    return `
      ${getHeader(post)}
      ${post.article}
      <footer>
        <menu>
          <li>
            <a href='/users/${props.dataUserHandle}/posts/${post.handle ?? post.id}/edit'>Edit</a>
          </li>
        </menu>
      </footer>
    `;
  }
</script>

<template>
  <section id="panel-posts" role="tabpanel" aria-labelledby="tab-posts">
    <header>
      <h2>Posts</h2>
      <RouterLink :to="{ name: 'UserPostCreate' }">+ Create Post</RouterLink>
    </header>

    <ul role="list" v-if="dataPosts !== null">
      <li v-for="post in dataPosts" :key="'post' + post.id">
        <article v-html="postHtml(post)" @click.prevent="(mouseEvent) => handleClick(mouseEvent, router)">
        </article>
      </li>
    </ul>

    <p v-else>You do not have any posts</p>
  </section>
</template>
