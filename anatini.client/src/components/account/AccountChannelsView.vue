<script setup lang="ts">
  import type { Channel } from '@/types';

  defineProps<{
    channels: Channel[] | null,
  }>();
</script>

<template>
  <section id="panel-channels" role="tabpanel" aria-labelledby="tab-channels">
    <header>
      <h2>Channels</h2>
      <RouterLink :to="{ name: 'ChannelCreate' }">+ Create Channel</RouterLink>
    </header>

    <section aria-labelledby="section-your-channels">
      <header>
        <h3 id="section-your-channels">Your Channels</h3>
      </header>

      <ul role="list" v-if="(channels?.length ?? 0) > 0">
        <li v-for="channel in channels" :key="`channel-${channel.defaultHandle}`">
          <article :aria-labelledby="`channel-${channel.defaultHandle}`">
            <header>
              <h4 :id="`channel-${channel.defaultHandle}`">
                <RouterLink :to="{ name: 'ChannelEdit', params: { channelId: channel.defaultHandle }}">{{ channel.name }}</RouterLink>
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
