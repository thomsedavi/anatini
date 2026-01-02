<script setup lang="ts">
  import type { ErrorMessage, InputError, StatusActions, UserEdit } from '@/types';
  import { nextTick, onMounted, ref } from 'vue';
  import { useRouter } from 'vue-router';
  import { apiFetchAuthenticated } from './common/apiFetch';
  import { tidy } from './common/utils';

  const router = useRouter();
  const user = ref<UserEdit | ErrorMessage | null>(null);
  const errorSummary = ref<HTMLElement | null>(null);
  const inputErrors = ref<InputError[]>([]);
  const tab = ref<'public' | 'private'>('public');
  const inputName = ref<string>('');
  const inputBio = ref<string>('');
  const status = ref<'saving' | 'saved' | 'inactive'>('inactive');
  
  onMounted(() => {
    const statusActions: StatusActions = {
      200: (response?: Response) => {
        response?.json()
          .then((value: UserEdit) => {
            user.value = value;
            user.value.bio = user.value.bio?.replace(/\r\n/g, "\n") ?? null;
            inputName.value = value.name;
            inputBio.value = value.bio ?? '';
          })
          .catch(() => { user.value = { error: true, heading: 'Unknown Error', body: 'There was a problem fetching your account, please reload the page' }});
      },
      401: () => {
        router.replace({ path: '/login', query: { redirect: '/account' } });
      },
      500: () => {
        user.value = { error: true, heading: 'Unknown Error', body: 'There was a problem fetching your account, please reload the page' };
      }
    };

    apiFetchAuthenticated('account', statusActions);
  });

  function getHeading(): string {
    if (user.value === null) {
      return 'Fetching...';
    } if ('heading' in user.value) {
      return user.value.heading;
    } else {
      return 'Account Settings';
    }
  }

  function noChange(): boolean {
    if (user.value !== null && 'name' in user.value) {
      return user.value.name === tidy(inputName.value) && (user.value.bio ?? '') === tidy(inputBio.value);
    } else {
      return true;
    }
  }

  function getError(id: string): string | undefined {
    return inputErrors.value.find(inputError => inputError.id === id)?.message;
  }

  async function patchAccount() {
    if (user.value === null || 'error' in user.value) {
      return;
    }

    status.value = 'saving';

    const statusActions: StatusActions = {
      204: () => {
        status.value = 'saved';

        if (user.value !== null && 'name' in user.value) {
          user.value.name = tidy(inputName.value);
        }

        if (user.value !== null && 'bio' in user.value) {
          user.value.bio = tidy(inputBio.value);
        }
      },
      400: (response?: Response) => {
        status.value = 'inactive';

        response?.json().then(value => {
          if (value.errors) {
            if ('Name' in value.errors) {
              inputErrors.value = [{id: 'name-input', message: value.errors['Name'][0]}]
            }

            if ('Bio' in value.errors) {
              inputErrors.value = [{id: 'bio-input', message: value.errors['Bio'][0]}]
            }

            nextTick(() => {
              errorSummary.value?.focus();
            });
          }
        });
      },
      500: () => {
        status.value = 'inactive';

        user.value = { error: true, heading: 'Unknown Error', body: 'There was a problem updating your account, please reload the page' };
      }
    };

    const body = new FormData();

    if (user.value.name !== tidy(inputName.value)) {
      body.append('name', tidy(inputName.value));
    }

    if (user.value.bio !== tidy(inputBio.value)) {
      body.append('bio', tidy(inputBio.value));
    }

    const init = { method: "PATCH", body: body };

    apiFetchAuthenticated('account', statusActions, undefined, init);
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

      <section v-else-if="'body' in user">
        <p>
          {{ user.body }}
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

        <aside>
          <nav aria-label="Account settings sections">
            <ul role="tablist">
              <li role="presentation">
                <button 
                  id="tab-public" 
                  role="tab" 
                  :aria-selected="tab === 'public'"
                  aria-controls="panel-public" 
                  type="button"
                  @click="tab = 'public'">
                  Public
                </button>
              </li>
              <li role="presentation">
                <button 
                  id="tab-private" 
                  role="tab" 
                  :aria-selected="tab === 'private'"
                  aria-controls="panel-private" 
                  type="button" 
                  tabindex="-1"
                  @click="tab = 'private'">
                  Private
                </button>
              </li>
            </ul>
          </nav>
        </aside>

        <section id="panel-public" role="tabpanel" aria-labelledby="tab-public" :hidden="tab !== 'public'">
          <form @submit.prevent="patchAccount" action="/api/account" method="PATCH">
            <header>
              <h2>Public Information</h2>
            </header>

            <fieldset>
              <legend>Public</legend>

              <ul>
                <li>
                  <label for="name-input">Name</label>
                  <input
                    type="text"
                    id="name-input"
                    v-model="inputName"
                    name="name"
                    maxlength="64"
                    aria-describedby="name-help"
                    :aria-invalid="getError('name-input') ? true : undefined"
                    :aria-errormessage="getError('name-input') ? 'name-error' : undefined"
                    autocomplete="name"
                    required />
                  <small id="name-help">Your Name</small>
                  <small v-if="getError('name-input')" id="name-error" role="alert">{{ getError('name-input') ?? 'Unknown Error' }}</small>
                </li>

                <li>
                  <label for="bio-input">Biography</label>
                  <textarea
                    id="bio-input"
                    v-model="inputBio"
                    name="bio"
                    maxlength="256"
                    :aria-invalid="getError('bio-input') ? true : undefined"
                    :aria-errormessage="getError('bio-input') ? 'bio-error' : undefined"
                    aria-describedby="bio-help bio-counter"></textarea>
                  <small id="bio-help">Briefly describe yourself for your public profile.</small>
                  <small v-if="getError('bio-input')" id="bio-error" role="alert">{{ getError('bio-input') ?? 'Unknown Error' }}</small>
                  <output
                    id="bio-counter"
                    :aria-live="256 - tidy(inputBio).length < 20 ? 'assertive' : 'polite'"
                    aria-atomic="true">
                    Characters remaining: {{ 256 - tidy(inputBio).length }}
                  </output>
                </li>
              </ul>
            </fieldset>

            <footer>
              <button type="submit" :disabled="status === 'saving' || noChange()">{{status === 'saving' ? 'Saving...' : 'Save' }}</button>

              <p role="status" class="visually-hidden">{{ status === 'saved' ? 'Saved!' : undefined }}</p>
            </footer>
          </form>
        </section>

        <section id="panel-private" role="tabpanel" aria-labelledby="tab-private" :hidden="tab !== 'private'">
          <h2>Private Information</h2>
        </section>
      </template>
    </article>
  </main>
</template>
