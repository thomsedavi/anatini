<script lang="ts">
export default {
  methods: {
    goToAbout() {
      this.$router.push('/about')
    },
  },
}
</script>

<template>
  <main>
    <h1>Anatini</h1>
    <button type="button" @click="goToAbout">Go to About</button>
    <section aria-labelledby="recent-heading">
      <h2 id="recent-heading">Most Recent</h2>
      <ol>
        <li>
          <article>
            <a href="/channels/abc/contents/abc" aria-hidden="true" tabindex="-1">
              <img src="https://94e01200-c64f-4ff6-87b6-ce5a316b9ea8.mdnplay.dev/shared-assets/images/examples/grapefruit-slice.jpg" alt="test" width="250" height="250" />
            </a>
            <header>
              <h3><a href="/channels/abc/contents/abc">Where to go</a></h3>
              <p>By <a href="/channels/abc">Nimosity</a></p>
            </header>
            <p>How are you? Are you doing any good today? How are you? Are you doing any good today? How are you? Are you doing any good today? How are you? Are you doing any good today? How are you? Are you doing any good today?</p>
            <footer>
              <p>Tags: <RouterLink to="/tags/test">Test</RouterLink></p>
              <p>Posted on <time datetime="2025-10-10">2025-10-10</time></p>
            </footer>
          </article>
        </li>
        <li>
          <article>
            <a href="/channels/abc/contents/abc" aria-hidden="true" tabindex="-1">
              <img src="https://94e01200-c64f-4ff6-87b6-ce5a316b9ea8.mdnplay.dev/shared-assets/images/examples/grapefruit-slice.jpg" alt="test" width="250" height="250" />
            </a>
            <header>
              <h3><a href="/channels/abc/contents/abc">Where to go</a></h3>
              <p>By <a href="/channels/abc">Nimosity</a></p>
            </header>
            <p>How are you? Are you doing any good today? How are you? Are you doing any good today? How are you? Are you doing any good today? How are you? Are you doing any good today? How are you? Are you doing any good today?</p>
            <footer>
              <p>Posted on <time datetime="2025-10-10">2025-10-10</time></p>
            </footer>
          </article>
        </li>
        <li>
          <article>
            <a href="/channels/abc/contents/abc" aria-hidden="true" tabindex="-1">
              <img src="https://94e01200-c64f-4ff6-87b6-ce5a316b9ea8.mdnplay.dev/shared-assets/images/examples/grapefruit-slice.jpg" alt="test" width="250" height="250" />
            </a>
            <header>
              <h3><a href="/channels/abc/contents/abc">Where to go</a></h3>
              <p>By <a href="/channels/abc">Nimosity</a></p>
            </header>
            <p>How are you?</p>
            <footer>
              <p>Tags: <a href="/tags/test">Test</a></p>
              <p>Posted on <time datetime="2025-10-10">2025-10-10</time></p>
            </footer>
          </article>
        </li>
        <li>
          <article>
            <a href="/channels/abc/contents/abc" aria-hidden="true" tabindex="-1">
              <img src="https://94e01200-c64f-4ff6-87b6-ce5a316b9ea8.mdnplay.dev/shared-assets/images/examples/grapefruit-slice.jpg" alt="test" width="250" height="250" />
            </a>
            <header>
              <h3><a href="/channels/abc/contents/abc">Where to go</a></h3>
              <p>By <a href="/channels/abc">Nimosity</a></p>
            </header>
            <p>How are you?</p>
            <footer>
              <p>Tags: <a href="/tags/test">Test</a></p>
              <p>Posted on <time datetime="2025-10-10">2025-10-10</time></p>
            </footer>
          </article>
        </li>
        <li>
          <article>
            <a href="/channels/abc/contents/abc" aria-hidden="true" tabindex="-1">
              <img src="https://94e01200-c64f-4ff6-87b6-ce5a316b9ea8.mdnplay.dev/shared-assets/images/examples/grapefruit-slice.jpg" alt="test" width="250" height="250" />
            </a>
            <header>
              <h3><a href="/channels/abc/contents/abc">Where to go</a></h3>
              <p>By <a href="/channels/abc">Nimosity</a></p>
            </header>
            <p>How are you?</p>
            <footer>
              <p>Tags: <a href="/tags/test">Test</a></p>
              <p>Posted on <time datetime="2025-10-10">2025-10-10</time></p>
            </footer>
          </article>
        </li>
        <li>
          <article>
            <a href="/channels/abc/contents/abc" aria-hidden="true" tabindex="-1">
              <img src="https://94e01200-c64f-4ff6-87b6-ce5a316b9ea8.mdnplay.dev/shared-assets/images/examples/grapefruit-slice.jpg" alt="test" width="250" height="250" />
            </a>
            <header>
              <h3><a href="/channels/abc/contents/abc">Where to go</a></h3>
              <p>By <a href="/channels/abc">Nimosity</a></p>
            </header>
            <p>How are you?</p>
            <footer>
              <p>Tags: <a href="/tags/test">Test</a></p>
              <p>Posted on <time datetime="2025-10-10">2025-10-10</time></p>
            </footer>
          </article>
        </li>
        <li>
          <article>
            <a href="/channels/abc/contents/abc" aria-hidden="true" tabindex="-1">
              <img src="https://94e01200-c64f-4ff6-87b6-ce5a316b9ea8.mdnplay.dev/shared-assets/images/examples/grapefruit-slice.jpg" alt="test" width="250" height="250" />
            </a>
            <header>
              <h3><a href="/channels/abc/contents/abc">Where to go</a></h3>
              <p>By <a href="/channels/abc">Nimosity</a></p>
            </header>
            <p>How are you?</p>
            <footer>
              <p>Tags: <a href="/tags/test">Test</a></p>
              <p>Posted on <time datetime="2025-10-10">2025-10-10</time></p>
            </footer>
          </article>
        </li>
        <li>
          <article>
            <a href="/channels/abc/contents/abc" aria-hidden="true" tabindex="-1">
              <img src="https://94e01200-c64f-4ff6-87b6-ce5a316b9ea8.mdnplay.dev/shared-assets/images/examples/grapefruit-slice.jpg" alt="test" width="250" height="250" />
            </a>
            <header>
              <h3><a href="/channels/abc/contents/abc">Where to go</a></h3>
              <p>By <a href="/channels/abc">Nimosity</a></p>
            </header>
            <p>How are you?</p>
            <footer>
              <p>Tags: <a href="/tags/test">Test</a></p>
              <p>Posted on <time datetime="2025-10-10">2025-10-10</time></p>
            </footer>
          </article>
        </li>
      </ol>
    </section>
    <section aria-labelledby="popular-heading">
      <h2 id="popular-heading">Most Popular</h2>
      <ol>
        <li>
          <article>
            <a href="/channels/abc/contents/abc" aria-hidden="true" tabindex="-1">
              <img src="https://94e01200-c64f-4ff6-87b6-ce5a316b9ea8.mdnplay.dev/shared-assets/images/examples/grapefruit-slice.jpg" alt="test" width="250" height="250" />
            </a>
            <header>
              <h3><a href="/channels/abc/contents/abc">Where to go</a></h3>
              <p>By <a href="/channels/abc">Nimosity</a></p>
            </header>
            <p>How are you? Are you doing any good today? How are you? Are you doing any good today? How are you? Are you doing any good today? How are you? Are you doing any good today? How are you? Are you doing any good today?</p>
            <footer>
              <p>Tags: <a href="/tags/test">Test</a></p>
              <p>Posted on <time datetime="2025-10-10">2025-10-10</time></p>
            </footer>
          </article>
        </li>
      </ol>
    </section>
  </main>
</template>
