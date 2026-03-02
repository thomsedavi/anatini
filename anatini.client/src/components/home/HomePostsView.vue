<script setup lang="ts">
  defineProps<{
    attributePosts: { name: string, postChannelHandle: string, postHandle: string, dateNZ: string }[] | null,
  }>();

  const formatter = new Intl.DateTimeFormat('en-US', {
    dateStyle: 'long'
  });
</script>

<template>
  <section id="panel-posts" role="tabpanel" aria-labelledby="tab-posts">
    <ul role="list">
      <li v-for="(attributePost, index) in attributePosts" :key="`index-${index}`">
        <article>
          <header>
            <h2>
              <RouterLink :to="{ name: 'Post', params: { channelId: attributePost.postChannelHandle, postId: attributePost.postHandle }}">{{ attributePost.name }}</RouterLink>
            </h2>
          </header>

          <p>Description goes here</p>

          <footer>
            <small><time :datetime="attributePost.dateNZ">{{ formatter.format(new Date(attributePost.dateNZ)) }}</time></small>
          </footer>
        </article>
      </li>
    </ul>
  </section>
</template>
