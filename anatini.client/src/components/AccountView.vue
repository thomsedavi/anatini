<script setup lang="ts">
  import type { InputError, UserEdit } from '@/types';
  import { onMounted, ref } from 'vue';
  import { useRouter } from 'vue-router';
  import { apiFetchAuthenticated } from './common/apiFetch';

  const router = useRouter();
  const user = ref<UserEdit | 'ERROR' | null>(null);
  const errorSummary = ref<HTMLElement | null>(null);
  const inputErrors = ref<InputError[]>([]);

  onMounted(() => {
    const onfulfilled = async (value: UserEdit) => {
      user.value = value;
    };

    const statusActions = {
      401: () => {
        router.replace({ path: '/login', query: { redirect: '/account' } });
      },
      500: () => {
        user.value = 'ERROR';
      }
    };

    apiFetchAuthenticated('account', onfulfilled, undefined, undefined, statusActions);
  });

  function getHeading(): string {
    if (user.value === null) {
      return 'Fetching...';
    } if (user.value === 'ERROR') {
      return 'Unknown Error';
    } else {
      return 'Account Settings';
    }
  }
</script>

<template>
  <main>
    <article :aria-busy="user === null" aria-live="polite" aria-labelledby="heading-main">
      <header>
        <h1 id="heading-main">{{ getHeading() }}</h1>
      </header>

      <section v-if="user === null">
        <p role="status" class="visually-hidden">Please wait while the user information is fetched.</p>
                
        <progress max="100">Fetching account...</progress>
      </section>

      <section v-else-if="user === 'ERROR'">
        <p>
          There was a problem fetching your account, please reload the page
        </p>
      </section>

      <template v-else>
        <section v-if="inputErrors.length > 0" ref="errorSummary" tabindex="-1" aria-live="assertive" aria-labelledby="heading-errors">
          <h2 id="heading-errors">There was a problem updating your account</h2>
          <ul>
            <li v-for="error in inputErrors" :key="'error' + error.id">
              <a :href="'#' + error.id">{{ error.message }}</a>
            </li>
          </ul>
        </section>
      </template>
    </article>
  </main>
</template>
