<script setup lang="ts">
  import type { ErrorMessage, InputError, StatusActions, UserEdit } from '@/types';
  import { nextTick, onMounted, ref } from 'vue';
  import { useRouter } from 'vue-router';
  import { apiFetchAuthenticated } from './common/apiFetch';
  import { tidy } from './common/utils';
  import InputListItem from './common/InputListItem.vue';

  const router = useRouter();
  const user = ref<UserEdit | ErrorMessage | null>(null);
  const errorSectionRef = ref<HTMLElement | null>(null);
  const inputErrors = ref<InputError[]>([]);
  const tab = ref<'public' | 'private' | 'channels'>('public');
  const inputName = ref<string>('');
  const inputChannelName = ref<string>('');
  const inputChannelSlug = ref<string>('');
  const inputBio = ref<string>('');
  const status = ref<'saving' | 'saved' | 'inactive'>('inactive');

  const channelPermissions = ["Admin", "Trusted"];
  
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

  async function postChannel() {
    inputErrors.value = [];

    if (tidy(inputChannelName.value) === '') {
      inputErrors.value.push({id: 'channel-name', message: 'Channel name is required'});
    }

    if (tidy(inputChannelSlug.value) === '') {
      inputErrors.value.push({id: 'channel-slug', message: 'Channel slug is required'});
    }

    if (inputErrors.value.length > 0) {
      nextTick(() => {
        errorSectionRef.value?.focus();
      });

      return;
    }

    alert('todo');
  }

  async function patchAccount() {
    if (user.value === null || 'error' in user.value) {
      return;
    }

    const tidiedName = tidy(inputName.value);
    const tidiedBio = tidy(inputBio.value);

    if (tidiedName === '') {
      inputErrors.value = [{id: 'name', message: 'Name is required'}]
      return;
    }

    status.value = 'saving';

    const statusActions: StatusActions = {
      204: () => {
        status.value = 'saved';

        if (user.value !== null && 'name' in user.value) {
          user.value.name = tidiedName;
        }

        if (user.value !== null && 'bio' in user.value) {
          user.value.bio = tidiedBio;
        }
      },
      400: (response?: Response) => {
        status.value = 'inactive';

        response?.json().then(value => {
          if (value.errors) {
            if ('Name' in value.errors) {
              inputErrors.value = [{id: 'name', message: value.errors['Name'][0]}]
            }

            if ('Bio' in value.errors) {
              inputErrors.value = [{id: 'bio', message: value.errors['Bio'][0]}]
            }

            nextTick(() => {
              errorSectionRef.value?.focus();
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

    if (user.value.name !== tidiedName) {
      body.append('name', tidiedName);
    }

    if (user.value.bio !== tidiedBio) {
      body.append('bio', tidiedBio);
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
        <section v-if="inputErrors.length > 0" ref="errorSectionRef" tabindex="-1" aria-live="assertive" aria-labelledby="heading-errors">
          <h2 id="heading-errors">There was a problem updating your account</h2>
          <ul>
            <li v-for="error in inputErrors" :key="'error' + error.id">
              <a :href="'#input-' + error.id">{{ error.message }}</a>
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
                  Display
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
                  Privacy & Security
                </button>
              </li>
              <li role="presentation">
                <button 
                  id="tab-channels" 
                  role="tab" 
                  :aria-selected="tab === 'channels'"
                  aria-controls="panel-channels" 
                  type="button" 
                  tabindex="-1"
                  @click="tab = 'channels'">
                  Channels
                </button>
              </li>
            </ul>
          </nav>
        </aside>

        <section id="panel-public" role="tabpanel" aria-labelledby="tab-public" :hidden="tab !== 'public'">
          <header>
            <h2>Display</h2>
          </header>

          <form @submit.prevent="patchAccount" action="/api/account" method="PATCH" novalidate>
            <fieldset>
              <legend>Public</legend>

              <ul>
                <InputListItem
                  v-model="inputName"
                  label="Name"
                  name="name"
                  id="name"
                  :maxlength="64"
                  :error="getError('name')"
                  help="Your display name"
                  autocomplete="name" />

                <li>
                  <label for="input-bio">Biography</label>
                  <textarea
                    id="input-bio"
                    v-model="inputBio"
                    name="bio"
                    maxlength="256"
                    :aria-invalid="getError('bio') ? true : undefined"
                    :aria-errormessage="getError('bio') ? 'bio-error' : undefined"
                    aria-describedby="bio-help bio-counter"></textarea>
                  <small id="bio-help">Briefly describe yourself for your public profile.</small>
                  <small v-if="getError('bio')" id="bio-error" role="alert">{{ getError('bio') ?? 'Unknown Error' }}</small>
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
          <header>
            <h2>Privacy & Security</h2>
          </header>
        </section>

        <section id="panel-channels" role="tabpanel" aria-labelledby="tab-channels" :hidden="tab !== 'channels'" >
          <header>
            <h2>Channels</h2>
          </header>

          <form @submit.prevent="postChannel" action="/api/channels" method="POST" novalidate v-if="user.permissions?.some(permission => channelPermissions.includes(permission))">
            <fieldset>
              <legend>Create Channel</legend>

              <ul>
                <InputListItem
                  v-model="inputChannelName"
                  label="Name*"
                  name="name"
                  id="channel-name"
                  :maxlength="64"
                  help="The display name of your channel"
                  :error="getError('channel-name')" />

                <InputListItem
                  v-model="inputChannelSlug"
                  label="Slug*"
                  name="slug"
                  id="channel-slug"
                  :maxlength="64"
                  help="The unique URL of your channel, lower case with hyphens (e.g. 'my-anatini-channel')"
                  :error="getError('channel-slug')" />
              </ul>
            </fieldset>

            <footer>
              <button type="submit" :disabled="status === 'saving' || tidy(inputChannelName) === '' || tidy(inputChannelSlug) === ''">{{status === 'saving' ? 'Creating...' : 'Create' }}</button>

              <p role="status" class="visually-hidden">{{ status === 'saved' ? 'Created!' : undefined }}</p>
            </footer>
          </form>

          <article v-else>
            <header>
              <h3>Insufficient permission</h3>
            </header>

            <p>You do not currently have permission to create Channels</p>
          </article>

          <section aria-labelledby="section-your-channels">
            <header>
              <h3 id="section-your-channels">Your Channels</h3>
            </header>

            <ul role="list" v-if="(user.channels?.length ?? 0) > 0">
              <li v-for="channel in user.channels" :key="`channel-${channel.defaultSlug}`">
                <article :aria-labelledby="`channel-${channel.defaultSlug}`">
                  <header>
                    <h4 :id="`channel-${channel.defaultSlug}`">
                      <a>{{channel.name}}</a>
                    </h4>
                  </header>

                  <p>Channel Description Goes Here</p>

                  <footer>
                    <small>Slug: <code>{{ channel.defaultSlug }}</code></small>
                  </footer>
                </article>
              </li>
            </ul>

            <p v-else>You do not have any channels</p>
          </section>
        </section>
      </template>
    </article>
  </main>
</template>
