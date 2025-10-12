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
  const isCreatingInviteCode = ref<boolean>(false);
  const isGettingEvents = ref<boolean>(false);
  const isCreatingUserSlug = ref<boolean>(false);
  const userSlugInput = useTemplateRef<HTMLInputElement>('user-slug');
  const isCreatingChannel = ref<boolean>(false);
  const channelSlugInput = useTemplateRef<HTMLInputElement>('channel-slug');
  const channelNameInput = useTemplateRef<HTMLInputElement>('channel-name');

  onMounted(() => {
    isFetching.value = true;

    fetch("api/authentication/account", {
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

  async function createInviteCode() {
    isCreatingInviteCode.value = true;

    fetch("api/authentication/invite", {
      method: "POST",
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
        alert("Already made one today!");
      } else {
        console.log("Unknown Error");
      }
    }).finally(() => {
      isCreatingInviteCode.value = false;
    });
  }

  async function createChannel(event: Event) {
    event.preventDefault();

    if (!validateInputs([
      { element: channelNameInput.value, error: 'Please enter a channel name.' },
      { element: channelSlugInput.value, error: 'Please enter a channel slug.' },
    ]))
      return;

    isCreatingChannel.value = true;

    const body: Record<string, string> = {
      name: channelNameInput.value!.value.trim(),
      slug: channelSlugInput.value!.value.trim(),
    };

    fetch("api/channels", {
      method: "POST",
      headers: {
        "Content-Type": "application/x-www-form-urlencoded",
      },
      body: new URLSearchParams(body),
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

    const body: Record<string, string> = {
      slug: userSlugInput.value!.value.trim(),
    };

    fetch("api/users/slugs", {
      method: "POST",
      headers: {
        "Content-Type": "application/x-www-form-urlencoded",
      },
      body: new URLSearchParams(body),
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
</script>

<template>
  <h2>AccountView</h2>
  <p v-if="isFetching">Loading...</p>
  <p v-if="error">{{ error }}</p>
  <template v-if="account">
    <h3>Name</h3>
    <p>{{ account.name }}</p>
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
    <button @click="createInviteCode" :disabled="isCreatingInviteCode">Create Invite Code</button>
    <template v-if="account.invites?.length">
      <h3>Invites</h3>
      <ul>
        <li v-for="(invite, index) in account.invites" :key="'invite' + index">
          {{ invite.code }}: {{ invite.used ? "Used" : "Not Used" }}: {{ invite.dateOnlyNZ }}
        </li>
      </ul>
    </template>
    <button @click="getEvents" :disabled="isGettingEvents">Get Events</button>
    <template v-if="events">
      <h3>Events</h3>
      <ul>
        <li v-for="(event, index) in events.events" :key="'event' + index">
          {{ event.type }}: {{ event.dateTimeUtc }}
        </li>
      </ul>
    </template>
    <h3>Create Slug</h3>
    <form id="createSlug" @submit="createSlug" action="???" method="post">
      <p>
        <label for="slug">Slug</label>
        <input id="slug" type="text" name="slug" maxlength="64" ref="user-slug" @input="event => userSlugInput?.setCustomValidity('')">
      </p>

      <p>
        <input type="submit" value="Submit" :disabled="isCreatingUserSlug">
      </p>
    </form>
    <h3>Slugs</h3>
    <ul>
      <li v-for="(alias, index) in account.aliases" :key="'slug' + index">
        {{ alias.slug }}: {{ account.defaultSlug === alias.slug ? 'Default' : 'Not Default'}}
      </li>
    </ul>
    <h3>Create Channel</h3>
    <form id="createChannel" @submit="createChannel" action="???" method="post">
      <p>
        <label for="channelName">Channel Name</label>
        <input id="channelName" type="text" name="channelName" maxlength="64" ref="channel-name" @input="event => channelNameInput?.setCustomValidity('')">
      </p>

      <p>
        <label for="channelSlug">Channel Slug</label>
        <input id="channelSlug" type="text" name="channelSlug" maxlength="64" ref="channel-slug" @input="event => channelSlugInput?.setCustomValidity('')">
      </p>

      <p>
        <input type="submit" value="Submit" :disabled="isCreatingChannel">
      </p>
    </form>
    <template v-if="account.channels?.length">
      <h3>Channels</h3>
      <div v-for="(channel, index) in account.channels" :key="'channel' + index">
        <RouterLink :to="{ name: 'ChannelEdit', params: { channelSlug: channel.defaultSlug }}">{{ channel.name }}</RouterLink>
      </div>
    </template>
  </template>
</template>
