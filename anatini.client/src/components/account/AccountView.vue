<script setup lang="ts">
  import type { ErrorMessage, InputError, Status, StatusActions, Tab, UserEdit } from '@/types';
  import { nextTick, onMounted, ref } from 'vue';
  import { useRoute, useRouter } from 'vue-router';
  import { apiFetchAuthenticated } from '../common/apiFetch';
  import { getTabIndex } from '../common/utils';
  import TabButton from '../common/TabButton.vue';

  const route = useRoute();

  const router = useRouter();
  const user = ref<UserEdit | ErrorMessage | null>(null);
  const errorSectionRef = ref<HTMLElement | null>(null);
  const inputErrors = ref<InputError[]>([]);
  const inputName = ref<string>('');
  const inputUserAbout = ref<string>('');
  const status = ref<Status>('idle');
  const tabIndex = ref<number>(-1);
  const pageStatus = ref<string>('Loading account information...'); // TODO add other statuses
  const headingMainRef = ref<HTMLElement | null>(null);

  const tabs: Tab[] = [
    { id: 'public', text: 'Display', name: 'AccountPublic' },
    { id: 'private', text: 'Privacy & Security', name: 'AccountPrivate' },
    { id: 'channels', text: 'Channels', name: 'AccountChannels' },
  ];

  const tabRefs = ref<HTMLButtonElement[]>([]);
  
  onMounted(() => {
    tabIndex.value = tabs.findIndex(tab => tab.name === route.name);

    const statusActions: StatusActions = {
      200: (response?: Response) => {
        response?.json()
          .then((value: UserEdit) => {
            user.value = value;
            user.value.about = user.value.about?.replace(/\r\n/g, "\n") ?? null;
            inputName.value = value.name;
            inputUserAbout.value = value.about ?? '';
            pageStatus.value = '';

            nextTick(() => {
              headingMainRef.value?.focus();
            });
          })
          .catch(() => { user.value = { error: true, heading: 'Unknown Error', body: 'There was a problem fetching your account, please reload the page' }});
      },
      401: () => {
        router.replace({ path: '/sign-in', query: { redirect: '/account' } });
      },
      500: () => {
        user.value = { error: true, heading: 'Unknown Error', body: 'There was a problem fetching your account, please reload the page' };
      }
    };

    apiFetchAuthenticated('account', statusActions);
  });

  function getHeading(): string {
    if (user.value === null) {
      return 'Fetching...';
    } if ('error' in user.value) {
      return user.value.heading;
    } else {
      return 'Account Settings';
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

  function handleUpdateName(newName: string): void {
    if (user.value !== null && 'displayName' in user.value) {
     user.value.displayName = newName;
    }
  }

  function handleUpdateAbout(newAbout: string): void {
    if (user.value !== null && 'about' in user.value) {
     user.value.about = newAbout;
    }
  }

  function handleUpdateProtected(newProtected: boolean | null): void {
    if (user.value !== null && 'protected' in user.value) {
     user.value.protected = newProtected;
    }
  }

  function handleUpdateStatus(newStatus: Status): void {
    status.value = newStatus;
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
  <main id="main" tabindex="-1" :aria-busy="user === null">
    <header>
      <h1 ref="headingMainRef" tabindex="-1">{{ getHeading() }}</h1>
    </header>

    <section v-if="user === null">                
      <progress max="100">Fetching account...</progress>
    </section>

    <section v-else-if="'error' in user">
      <p>
        {{ user.body }}
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
          :name="user.name"
          :about="user.about"
          :icon-image="user.iconImage"
          :channels="user.channels"
          :protected="user.protected"
          :status="status"
          :inputErrors="inputErrors"
          @update-name="handleUpdateName"
          @update-about="handleUpdateAbout"
          @update-protected="handleUpdateProtected"
          @update-status="handleUpdateStatus"
          @update-errors="handleUpdateErrors"
        />
      </RouterView>
    </template>

    <p role="status" class="visuallyhidden">{{ pageStatus }}</p>
  </main>
</template>
