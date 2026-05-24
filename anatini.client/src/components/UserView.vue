<script setup lang="ts">
  import type { ErrorMessage, StatusActions, User } from '@/types';
  import { ref, watch } from 'vue';
  import { useRoute } from 'vue-router';
  import { apiFetch, apiFetchAuthenticated } from './common/apiFetch';import { formatParagraph } from './common/utils';

  const route = useRoute();

  const user = ref<User | ErrorMessage | null>(null);

  watch([() => route.params.userId], fetchUser, { immediate: true });

  async function fetchUser(array: (() => string | string[])[]) {
    user.value = null;

    const statusActions: StatusActions = {
      200: (response?: Response) => {
        response?.json()
          .then((value: User) => {
            user.value = value;
            user.value.about = user.value.about?.replace(/\r\n/g, "\n") ?? null;
          })
          .catch(() => { user.value = { error: true, heading: 'Unknown Error', body: 'There was a problem fetching your account, please reload the page' }});
      },
      404: () => {
        user.value = { error: true, heading: '404 Not Found', body: 'User not found' };
      },
      500: () => {
        user.value = { error: true, heading: 'Unknown Error', body: 'There was a problem fetching this user, please reload the page' };
      }
    };

    apiFetch(`users/${array[0]}`, statusActions);
  }

  function getHeading(): string {
    if (user.value === null) {
      return 'Fetching...';
    } else if ('heading' in user.value) {
      return user.value.heading;
    } else {
      return user.value.name;
    }
  }

  function toggleTrust(): void {
    if (user.value === null || 'heading' in user.value || user.value.hasTrusted === null) {
      return;
    }

    if (user.value.hasTrusted) {
      const statusActions: StatusActions = {
        204: () => {
          if (user.value === null || 'heading' in user.value) {
            return;
          }

          user.value.hasTrusted = false;
        }
      }

      const init: RequestInit = { method: "DELETE" };

      apiFetchAuthenticated(`users/${route.params.userId}/trust`, statusActions, init);
    } else {
      const statusActions: StatusActions = {
        201: () => {
          if (user.value === null || 'heading' in user.value) {
            return;
          }

          user.value.hasTrusted = true;
        }
      }

      const init: RequestInit = { method: "POST" };

      apiFetchAuthenticated(`users/${route.params.userId}/trust`, statusActions, init);
    }
  }
</script>

<template>
  <main id="main" tabindex="-1">
    <article :aria-busy="user === null" aria-labelledby="heading-main">
      <header>
        <figure>
          <img v-if="user && 'iconImage' in user && user.iconImage" :alt="user.iconImage.altText ?? 'User icon'" :src="user.iconImage.uri" width="400" height="400" />
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

      <section v-if="user === null">
        <p role="status" class="visuallyhidden" aria-live="polite">Please wait while the user information is fetched.</p>
                
        <progress max="100">Fetching user...</progress>
      </section>

      <section v-else-if="'error' in user">
        <p>
          {{ user.body }}
        </p>
      </section>

      <template v-else>
        <section v-if="user.about !== null" aria-label="About user" v-html="formatParagraph(user.about)">
        </section>

        <menu v-if="user.hasTrusted !== null">
          <li>
            <button type="button" :aria-pressed="user.hasTrusted" @click="toggleTrust">{{ user.hasTrusted ? "Remove Trust" : "Trust" }}</button>
          </li>
        </menu>
      </template>
    </article>
  </main>
</template>
