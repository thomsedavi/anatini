<script setup lang="ts">
  import type { InputError, Status, StatusActions } from '@/types';
  import { apiFetchAuthenticated } from '../common/apiFetch';
  import SubmitButton from '../common/SubmitButton.vue';
  import { ref } from 'vue';

  const props = defineProps<{
    visibility: 'Private' | 'Protected' | 'Public';
    status: Status,
    inputErrors: InputError[],
  }>();

  const emit = defineEmits<{
    'update-protected': [newProtected: string],
    'update-status': [newStatus: Status],
    'update-errors': [newInputErrors: InputError[]],
  }>();

  const visibilityOptions = ref([
    { text: 'Public', value: 'Public' },
    { text: 'Protected', value: 'Protected' },
    { text: 'Private', value: 'Private' }
  ]);

  const inputVisibility = ref<string>(props.visibility);

  function noChangePrivacy(): boolean {
    return props.visibility === inputVisibility.value;
  }

  async function patchAccountPrivacy() {
    emit('update-errors', []);
    emit('update-status', 'pending');

    const statusActions: StatusActions = {
      204: () => {
        emit('update-status', 'success');
        emit('update-protected', inputVisibility.value);
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

    body.append('visibility', inputVisibility.value);

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

        <label for="input-visibility">Privacy Level</label>
        <select name="visibility" id="input-visibility" v-model="inputVisibility" aria-describedby="help-visibility">
          <option v-for="option in visibilityOptions" :value="option.value" :key="'visibility' + option.value">
            {{ option.text }}
          </option>
        </select>
        <small id="help-visibility">Publicly visible, protected to only be visible to trusted users, or private to only be visible to privately trusted users, I need to reword this to explain it better</small>
      </fieldset>

      <SubmitButton
        :busy="status === 'pending'"
        :disabled="noChangePrivacy()"
        text="Save"
        busy-text="Saving..." />
    </form>
  </section>
</template>
