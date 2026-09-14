<script setup lang="ts">
  import type { Post, StatusActions } from '@/common/types';
  import { formatLong } from '@/common/dateUtils';
  import { onMounted } from 'vue';
  import { apiFetchAuthenticated } from '@/common/apiFetch';
  import { handleClick } from '@/common/utils';
  import { useRouter } from 'vue-router';

  const router = useRouter();

  const props = defineProps<{
    dataSpaceId: string,
    dataPosts: Post[] | null,
  }>();

  const emit = defineEmits<{
    'update-posts': [newPosts: Post[]],
  }>();

  onMounted(() => {
    if (props.dataPosts === null) {
      const input = `spaces/${props.dataSpaceId}/posts`;

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
</script>

<template>
  <section id="panel-posts" role="tabpanel" aria-labelledby="tab-posts">
    <header>
      <h2>Posts</h2>
      <RouterLink :to="{ name: 'SpaceEditPostCreate' }">+ Create Post</RouterLink>
    </header>

    <ul role="list" v-if="dataPosts !== null">
      <li v-for="post in dataPosts" :key="'post' + post.id">
        <article v-html="`${post.article}<footer><time datetime='${post.publishedAtNz}'>${formatLong(post.publishedAtNz)}</time><menu><li><a href='/spaces/${dataSpaceId}/edit/posts/${post.handle ?? post.id}/edit'>Edit</a></li></menu></footer>`" @click.prevent="(mouseEvent) => handleClick(mouseEvent, router)">
        </article>
      </li>
    </ul>

    <p v-else>You do not have any posts</p>
  </section>
</template>
