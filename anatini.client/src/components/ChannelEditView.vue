<script setup lang="ts">
  import type { ChannelEdit, DraftContent, ErrorMessage, InputError, StatusActions } from '@/types';
  import { nextTick, ref, watch } from 'vue';
  import { apiFetchAuthenticated } from './common/apiFetch';
  import { useRoute } from 'vue-router';
  import { getTabIndex, tidy } from './common/utils';
  import InputText from './common/InputText.vue';
  import TabButton from './common/TabButton.vue';

  const route = useRoute();

  const channel = ref<ChannelEdit | ErrorMessage | null>(null);
  const tabIndex = ref<number>(0);
  const inputName = ref<string>('');
  const inputErrors = ref<InputError[]>([]);
  const status = ref<'saving' | 'saved' | 'inactive'>('inactive');
  const errorSectionRef = ref<HTMLElement | null>(null);
  const inputContentName = ref<string>('');
  const inputContentSlug = ref<string>('');

  const tabs = ref([
    { id: 'contents', text: 'Contents' },
    { id: 'public', text: 'Display' },
  ]);

  const tabRefs = ref<HTMLButtonElement[]>([]);

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

  async function postContent() {
    inputErrors.value = [];

    if (channel.value === null || 'error' in channel.value) {
      return;
    }

    const tidiedName = tidy(inputContentName.value);
    const tidiedSlug = tidy(inputContentSlug.value);

    if (tidiedName === '') {
      inputErrors.value.push({id: 'content-name', message: 'Content name is required'});
    }

    if (tidiedSlug === '') {
      inputErrors.value.push({id: 'content-slug', message: 'Content slug is required'});
    }

    if (inputErrors.value.length > 0) {
      nextTick(() => {
        errorSectionRef.value?.focus();
      });

      return;
    }

    status.value = 'saving';

    const statusActions: StatusActions = {
      201: (response?: Response) => {
        status.value = 'saved';

        response?.json().then((value: DraftContent) => {
          if (channel.value !== null && 'topDraftContents' in channel.value) {
            const topDraftContents = channel.value.topDraftContents ?? [];
            topDraftContents.push(value);
            channel.value.topDraftContents = topDraftContents;
          }
        });

        inputContentName.value = '';
        inputContentSlug.value = '';
      },
      409: () => {
        status.value = 'inactive';

        inputErrors.value = [{ id: 'channel-slug', message: 'Slug already in use' }]

        nextTick(() => {
          errorSectionRef.value?.focus();
        });
      }
    }

    const body = new FormData();

    body.append('name', tidiedName);
    body.append('slug', tidiedSlug);

    const init = { method: "POST", body: body };

    apiFetchAuthenticated(`channels/${channel.value.id}/contents`, statusActions, undefined, init);
  }

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
  <main>
    <article :aria-busy="channel === null" aria-live="polite" aria-labelledby="heading-main">
      <header>
        <h1 id="heading-main">{{ getHeading() }}</h1>
      </header>

      <section v-if="channel === null">
        <p role="status" class="visuallyhidden">Please wait while the channel information is fetched.</p>
                
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

        <section role="tablist" aria-label="Settings Options">
          <TabButton v-for="(tab, index) in tabs"
            :key="tab.id"
            :selected="tabIndex === index"
            @click="tabIndex = index"
            @keydown="handleKeyDown($event, index)"
            :text="tab.text"
            :id="tab.id"
            :add-button-ref="(el: HTMLButtonElement | null) => {if (el) tabRefs.push(el)}" />
        </section>

        <section id="panel-contents" role="tabpanel" tabindex="0" aria-labelledby="tab-contents" :hidden="tabIndex !== 0">
          <header>
            <h2>Contents</h2>
          </header>

          <form @submit.prevent="postContent" :action="`/api/channels/${channel.id}/contents`" method="POST" novalidate>
            <fieldset>
              <legend>Create Content</legend>
                
              <InputText
                v-model="inputContentName"
                label="Name*"
                name="name"
                id="content-name"
                :maxlength="64"
                help="The display name of your content"
                :error="getError('content-name')" />

              <br>

              <InputText
                v-model="inputContentSlug"
                label="Slug*"
                name="slug"
                id="content-slug"
                :maxlength="64"
                help="lower case with hyphens (e.g. 'my-anatini-content')"
                :error="getError('content-slug')" />
            </fieldset>

            <footer>
              <button type="submit" :disabled="status === 'saving' || tidy(inputContentName) === '' || tidy(inputContentSlug) === ''">{{status === 'saving' ? 'Creating...' : 'Create' }}</button>

              <p role="status" class="visuallyhidden">{{ status === 'saved' ? 'Created!' : undefined }}</p>
            </footer>
          </form>

          <section aria-labelledby="section-your-contents">
            <header>
              <h3 id="section-your-contents">Your Contents</h3>
            </header>

            <ul role="list" v-if="(channel.topDraftContents?.length ?? 0) > 0" aria-live="polite" aria-relevant="additions">
              <li v-for="content in channel.topDraftContents" :key="`content-${content.defaultSlug}`">
                <article :aria-labelledby="`content-${content.defaultSlug}`">
                  <header>
                    <h4 :id="`content-${content.defaultSlug}`">
                      <RouterLink :to="{ name: 'ContentEdit', params: { channelId: channel.defaultSlug, contentId: content.defaultSlug }}">{{ content.name }}</RouterLink>
                    </h4>
                  </header>

                  <p>Content Description Goes Here</p>

                  <footer>
                    <small>Slug: <code>{{ content.defaultSlug }}</code></small>
                  </footer>
                </article>
              </li>
            </ul>

            <p v-else>You do not have any contents</p>
          </section>
        </section>

        <section id="panel-public" role="tabpanel" tabindex="0" aria-labelledby="tab-public" :hidden="tabIndex !== 1">
          <header>
            <h2>Display</h2>
          </header>

          <form @submit.prevent="patchChannel" :action="`/api/channels/${channel.id}`" method="PATCH" novalidate>
            <fieldset>
              <legend>Display</legend>

              <InputText
                v-model="inputName"
                label="Name"
                name="name"
                id="name"
                :maxlength="64"
                :error="getError('name')"
                help="Channel display name"
                autocomplete="name" />
            </fieldset>

            <footer>
              <button type="submit" :disabled="status === 'saving' || noChange()">{{status === 'saving' ? 'Saving...' : 'Save' }}</button>

              <p role="status" class="visuallyhidden">{{ status === 'saved' ? 'Saved!' : undefined }}</p>
            </footer>
          </form>
        </section>
      </template>
    </article>
  </main>
</template>
