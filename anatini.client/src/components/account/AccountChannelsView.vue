<script setup lang="ts">
  import { store } from '@/store';
  import type { Channel } from '@/types';

  defineProps<{
    channels: Channel[] | null,
  }>();
</script>

<template>
  <section id="panel-channels" role="tabpanel" aria-labelledby="tab-channels">
    <header>
      <h2>Channels</h2>
      <RouterLink v-if="store.isTrusted" :to="{ name: 'ChannelCreate' }">+ Create Channel</RouterLink>
    </header>

    <section aria-labelledby="section-your-channels">
      <header>
        <h3 id="section-your-channels">Your Channels</h3>
      </header>

      <ul role="list" v-if="(channels?.length ?? 0) > 0">
        <li v-for="channel in channels" :key="`channel-${channel.handle}`">
          <article :aria-labelledby="`channel-${channel.handle}`">
            <header>
              <h4 :id="`channel-${channel.handle}`">
                <RouterLink :to="{ name: 'ChannelEdit', params: { channelId: channel.handle }}">{{ channel.name }}</RouterLink>
              </h4>
            </header>

            <p v-if="channel.about">{{ channel.about }}</p>
          </article>
        </li>
      </ul>

      <p v-else>You do not have any channels</p>
    </section>
  </section>
</template>
