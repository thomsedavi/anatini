<script setup lang="ts">
  import type { APIResponse, InputError, Note, Status, StatusActions, Tab, User } from '@/common/types';
  import { nextTick, ref, watch } from 'vue';
  import { useRoute, useRouter } from 'vue-router';
  import { apiFetch, apiFetchAuthenticated } from '@/common/apiFetch';
  import { getTabIndex, parseSource, type Source } from '@/common/utils';
  import TabButton from '@/common/TabButton.vue';

  const route = useRoute();
  const router = useRouter();

  const user = ref<APIResponse<User>>({ fetching: true });
  const errorSectionRef = ref<HTMLElement | null>(null);
  const inputErrors = ref<InputError[]>([]);
  const status = ref<Status>('idle');
  const tabIndex = ref<number>(-1);
  const notes = ref<Note[] | null>(null);

  const tabs: Tab[] = [
    { id: 'notes', text: 'Notes', name: 'UserNotes', childNames: ['UserNoteCreate', 'UserNoteEdit'] },
    { id: 'events', text: 'Events', name: 'UserEvents', childNames: ['UserEventCreate'] },
  ];

  const tabRefs = ref<HTMLButtonElement[]>([]);

  watch([() => route.params.userId], (source: Source) => fetchUser(parseSource(source)), { immediate: true });

  async function fetchUser(params: string[]) {
    tabIndex.value = tabs.findIndex(tab => tab.name === route.name || tab.childNames?.includes(route.name));

    user.value = { fetching: true };

    const input = `users/${params[0]}`;

    const statusActions: StatusActions = {
      200: (response?: Response) => {
        response?.json()
          .then((value: User) => {
            user.value = {
              data: { ...value, about: value.about?.replace(/\r\n/g, "\n") ?? null }
            };
          })
          .catch(() => {
            user.value = { error: { heading: 'Unknown Error', body: 'There was a problem fetching your account, please reload the page' }};
          });
      },
      404: () => {
        user.value = { error: { heading: '404 Not Found', body: 'User not found' }};
      },
      500: () => {
        user.value = { error: { heading: 'Unknown Error', body: 'There was a problem fetching this user, please reload the page' }};
      }
    };

    apiFetch({ input, statusActions });
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

  function getHeading(): string {
    if (user.value.fetching === true) {
      return 'Fetching...';
    } else if (user.value.error !== undefined) {
      return user.value.error.heading;
    } else if (user.value.data !== undefined) {
      return user.value.data.name;
    } else {
      return 'Unknown Error';
    }
  }

  function toggleTrust(): void {
    if (user.value.data?.hasTrusted === true) {
      const input = `users/${route.params.userId}/trust`;

      const statusActions: StatusActions = {
        204: () => {
          if (user.value.data !== undefined) user.value.data.hasTrusted = false;
        }
      }

      const init: RequestInit = { method: "DELETE" };

      apiFetchAuthenticated({ input, statusActions, init });
    } else if (user.value.data?.hasTrusted === false) {
      const input = `users/${route.params.userId}/trust`;

      const statusActions: StatusActions = {
        201: () => {
          if (user.value.data !== undefined) user.value.data.hasTrusted = true;
        }
      }

      const init: RequestInit = { method: "POST" };

      apiFetchAuthenticated({ input, statusActions, init });
    }
  }

  function toggleFollow(): void {
    if (user.value.data?.hasFollowed === true) {
      const input = `users/${route.params.userId}/follow`;

      const statusActions: StatusActions = {
        204: () => {
          if (user.value.data !== undefined) user.value.data.hasFollowed = false;
        }
      }

      const init: RequestInit = { method: "DELETE" };

      apiFetchAuthenticated({ input, statusActions, init });
    } else if (user.value.data?.hasFollowed === false) {
      const input = `users/${route.params.userId}/follow`;

      const statusActions: StatusActions = {
        201: () => {
          if (user.value.data !== undefined) user.value.data.hasFollowed = true;
        }
      }

      const init: RequestInit = { method: "POST" };

      apiFetchAuthenticated({ input, statusActions, init });
    }
  }

  function handleUpdateNotes(newNotes: Note[]): void {
    notes.value = newNotes;
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
  <main id="main" tabindex="-1">
    <section id="errors" v-if="inputErrors.length > 0" ref="errorSectionRef" tabindex="-1" aria-live="assertive" aria-labelledby="heading-errors">
      <h2 id="heading-errors">There was a problem updating your account</h2>
      <ul role="list">
        <li v-for="error in inputErrors" :key="'error' + error.id">
          <a :href="'#input-' + error.id">{{ error.message }}</a>
        </li>
      </ul>
    </section>

    <article :aria-busy="user.fetching === true" aria-labelledby="heading-main">
      <header>
        <figure>
          <img v-if="user.data !== undefined && user.data.iconImage !== null" :alt="user.data.iconImage.altText ?? 'User icon'" :src="user.data.iconImage.uri" width="400" height="400" />
          <svg v-else
            view-box="0 0 400 400"
            width="400"
            height="400">
            <rect width="400" height="400" fill="#f0f" />
          </svg>
          <figcaption>Picture Of User</figcaption>
        </figure>

        <h1 id="heading-main">{{ getHeading() }}</h1>
      </header>

      <section v-if="user.fetching === true">
        <p role="status" class="visuallyhidden" aria-live="polite">Please wait while the user information is fetched.</p>
                
        <progress max="100">Fetching user...</progress>
      </section>

      <section v-if="user.error !== undefined">
        <p>
          {{ user.error.body }}
        </p>
      </section>

      <template v-if="user.data !== undefined">
        <section v-if="user.data.about !== null" aria-label="About user" v-html="user.data.about">
        </section>

        <menu v-if="user.data.hasTrusted !== null || user.data.hasFollowed !== null">
          <li v-if="user.data.hasTrusted !== null">
            <button type="button" :aria-pressed="user.data.hasTrusted" @click="toggleTrust">{{ user.data.hasTrusted ? "Remove Trust" : "Trust" }}</button>
          </li>
          <li v-if="user.data.hasFollowed !== null">
            <button type="button" :aria-pressed="user.data.hasFollowed" @click="toggleFollow">{{ user.data.hasFollowed ? "Remove Follow" : "Follow" }}</button>
          </li>
        </menu>
      </template>
    </article>

    <template v-if="user.data !== undefined">
      <ul role="tablist" aria-label="User Content">
        <TabButton v-for="(tab, index) in tabs"
          :key="tab.id"
          :selected="tabIndex === index"
          @click="() => handleClick(index)"
          @keydown="(event: KeyboardEvent) => handleKeyDown(event, index)"
          :text="tab.text"
          :id="tab.id"
          :add-button-ref="(el: HTMLButtonElement) => { tabRefs.push(el); }" />
      </ul>

      <RouterView v-slot="{ Component }">
        <component
          :is="Component"
          :status="status"
          :inputErrors="inputErrors"
          :notes="notes"
          :userId="user.data.id"
          @update-notes="handleUpdateNotes"
          @update-errors="handleUpdateErrors"
        />
      </RouterView>
    </template>
  </main>
</template>
