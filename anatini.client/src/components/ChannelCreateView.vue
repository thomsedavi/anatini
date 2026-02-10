<script setup lang="ts">
  import type { InputError, Status, StatusActions } from '@/types';
  import { nextTick, ref } from 'vue';
  import InputText from './common/InputText.vue';
  import SubmitButton from './common/SubmitButton.vue';
  import { tidy } from './common/utils';
  import { apiFetchAuthenticated } from './common/apiFetch';
  import { useRouter } from 'vue-router';

  const router = useRouter();

  const inputErrors = ref<InputError[]>([]);
  const inputChannelName = ref<string>('');
  const inputChannelHandle = ref<string>('');
  const status = ref<Status>('idle');
  const errorSectionRef = ref<HTMLElement | null>(null);

  function getError(id: string): string | undefined {
    return inputErrors.value.find(inputError => inputError.id === id)?.message;
  }

  async function postChannel() {
    inputErrors.value = [];

    const tidiedName = tidy(inputChannelName.value);
    const tidiedHandle = tidy(inputChannelHandle.value);

    if (tidiedName === '') {
      inputErrors.value.push({id: 'name', message: 'Channel name is required'});
    }

    if (tidiedHandle === '') {
      inputErrors.value.push({id: 'handle', message: 'Channel handle is required'});
    }

    if (inputErrors.value.length > 0) {
      nextTick(() => {
        errorSectionRef.value?.focus();
      });

      return;
    }

    status.value = 'pending';

    const statusActions: StatusActions = {
      201: () => {
        status.value = 'success';

        router.push({ name: 'ChannelEdit', params: { channelId: tidiedHandle } });
      },
      409: () => {
        status.value = 'error';

        inputErrors.value = [{ id: 'handle', message: 'Handle already in use' }]

        nextTick(() => {
          errorSectionRef.value?.focus();
        });
      }
    }

    const body = new FormData();

    body.append('name', tidiedName);
    body.append('handle', tidiedHandle);

    const init = { method: "POST", body: body };

    apiFetchAuthenticated('channels', statusActions, init);
  }
</script>

<template>
  <main id="main" tabindex="-1">
    <header>
      <h1>Create Channel</h1>
    </header>

    <section id="errors" v-if="inputErrors.length > 0" ref="errorSectionRef" tabindex="-1" aria-live="assertive" aria-labelledby="heading-errors">
      <h2 id="heading-errors">There was a problem creating your channel</h2>
      <ul>
        <li v-for="error in inputErrors" :key="'error' + error.id">
          <a :href="'#input-' + error.id">{{ error.message }}</a>
        </li>
      </ul>
    </section>

    <form @submit.prevent="postChannel" action="/api/channels" method="POST" novalidate>
      <fieldset>
        <legend class="visuallyhidden">Create Channel</legend>

        <InputText
          v-model="inputChannelName"
          label="Name"
          name="name"
          id="name"
          :maxlength="64"
          help="The display name of your channel"
          :error="getError('name')" />

        <InputText
          v-model="inputChannelHandle"
          label="Handle"
          name="handle"
          id="handle"
          :maxlength="64"
          help="lower case with hyphens (e.g. 'my-anatini-channel')"
          :error="getError('handle')" />
      </fieldset>

      <SubmitButton
        :busy="status === 'pending'"
        :disabled="tidy(inputChannelName) === '' || tidy(inputChannelHandle) === ''"
        text="Create"
        busy-text="Creating..." />
    </form>

    <p role="status" class="visuallyhidden" aria-live="polite">{{ status === 'success' ? 'Created!' : undefined }}</p>
  </main>
</template>
