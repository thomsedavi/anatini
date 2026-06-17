<script setup lang="ts">
  import { ref, onMounted, onUnmounted } from 'vue';
  import { useRouter } from 'vue-router';
  import { store } from './store.ts';
  import type { IsAuthenticated } from '@/types';

  const router = useRouter();

  const dropdownRef = ref<Node | null>(null);
  const isFetching = ref<boolean>(false);
  const isShowing = ref<string[]>([]);

  const handleClickOutside = (event: PointerEvent) => {
    if (dropdownRef.value === null || isShowing.value.length === 0) {
      return;
    } else if (dropdownRef.value && !dropdownRef.value.contains(event.target as Node)) {
      isShowing.value = [];
    }
  }

  onMounted(() => {
    document.addEventListener('click', handleClickOutside);

    isFetching.value = true;

    fetch("/api/authentication/is-authenticated", {
      method: "GET",
    }).then((response: Response) => {
      if (response.ok) {
        response.json()
          .then((value: IsAuthenticated) => {
            store.isAuthenticated = value.isAuthenticated;
            store.isTrusted = value.isTrusted;
            store.spaces = value.spaces;
          })
          .catch(() => {
            store.isAuthenticated = false;
          });
      } else {
        store.isAuthenticated = false;
      }
    }).catch(() => {
      store.isAuthenticated = false;
    }).finally(() => {
      isFetching.value = false;
    });
  });

  onUnmounted(() => {
    document.removeEventListener('click', handleClickOutside)
  })

  async function signOut() {
    fetch("/api/authentication/sign-out", {
      method: "POST",
    }).then((response: Response) => {
      if (response.ok) {
        store.isAuthenticated = false;
      } else {
        store.isAuthenticated = false;
      }
    }).catch(() => {
      store.isAuthenticated = false;
    }).finally(() => {
      isShowing.value = [];
      router.replace({ path: '/' });
    });
  }

  function toggleShow(show: string) {
    if (show === 'main' && isShowing.value.includes('main')) {
      isShowing.value = [];
      return;
    }

    const index = isShowing.value.indexOf(show);
  
    if (index === -1) {
      isShowing.value.push(show);
    } else {
      isShowing.value.splice(index, 1);
    }
  }
</script>

<template>
  <template v-if="store.isAuthenticated !== null">
    <a href="#main" id="skip">Skip to main content</a>

    <header id="main-header">
      <nav id="main-nav" aria-label="Main">
        <RouterLink to="/"><strong>ANATINI</strong></RouterLink>
          <button 
            type="button" 
            :aria-expanded="isShowing.includes('main')" 
            aria-controls="main-dropdown"
            @click.stop="toggleShow('main')"
            aria-label="Open main menu"
          >
          <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" width="1em" height="1em" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true" focusable="false">
            <path d="M3 6h18M3 12h18M3 18h18" />
          </svg>
        </button>
        <ul id="main-dropdown" ref="dropdownRef" role="list" :hidden="!isShowing.includes('main')">
          <li><RouterLink to="/about" @click="isShowing = []">About</RouterLink></li>

          <li id="account-list" v-if="store.isAuthenticated === true">
            <strong id="account">Account</strong>
            <ul aria-labelledby="account" role="list">
              <li>
                <RouterLink to="/account" @click="isShowing = []">Settings</RouterLink>
              </li>
              <li>
                <RouterLink to="/account/notes/create" @click="isShowing = []">Create Note</RouterLink>
              </li>
            </ul>
          </li>

          <li id="spaces-list" v-if="store.spaces !== null">
            <button 
              type="button" 
              :aria-expanded="isShowing.includes('spaces')" 
              aria-controls="spaces-dropdown"
              @click="toggleShow('spaces')"
            >
              Spaces
            </button>
          
            <ul id="spaces-dropdown" role="list" :hidden="!isShowing.includes('spaces')">
              <li v-for="space in store.spaces" :key="'space' + space.id">
                <strong id="space-tech-heading">{{ space.name }}</strong>
                <ul aria-labelledby="space-tech-heading" role="list">
                  <li>
                    <RouterLink :to="`/spaces/${space.handle}/edit`" @click="isShowing = []">Settings</RouterLink>
                  </li>
                  <li>
                    <RouterLink :to="`/spaces/${space.handle}/edit/notes/create`" @click="isShowing = []">Create Note</RouterLink>
                  </li>
                </ul>
              </li>
            </ul>
          </li>
          <template v-if="store.isAuthenticated === null || store.isAuthenticated === false">
            <li id="sign-up"><RouterLink to="/sign-up" @click="isShowing = []">Sign Up</RouterLink></li>
            <li id="sign-in"><RouterLink to="/sign-in" @click="isShowing = []">Sign In</RouterLink></li>
          </template>
          <template v-else>
            <li id="sign-out"><RouterLink to="/" @click.prevent="signOut">Sign Out</RouterLink></li>
          </template>
        </ul>
      </nav>
    </header>

    <RouterView />
  </template>
  <template v-else>
    <p>...</p>
  </template>
</template>
