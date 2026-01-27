<script setup lang="ts">
  import type { Channel, ErrorMessage, InputError, Status, StatusActions, UserEdit } from '@/types';
  import { nextTick, onMounted, ref } from 'vue';
  import { useRouter } from 'vue-router';
  import { apiFetchAuthenticated } from './common/apiFetch';
  import { getTabIndex, tidy } from './common/utils';
  import InputText from './common/InputText.vue';
  import TabButton from './common/TabButton.vue';
  import SubmitButton from './common/SubmitButton.vue';
  import InputTextArea from './common/InputTextArea.vue';

  const router = useRouter();
  const user = ref<UserEdit | ErrorMessage | null>(null);
  const errorSectionRef = ref<HTMLElement | null>(null);
  const inputErrors = ref<InputError[]>([]);
  const inputUserName = ref<string>('');
  const inputUserAbout = ref<string>('');
  const inputProtected = ref<boolean>(false);
  const inputChannelName = ref<string>('');
  const inputChannelSlug = ref<string>('');
  const inputChannelAbout = ref<string>('');
  const status = ref<Status>('idle');
  const tabIndex = ref<number>(0);
  const pageStatus = ref<string>('Loading account information...'); // TODO add other statuses
  const headingMainRef = ref<HTMLElement | null>(null);

  const fileUserIcon = ref<File | null>(null);
  const previewUrl = ref<string | null>(null);
  const uploadStatus = ref<string>('No file selected');

  const tabs = ref([
    { id: 'public', text: 'Display' },
    { id: 'private', text: 'Privacy & Security' },
    { id: 'channels', text: 'Channels' },
  ]);

  const tabRefs = ref<HTMLButtonElement[]>([]);

  const channelPermissions = ["Admin", "Trusted"];

  const onChooseFile = (event: Event) => {
    const input = event?.target as HTMLInputElement

    const file = input?.files?.[0];
  
    if (!file) {
      return
    }

    if (!file.type.startsWith('image/')) {
      uploadStatus.value = 'Please select an image file';
      return;
    }

    fileUserIcon.value = file;
    previewUrl.value = URL.createObjectURL(file);
    uploadStatus.value = 'File selected';
  };
  
  onMounted(() => {
    const statusActions: StatusActions = {
      200: (response?: Response) => {
        response?.json()
          .then((value: UserEdit) => {
            user.value = value;
            user.value.about = user.value.about?.replace(/\r\n/g, "\n") ?? null;
            inputUserName.value = value.name;
            inputUserAbout.value = value.about ?? '';
            inputProtected.value = value.protected ?? false;
            pageStatus.value = '';

            nextTick(() => {
              headingMainRef.value?.focus();
            });
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
      return user.value.name === tidy(inputUserName.value) && (user.value.about ?? '') === tidy(inputUserAbout.value) && fileUserIcon.value === null;
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
    const tidiedAbout = tidy(inputChannelAbout.value);

    if (tidiedName === '') {
      inputErrors.value.push({id: 'name-channel', message: 'Channel name is required'});
    }

    if (tidiedSlug === '') {
      inputErrors.value.push({id: 'slug-channel', message: 'Channel slug is required'});
    }

    if (inputErrors.value.length > 0) {
      nextTick(() => {
        errorSectionRef.value?.focus();
      });

      return;
    }

    status.value = 'pending';

    const statusActions: StatusActions = {
      201: (response?: Response) => {
        status.value = 'success';
        
        response?.json().then((value: Channel) => {
          if (user.value !== null && 'channels' in user.value) {
            const channels = user.value.channels ?? [];
            channels.push(value);
            user.value.channels = channels;
          }
        });

        inputChannelName.value = '';
        inputChannelSlug.value = '';
        inputChannelAbout.value = '';
      },
      400: (response?: Response) => {
        status.value = 'error';

        response?.json().then(value => {
          if (value.errors) {
            if ('Name' in value.errors) {
              inputErrors.value = [{id: 'name-channel', message: value.errors['Name'][0]}]
            }

            if ('Slug' in value.errors) {
              inputErrors.value = [{id: 'slug-channel', message: value.errors['Slug'][0]}]
            }

            nextTick(() => {
              errorSectionRef.value?.focus();
            });
          }
        });
      },
      409: () => {
        status.value = 'error';

        inputErrors.value = [{ id: 'slug-channel', message: 'Slug already in use' }]

        nextTick(() => {
          errorSectionRef.value?.focus();
        });
      },
      500: () => {
        status.value = 'error';

        // TODO handle this better
        inputErrors.value = [{ id: 'name-channel', message: 'Unknown Error' }]

        nextTick(() => {
          errorSectionRef.value?.focus();
        });
      }
    }

    const body = new FormData();

    body.append('name', tidiedName);
    body.append('slug', tidiedSlug);

    if (tidiedAbout !== '') {
      body.append('about', tidiedAbout);
    }

    const init = { method: "POST", body: body };

    apiFetchAuthenticated('channels', statusActions, init);
  }

  async function patchAccountDisplay() {
    inputErrors.value = [];

    if (user.value === null || 'error' in user.value) {
      return;
    }

    const tidiedName = tidy(inputUserName.value);
    const tidiedAbout = tidy(inputUserAbout.value);

    if (tidiedName === '') {
      inputErrors.value = [{ id: 'name-user', message: 'Name is required' }];

      nextTick(() => {
        errorSectionRef.value?.focus();
      });

      return;
    }

    status.value = 'pending';

    const body = new FormData();

    if (user.value.name !== tidiedName) {
      body.append('name', tidiedName);
    }

    if (user.value.about !== tidiedAbout) {
      body.append('about', tidiedAbout);
    }

    if (fileUserIcon.value !== null) {
      const bodyIcon = new FormData();

      bodyIcon.append('file', fileUserIcon.value);
      bodyIcon.append('type', 'Icon');

      const statusActionsIcon: StatusActions = {
        201: (response?: Response) => {
          response?.json()
            .then((value: { id: string }) => {
              body.append('iconImageId', value.id);

              patch(body, tidiedName, tidiedAbout);

              fileUserIcon.value = null; 
            });
        },
        500: () => {
          status.value = 'error';

          // TODO handle this better
          inputErrors.value = [{ id: 'icon-user', message: 'Unknown Error' }]

          nextTick(() => {
            errorSectionRef.value?.focus();
          });
        }
      }

      const initIcon = { method: "POST", body: bodyIcon };

      apiFetchAuthenticated('account/images', statusActionsIcon, initIcon);
    } else {
      patch(body, tidiedName, tidiedAbout);
    }
  }

  async function patch(body: FormData, tidiedName: string, tidiedAbout: string) {
    const statusActions: StatusActions = {
      204: () => {
        status.value = 'success';

        if (user.value !== null && 'name' in user.value) {
          user.value.name = tidiedName;
        }

        if (user.value !== null && 'about' in user.value) {
          user.value.about = tidiedAbout;
        }
      },
      400: (response?: Response) => {
        status.value = 'error';

        response?.json().then(value => {
          if (value.errors) {
            if ('Name' in value.errors) {
              inputErrors.value = [{id: 'name-user', message: value.errors['Name'][0]}]
            }

            if ('About' in value.errors) {
              inputErrors.value = [{id: 'about-user', message: value.errors['About'][0]}]
            }

            nextTick(() => {
              errorSectionRef.value?.focus();
            });
          }
        });
      },
      500: () => {
        status.value = 'error';

        // TODO handle this better
        inputErrors.value = [{ id: 'name-user', message: 'Unknown Error' }]

        nextTick(() => {
          errorSectionRef.value?.focus();
        });
      }
    };

    const init = { method: "PATCH", body: body };

    apiFetchAuthenticated('account', statusActions, init);
  }

  async function patchAccountPrivacy() {
    inputErrors.value = [];

    if (user.value === null || 'error' in user.value) {
      return;
    }

    status.value = 'pending';

    const statusActions: StatusActions = {
      204: () => {
        status.value = 'success';

        if (user.value !== null && 'name' in user.value) {
          user.value.protected = inputProtected.value === false ? null : true;
        }
      },
      400: (response?: Response) => {
        status.value = 'error';

        response?.json().then(value => {
          if (value.errors) {
            if ('Protected' in value.errors) {
              inputErrors.value = [{id: 'protected-user', message: value.errors['Protected'][0]}]
            }

            nextTick(() => {
              errorSectionRef.value?.focus();
            });
          }
        });
      },
      500: () => {
        status.value = 'error';

        // TODO handle this better
        inputErrors.value = [{ id: 'name-user', message: 'Unknown Error' }]

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

    apiFetchAuthenticated('account', statusActions, init);
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
  <main id="main" tabindex="-1" :aria-busy="user === null">
    <header>
      <h1 ref="headingMainRef" tabindex="-1">{{ getHeading() }}</h1>
    </header>

    <section v-if="user === null">                
      <progress max="100">Fetching account...</progress>
    </section>

    <section v-else-if="'error' in user">
      <p>
        {{ user.body }}
      </p>
    </section>

    <template v-else>
      <section id="errors" v-if="inputErrors.length > 0" ref="errorSectionRef" tabindex="-1" aria-live="assertive" aria-labelledby="heading-errors">
        <h2 id="heading-errors">There was a problem updating your account</h2>
        <ul>
          <li v-for="error in inputErrors" :key="'error' + error.id">
            <a :href="'#input-' + error.id">{{ error.message }}</a>
          </li>
        </ul>
      </section>

      <ul role="tablist" aria-label="Settings Options">
        <TabButton v-for="(tab, index) in tabs"
          :key="tab.id"
          :selected="tabIndex === index"
          @click="tabIndex = index"
          @keydown="handleKeyDown($event, index)"
          :text="tab.text"
          :id="tab.id"
          :add-button-ref="(el: HTMLButtonElement | null) => {if (el) tabRefs.push(el)}" />
      </ul>

      <section id="panel-public" role="tabpanel" aria-labelledby="tab-public" :hidden="tabIndex !== 0">
        <header>
          <h2>Display</h2>
        </header>

        <form @submit.prevent="patchAccountDisplay" action="/api/account" method="POST" novalidate>
          <fieldset>
            <legend class="visuallyhidden">Display Details</legend>

            <InputText
              v-model="inputUserName"
              label="Name"
              name="name"
              id="name-user"
              :maxlength="64"
              :error="getError('name-user')"
              help="Your display name"
              autocomplete="name" />

            <InputTextArea
              v-model="inputUserAbout"
              label="About"
              name="about"
              id="about-user"
              :maxlength="256"
              :error="getError('about-user')"
              help="Briefly describe yourself for your public profile" />

            <label for="icon-user">Icon</label>
            <input
              type="file"
              accept="image/*"
              id="icon-user"
              @change="onChooseFile"
              aria-describedby="help-icon"
              aria-controls="file-preview"
            />
            <small id="help-icon">Files must be JPG or PNG, under 1MB, and have dimensions 400 wide by 400 high</small>

            <output id="file-preview" for="icon-user">
              <figure>
                <img :alt="user.iconImage?.altText ?? 'User icon'" :src="previewUrl ?? user.iconImage?.uri ?? 'https://94e01200-c64f-4ff6-87b6-ce5a316b9ea8.mdnplay.dev/shared-assets/images/examples/grapefruit-slice.jpg'" />
                <figcaption>A preview will appear here</figcaption>
              </figure>
            </output>
          </fieldset>

          <SubmitButton
            :busy="status === 'pending'"
            :disabled="noChangeDisplay()"
            text="Save"
            busy-text="Saving..." />
        </form>
      </section>

      <section id="panel-private" role="tabpanel" aria-labelledby="tab-private" :hidden="tabIndex !== 1">
        <header>
          <h2>Privacy & Security</h2>
        </header>

        <form @submit.prevent="patchAccountPrivacy" action="/api/account" method="POST" novalidate>
          <fieldset>
            <legend class="visuallyhidden">Privacy</legend>

            <input type="checkbox" id="input-protected" name="protected" v-model="inputProtected" aria-describedby="help-protected" />
            <label for="input-protected">Protected</label>
            <small id="help-protected">Your account will only be visible to trusted users</small>
          </fieldset>

          <SubmitButton
            :busy="status === 'pending'"
            :disabled="noChangePrivacy()"
            text="Save"
            busy-text="Saving..." />
        </form>
      </section>

      <section id="panel-channels" role="tabpanel" aria-labelledby="tab-channels" :hidden="tabIndex !== 2" >
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
              id="name-channel"
              :maxlength="64"
              help="The display name of your channel"
              :error="getError('name-channel')"
              :required="true" />

            <InputText
              v-model="inputChannelSlug"
              label="Slug*"
              name="slug"
              id="slug-channel"
              :maxlength="64"
              help="lower case with hyphens (e.g. 'my-anatini-channel')"
              :error="getError('slug-channel')"
              :required="true" />

            <InputTextArea
              v-model="inputChannelAbout"
              label="About"
              name="about"
              id="about-channel"
              :maxlength="256"
              :error="getError('about-channel')"
              help="Briefly describe your channel" />
          </fieldset>

          <SubmitButton
            :busy="status === 'pending'"
            :disabled="tidy(inputChannelName) === '' || tidy(inputChannelSlug) === ''"
            text="Create"
            busy-text="Creating..." />
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

                <p v-if="channel.about">{{ channel.about }}</p>
              </article>
            </li>
          </ul>

          <p v-else>You do not have any channels</p>
        </section>
      </section>
    </template>

    <p role="status" class="visuallyhidden">{{ pageStatus }}</p>
  </main>
</template>
