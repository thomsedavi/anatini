<script setup lang="ts">
  import type { ChannelEdit, ErrorMessage, InputError, Status, StatusActions } from '@/types';
  import { nextTick, ref, watch } from 'vue';
  import { parseSource, tidy, type Source } from './common/utils';
  import { apiFetchAuthenticated } from './common/apiFetch';
  import { useRoute, useRouter } from 'vue-router';
  import InputText from './common/InputText.vue';
  import SubmitButton from './common/SubmitButton.vue';

  const route = useRoute();
  const router = useRouter();

  const channel = ref<ChannelEdit | ErrorMessage | null>(null);
  const inputErrors = ref<InputError[]>([]);
  const inputContentName = ref<string>('');
  const inputContentHandle = ref<string>('');
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

  function getError(id: string): string | undefined {
    return inputErrors.value.find(inputError => inputError.id === id)?.message;
  }

  async function postContent() {
    inputErrors.value = [];

    if (channel.value === null || 'error' in channel.value) {
      return;
    }

    const tidiedName = tidy(inputContentName.value);
    const tidiedHandle = tidy(inputContentHandle.value);

    if (tidiedName === '') {
      inputErrors.value.push({id: 'name', message: 'Content name is required'});
    }

    if (tidiedHandle === '') {
      inputErrors.value.push({id: 'handle', message: 'Content handle is required'});
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

        if (channel.value !== null && 'id' in channel.value) {
          router.push({ name: 'ContentEdit', params: { channelId: channel.value.id, contentId: tidiedHandle } });
        }
      },
      400: (response?: Response) => {
        status.value = 'error';

        response?.json().then((value) => {
          if (value.errors) {
            if ('Name' in value.errors) {
              inputErrors.value = [{id: 'name', message: value.errors['Name'][0]}]
            }

            if ('Handle' in value.errors) {
              inputErrors.value = [{id: 'handle', message: value.errors['Handle'][0]}]
            }

            nextTick(() => {
              errorSectionRef.value?.focus();
            });
          }
        });
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

    apiFetchAuthenticated(`channels/${channel.value.id}/posts`, statusActions, init);
  }
</script>

<template>
  <main id="main" tabindex="-1">
    <header>
      <h1>Create Content</h1>
    </header>

    <section v-if="channel === null">
      <p role="status" class="visuallyhidden" aria-live="polite">Please wait while the channel information is fetched.</p>
                
      <progress max="100">Fetching account...</progress>
    </section>

    <section v-else-if="'error' in channel">
      <p>
        {{ channel.body }}
      </p>
    </section>

    <template v-else>
      <section id="errors" v-if="inputErrors.length > 0" ref="errorSectionRef" tabindex="-1" aria-live="assertive" aria-labelledby="heading-errors">
        <h2 id="heading-errors">There was a problem creating your content</h2>
        <ul>
          <li v-for="error in inputErrors" :key="'error' + error.id">
            <a :href="'#input-' + error.id">{{ error.message }}</a>
          </li>
        </ul>
      </section>

      <form @submit.prevent="postContent" :action="`/api/channels/${channel.id}/posts`" method="POST" novalidate>
        <fieldset>
          <legend class="visuallyhidden">Create Content</legend>

          <InputText
            v-model="inputContentName"
            label="Name"
            name="name"
            id="name"
            :maxlength="64"
            help="The display name of your content"
            :error="getError('name')" />

          <InputText
            v-model="inputContentHandle"
            label="Handle"
            name="handle"
            id="handle"
            :maxlength="64"
            help="lower case with hyphens (e.g. 'my-anatini-content')"
            :error="getError('handle')" />
        </fieldset>

        <SubmitButton
          :busy="status === 'pending'"
          :disabled="tidy(inputContentName) === '' || tidy(inputContentHandle) === ''"
          text="Create"
          busy-text="Creating..." />
      </form>

      <p role="status" class="visuallyhidden" aria-live="polite">{{ status === 'success' ? 'Created!' : undefined }}</p>
    </template>
  </main>
</template>
