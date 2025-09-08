<script setup lang="ts">
  import { ref, onMounted, useTemplateRef } from 'vue';
  import { useRouter } from 'vue-router';
  import { reportValidity, validateInputs } from './common/validity';

  type Account = {
    id: string,
    name: string;
    defaultHandleId: string | null;
    emails: {
      emailId: string;
      value: string;
      verified: boolean;
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
    handles: {
      handleId: string;
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
  const isCreatingHandle = ref<boolean>(false);
  const handleInput = useTemplateRef<HTMLInputElement>('handle');

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

  async function createHandle(event: Event) {
    event.preventDefault();

    if (!validateInputs([
      {element: handleInput.value, error: 'Please enter a handle.'},
    ]))
      return;

    isCreatingHandle.value = true;

    const body: Record<string, string> = {
      handle: handleInput.value!.value.trim(),
    };

    fetch("api/users/handles", {
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
        handleInput.value!.setCustomValidity("Handle limit reached!");
        reportValidity([handleInput.value]);
      } else if (response.status === 409) {
        handleInput.value!.setCustomValidity("Handle already in use!");
        reportValidity([handleInput.value]);
      } else {
        console.log("Unknown Error");
      }
    }).finally(() => {
      isCreatingHandle.value = false;
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
    <h3>Create Handle</h3>
    <form id="createHandle" @submit="createHandle" action="???" method="post">
      <p>
        <label for="handle">Handle</label>
        <input id="handle" type="text" name="handle" maxlength="64" ref="handle" @input="event => handleInput?.setCustomValidity('')">
      </p>

      <p>
        <input type="submit" value="Submit" :disabled="isCreatingHandle">
      </p>
    </form>
    <template v-if="account.handles?.length">
      <h3>Handles</h3>
      <ul>
        <li v-for="(handle, index) in account.handles" :key="'handle' + index">
          {{ handle.value }}: {{ account.defaultHandleId === handle.handleId ? 'Default' : 'Not Default'}}
        </li>
      </ul>
    </template>
  </template>
</template>
