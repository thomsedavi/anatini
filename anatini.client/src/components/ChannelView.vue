<script setup lang="ts">
  import type { Channel } from '@/types';
  import { ref, watch } from 'vue';
  import { useRoute } from 'vue-router';
  
  const route = useRoute();

  const loading = ref<boolean>(false);
  const error = ref<string | null>(null);
  const channel = ref<Channel | null>(null);

  watch([() => route.params.channelSlug], fetchChannel, { immediate: true });

  async function fetchChannel(array: (() => string | string[])[]) {
    error.value = channel.value = null
    loading.value = true

    fetch(`/api/channels/${array[0]}`, { method: "GET" })
      .then((value: Response) => {
        if (value.ok) {
          value.json()
            .then((value: Channel) => {
              channel.value = value;
            })
            .catch(() => {
              error.value = 'Unknown Error';
            });
        } else if (value.status === 401) {
          error.value = 'Unauthorised';
        } else {
          error.value = 'Unkown Error';
        }
      })
      .catch(() => {
        error.value = 'Unknown Error';
      }).
      finally(() => {
        loading.value = false
      });
  }
</script>

<template>
  <h2>ChannelView</h2>
  <h3 v-if="channel">{{ channel.name }}</h3>
</template>
