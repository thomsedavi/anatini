<script setup lang="ts">
  import { ref, onMounted, useTemplateRef } from 'vue';
  import { useRouter } from 'vue-router';
  import { reportValidity, validateInputs } from './common/validity';
  import type { UserEdit, Events } from '@/types';
  import { apiFetch } from './common/apiFetch';

  const router = useRouter();
  const account = ref<UserEdit | null>(null);
  const events = ref<Events | null>(null);
  const error = ref<string | null>(null);
  const isFetching = ref<boolean>(false);
  const isGettingEvents = ref<boolean>(false);
  const isCreatingUserSlug = ref<boolean>(false);
  const userSlugInput = useTemplateRef<HTMLInputElement>('user-slug');
  const isCreatingChannel = ref<boolean>(false);
  const channelSlugInput = useTemplateRef<HTMLInputElement>('channel-slug');
  const channelNameInput = useTemplateRef<HTMLInputElement>('channel-name');

  const chosenFile = ref<File | null>(null);
  const previewUrl = ref<string | null>(null);
  const uploadStatus = ref('No file selected');

  onMounted(() => {
    isFetching.value = true;

    fetch("/api/authentication/account", {
      method: "GET",
    }).then((response: Response) => {
      if (response.ok) {
        response.json()
          .then((value: UserEdit) => {
            account.value = value;
          })
          .catch(() => {
            console.log('Unknown Error');
          });
      } else {
        router.replace({ path: '/login', query: { redirect: '/account' } });
      }
    }).finally(() => {
      isFetching.value = false;
    });
  });

  async function createChannel(event: Event) {
    event.preventDefault();

    if (!validateInputs([
      { element: channelNameInput.value, error: 'Please enter a channel name.' },
      { element: channelSlugInput.value, error: 'Please enter a channel slug.' },
    ]))
      return;

    isCreatingChannel.value = true;

    const body = new FormData();

    body.append('name', channelNameInput.value!.value.trim());
    body.append('slug', channelSlugInput.value!.value.trim());

    fetch("/api/channels", {
      method: "POST",
      body: body,
    }).then((response: Response) => {
      if (response.ok) {
        response.json()
          .then((value: UserEdit) => {
            account.value = value;
          })
          .catch(() => {
            console.log('Unknown Error');
          });
      } else if (response.status === 409) {
        channelSlugInput.value!.setCustomValidity("Slug already in use!");
        reportValidity([channelSlugInput.value]);
      } else {
        console.log("Unknown Error");
      }
    }).finally(() => {
      isCreatingChannel.value = false;
    });
  }

  async function createSlug(event: Event) {
    event.preventDefault();

    if (!validateInputs([
      {element: userSlugInput.value, error: 'Please enter a slug.'},
    ]))
      return;

    isCreatingUserSlug.value = true;

    const body = new FormData();

    body.append('slug', userSlugInput.value!.value.trim())

    fetch("/api/users/slugs", {
      method: "POST",
      body: body,
    }).then((response: Response) => {
      if (response.ok) {
        response.json()
          .then((value: UserEdit) => {
            account.value = value;
          })
          .catch(() => {
            console.log('Unknown Error');
          });
      } else if (response.status === 403) {
        userSlugInput.value!.setCustomValidity("Slug limit reached!");
        reportValidity([userSlugInput.value]);
      } else if (response.status === 409) {
        userSlugInput.value!.setCustomValidity("Slug already in use!");
        reportValidity([userSlugInput.value]);
      } else {
        console.log("Unknown Error");
      }
    }).finally(() => {
      isCreatingUserSlug.value = false;
    });
  }

  async function getEvents() {
    isGettingEvents.value = true;

    const onfulfilled = (value: Events) => {
      events.value = value;
    };

    const onfinally = () => {
      isGettingEvents.value = false;
    };

    await apiFetch("users/events", onfulfilled, onfinally);
  }

  const onChooseFile = (event: Event) => {
    const input = event?.target as HTMLInputElement

    const file = input?.files?.[0];
  
    if (!file) return;

    if (!file.type.startsWith('image/')) {
      uploadStatus.value = 'Please select an image file';
      return;
    }

    chosenFile.value = file;
    previewUrl.value = URL.createObjectURL(file);
    uploadStatus.value = 'File selected';
  };

