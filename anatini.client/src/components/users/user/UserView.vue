<script setup lang="ts">
  import { nextTick, ref, watch } from 'vue';
  import { useRoute, useRouter } from 'vue-router';
  import { apiFetch, apiFetchAuthenticated } from '@/common/apiFetch';
  import { store } from '@/common/store';
  import TabButton from '@/common/TabButton.vue';
  import type { APIResponse, EventOccurrence, InputError, Note, Space, Status, StatusActions, Tab, User, Work } from '@/common/types';
  import { getTabIndex, parseParams, type Params } from '@/common/utils';

  const route = useRoute();
  const router = useRouter();

  const userResponse = ref<APIResponse<User>>({ fetching: true });
  const errorSectionRef = ref<HTMLElement | null>(null);
  const inputErrors = ref<InputError[]>([]);
  const status = ref<Status>('idle');
  const tabIndex = ref<number>(-1);
  const Notes = ref<Note[] | null>(null);
  const works = ref<Work[] | null>(null);
  const eventOccurrenceGroups = ref<{ date: string, eventOccurrences: EventOccurrence[] }[] | null>(null);
  const spaces = ref<Space[] | null>(null);

  const tabs: Tab[] = [
    { id: 'notes', text: 'Notes', name: 'UserNotes', childNames: ['UserNote', 'UserNoteCreate', 'UserNoteEdit'] },
    { id: 'works', text: 'Works', name: 'UserWorks', childNames: ['UserWork', 'UserWorkCreate'] },
    { id: 'events', text: 'Events', name: 'UserEvents', childNames: ['UserEventCreate'] },
    { id: 'spaces', text: 'Spaces', name: 'UserSpaces', childNames: ['UserSpace', 'UserSpaceCreate'] },
  ];

  const tabRefs = ref<HTMLButtonElement[]>([]);

  watch([() => route.params.userId], (params: Params) => fetchUser(parseParams(params)), { immediate: true });

  async function fetchUser(params: string[]) {
    tabIndex.value = tabs.findIndex(tab => tab.name === route.name || tab.childNames?.includes(route.name));

    const input = `users/${params[0]}`;

    const statusActions: StatusActions = {
      200: (response?: Response) => {
        response?.json()
          .then((value: User) => {
            userResponse.value = {
              data: { ...value, about: value.about?.replace(/\r\n/g, "\n") ?? null }
            };
          })
          .catch(() => {
            userResponse.value = { error: { status: 'unknown', heading: 'Unknown Error', body: 'There was a problem fetching this user, please reload the page' }};
          });
      },
      404: () => {
        userResponse.value = { error: { status: '404', heading: '404 Not Found', body: 'User not found' }};
      },
      500: () => {
        userResponse.value = { error: { status: '500', heading: '500 Internal Server Error', body: 'There was a problem fetching this user, please reload the page' }};
      }
    };

    apiFetch({ input, statusActions });
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

  function handleUpdateNotes(newNotes: Note[]): void {
    Notes.value = newNotes;
  }

  function handleUpdateWorks(newWorks: Work[]): void {
    works.value = newWorks;
  }

  function handleUpdateEventOccurrenceGroups(newEventOccurrenceGroups: { date: string, eventOccurrences: EventOccurrence[] }[]): void {
    eventOccurrenceGroups.value = newEventOccurrenceGroups;
  }

  function handleUpdateSpaces(newSpaces: Space[]): void {
    spaces.value = newSpaces;
  }

  function handleUpdateErrors(newInputErrors: InputError[]): void {
    inputErrors.value = newInputErrors;

    if (newInputErrors.length > 0) {
      nextTick(() => {
        errorSectionRef.value?.focus();
      });
    }
  }

  function userButtonAction(action: string, isPressed: boolean | null): void {
    const user = userResponse.value.data;

    if (user !== undefined) {
      const ownerAddress = `users/${user.id}`;

      if (isPressed) {
        const statusActions: StatusActions = {
          204: () => {
            if (action === "follow") {
              user.isFollowed = false;
            } else if (action === "trust") {
              user.isTrusted = false;
            }
          }
        }

        const init: RequestInit = { method: "DELETE" };

        apiFetchAuthenticated({ input: `${ownerAddress}/${action}`, statusActions, init });
      } else {
        const statusActions: StatusActions = {
          201: () => {
            if (action === "follow") {
              user.isFollowed = true;
            } else if (action === "trust") {
              user.isTrusted = true;
            }
          }
        }

        const init: RequestInit = { method: "POST" };

        apiFetchAuthenticated({ input: `${ownerAddress}/${action}`, statusActions, init });
      }
    }
  }
</script>

<template>
  <main role="main" id="main" tabindex="-1">
    <section id="errors" v-if="inputErrors.length > 0" ref="errorSectionRef" tabindex="-1" aria-live="assertive" aria-labelledby="heading-errors">
      <h2 id="heading-errors">There was a problem updating your account</h2>
      <ul role="list">
        <li v-for="error in inputErrors" :key="'error' + error.id">
          <a :href="'#input-' + error.id">{{ error.message }}</a>
        </li>
      </ul>
    </section>

    <header v-if="false">
      <figure>
        <img v-if="userResponse.data !== undefined && userResponse.data?.iconImage !== null" :alt="userResponse.data?.iconImage?.altText ?? 'User icon'" :src="userResponse.data?.iconImage?.uri" width="400" height="400" />
        <svg v-else
          view-box="0 0 400 400"
          width="400"
          height="400">
          <rect width="400" height="400" fill="#f0f" />
        </svg>
        <figcaption>Picture Of User</figcaption>
      </figure>
    </header>

    <header v-if="userResponse.data !== undefined">
      <h1>{{ userResponse.data.name }}</h1>

      <menu v-if="store.account.isAuthenticated && store.account.userId !== userResponse.data.id" role="toolbar" aria-label="User Actions">
        <li>
          <button type="button" aria-label="Follow" :aria-pressed="userResponse.data.isFollowed ?? false" @click="() => userButtonAction('follow', userResponse.data!.isFollowed)">{{ userResponse.data.isFollowed ? 'Following' : 'Follow' }}</button>
        </li>
        <li>
          <button v-if="store.account.isTrusted" type="button" aria-label="Trust" :aria-pressed="userResponse.data.isTrusted ?? false" @click="() => userButtonAction('trust', userResponse.data!.isTrusted)">{{ userResponse.data.isTrusted ? 'Trusted' : 'Trust' }}</button>
          <button v-else type="button" aria-label="Trust" :aria-pressed="userResponse.data.isTrusted ?? false" aria-describedby="trust-requirement-hint" aria-disabled="true">{{  userResponse.data.isTrusted ? 'Trusted' : 'Trust'}}</button>
        </li>
      </menu>

      <small v-if="!store.account.isTrusted" id="trust-requirement-hint">Trust is disabled until your account has been trusted by at least 2 other users.</small>
    </header>

    <section v-if="userResponse.fetching === true">
      <p role="status" class="visually-hidden" aria-live="polite">Please wait while the user information is fetched.</p>
              
      <progress max="100">Fetching user...</progress>
    </section>

    <section v-if="userResponse.error !== undefined">
      <p>
        {{ userResponse.error.body }}
      </p>
    </section>

    <template v-if="userResponse.data !== undefined">
      <ul role="tablist" aria-label="User Content">
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
          :data-status="status"
          :data-input-errors="inputErrors"
          :data-notes="Notes"
          :data-works="works"
          :data-event-occurrence-groups="eventOccurrenceGroups"
          :data-spaces="spaces"
          :data-user-id="userResponse.data.id"
          :data-user-handle="userResponse.data.handle"
          :data-user-name="userResponse.data.name"
          @update-notes="handleUpdateNotes"
          @update-works="handleUpdateWorks"
          @update-event-occurrence-groups="handleUpdateEventOccurrenceGroups"
          @update-spaces="handleUpdateSpaces"
          @update-errors="handleUpdateErrors"
        />
      </RouterView>
    </template>
  </main>
</template>
