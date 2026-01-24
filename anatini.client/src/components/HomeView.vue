<script setup lang="ts">
  import { nextTick, ref } from 'vue';
  import TabButton from './common/TabButton.vue';
  import { getTabIndex } from './common/utils';

  const tabIndex = ref<number>(0);

  const tabs = ref([
    { id: 'recent', text: 'Recent' },
    { id: 'popular', text: 'Popular' },
  ]);

  const tabRefs = ref<HTMLButtonElement[]>([]);

  function handleKeyDown(event: KeyboardEvent, index: number): void {
    const newIndex = getTabIndex(event.key, index, tabs.value.length);

    if (newIndex === undefined) {
      return;
    }

    event.preventDefault();

    tabIndex.value = newIndex;
    
    nextTick(() => {
      tabRefs.value[newIndex].focus();
    })
  }
</script>

<template>
  <main id="main" tabindex="-1">
    <header class="visuallyhidden">
      <h1>Content Feed</h1>
    </header>

    <ul role="tablist" aria-label="Feed Options">
      <TabButton v-for="(tab, index) in tabs"
        :key="tab.id"
        :selected="tabIndex === index"
        @click="tabIndex = index"
        @keydown="handleKeyDown($event, index)"
        :text="tab.text"
        :id="tab.id"
        :add-button-ref="(el: HTMLButtonElement | null) => {if (el) tabRefs.push(el)}" />
    </ul>

    <section id="panel-recent" role="tabpanel" aria-labelledby="tab-recent" :hidden="tabIndex !== 0" >
      <header class="visuallyhidden">
        <h2>Recent</h2>
      </header>

      <ul role="list">
        <li>
          <article>
            <header>
              <h3>
                <a href="/channels/my-channel/contents/my-content">My Diary 1</a>
              </h3>
            </header>

            <p>Thoughts about living in Africa</p>

            <footer>
              <small><time datetime="2025-10-10">October 10, 2025</time></small>
            </footer>
          </article>
        </li>

        <li>
          <article>
            <header>
              <h3>
                <a href="/channels/my-channel/contents/my-content">My Diary 2</a>
              </h3>
            </header>

            <p>Thoughts about living in Poland</p>

            <footer>
              <small><time datetime="2025-10-10">October 10, 2025</time></small>
            </footer>
          </article>
        </li>
      </ul>
    </section>

    <section id="panel-popular" role="tabpanel" aria-labelledby="tab-popular" :hidden="tabIndex !== 1" >
      <header class="visuallyhidden">
        <h2>Popular</h2>
      </header>

      <ul role="list">
        <li>
          <article>
            <header>
              <h3>
                <a href="/channels/my-channel/contents/my-content">My Diary 1</a>
              </h3>
            </header>

            <p>Thoughts about living in Africa</p>

            <footer>
              <small><time datetime="2025-10-10">October 10, 2025</time></small>
            </footer>
          </article>
        </li>
      </ul>
    </section>
  </main>
</template>
