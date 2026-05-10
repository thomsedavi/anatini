<script setup lang="ts">
  import { ref } from 'vue';
  import { tidy } from '../common/utils';
  import type { Image, InputError, Status, StatusActions } from '@/types';
  import { apiFetchAuthenticated } from '../common/apiFetch';
  import InputText from '../common/InputText.vue';
  import SubmitButton from '../common/SubmitButton.vue';

  const props = defineProps<{
    channelId: string,
    name: string,
    iconImage: Image | null,
    status: Status,
    inputErrors: InputError[],
  }>();

  const emit = defineEmits<{
    'update-status': [newStatus: Status],
    'update-name': [newName: string],
    'update-errors': [newInputErrors: InputError[]],
  }>();

  const inputName = ref<string>(props.name ?? '');
  const fileUserIcon = ref<File | null>(null);
  const previewUrl = ref<string | null>(null);
  const uploadStatus = ref<string>('No file selected');

  function noChange(): boolean {
    return props.name == tidy(inputName.value);
  }

  function getError(id: string): string | undefined {
    return props.inputErrors.find(inputError => inputError.id === id)?.message;
  }

  const onChooseFile = (event: Event) => {
    const input = event?.target as HTMLInputElement

    const file = input?.files?.[0];
  
    if (!file) {
      return
    }

    if (!file.type.startsWith('image/')) {
      uploadStatus.value = 'Please select an image file';
      return;
    }

    fileUserIcon.value = file;
    previewUrl.value = URL.createObjectURL(file);
    uploadStatus.value = 'File selected';
  };

  async function patchChannel() {
    emit('update-errors', []);

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
      <fieldset>
      <InputText
          v-model="inputName"
          label="Name"
          name="name"
          id="name"
          :maxlength="64"
          :error="getError('name')"
          help="Channel display name"
          autocomplete="name" />

        <label for="icon-user">Icon</label>
        <input
          type="file"
          accept="image/*"
          id="icon-channel"
          @change="onChooseFile"
          aria-describedby="help-icon"
          aria-controls="file-preview"
        />
        <small id="help-icon">Files must be JPG or PNG, under 1MB, and have dimensions 400 wide by 400 high</small>

        <output id="file-preview" for="icon-user">
          <figure>
            <img :alt="iconImage?.altText ?? 'User icon'" :src="previewUrl ?? iconImage?.uri ?? 'https://94e01200-c64f-4ff6-87b6-ce5a316b9ea8.mdnplay.dev/shared-assets/images/examples/grapefruit-slice.jpg'" />
            <figcaption>A preview will appear here</figcaption>
          </figure>
        </output>
      </fieldset>

      <SubmitButton
        :busy="status === 'pending'"
        :disabled="noChange()"
        text="Save"
        busy-text="Saving..." />
    </form>
  </section>
</template>
