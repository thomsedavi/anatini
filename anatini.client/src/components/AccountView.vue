<script setup lang="ts">
  import type { Channel, ErrorMessage, InputError, StatusActions, UserEdit } from '@/types';
  import { nextTick, onMounted, ref } from 'vue';
  import { useRouter } from 'vue-router';
  import { apiFetchAuthenticated } from './common/apiFetch';
  import { getTabIndex, tidy } from './common/utils';
  import InputText from './common/InputText.vue';
  import TabListItem from './common/TabListItem.vue';

  const router = useRouter();
  const user = ref<UserEdit | ErrorMessage | null>(null);
  const errorSectionRef = ref<HTMLElement | null>(null);
  const inputErrors = ref<InputError[]>([]);
  const inputName = ref<string>('');
  const inputBio = ref<string>('');
  const inputProtected = ref<boolean>(false);
  const inputChannelName = ref<string>('');
  const inputChannelSlug = ref<string>('');
  const status = ref<'saving' | 'saved' | 'inactive'>('inactive');
  const tabIndex = ref<number>(0);

  const tabs = ref([
    { id: 'public', text: 'Display' },
    { id: 'private', text: 'Privacy & Security' },
    { id: 'channels', text: 'Channels' }
  ]);

  const tabRefs = ref<HTMLButtonElement[]>([]);

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
            inputProtected.value = value.protected ?? false;
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
    } if ('error' in user.value) {
      return user.value.heading;
    } else {
      return 'Account Settings';
    }
  }

  function noChangeDisplay(): boolean {
    if (user.value !== null && 'name' in user.value) {
      return user.value.name === tidy(inputName.value) && (user.value.bio ?? '') === tidy(inputBio.value);
    } else {
      return true;
    }
  }

  function noChangePrivacy(): boolean {
    if (user.value !== null && 'protected' in user.value) {
      return (user.value.protected ?? false) === inputProtected.value;
    } else {
      return true;
    }
  }

  function getError(id: string): string | undefined {
    return inputErrors.value.find(inputError => inputError.id === id)?.message;
  }

  async function postChannel() {
    inputErrors.value = [];

    const tidiedName = tidy(inputChannelName.value);
    const tidiedSlug = tidy(inputChannelSlug.value);

    if (tidiedName === '') {
      inputErrors.value.push({id: 'channel-name', message: 'Channel name is required'});
    }

    if (tidiedSlug === '') {
      inputErrors.value.push({id: 'channel-slug', message: 'Channel slug is required'});
    }

    if (inputErrors.value.length > 0) {
      nextTick(() => {
        errorSectionRef.value?.focus();
      });

      return;
    }

    status.value = 'saving';

    const statusActions: StatusActions = {
      201: (response?: Response) => {
        status.value = 'saved';
        
        response?.json().then((value: Channel) => {
          if (user.value !== null && 'channels' in user.value) {
            const channels = user.value.channels ?? [];
            channels.push(value);
            user.value.channels = channels;
          }
        });

        inputChannelName.value = '';
        inputChannelSlug.value = '';
      },
      400: (response?: Response) => {
        status.value = 'inactive';

        response?.json().then(value => {
          if (value.errors) {
            if ('Name' in value.errors) {
              inputErrors.value = [{id: 'channel-name', message: value.errors['Name'][0]}]
            }

            if ('Slug' in value.errors) {
              inputErrors.value = [{id: 'channel-slug', message: value.errors['Slug'][0]}]
            }

            nextTick(() => {
              errorSectionRef.value?.focus();
            });
          }
        });
      },
      409: () => {
        status.value = 'inactive';

        inputErrors.value = [{ id: 'channel-slug', message: 'Slug already in use' }]

        nextTick(() => {
          errorSectionRef.value?.focus();
        });
      },
      500: () => {
        status.value = 'inactive';

        // TODO handle this better
        inputErrors.value = [{ id: 'channel-name', message: 'Unknown Error' }]

        nextTick(() => {
          errorSectionRef.value?.focus();
        });
      }
    }

    const body = new FormData();

    body.append('name', tidiedName);
    body.append('slug', tidiedSlug);

    const init = { method: "POST", body: body };

    apiFetchAuthenticated('channels', statusActions, undefined, init);
  }

  async function patchAccountDisplay() {
    inputErrors.value = [];

    if (user.value === null || 'error' in user.value) {
      return;
    }

    const tidiedName = tidy(inputName.value);
    const tidiedBio = tidy(inputBio.value);

    if (tidiedName === '') {
      inputErrors.value = [{ id: 'name', message: 'Name is required' }];

      nextTick(() => {
        errorSectionRef.value?.focus();
      });

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

        // TODO handle this better
        inputErrors.value = [{ id: 'name', message: 'Unknown Error' }]

        nextTick(() => {
          errorSectionRef.value?.focus();
        });
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

  async function patchAccountPrivacy() {
    inputErrors.value = [];

    if (user.value === null || 'error' in user.value) {
      return;
    }

    status.value = 'saving';

    const statusActions: StatusActions = {
      204: () => {
        status.value = 'saved';

        if (user.value !== null && 'name' in user.value) {
          user.value.protected = inputProtected.value === false ? null : true;
        }
      },
      400: (response?: Response) => {
        status.value = 'inactive';

        response?.json().then(value => {
          if (value.errors) {
            if ('Protected' in value.errors) {
              inputErrors.value = [{id: 'protected', message: value.errors['Protected'][0]}]
            }

            nextTick(() => {
              errorSectionRef.value?.focus();
            });
          }
        });
      },
      500: () => {
        status.value = 'inactive';

        // TODO handle this better
        inputErrors.value = [{ id: 'name', message: 'Unknown Error' }]

        nextTick(() => {
          errorSectionRef.value?.focus();
        });
      }
    };

    const body = new FormData();

    if ((user.value.protected ?? false) !== inputProtected.value) {
      body.append('protected', inputProtected.value ? 'true' : 'false');
    }

    const init = { method: "PATCH", body: body };

    apiFetchAuthenticated('account', statusActions, undefined, init);
  }

  function handleKeyDown(event: KeyboardEvent, index: number): void {
    const newIndex = getTabIndex(event.key, index, tabs.value.length);

    if (newIndex === undefined) {
      return;
    }

    event.preventDefault();
    tabIndex.value = newIndex;
    
    nextTick(() => {
      tabRefs.value[newIndex].focus();
    })
  }
</script>

<template>
  <main>
    <article :aria-busy="user === null" aria-live="polite" aria-labelledby="heading-main">
      <header>
        <h1 id="heading-main">{{ getHeading() }}</h1>
      </header>

      <section v-if="user === null">
        <p role="status" class="visuallyhidden">Please wait while the user information is fetched.</p>
                
        <progress max="100">Fetching account...</progress>
      </section>

      <section v-else-if="'error' in user">
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

        <ul role="tablist" aria-label="Settings Options">
          <TabListItem v-for="(tab, index) in tabs"
            :key="tab.id"
            :selected="tabIndex === index"
            @click="tabIndex = index"
            @keydown="handleKeyDown($event, index)"
            :text="tab.text"
            :id="tab.id"
            :add-button-ref="(el: HTMLButtonElement | null) => {if (el) tabRefs.push(el)}" />
        </ul>

        <section id="panel-public" role="tabpanel" tabindex="0" aria-labelledby="tab-public" :hidden="tabIndex !== 0">
          <header>
            <h2>Display</h2>
          </header>

          <form @submit.prevent="patchAccountDisplay" action="/api/account" method="PATCH" novalidate>
            <fieldset>
              <legend>Public</legend>

              <InputText
                v-model="inputName"
                label="Name"
                name="name"
                id="name"
                :maxlength="64"
                :error="getError('name')"
                help="Your display name"
                autocomplete="name" />

              <br>

              <label for="input-bio">Biography</label>
              <span>
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
              </span>
            </fieldset>

            <footer>
              <button type="submit" :disabled="status === 'saving' || noChangeDisplay()">{{status === 'saving' ? 'Saving...' : 'Save' }}</button>

              <p role="status" class="visuallyhidden">{{ status === 'saved' ? 'Saved!' : undefined }}</p>
            </footer>
          </form>
        </section>

        <section id="panel-private" role="tabpanel" tabindex="0" aria-labelledby="tab-private" :hidden="tabIndex !== 1">
          <header>
            <h2>Privacy & Security</h2>
          </header>

          <form @submit.prevent="patchAccountPrivacy" action="/api/account" method="PATCH" novalidate>
            <fieldset>
              <legend>Privacy & Security</legend>

              <input type="checkbox" id="input-protected" name="protected" v-model="inputProtected" aria-describedby="help-protected" />
              <span>
                <label for="input-protected">Protected</label>
                <small id="help-protected">Your account will only be visible to trusted users</small>
              </span>
            </fieldset>

            <footer>
              <button type="submit" :disabled="status === 'saving' || noChangePrivacy()">{{status === 'saving' ? 'Saving...' : 'Save' }}</button>

              <p role="status" class="visuallyhidden">{{ status === 'saved' ? 'Saved!' : undefined }}</p>
            </footer>
          </form>
        </section>

        <section id="panel-channels" role="tabpanel" tabindex="0" aria-labelledby="tab-channels" :hidden="tabIndex !== 2" >
          <header>
            <h2>Channels</h2>
          </header>

          <form @submit.prevent="postChannel" action="/api/channels" method="POST" novalidate v-if="user.permissions?.some(permission => channelPermissions.includes(permission))">
            <fieldset>
              <legend>Create Channel</legend>
                <InputText
                  v-model="inputChannelName"
                  label="Name*"
                  name="name"
                  id="channel-name"
                  :maxlength="64"
                  help="The display name of your channel"
                  :error="getError('channel-name')" />

                <br>

                <InputText
                  v-model="inputChannelSlug"
                  label="Slug*"
                  name="slug"
                  id="channel-slug"
                  :maxlength="64"
                  help="lower case with hyphens (e.g. 'my-anatini-channel')"
                  :error="getError('channel-slug')" />
            </fieldset>

            <footer>
              <button type="submit" :disabled="status === 'saving' || tidy(inputChannelName) === '' || tidy(inputChannelSlug) === ''">{{status === 'saving' ? 'Creating...' : 'Create' }}</button>

              <p role="status" class="visuallyhidden">{{ status === 'saved' ? 'Created!' : undefined }}</p>
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

            <ul role="list" v-if="(user.channels?.length ?? 0) > 0" aria-live="polite" aria-relevant="additions">
              <li v-for="channel in user.channels" :key="`channel-${channel.defaultSlug}`">
                <article :aria-labelledby="`channel-${channel.defaultSlug}`">
                  <header>
                    <h4 :id="`channel-${channel.defaultSlug}`">
                      <RouterLink :to="{ name: 'ChannelEdit', params: { channelId: channel.defaultSlug }}">{{ channel.name }}</RouterLink>
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
