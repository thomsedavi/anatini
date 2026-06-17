<script setup lang="ts">
  import { store } from '@/store';
  import type { Space } from '@/types';

  defineProps<{
    spaces: Space[] | null,
  }>();
</script>

<template>
  <section id="panel-spaces" role="tabpanel" aria-labelledby="tab-spaces">
    <header>
      <h2>Spaces</h2>
      <RouterLink v-if="store.isTrusted" :to="{ name: 'SpaceCreate' }">+ Create Space</RouterLink>
    </header>

    <section aria-labelledby="section-your-spaces">
      <header>
        <h3 id="section-your-spaces">Your Spaces</h3>
      </header>

      <ul role="list" v-if="(spaces?.length ?? 0) > 0">
        <li v-for="space in spaces" :key="`space-${space.handle}`">
          <article :aria-labelledby="`space-${space.handle}`">
            <header>
              <h4 :id="`space-${space.handle}`">
                <RouterLink :to="{ name: 'SpaceEdit', params: { spaceId: space.handle }}">{{ space.name }}</RouterLink>
              </h4>
            </header>

            <p v-if="space.about">{{ space.about }}</p>
          </article>
        </li>
      </ul>

      <p v-else>You do not have any spaces</p>
    </section>
  </section>
</template>
