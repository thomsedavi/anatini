<script setup lang="ts">

</script>

<template>
  <main id="main" tabindex="-1">
    <article aria-labelledby="heading-main">
      <header>
        <h1 id="heading-main">Title Of Main Article</h1>

        <address>
          <a href="/channels/channel-name">Island Style</a>
        </address>

        <time datetime="2025-10-10">October 10, 2025</time>
      </header>

      <p>Lorem ipsum dolor sit amet.</p>

      <p>Lorem ipsum dolor sit amet.</p>

      <aside aria-labelledby="comp1">
        <article>
          <header>
            <h2 id="comp1">
              <a href="/channels/my-channel/contents/my-content">Title Of Related Article With Embedded Card</a>
            </h2>
          </header>

          <p>Lorem ipsum dolor sit amet.</p>

          <footer>
            <time datetime="2025-10-10">October 10, 2025</time>
          </footer>
        </article>
      </aside>

      <section aria-labelledby="h2">
        <h2 id="h2">Lorem ipsum dolor sit amet.</h2>

        <p>Lorem ipsum dolor sit amet.</p>

        <section aria-labelledby="h3">
          <h3 id="h3">Lorem ipsum dolor sit amet.</h3>

          <p>Lorem ipsum dolor sit amet.</p>
        </section>
      </section>

      <footer>
        <p><a href="#">Back to top</a></p>
      </footer>
    </article>
  </main>
</template>
