<script setup lang="ts">
  import { ref, onMounted, useTemplateRef } from 'vue';
  import { useRouter } from 'vue-router';
  import { reportValidity, validateInputs } from './common/validity';

  type Account = {
    id: string,
    name: string;
    defaultSlugId: string | null;
    emails: {
      emailId: string;
      value: string;
      verified: boolean;
    }[];
    channels: {
      channelId: string;
      name: string;
    }[];
    sessions: {
      userAgent: string;
      revoked: boolean;
      createdDateUtc: string;
      updatedDateUtc: string;
      ipAddress: string;
    }[];
    invites: {
      inviteId: string;
      value: string;
      dateNZ: string;
      used: boolean;
    }[];
    slugs: {
      slugId: string;
      value: string;
    }[];
  };

  type Events = {
    events: {
      type: string;
      dateUtc: string;
    }[];
  };

  const router = useRouter();
  const account = ref<Account | null>(null);
  const events = ref<Events | null>(null);
  const error = ref<string | null>(null);
  const isFetching = ref<boolean>(false);
  const isCreatingInviteCode = ref<boolean>(false);
  const isGettingEvents = ref<boolean>(false);
  const isCreatingSlug = ref<boolean>(false);
  const slugInput = useTemplateRef<HTMLInputElement>('slug');

  onMounted(() => {
    isFetching.value = true;

    fetch("api/authentication/account", {
      method: "GET",
    }).then((response: Response) => {
      if (response.ok) {
        response.json()
          .then((value: Account) => {
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

    fetch("api/authentication/inviteCode", {
      method: "POST",
    }).then((response: Response) => {
      if (response.ok) {
        response.json()
          .then((value: Account) => {
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

  async function createSlug(event: Event) {
    event.preventDefault();

    if (!validateInputs([
      {element: slugInput.value, error: 'Please enter a slug.'},
    ]))
      return;

    isCreatingSlug.value = true;

    const body: Record<string, string> = {
      slug: slugInput.value!.value.trim(),
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
          .then((value: Account) => {
            account.value = value;
          })
          .catch(() => {
            console.log('Unknown Error');
          });
      } else if (response.status === 403) {
        slugInput.value!.setCustomValidity("Slug limit reached!");
        reportValidity([slugInput.value]);
      } else if (response.status === 409) {
        slugInput.value!.setCustomValidity("Slug already in use!");
        reportValidity([slugInput.value]);
      } else {
        console.log("Unknown Error");
      }
    }).finally(() => {
      isCreatingSlug.value = false;
    });
  }

  async function getEvents() {
    isGettingEvents.value = true;

    fetch("api/users/events", {
      method: "GET"
    }).then((response: Response) => {
      if (response.ok) {
        response.json()
          .then((value: Events) => {
            events.value = value;
          })
          .catch(() => {
            console.log('Unknown Error');
          });
      } else {
        console.log("Unknown Error");
      }
    }).finally(() => {
      isGettingEvents.value = false;
    });
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
        {{ email.value }}: {{ email.verified ? "Verified" : "Not Verified" }}
      </li>
    </ul>
    <h3>Sessions</h3>
    <ul>
      <li v-for="(refreshToken, index) in account.sessions" :key="'refreshToken' + index">
        {{ refreshToken.ipAddress }}: {{ refreshToken.userAgent }}
      </li>
    </ul>
    <button @click="createInviteCode" :disabled="isCreatingInviteCode">Create Invite Code</button>
    <template v-if="account.invites.length">
      <h3>Invites</h3>
      <ul>
        <li v-for="(invite, index) in account.invites" :key="'invite' + index">
          {{ invite.value }}: {{ invite.used ? "Used" : "Not Used" }}: {{  invite.dateNZ }}
        </li>
      </ul>
    </template>
    <button @click="getEvents" :disabled="isGettingEvents">Get Events</button>
    <template v-if="events">
      <h3>Events</h3>
      <ul>
        <li v-for="(event, index) in events.events" :key="'event' + index">
          {{ event.type }}: {{ event.dateUtc }}
        </li>
      </ul>
    </template>
    <h3>Create Slug</h3>
    <form id="createSlug" @submit="createSlug" action="???" method="post">
      <p>
        <label for="slug">Slug</label>
        <input id="slug" type="text" name="slug" maxlength="64" ref="slug" @input="event => slugInput?.setCustomValidity('')">
      </p>

      <p>
        <input type="submit" value="Submit" :disabled="isCreatingSlug">
      </p>
    </form>
    <template v-if="account.slugs?.length">
      <h3>Slugs</h3>
      <ul>
        <li v-for="(slug, index) in account.slugs" :key="'slug' + index">
          {{ slug.value }}: {{ account.defaultSlugId === slug.slugId ? 'Default' : 'Not Default'}}
        </li>
      </ul>
    </template>
  </template>
</template>
