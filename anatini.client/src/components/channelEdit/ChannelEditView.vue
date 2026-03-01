<script setup lang="ts">
  import type { ChannelEdit, ErrorMessage, InputError, Status, StatusActions, Tab } from '@/types';
  import { nextTick, ref, watch } from 'vue';
  import { apiFetchAuthenticated } from '../common/apiFetch';
  import { useRoute, useRouter } from 'vue-router';
  import { getTabIndex, parseSource, type Source } from '../common/utils';
  import TabButton from '../common/TabButton.vue';

  const route = useRoute();
  const router = useRouter();

  const channel = ref<ChannelEdit | ErrorMessage | null>(null);
  const tabIndex = ref<number>(0);
  const inputName = ref<string>('');
  const inputErrors = ref<InputError[]>([]);
  const status = ref<Status>('idle');
  const errorSectionRef = ref<HTMLElement | null>(null);

  const tabs: Tab[] = [
    { id: 'posts', text: 'Posts', name: 'ChannelEditPosts' },
    { id: 'notes', text: 'Notes', name: 'ChannelEditNotes' },
    { id: 'public', text: 'Display', name: 'ChannelEditDisplay' },
  ];

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
      404: () => {
        channel.value = { error: true, heading: '404 Not Found', body: 'Channel not found' };
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

  function handleKeyDown(event: KeyboardEvent, index: number): void {
    const newIndex = getTabIndex(event.key, index, tabs.length);

    if (newIndex === undefined) {
      return;
    }

    event.preventDefault();
    tabIndex.value = newIndex;

    router.push({ name: tabs[newIndex].name });
    
    nextTick(() => {
      tabRefs.value[newIndex].focus();
    })
  }

  function handleClick(index: number): void {
    tabIndex.value = index;

    router.push({ name: tabs[index].name });
    
    nextTick(() => {
      tabRefs.value[index].focus();
    })
  }

  function handleUpdateStatus(newStatus: Status): void {
    status.value = newStatus;
  }

  function handleUpdateName(newName: string): void {
    if (channel.value !== null && 'name' in channel.value) {
     channel.value.name = newName;
    }
  }

  function handleUpdateErrors(newInputErrors: InputError[]): void {
    inputErrors.value = newInputErrors;

    if (newInputErrors.length > 0) {
      nextTick(() => {
        errorSectionRef.value?.focus();
      });
    }
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
          @click="() => handleClick(index)"
          @keydown="event => handleKeyDown(event, index)"
          :text="tab.text"
          :id="tab.id"
          :add-button-ref="(el: HTMLButtonElement) => { tabRefs.push(el); }" />
      </ul>

      <RouterView v-slot="{ Component }">
        <component
          :is="Component"    
          :channelId="channel.defaultHandle"
          :name="channel.name"
          :status="status"
          :inputErrors="inputErrors"
          @update-status="handleUpdateStatus"
          @update-name="handleUpdateName"
          @update-errors="handleUpdateErrors"
        />
      </RouterView>
    </template>

    <p role="status" class="visuallyhidden" aria-live="polite">{{ status === 'success' ? 'Created!' : undefined }}</p>
  </main>
</template>
