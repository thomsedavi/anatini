<script setup lang="ts">
  import { ref } from 'vue';
  import { tidy } from '../common/utils';
  import type { InputError, Status, StatusActions } from '@/types';
  import { apiFetchAuthenticated } from '../common/apiFetch';
  import InputText from '../common/InputText.vue';
  import SubmitButton from '../common/SubmitButton.vue';

  const props = defineProps<{
    channelId: string,
    name: string,
    status: Status,
    inputErrors: InputError[],
  }>();

  const emit = defineEmits<{
    'update-status': [newStatus: Status],
    'update-name': [newName: string],
    'update-errors': [newInputErrors: InputError[]],
  }>();

  const inputName = ref<string>(props.name ?? '');

  function noChange(): boolean {
    return props.name == tidy(inputName.value);
  }

  function getError(id: string): string | undefined {
    return props.inputErrors.find(inputError => inputError.id === id)?.message;
  }

  async function patchChannel() {
    emit('update-errors', [])

    const tidiedName = tidy(inputName.value);

    if (tidiedName === '') {
      emit('update-errors', [{ id: 'name', message: 'Name is required' }]);

      return;
    }

    emit('update-status', 'pending');

    const statusActions: StatusActions = {
      204: () => {
        emit('update-status', 'success');
        emit('update-name', tidiedName);
      },
      400: (response?: Response) => {
        emit('update-status', 'error');

        response?.json().then((value) => {
          if (value.errors) {
            if ('Name' in value.errors) {
              emit('update-errors', [{ id: 'name', message: value.errors['Name'][0] }]);
            }
          }
        });
      },
      500: () => {
        emit('update-status', 'error');
        emit('update-errors', [{ id: 'name', message: 'Unknown Error' }]);
      }
    }

    const body = new FormData();

    if (props.name !== tidiedName) {
      body.append('name', tidiedName);
    }

    const init = { method: "PATCH", body: body };

    apiFetchAuthenticated(`channels/${props.channelId}`, statusActions, init);
  }
</script>

<template>
  <section id="panel-public" role="tabpanel" aria-labelledby="tab-public">
    <header>
      <h2>Display</h2>
    </header>

    <form @submit.prevent="patchChannel" :action="`/api/channels/${channelId}`" method="POST" novalidate>
      <InputText
        v-model="inputName"
        label="Name"
        name="name"
        id="name"
        :maxlength="64"
        :error="getError('name')"
        help="Channel display name"
        autocomplete="name" />

      <SubmitButton
        :busy="status === 'pending'"
        :disabled="noChange()"
        text="Save"
        busy-text="Saving..." />
    </form>
  </section>
</template>
