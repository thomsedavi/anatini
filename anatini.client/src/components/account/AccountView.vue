<script setup lang="ts">
  import type { APIResponse, InputError, Note, Status, StatusActions, Tab, UserEdit, Visibility } from '@/types';
  import { nextTick, onMounted, ref } from 'vue';
  import { useRoute, useRouter } from 'vue-router';
  import { apiFetchAuthenticated } from '../common/apiFetch';
  import { getTabIndex } from '../common/utils';
  import TabButton from '../common/TabButton.vue';

  const route = useRoute();
  const router = useRouter();

  const user = ref<APIResponse<UserEdit>>({ fetching: true });
  const errorSectionRef = ref<HTMLElement | null>(null);
  const inputErrors = ref<InputError[]>([]);
  const inputName = ref<string>('');
  const inputUserAbout = ref<string>('');
  const status = ref<Status>('idle');
  const tabIndex = ref<number>(-1);
  const pageStatus = ref<string>('Loading account information...');
  const headingMainRef = ref<HTMLElement | null>(null);
  const notes = ref<Note[] | null>(null);

  const tabs: Tab[] = [
    { id: 'public', text: 'Display', name: 'AccountPublic' },
    { id: 'notes', text: 'Notes', name: 'AccountNotes', childNames: ['AccountNoteCreate', 'AccountNoteEdit'] },
    { id: 'calendar', text: 'Calendar', name: 'AccountCalendar', childNames: ['AccountEventCreate'] },
    { id: 'spaces', text: 'Spaces', name: 'AccountSpaces' },
    { id: 'private', text: 'Privacy & Security', name: 'AccountPrivate' },
  ];

  const tabRefs = ref<HTMLButtonElement[]>([]);
  
  onMounted(() => {
    tabIndex.value = tabs.findIndex(tab => tab.name === route.name || tab.childNames?.includes(route.name));

    const statusActions: StatusActions = {
      200: (response?: Response) => {
        response?.json()
          .then((value: UserEdit) => {
            user.value = { data: { ...value, about: value.about?.replace(/\r\n/g, "\n") ?? null } };
            inputName.value = value.name;
            inputUserAbout.value = value.about ?? '';
            pageStatus.value = '';

            nextTick(() => {
              headingMainRef.value?.focus();
            });
          })
          .catch(() => { user.value = { error: { heading: 'Unknown Error', body: 'There was a problem fetching your account, please reload the page' }}});
      },
      401: () => {
        router.replace({ path: '/sign-in', query: { redirect: '/account' } });
      },
      500: () => {
        user.value = { error: { heading: 'Unknown Error', body: 'There was a problem fetching your account, please reload the page' }};
      }
    };

    apiFetchAuthenticated('account', statusActions);
  });

  function getHeading(): string {
    if (user.value.fetching === true) {
      return 'Fetching...';
    } else if (user.value.error !== undefined) {
      return user.value.error.heading;
    } else if (user.value.data !== undefined) {
      return 'Account Settings';
    } else {
      return 'Unknown Error';
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

  function handleUpdateNotes(newNotes: Note[]): void {
    notes.value = newNotes;
  }

  function handleUpdateAbout(newAbout: string): void {
    if (user.value !== null && 'about' in user.value) {
     user.value.about = newAbout;
    }
  }

  function handleUpdateVisibility(newVisibility: Visibility): void {
    if (user.value.data !== undefined) {
     user.value.data.visibility = newVisibility;
    }
  }

  function handleUpdateStatus(newStatus: Status): void {
    status.value = newStatus;
  }

  function handleUpdatePageStatus(newPageStatus: string): void {
    pageStatus.value = newPageStatus;

    setTimeout(() => {
      pageStatus.value = '';
    }, 3000);
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

    <section v-if="user.error !== undefined">
      <p>
        {{ user.error.body }}
      </p>
    </section>

    <template v-if="user.data !== undefined">
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
          @click="() => handleClick(index)"
          @keydown="event => handleKeyDown(event, index)"
          :text="tab.text"
          :id="tab.id"
          :add-button-ref="(el: HTMLButtonElement) => { tabRefs.push(el); }" />
      </ul>

      <RouterView v-slot="{ Component }">
        <component
          :is="Component"
          :name="user.data.name"
          :about="user.data.about"
          :icon-image="user.data.iconImage"
          :spaces="user.data.spaces"
          :visibility="user.data.visibility"
          :status="status"
          :inputErrors="inputErrors"
          :notes="notes"
          @update-name="handleUpdateName"
          @update-about="handleUpdateAbout"
          @update-visibility="handleUpdateVisibility"
          @update-status="handleUpdateStatus"
          @update-page-status="handleUpdatePageStatus"
          @update-notes="handleUpdateNotes"
          @update-errors="handleUpdateErrors"
        />
      </RouterView>
    </template>

    <p role="status" aria-live="polite" class="visuallyhidden" aria-atomic="true">{{ pageStatus }}</p>
  </main>
</template>