const updateProfileIconImage = async (id: string) => {
    try {
      const formData = new FormData();

      formData.append('iconImageId', id);

      const onfulfilled = () => {
        //loading.value = false;
      };

      const onfinally = () => {
        //loading.value = false;
      };

      const init: RequestInit = { method: "PATCH", body: formData };

      apiFetch('account', onfulfilled, onfinally, init);
    } catch (error) {
      console.error(error);
      uploadStatus.value = "Upload failed";
    }
  }

  const uploadImage = async (event: Event) => {
    event.preventDefault();

    if (!chosenFile.value) return;

    uploadStatus.value = 'Uploading';

    const formData = new FormData();

    formData.append('file', chosenFile.value);
    formData.append('type', 'Icon');

    try {
      const onfulfilled = async (value: { id: string, channelId: string }) => {
        updateProfileIconImage(value.id);

        uploadStatus.value = 'Upload successful';

        chosenFile.value = null; 
      };

      const onfinally = () => {
        //loading.value = false;
      };

      const init: RequestInit = { method: "POST", body: formData };

      apiFetch(`account/images`, onfulfilled, onfinally, init);
    } catch (error) {
      console.error(error);
      uploadStatus.value = "Upload failed";
    }
  }
</script>

<template>
  <main>
    <h1>Account</h1>
    <p v-if="isFetching">Loading...</p>
    <p v-if="error">{{ error }}</p>
    <template v-if="account">
      <p>{{ account.name }}</p>

      <form @submit="uploadImage" action="???" method="POST">
        <fieldset>
          <legend>Set Profile Image</legend>

          <label for="file-input">Choose File</label>
          <input
            type="file"
            accept="image/*"
            id="file-input"
            @change="onChooseFile"
            aria-describedby="file-help"
            required
            aria-controls="file-preview upload-status"
          />
          <small id="file-help">Files must be JPG or PNG, under 1MB, and have dimensions 400 wide by 400 high</small>
          <hr>

          <output id="file-preview" for="file-input">
            <figure>
              <img :src="previewUrl ?? 'https://94e01200-c64f-4ff6-87b6-ce5a316b9ea8.mdnplay.dev/shared-assets/images/examples/grapefruit-slice.jpg'" />
              <figcaption>A preview will appear here</figcaption>
            </figure>
          </output>
          <hr>

          <button type="submit" :disabled="!chosenFile">Upload</button>
          <output id="upload-status" for="file-input" aria-live="polite">{{ uploadStatus}}</output>

          <footer>
            <small>This will be your profile image</small>
          </footer>
        </fieldset>
      </form>

      <h3>Emails</h3>
      <ul>
        <li v-for="(email, index) in account.emails" :key="'email' + index">
          {{ email.address }}: {{ email.verified ? "Verified" : "Not Verified" }}
        </li>
      </ul>
      <h3>Sessions</h3>
      <ul>
        <li v-for="(refreshToken, index) in account.sessions" :key="'refreshToken' + index">
          {{ refreshToken.ipAddress }}: {{ refreshToken.userAgent }}
        </li>
      </ul>
      <button type="button" @click="getEvents" :disabled="isGettingEvents">Get Events</button>
      <template v-if="events">
        <h3>Events</h3>
        <ul>
          <li v-for="(event, index) in events.events" :key="'event' + index">
            {{ event.type }}: {{ event.dateTimeUtc }}
          </li>
        </ul>
      </template>
      <h3>Create Slug</h3>
      <form @submit="createSlug" action="/api/users/slugs" method="POST">
        <fieldset>
          <legend>Create Alias</legend>

          <label for="slug">Slug</label>
          <input id="slug" type="text" name="slug" maxlength="64" ref="user-slug" @input="event => userSlugInput?.setCustomValidity('')">
          <hr>

          <button type="submit" :disabled="isCreatingUserSlug">Submit</button>
        </fieldset>
      </form>
      <h3>Slugs</h3>
      <ul>
        <li v-for="(alias, index) in account.aliases" :key="'slug' + index">
          {{ alias.slug }}: {{ account.defaultSlug === alias.slug ? 'Default' : 'Not Default'}}
        </li>
      </ul>
      <h3>Create Channel</h3>
      <form @submit="createChannel" action="/api/channels" method="POST">
        <fieldset>
          <legend>Create Channel</legend>

          <label for="channelName">Channel Name</label>
          <input id="channelName" type="text" name="name" maxlength="64" ref="channel-name" @input="event => channelNameInput?.setCustomValidity('')">
          <hr>

          <label for="channelSlug">Channel Slug</label>
          <input id="channelSlug" type="text" name="slug" maxlength="64" ref="channel-slug" @input="event => channelSlugInput?.setCustomValidity('')">
          <hr>

          <button type="submit" :disabled="isCreatingChannel">Submit</button>
        </fieldset>
      </form>
      <template v-if="account.channels?.length">
        <h3>Channels</h3>
        <div v-for="(channel, index) in account.channels" :key="'channel' + index">
          <RouterLink :to="{ name: 'ChannelEdit', params: { channelId: channel.defaultSlug }}">{{ channel.name }}</RouterLink>
        </div>
      </template>
    </template>
  </main>
</template>
