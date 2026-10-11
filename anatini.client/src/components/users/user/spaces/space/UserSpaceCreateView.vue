<script setup lang="ts">
  import { ref } from 'vue';
  import { useRouter } from 'vue-router';
  import { apiFetchAuthenticated } from '@/common/apiFetch';
  import type { InputError, Space, Status, StatusActions, Visibility } from '@/common/types';
  import { tidy } from '@/common/utils';
  import InputText from '@/common/InputText.vue';
  import SubmitButton from '@/common/SubmitButton.vue';
  import VisibilitySelect from '@/common/VisibilitySelect.vue';

  const router = useRouter();

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
  const inputVisibility = ref<Visibility>('Public');
  const inputHandle = ref<string>('');

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
              router.push({ name: 'Space', params: { spaceId: value.handle } });
            });
      },
      400: () => {
        emit('update-status', 'error');
      }
    }

    const body = new FormData();

    body.append('name', tidiedName);
    body.append('visibility', inputVisibility.value);

    if (tidy(inputHandle.value) !== '') {
      body.append('handle', tidy(inputHandle.value));
    }

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

      <VisibilitySelect v-model="inputVisibility" />

      <InputText
        v-model="inputHandle"
        label="Handle"
        name="handle"
        id="handle"
        :maxlength="64"
        help="lower case with hyphens (e.g. 'my-anatini-space'), optional custom web address"
        :error="getError('handle')" />

      <SubmitButton
        :busy="dataStatus === 'pending'"
        text="Create"
        busy-text="Creating..." />
    </form>
  </section>
</template>
