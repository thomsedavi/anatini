<script setup lang="ts">
  import type { User } from '@/types';
  import { ref, watch } from 'vue';
  import { useRoute } from 'vue-router';
  import { apiFetch } from './common/apiFetch';

  const route = useRoute();

  const fetchingUser = ref<boolean>(true);
  const user = ref<User | null>(null);
  const error = ref<{ header: string, body: string } | null>(null);

  watch([() => route.params.userId], fetchUser, { immediate: true });

  async function fetchUser(array: (() => string | string[])[]) {
    fetchingUser.value = true;
    error.value = user.value = null;

    const onfulfilled = async (value: User) => {
      user.value = value;
    };

    const onfinally = () => {
      fetchingUser.value = false
    };

    const statusActions = {
      404: () => {
        error.value = {
          header: '404 Not Found',
          body: 'This user cannot be found',
        };
      }
    };

    apiFetch(`users/${array[0]}`, onfulfilled, onfinally, undefined, statusActions);
  }

  function getHeading(): string {
    if (fetchingUser.value) {
      return 'Fetching...';
    } if (user.value) {
      return user.value.name;
    } else if (error.value) {
      return error.value.header;
    } else {
      return 'Unknown Error';
    }
  }
</script>

<template>
  <main>
    <article :aria-busy="fetchingUser" aria-live="polite" aria-labelledby="heading-main">
      <header>
        <figure>
          <img v-if="user?.iconImage" :alt="user.iconImage.altText ?? 'User Icon'" :src="user.iconImage.uri" width="400" height="400" />
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

      <section v-if="fetchingUser">
        <p role="status" class="visually-hidden">Please wait while the user information is fetched.</p>
                
        <progress max="100">Loading...</progress>
      </section>
      <section v-else-if="user" aria-label="User biography">
        <p>User biography goes here</p>
      </section>
      <section v-else>
        <p>
          {{error?.body ?? 'Unknown Error'}}
        </p>
      </section>
    </article>
  </main>
</template>
