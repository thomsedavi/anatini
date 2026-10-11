<script setup lang="ts">
  import { ref } from 'vue';
  import { apiFetchAuthenticated } from '@/common/apiFetch';
  import type { InputError, Space, Status, StatusActions } from '@/common/types';
  import { tidy } from '@/common/utils';
  import InputText from '@/common/InputText.vue';
  import SubmitButton from '@/common/SubmitButton.vue';

  const props = defineProps<{
    dataUserId: string,
    dataStatus: Status,
    dataInputErrors: InputError[],
  }>();

  const emit = defineEmits<{
    'update-status': [newStatus: Status],
    'update-errors': [newInputErrors: InputError[]],
  }>();

  const inputName = ref<string>('');

  function getError(id: string): string | undefined {
    return props.dataInputErrors.find(inputError => inputError.id === id)?.message;
  }

  async function postSpace() {
    emit('update-errors', []);

    const tidiedName = tidy(inputName.value);

    const inputErrors: InputError[] = [];

    if (tidiedName === '') {
      inputErrors.push({ id: 'name', message: 'Name is required' });
    }

    if (inputErrors.length > 0) {
      emit('update-errors', inputErrors);

      return;
    }

    emit('update-status', 'pending');

    const input = `users/${props.dataUserId}/spaces`;

    const statusActions: StatusActions = {
      201: (response?: Response) => {
          response?.json()
            .then((value: Space) => {
              console.log('value', value);
            });
      },
      400: () => {
        emit('update-status', 'error');
      }
    }

    const body = new FormData();

    body.append('name', tidiedName);

    const init = { method: "POST", body: body };

    apiFetchAuthenticated({ input, statusActions, init });
  }
</script>

<template>
  <section id="panel-spaces" role="tabpanel" aria-labelledby="tab-spaces">
    <header>
      <h2>Create Space</h2>
    </header>

    <form @submit.prevent="postSpace" :action="`/api/users/${dataUserId}/spaces`" method="POST" novalidate>
      <InputText
        v-model="inputName"
        label="Name"
        name="name"
        id="name"
        :maxlength="256"
        :required="true"
        help="The name of your space"
        :error="getError('name')" />

      <SubmitButton
        :busy="dataStatus === 'pending'"
        text="Create"
        busy-text="Creating..." />
    </form>
  </section>
</template>
