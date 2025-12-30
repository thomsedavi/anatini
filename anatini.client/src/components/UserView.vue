<script setup lang="ts">
  import type { User } from '@/types';
  import { ref, watch } from 'vue';
  import { useRoute } from 'vue-router';
  import { apiFetch } from './common/apiFetch';

  const route = useRoute();

  const loading = ref<boolean>(true);
  const error = ref<string | null>(null);
  const user = ref<User | null>(null);

  watch([() => route.params.userId], fetchUser, { immediate: true });

  async function fetchUser(array: (() => string | string[])[]) {
    error.value = user.value = null;
    loading.value = true;

    const onfulfilled = async (value: User) => {
      user.value = value;
    };

    const onfinally = () => {
      loading.value = false;
    };

    apiFetch(`users/${array[0]}`, onfulfilled, onfinally);
  }

  async function trust() {
    if (user.value === null) {
      return;
    }

    const onfulfilled = async (value: string) => {
      console.log(value);
    };

    const onfinally = () => {
      loading.value = false;
    };

    const body = new FormData();

    body.append('type', 'Trusts');

    const init = { method: "POST", body: body };

    apiFetch(`users/${user.value.id}/relationships`, onfulfilled, onfinally, init);
  }
</script>

<template>
  <main v-if="user">
    <article>
      <header aria-busy="false">
        <figure>
          <img v-if="user.iconImageUri" alt="Image" :src="user.iconImageUri" width="400" height="400" />
          <figcaption>Member since 2023</figcaption>
        </figure>

        <h1>{{ user.name }}</h1>
      </header>

      <section aria-label="User biography">
        <p>User biography goes here</p>
      </section>

      <footer>
        <menu type="toolbar">
          <li>
            <button type="button" aria-pressed="false" @click="() => trust()">Trust</button>
          </li>
          <li>
            <button type="button" class="danger">Block</button>
          </li>
        </menu>
      </footer>
    </article>
  </main>
  <main v-else>
    <article aria-busy="true" aria-live="polite" aria-labelledby="loading-title">
      <header>
        <h1 id="loading-title">Loading user...</h1>
      </header>

      <section>
        <p role="status">Please wait while the user information is fetched.</p>
                
        <progress max="100">Loading...</progress>
      </section>
    </article>
  </main>
</template>
