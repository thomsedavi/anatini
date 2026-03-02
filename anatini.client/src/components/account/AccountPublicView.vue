<script setup lang="ts">
  import type { Image, InputError, Status, StatusActions } from '@/types';
  import { tidy } from '../common/utils';
  import { ref } from 'vue';
  import InputText from '../common/InputText.vue';
  import SubmitButton from '../common/SubmitButton.vue';
  import InputTextArea from '../common/InputTextArea.vue';
  import { apiFetchAuthenticated } from '../common/apiFetch';

  const props = defineProps<{
    name: string,
    about: string | null,
    iconImage: Image | null,
    status: Status,
    inputErrors: InputError[],
  }>();

  const emit = defineEmits<{
    'update-name': [newName: string],
    'update-about': [newAbout: string],
    'update-status': [newStatus: Status],
    'update-errors': [newInputErrors: InputError[]],
  }>();

  const inputUserName = ref<string>(props.name);
  const inputUserAbout = ref<string>(props.about ?? '');
  const fileUserIcon = ref<File | null>(null);
  const previewUrl = ref<string | null>(null);
  const uploadStatus = ref<string>('No file selected');

  async function patch(body: FormData, tidiedName: string, tidiedAbout: string) {
    const statusActions: StatusActions = {
      204: () => {
        emit('update-status', 'success');
        emit('update-name', tidiedName);
        emit('update-about', tidiedAbout);
      },
      400: (response?: Response) => {
        emit('update-status', 'error');

        response?.json().then(value => {
          if (value.errors) {
            const inputErrors: InputError[] = [];
            
            if ('Name' in value.errors) {
              inputErrors.push({id: 'name-user', message: value.errors['Name'][0]});
            }

            if ('About' in value.errors) {
              inputErrors.push({id: 'about-user', message: value.errors['About'][0]});
            }

            emit('update-errors', inputErrors);
          }
        });
      },
      500: () => {
        emit('update-status', 'error');
        emit('update-errors', [{ id: 'name-user', message: 'Unknown Error' }]);
      }
    };

    const init = { method: "PATCH", body: body };

    apiFetchAuthenticated('account', statusActions, init);
  }

  async function patchAccountDisplay() {
    emit('update-errors', []);

    const tidiedName = tidy(inputUserName.value);
    const tidiedAbout = tidy(inputUserAbout.value);

    if (tidiedName === '') {
      emit('update-errors', [{ id: 'name-user', message: 'Name is required' }]);

      return;
    }

    emit('update-status', 'pending');

    const body = new FormData();

    if (props.name !== tidiedName) {
      body.append('name', tidiedName);
    }

    if (props.about !== tidiedAbout) {
      body.append('about', tidiedAbout);
    }

    if (fileUserIcon.value !== null) {
      const bodyIcon = new FormData();

      bodyIcon.append('file', fileUserIcon.value);
      bodyIcon.append('type', 'Icon');

      const statusActionsIcon: StatusActions = {
        201: (response?: Response) => {
          response?.json()
            .then((value: { id: string }) => {
              body.append('iconImageId', value.id);

              patch(body, tidiedName, tidiedAbout);

              fileUserIcon.value = null; 
            });
        },
        500: () => {
          emit('update-status', 'error');
          emit('update-errors', [{ id: 'icon-user', message: 'Unknown Error' }]);
        }
      }

      const initIcon = { method: "POST", body: bodyIcon };

      apiFetchAuthenticated('account/images', statusActionsIcon, initIcon);
    } else {
      patch(body, tidiedName, tidiedAbout);
    }
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

  function noChangeDisplay(): boolean {
    return props.name === tidy(inputUserName.value) && (props.about ?? '') === tidy(inputUserAbout.value) && fileUserIcon.value === null;
  }

  function getError(id: string): string | undefined {
    return props.inputErrors.find(inputError => inputError.id === id)?.message;
  }
</script>

<template>
  <section id="panel-public" role="tabpanel" aria-labelledby="tab-public">
    <header>
      <h2>Display</h2>
    </header>

    <form @submit.prevent="patchAccountDisplay" action="/api/account" method="POST" novalidate>
      <fieldset>
        <legend class="visuallyhidden">Display Details</legend>

        <InputText
          v-model="inputUserName"
          label="Name"
          name="name"
          id="name-user"
          :maxlength="64"
          :error="getError('name-user')"
          help="Your display name"
          autocomplete="name" />

        <InputTextArea
          v-model="inputUserAbout"
          label="About"
          name="about"
          id="about-user"
          :maxlength="256"
          :error="getError('about-user')"
          help="Briefly describe yourself for your public profile" />

        <label for="icon-user">Icon</label>
        <input
          type="file"
          accept="image/*"
          id="icon-user"
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
        :disabled="noChangeDisplay()"
        text="Save"
        busy-text="Saving..." />
    </form>
  </section>
</template>
