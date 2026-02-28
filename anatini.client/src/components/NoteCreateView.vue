<script setup lang="ts">
  import type { ChannelEdit, ErrorMessage, InputError, Status, StatusActions } from '@/types';
  import { nextTick, ref, watch } from 'vue';
  import { useRoute, useRouter } from 'vue-router';
  import { parseSource, tidy, type Source } from './common/utils';
  import { apiFetchAuthenticated } from './common/apiFetch';
  import SubmitButton from './common/SubmitButton.vue';

  const route = useRoute();
  const router = useRouter();

  const channel = ref<ChannelEdit | ErrorMessage | null>(null);
  const inputErrors = ref<InputError[]>([]);
  const inputNote = ref<string>('');
  const status = ref<Status>('idle');
  const errorSectionRef = ref<HTMLElement | null>(null);

  watch([() => route.params.channelId], (source: Source) => fetchChannel(parseSource(source)), { immediate: true });

  async function fetchChannel(params: string[]) {
    const statusActions: StatusActions = {
      200: (response?: Response) => {
        response?.json()
          .then((value: ChannelEdit) => {
            channel.value = value;
          })
          .catch(() => { channel.value = { error: true, heading: 'Unknown Error', body: 'There was a problem fetching your account, please reload the page' }});
      },
      401: () => {
        router.replace({ path: '/login', query: { redirect: `/channels/${params[0]}/posts/create` } });
      },
      403: () => {
        channel.value = { error: true, heading: 'Unknown Error', body: 'No access to channel' };
      },
      404: () => {
        channel.value = { error: true, heading: '404 Not Found', body: 'Channel not found' };
      },
      500: () => {
        channel.value = { error: true, heading: 'Unknown Error', body: 'There was a problem fetching your account, please reload the page' };
      }
    };

    apiFetchAuthenticated(`channels/${params[0]}/edit`, statusActions);
  };

  async function postNote() {
    inputErrors.value = [];

    if (channel.value === null || 'error' in channel.value) {
      return;
    }

    const tidiedNote = tidy(inputNote.value);

    if (tidiedNote === '') {
      inputErrors.value.push({id: 'note', message: 'Note is required'});
    }

    if (inputErrors.value.length > 0) {
      nextTick(() => {
        errorSectionRef.value?.focus();
      });

      return;
    }

    status.value = 'pending';
  }
</script>

<template>
  <main id="main" tabindex="-1">
    <header>
      <h1>Create Note</h1>
    </header>

    <section v-if="channel === null">
      <p role="status" class="visuallyhidden" aria-live="polite">Please wait while the channel information is fetched.</p>
                
      <progress max="100">Fetching channel...</progress>
    </section>

    <section v-else-if="'error' in channel">
      <p>
        {{ channel.body }}
      </p>
    </section>

    <template v-else>
      <section id="errors" v-if="inputErrors.length > 0" ref="errorSectionRef" tabindex="-1" aria-live="assertive" aria-labelledby="heading-errors">
        <h2 id="heading-errors">There was a problem creating your note</h2>
        <ul>
          <li v-for="error in inputErrors" :key="'error' + error.id">
            <a :href="'#input-' + error.id">{{ error.message }}</a>
          </li>
        </ul>
      </section>

      <form @submit.prevent="postNote" :action="`/api/channels/${channel.id}/notes`" method="POST" novalidate>
        <fieldset>
          <legend class="visuallyhidden">Create Note</legend>

        </fieldset>

        <SubmitButton
          :busy="status === 'pending'"
          :disabled="tidy(inputNote) === ''"
          text="Create"
          busy-text="Creating..." />
      </form>

      <p role="status" class="visuallyhidden" aria-live="polite">{{ status === 'success' ? 'Created!' : undefined }}</p>
    </template>
  </main>
</template>
