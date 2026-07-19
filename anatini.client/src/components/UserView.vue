<script setup lang="ts">
  import type { APIResponse, StatusActions, User } from '@/types';
  import { ref, watch } from 'vue';
  import { useRoute } from 'vue-router';
  import { apiFetch, apiFetchAuthenticated } from './common/apiFetch';
  import { formatParagraph } from './common/utils';

  const route = useRoute();

  const user = ref<APIResponse<User>>({ fetching: true });

  watch([() => route.params.userId], fetchUser, { immediate: true });

  async function fetchUser(array: (() => string | string[])[]) {
    user.value = { fetching: true };

    const input = `users/${array[0]}`;

    const statusActions: StatusActions = {
      200: (response?: Response) => {
        response?.json()
          .then((value: User) => {
            user.value = {
              data: { ...value, about: value.about?.replace(/\r\n/g, "\n") ?? null }
            };
          })
          .catch(() => { user.value = { error: { heading: 'Unknown Error', body: 'There was a problem fetching your account, please reload the page' }}});
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
</script>

<template>
  <main id="main" tabindex="-1">
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
        <section v-if="user.data.about !== null" aria-label="About user" v-html="formatParagraph(user.data.about)">
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
  </main>
</template>
