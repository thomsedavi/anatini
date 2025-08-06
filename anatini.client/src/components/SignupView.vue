<script setup lang="ts">
  import { useTemplateRef, onMounted } from 'vue';
  import { useRouter } from 'vue-router'
  import { store } from '../store.ts'

  type ResponseErrors = {
    email?: string[];
    inviteCode?: string[];
    name?: string[];
    password?: string[];
  }

  type OkResponseJson = {
    bearer: string;
  }

  type BadRequestResponseJson = {
    errors: ResponseErrors;
  };

  const router = useRouter();

  const nameInput = useTemplateRef<HTMLInputElement>('name')
  const emailInput = useTemplateRef<HTMLInputElement>('email')
  const passwordInput = useTemplateRef<HTMLInputElement>('password')
  const inviteCodeInput = useTemplateRef<HTMLInputElement>('invite-code')

  onMounted(() => {
    nameInput.value?.focus()
  })

  async function signup(e: Event) {
    e.preventDefault();

    nameInput.value?.setCustomValidity('');
    emailInput.value?.setCustomValidity('');
    passwordInput.value?.setCustomValidity('');
    inviteCodeInput.value?.setCustomValidity('');

    const body = {
      name: nameInput.value?.value,
      email: emailInput.value?.value,
      password: passwordInput.value?.value,
      inviteCode: inviteCodeInput.value?.value
    };

    fetch("api/authentication/signup", {
      method: "POST",
      headers: {
        "Content-Type": "application/x-www-form-urlencoded",
      },
      body: new URLSearchParams(body),
    }).then((response: Response) => {
      if (response.ok) {
        response.json()
          .then((json: OkResponseJson) => {
            store.logIn(json.bearer);

            router.replace({ path: '/' });
          })
          .catch(() => {
            console.log('Unknown Error');
          });
      } else if (response.status === 400) {
        response.json()
          .then((json: BadRequestResponseJson) => {
            if (json.errors) {
              // TODO for some reason the controller is returning errors in PascalCase instead of camelCase? :(
              // I've spent hours trying to figure out why, but nothing works
              // repair JSON here for now, revisit later
              // See: AuthenticationController.cs

              console.log('raw errors', json.errors);

              json.errors.Name && (json.errors.name = json.errors.Name) && (delete json.errors.Name);
              json.errors.Email && (json.errors.email = json.errors.Email) && (delete json.errors.Email);
              json.errors.Password && (json.errors.password = json.errors.Password) && (delete json.errors.Password);
              json.errors.InviteCode && (json.errors.inviteCode = json.errors.InviteCode) && (delete json.errors.InviteCode);

              console.log('fixed errors', json.errors);

              json.errors.name && nameInput.value?.setCustomValidity(json.errors.name.join(';'));
              json.errors.email && emailInput.value?.setCustomValidity(json.errors.email.join(';'));
              json.errors.password && passwordInput.value?.setCustomValidity(json.errors.password.join(';'));
              json.errors.inviteCode && inviteCodeInput.value?.setCustomValidity(json.errors.inviteCode.join(';'));
            } else {
              console.log("Unknown Error");
            }
          }
        );
      } else {
        console.log("Unknown Error");
      }
    });
  }
</script>

<template>
  <h2>SignupView</h2>
  <form id="signup" @submit="signup" action="???" method="post">
    <p>
      <label for="name">Name</label>
      <input id="name" type="text" name="name" ref="name" @input="event => nameInput?.setCustomValidity('')">
    </p>

    <p>
      <label for="email">Email</label>
      <input id="email" type="email" name="email" ref="email" @input="event => emailInput?.setCustomValidity('')">
    </p>

    <p>
      <label for="password">Password</label>
      <input id="password" type="password" name="password" ref="password" @input="event => passwordInput?.setCustomValidity('')">
    </p>

    <p>
      <label for="inviteCode">Invite Code</label>
      <input id="inviteCode" type="text" name="inviteCode" ref="invite-code" @input="event => inviteCodeInput?.setCustomValidity('')">
    </p>

    <p>
      <input type="submit" value="Submit">
    </p>
  </form>
</template>
