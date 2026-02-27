<script setup lang="ts">
  import type { ChannelEdit, ErrorMessage, InputError, Status, StatusActions } from '@/types';
  import { nextTick, ref, watch } from 'vue';
  import { apiFetchAuthenticated } from './common/apiFetch';
  import { useRoute } from 'vue-router';
  import { getTabIndex, parseSource, tidy, type Source } from './common/utils';
  import InputText from './common/InputText.vue';
  import TabButton from './common/TabButton.vue';
  import SubmitButton from './common/SubmitButton.vue';

  const route = useRoute();

  const channel = ref<ChannelEdit | ErrorMessage | null>(null);
  const tabIndex = ref<number>(0);
  const inputName = ref<string>('');
  const inputErrors = ref<InputError[]>([]);
  const status = ref<Status>('idle');
  const errorSectionRef = ref<HTMLElement | null>(null);

  const tabs = ref([
    { id: 'posts', text: 'Posts' },
    { id: 'public', text: 'Display' },
  ]);

  const tabRefs = ref<HTMLButtonElement[]>([]);

  watch([() => route.params.channelId], (source: Source) => fetchChannel(parseSource(source)), { immediate: true });

  async function fetchChannel(params: string[]) {
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

    apiFetchAuthenticated(`channels/${params[0]}/edit`, statusActions);
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

    status.value = 'pending';

    const statusActions: StatusActions = {
      204: () => {
        status.value = 'success';

        if (channel.value !== null && 'name' in channel.value) {
          channel.value.name = tidiedName;
        }
      },
      500: () => {
        status.value = 'error';

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

    apiFetchAuthenticated(`channels/${channel.value.id}`, statusActions, init);
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
  <main id="main" tabindex="-1" :aria-busy="channel === null">
    <header>
      <h1 id="heading-main">{{ getHeading() }}</h1>
    </header>

    <section v-if="channel === null">
      <progress max="100">Fetching account...</progress>
    </section>

    <section v-else-if="'error' in channel">
      <p>
        {{ channel.body }}
      </p>
    </section>

    <template v-else>
      <section id="errors" v-if="inputErrors.length > 0" ref="errorSectionRef" tabindex="-1" aria-live="assertive" aria-labelledby="heading-errors">
        <h2 id="heading-errors">There was a problem updating your account</h2>
        <ul>
          <li v-for="error in inputErrors" :key="'error' + error.id">
            <a :href="'#input-' + error.id">{{ error.message }}</a>
          </li>
        </ul>
      </section>

      <ul role="tablist" aria-label="Settings Options">
        <TabButton v-for="(tab, index) in tabs"
          :key="tab.id"
          :selected="tabIndex === index"
          @click="tabIndex = index"
          @keydown="handleKeyDown($event, index)"
          :text="tab.text"
          :id="tab.id"
          :add-button-ref="(el: HTMLButtonElement | null) => {if (el) tabRefs.push(el)}" />
      </ul>

      <section id="panel-posts" role="tabpanel" aria-labelledby="tab-posts" :hidden="tabIndex !== 0">
        <header>
          <h2>Posts</h2>
          <RouterLink :to="{ name: 'PostCreate' }">+ Create Post</RouterLink>
        </header>

        <section aria-labelledby="section-your-posts">
          <header>
            <h3 id="section-your-posts">Your Posts</h3>
          </header>

          <ul role="list" v-if="(channel.topDraftPosts?.length ?? 0) > 0">
            <li v-for="post in channel.topDraftPosts" :key="`post-${post.defaultHandle}`">
              <article :aria-labelledby="`post-${post.defaultHandle}`">
                <header>
                  <h4 :id="`post-${post.defaultHandle}`">
                    <RouterLink :to="{ name: 'PostEdit', params: { channelId: channel.defaultHandle, postId: post.defaultHandle }}">{{ post.name }}</RouterLink>
                  </h4>
                </header>

                <p>Post Description Goes Here</p>

                <footer>
                  <small>Handle: <code>{{ post.defaultHandle }}</code></small>
                </footer>
              </article>
            </li>
          </ul>

          <p v-else>You do not have any posts</p>
        </section>
      </section>

      <section id="panel-public" role="tabpanel" aria-labelledby="tab-public" :hidden="tabIndex !== 1">
        <header>
          <h2>Display</h2>
        </header>

        <form @submit.prevent="patchChannel" :action="`/api/channels/${channel.id}`" method="POST" novalidate>
          <InputText
            v-model="inputName"
            label="Name"
            name="name"
            id="name"
            :maxlength="64"
            :error="getError('name')"
            help="Channel display name"
            autocomplete="name" />

          <SubmitButton
            :busy="status === 'pending'"
            :disabled="noChange()"
            text="Save"
            busy-text="Saving..." />
        </form>
      </section>
    </template>

    <p role="status" class="visuallyhidden" aria-live="polite">{{ status === 'success' ? 'Created!' : undefined }}</p>
  </main>
</template>
