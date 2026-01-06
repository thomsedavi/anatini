<script setup lang="ts">
  import type { ChannelEdit, ErrorMessage, InputError, StatusActions } from '@/types';
  import { nextTick, ref, watch } from 'vue';
  import { apiFetchAuthenticated } from './common/apiFetch';
  import { useRoute } from 'vue-router';
  import { tidy } from './common/utils';
  import InputListItem from './common/InputListItem.vue';

  const route = useRoute();

  const channel = ref<ChannelEdit | ErrorMessage | null>(null);
  const tab = ref<'public'>('public');
  const inputName = ref<string>('');
  const inputErrors = ref<InputError[]>([]);
  const status = ref<'saving' | 'saved' | 'inactive'>('inactive');
  const errorSectionRef = ref<HTMLElement | null>(null);

  watch([() => route.params.channelId], fetchChannel, { immediate: true });

  async function fetchChannel(array: (() => string | string[])[]) {
    const statusActions: StatusActions = {
      200: (response?: Response) => {
        response?.json()
          .then((value: ChannelEdit) => {
            channel.value = value;
            inputName.value = value.name;
          })
          .catch(() => { channel.value = { error: true, heading: 'Unknown Error', body: 'There was a problem fetching your account, please reload the page' }});
      },
      500: () => {
        channel.value = { error: true, heading: 'Unknown Error', body: 'There was a problem fetching your account, please reload the page' };
      }
    };

    apiFetchAuthenticated(`channels/${array[0]}/edit`, statusActions);
  };

  function getHeading(): string {
    if (channel.value === null) {
      return 'Fetching...';
    } if ('error' in channel.value) {
      return channel.value.heading;
    } else {
      return 'Channel Settings';
    }
  }

  function noChange(): boolean {
    if (channel.value !== null && 'name' in channel.value) {
      return channel.value.name === tidy(inputName.value);
    } else {
      return true;
    }
  }

  function getError(id: string): string | undefined {
    return inputErrors.value.find(inputError => inputError.id === id)?.message;
  }

  async function patchChannel() {
    inputErrors.value = [];

    if (channel.value === null || 'error' in channel.value) {
      return;
    }

    const tidiedName = tidy(inputName.value);

    if (tidiedName === '') {
      inputErrors.value = [{ id: 'name', message: 'Name is required' }];

      nextTick(() => {
        errorSectionRef.value?.focus();
      });

      return;
    }

    status.value = 'saving';

    const statusActions: StatusActions = {
      204: () => {
        status.value = 'saved';

        if (channel.value !== null && 'name' in channel.value) {
          channel.value.name = tidiedName;
        }
      },
      500: () => {
        status.value = 'inactive';

        // TODO handle this better
        inputErrors.value = [{ id: 'name', message: 'Unknown Error' }]

        nextTick(() => {
          errorSectionRef.value?.focus();
        });
      }
    }

    const body = new FormData();

    if (channel.value.name !== tidiedName) {
      body.append('name', tidiedName);
    }

    const init = { method: "PATCH", body: body };

    apiFetchAuthenticated(`channels/${channel.value.id}`, statusActions, undefined, init);
  }
</script>

<template>
  <main>
    <article :aria-busy="channel === null" aria-live="polite" aria-labelledby="heading-main">
      <header>
        <h1 id="heading-main">{{ getHeading() }}</h1>
      </header>

      <section v-if="channel === null">
        <p role="status" class="visually-hidden">Please wait while the channel information is fetched.</p>
                
        <progress max="100">Fetching account...</progress>
      </section>

      <section v-else-if="'error' in channel">
        <p>
          {{ channel.body }}
        </p>
      </section>

      <template v-else>
        <section v-if="inputErrors.length > 0" ref="errorSectionRef" tabindex="-1" aria-live="assertive" aria-labelledby="heading-errors">
          <h2 id="heading-errors">There was a problem updating your account</h2>
          <ul>
            <li v-for="error in inputErrors" :key="'error' + error.id">
              <a :href="'#input-' + error.id">{{ error.message }}</a>
            </li>
          </ul>
        </section>

        <aside>
          <nav aria-label="Channel settings sections">
            <ul role="tablist">
              <li role="presentation">
                <button 
                  id="tab-public" 
                  role="tab" 
                  :aria-selected="tab === 'public'"
                  aria-controls="panel-public" 
                  type="button"
                  @click="tab = 'public'">
                  Display
                </button>
              </li>
            </ul>
          </nav>
        </aside>

        <section id="panel-public" role="tabpanel" aria-labelledby="tab-public" :hidden="tab !== 'public'">
          <header>
            <h2>Display</h2>
          </header>

          <form @submit.prevent="patchChannel" :action="`/api/channels/${channel.id}`" method="PATCH" novalidate>
            <fieldset>
              <legend>Public</legend>

              <ul>
                <InputListItem
                  v-model="inputName"
                  label="Name"
                  name="name"
                  id="name"
                  :maxlength="64"
                  :error="getError('name')"
                  help="Channel display name"
                  autocomplete="name" />
              </ul>
            </fieldset>

            <footer>
              <button type="submit" :disabled="status === 'saving' || noChange()">{{status === 'saving' ? 'Saving...' : 'Save' }}</button>

              <p role="status" class="visually-hidden">{{ status === 'saved' ? 'Saved!' : undefined }}</p>
            </footer>
          </form>
        </section>
      </template>
    </article>
  </main>
</template>
