<script setup lang="ts">
  import { ref, onMounted } from 'vue';
  import { useRouter } from 'vue-router';

  type User = {
    name: string;
    emails: {
      email: string;
      verified: boolean;
    }[];
    invites?: {
      inviteCode: string;
      createdDateNZ: string;
      used: boolean;
    }[];
  }

  const router = useRouter();
  const user = ref<User | null>(null);
  const error = ref<string | null>(null);
  const isFetching = ref<boolean>(false);
  const isCreatingInviteCode = ref<boolean>(false);

  onMounted(() => {
    isFetching.value = true;

    fetch("api/users/account", {
      method: "GET",
    }).then((response: Response) => {
      if (response.ok) {
        response.json()
          .then((value: User) => {
            user.value = value;
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
          .then((value: User) => {
            user.value = value;
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
</script>

<template>
  <h2>AccountView</h2>
  <p v-if="isFetching">Loading...</p>
  <p v-if="error">{{ error }}</p>
  <template v-if="user">
    <h3>Name</h3>
    <p>{{ user.name }}</p>
    <h3>Emails</h3>
    <ul>
      <li v-for="(email, index) in user.emails" :key="'email' + index">
        {{ email.email }}: {{ email.verified ? "Verified" : "Not Verified" }}
      </li>
    </ul>
    <button @click="createInviteCode" :disabled="isCreatingInviteCode">Create Invite Code</button>
    <template v-if="user.invites">
      <h3>Invites</h3>
    <ul>
      <li v-for="(invite, index) in user.invites" :key="'invite' + index">
        {{ invite.inviteCode }}: {{ invite.used ? "Used" : "Not Used" }}: {{  invite.createdDateNZ }}
      </li>
    </ul>
    </template>
  </template>
</template>
