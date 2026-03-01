<script setup lang="ts">
  defineProps<{
    channelId: string,
  }>();
</script>

<template>
  <section id="panel-posts" role="tabpanel" aria-labelledby="tab-posts">
    <header>
      <h2>Posts</h2>
      <RouterLink :to="{ name: 'PostCreate' }">+ Create Post</RouterLink>
    </header>

    <section aria-labelledby="section-your-posts">
      <header>
        <h3 id="section-your-posts">Your Posts</h3>
      </header>

      <ul role="list" v-if="true">
        <li>
          <article :aria-labelledby="`post-handle`">
            <header>
              <h4 :id="`post-handle`">
                <RouterLink :to="{ name: 'PostEdit', params: { channelId: channelId, postId: 'handle' }}">Name</RouterLink>
              </h4>
            </header>

            <p>Post Description Goes Here</p>

            <footer>
              <small>Handle: <code>handle</code></small>
            </footer>
          </article>
        </li>
      </ul>

      <p v-else>You do not have any posts</p>
    </section>
  </section>
</template>
