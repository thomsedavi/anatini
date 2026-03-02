<script setup lang="ts">
  import type { InputError, Status, StatusActions } from '@/types';
  import { apiFetchAuthenticated } from '../common/apiFetch';
  import { ref } from 'vue';
  import SubmitButton from '../common/SubmitButton.vue';

  const props = defineProps<{
    protected: boolean | null;
    status: Status,
    inputErrors: InputError[],
  }>();

  const emit = defineEmits<{
    'update-protected': [newProtected: boolean | null],
    'update-status': [newStatus: Status],
    'update-errors': [newInputErrors: InputError[]],
  }>();

  const inputProtected = ref<boolean>(props.protected ?? false);

  function noChangePrivacy(): boolean {
    return (props.protected ?? false) === inputProtected.value;
  }

  async function patchAccountPrivacy() {
    emit('update-errors', []);
    emit('update-status', 'pending');

    const statusActions: StatusActions = {
      204: () => {
        emit('update-status', 'success');
        emit('update-protected', inputProtected.value === false ? null : true);
      },
      400: (response?: Response) => {
        emit('update-status', 'error');

        response?.json().then(value => {
          if (value.errors) {
            if ('Protected' in value.errors) {
              emit('update-errors', [{ id: 'protected-user', message: value.errors['Protected'][0] }]);
            }
          }
        });
      },
      500: () => {
        emit('update-status', 'error');
        emit('update-errors', [{ id: 'name-user', message: 'Unknown Error' }]);
      }
    };

    const body = new FormData();

    if ((props.protected ?? false) !== inputProtected.value) {
      body.append('protected', inputProtected.value ? 'true' : 'false');
    }

    const init = { method: "PATCH", body: body };

    apiFetchAuthenticated('account', statusActions, init);
  }
</script>

<template>
  <section id="panel-private" role="tabpanel" aria-labelledby="tab-private">
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
</template>
