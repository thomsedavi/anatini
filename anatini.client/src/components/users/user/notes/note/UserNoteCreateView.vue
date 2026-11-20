<script setup lang="ts">
  import { ref } from 'vue';
  import { useRouter } from 'vue-router';
  import { apiFetchAuthenticated } from '@/common/apiFetch';
  import InputText from '@/common/InputText.vue';
  import InputTextArea from '@/common/InputTextArea.vue';
  import StatusSelect from '@/common/StatusSelect.vue';
  import SubmitButton from '@/common/SubmitButton.vue';
  import type { ContentStatus, InputError, Note, Status, StatusActions, Visibility } from '@/common/types';
  import { formatParagraphs, tidy } from '@/common/utils';
  import VisibilitySelect from '@/common/VisibilitySelect.vue';

  const router = useRouter();

  const props = defineProps<{
    dataUserId: string,
    dataUserHandle: string,
    dataStatus: Status,
    dataInputErrors: InputError[],
  }>();

  const emit = defineEmits<{
    'update-status': [newStatus: Status],
    'update-errors': [newInputErrors: InputError[]],
  }>();

  const inputBody = ref<string>('');
  const inputVisibility = ref<Visibility>('Public');
  const inputStatus = ref<ContentStatus>('Draft');
  const inputHandle = ref<string>('');
  const inputPublishedAtNz = ref<string>('');

  function getError(id: string): string | undefined {
    return props.dataInputErrors.find(inputError => inputError.id === id)?.message;
  }

  async function postNote() {
    emit('update-errors', []);

    if (tidy(inputBody.value) === '') {
      emit('update-errors', [{ id: 'body', message: 'Content is required' }]);

      return;
    }

    emit('update-status', 'pending');

    const input = `users/${props.dataUserId}/notes`;

    const statusActions: StatusActions = {
      201: (response?: Response) => {
          response?.json()
            .then((value: Note) => {
              router.push({ name: 'UserNote', params: { userId: props.dataUserHandle, noteId: value.handle ?? value.id } });
            });
      },
      400: () => {
        emit('update-status', 'error');
      }
    }

    const body = new FormData();

    body.append('body', formatParagraphs(inputBody.value));
    body.append('visibility', inputVisibility.value);
    body.append('status', inputStatus.value);

    if (tidy(inputHandle.value) !== '') {
      body.append('handle', tidy(inputHandle.value));
    }

    if (inputPublishedAtNz.value !== '') {
      body.append('publishedAtNz', inputPublishedAtNz.value);
    }

    const init = { method: "POST", body: body };

    apiFetchAuthenticated({ input, statusActions, init });
  }
</script>

<template>
  <section id="panel-notes" role="tabpanel" aria-labelledby="tab-notes">
    <header>
      <h2>Create Note</h2>
    </header>

    <form @submit.prevent="postNote" :action="`/api/users/${dataUserId}/notes`" method="POST" novalidate>
      <InputTextArea
        v-model="inputBody"
        label="Content"
        name="body"
        id="body"
        :maxLength="512"
        :error="getError('body')"
        :isArticle="true"
        help="This is your note. Asterisks allow for *emphasis* and **strong text**." />

      <VisibilitySelect v-model="inputVisibility" />

      <StatusSelect v-model="inputStatus" :can-archive="false" />

      <InputText
        v-model="inputHandle"
        label="Handle"
        name="handle"
        id="handle"
        :maxlength="64"
        help="lower case with hyphens (e.g. 'my-anatini-note'), optional custom web address"
        :error="getError('handle')" />

      <InputText
        v-model="inputPublishedAtNz"
        type="datetime-local"
        label="Date & Time (NZ)"
        name="publishedAtNz"
        id="publishedAtNz"
        help="Leave blank to publish immediately. Notes published in the future will not be visible until the scheduled time."
        :error="getError('publishedAtNz')" />

      <SubmitButton
        :busy="dataStatus === 'pending'"
        text="Create"
        busy-text="Creating..." />
    </form>
  </section>
</template>
