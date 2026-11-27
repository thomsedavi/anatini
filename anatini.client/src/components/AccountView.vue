<script setup lang="ts">
  import type { APIResponse, InputError, Status, StatusActions, Tab, UserEdit, Visibility } from '@/common/types';
  import { nextTick, onMounted, ref } from 'vue';
  import { useRoute, useRouter } from 'vue-router';
  import { apiFetchAuthenticated } from '@/common/apiFetch';
  import { getTabIndex } from '@/common/utils';
  import TabButton from '@/common/TabButton.vue';

  const route = useRoute();
  const router = useRouter();

  const userResponse = ref<APIResponse<UserEdit>>({ fetching: true });
  const errorSectionRef = ref<HTMLElement | null>(null);
  const inputErrors = ref<InputError[]>([]);
  const inputName = ref<string>('');
  const inputUserAbout = ref<string>('');
  const status = ref<Status>('idle');
  const tabIndex = ref<number>(-1);

  const headingMainRef = ref<HTMLElement | null>(null);

  const tabs: Tab[] = [
    { id: 'public', text: 'Display', name: 'AccountPublic' },
    { id: 'private', text: 'Privacy & Security', name: 'AccountPrivate' },
    { id: 'spaces', text: 'Spaces', name: 'AccountSpaces' },
  ];

  const tabRefs = ref<HTMLButtonElement[]>([]);
  
  onMounted(() => {
    tabIndex.value = tabs.findIndex(tab => tab.name === route.name || tab.childNames?.includes(route.name));

    const input = 'account';

    const statusActions: StatusActions = {
      200: (response?: Response) => {
        response?.json()
          .then((value: UserEdit) => {
            userResponse.value = { data: { ...value, about: value.about?.replace(/\r\n/g, "\n") ?? null } };
            inputName.value = value.name;
            inputUserAbout.value = value.about ?? '';

            nextTick(() => {
              headingMainRef.value?.focus();
            });
          })
          .catch(() => {
            userResponse.value = { error: { status: 'unknown', heading: 'Unknown Error', body: 'There was a problem fetching your account, please reload the page' }};
          });
      },
      401: () => {
        router.replace({ path: '/sign-in', query: { redirect: '/account' } });
      },
      500: () => {
        userResponse.value = { error: { status: '500', heading: '500 Internal Server Error', body: 'There was a problem fetching your account, please reload the page' }};
      }
    };

    apiFetchAuthenticated({ input, statusActions });
  });

  function getHeading(): string {
    if (userResponse.value.fetching === true) {
      return 'Fetching...';
    } else if (userResponse.value.error !== undefined) {
      return userResponse.value.error.heading;
    } else if (userResponse.value.data !== undefined) {
      return 'Account Settings';
    } else {
      return 'Unknown Error';
    }
  }

  function handleKeyDown(event: KeyboardEvent, index: number): void {
    event.preventDefault();
    
    const newIndex = getTabIndex(event.key, index, tabs.length);

    if (newIndex !== undefined) {
      const tab = tabs[newIndex];

      if (tab !== undefined) {
        tabIndex.value = newIndex;

        router.push({ name: tab.name });
      
        nextTick(() => {
          tabRefs.value[newIndex]?.focus();
        });
      }
    }
  }

  function handleClickTab(index: number): void {
    const tab = tabs[index];

    if (tab !== undefined) {
      tabIndex.value = index;

      router.push({ name: tab.name });
      
      nextTick(() => {
        tabRefs.value[index]?.focus();
      });
    }
  }

  function handleUpdateName(newName: string): void {
    if (userResponse.value !== null && 'displayName' in userResponse.value) {
     userResponse.value.displayName = newName;
    }
  }

  function handleUpdateAbout(newAbout: string): void {
    if (userResponse.value !== null && 'about' in userResponse.value) {
     userResponse.value.about = newAbout;
    }
  }

  function handleUpdateVisibility(newVisibility: Visibility): void {
    if (userResponse.value.data !== undefined) {
     userResponse.value.data.visibility = newVisibility;
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
  <main role="main" id="main" tabindex="-1" :aria-busy="userResponse === null">
    <header>
      <h1 ref="headingMainRef" tabindex="-1">{{ getHeading() }}</h1>
    </header>

    <section v-if="userResponse === null">                
      <progress max="100">Fetching account...</progress>
    </section>

    <section v-if="userResponse.error !== undefined">
      <p>
        {{ userResponse.error.body }}
      </p>
    </section>

    <template v-if="userResponse.data !== undefined">
      <section id="errors" v-if="inputErrors.length > 0" ref="errorSectionRef" tabindex="-1" aria-live="assertive" aria-labelledby="heading-errors">
        <h2 id="heading-errors">There was a problem updating your account</h2>
        <ul role="list">
          <li v-for="error in inputErrors" :key="'error' + error.id">
            <a :href="'#input-' + error.id">{{ error.message }}</a>
          </li>
        </ul>
      </section>

      <ul role="tablist" aria-label="Settings Options">
        <TabButton v-for="(tab, index) in tabs"
          :key="tab.id"
          :selected="tabIndex === index"
          @click="() => handleClickTab(index)"
          @keydown="(event: KeyboardEvent) => handleKeyDown(event, index)"
          :text="tab.text"
          :id="tab.id"
          :add-button-ref="(el: HTMLButtonElement) => { tabRefs.push(el); }" />
      </ul>

      <RouterView v-slot="{ Component }">
        <component
          :is="Component"
          :data-name="userResponse.data.name"
          :data-about="userResponse.data.about"
          :data-icon-image="userResponse.data.iconImage"
          :data-spaces="userResponse.data.spaces"
          :data-visibility="userResponse.data.visibility"
          :data-status="status"
          :data-input-errors="inputErrors"
          @update-name="handleUpdateName"
          @update-about="handleUpdateAbout"
          @update-visibility="handleUpdateVisibility"
          @update-status="handleUpdateStatus"
          @update-errors="handleUpdateErrors"
        />
      </RouterView>
    </template>

    <p role="status" aria-live="polite" class="visually-hidden" aria-atomic="true">TODO</p>
  </main>
</template>
