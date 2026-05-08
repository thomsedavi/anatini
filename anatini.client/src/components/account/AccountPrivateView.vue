<script setup lang="ts">
  import type { InputError, Status, StatusActions, Visibility } from '@/types';
  import { apiFetchAuthenticated } from '../common/apiFetch';
  import SubmitButton from '../common/SubmitButton.vue';
  import { ref } from 'vue';
  import VisibilitySelect from '../common/VisibilitySelect.vue';

  const props = defineProps<{
    visibility: Visibility;
    status: Status,
    inputErrors: InputError[],
  }>();

  const emit = defineEmits<{
    'update-protected': [newProtected: string],
    'update-status': [newStatus: Status],
    'update-errors': [newInputErrors: InputError[]],
  }>();

  const inputVisibility = ref<Visibility>(props.visibility);

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

        <VisibilitySelect v-model="inputVisibility" />
      </fieldset>

      <SubmitButton
        :busy="status === 'pending'"
        :disabled="noChangePrivacy()"
        text="Save"
        busy-text="Saving..." />
    </form>
  </section>
</template>
