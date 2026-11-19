<script setup lang="ts">
  import type { APIResponse, InputError, Note, Status, StatusActions, Visibility } from '@/common/types';
  import { ref, watch } from 'vue';
  import { formatParagraphs, parseFromBodyString, parseParams, tidy, type Params } from '@/common/utils';
  import SubmitButton from '@/common/SubmitButton.vue';
  import InputText from '@/common/InputText.vue';
  import InputTextArea from '@/common/InputTextArea.vue';
  import { useRoute, useRouter } from 'vue-router';
  import { apiFetchAuthenticated } from '@/common/apiFetch';
  import VisibilitySelect from '@/common/VisibilitySelect.vue';
  import { formatDateTimeNz } from '@/common/dateUtils';

  const route = useRoute();
  const router = useRouter();

  const props = defineProps<{
    dataUserHandle: string,
    dataStatus: Status,
    dataInputErrors: InputError[],
  }>();

  const emit = defineEmits<{
    'update-status': [newStatus: Status],
    'update-errors': [newInputErrors: InputError[]],
  }>();

  const note = ref<APIResponse<Note>>({ fetching: true });
  const inputBody = ref<string>('');
  const inputVisibility = ref<Visibility>('Public');
  const inputNotePublishedAtNz = ref<string>('');

  watch([() => route.params.userId, () => route.params.noteId], (params: Params) => fetchNote(parseParams(params)), { immediate: true });

  async function fetchNote(params: string[]) {
    const input = `users/${params[0]}/notes/${params[1]}`;

    const statusActions: StatusActions = {
      200: (response?: Response) => {
        response?.json()
          .then((value: Note) => {
            note.value = { data: value };
            inputBody.value = parseFromBodyString(`<article>${value.body}</article>`);
            inputVisibility.value = value.visibility;
            inputNotePublishedAtNz.value = formatDateTimeNz(value.publishedAtNz);
          })
          .catch(() => {
            note.value = { error: { heading: 'Unknown Error', body: 'There was a problem fetching your note, please reload the page' }};
          });
      },
      404: () => {
        note.value = { error: { heading: '404 Not Found', body: 'Note not found' }};
      },
      500: () => {
        note.value = { error: { heading: 'Unknown Error', body: 'There was a problem fetching your note, please reload the page' }};
      },
    };

    apiFetchAuthenticated({ input, statusActions });
  };

  function noChange(): boolean {
    if (note.value.data === undefined) {
      return true;
    } else if (tidy(inputBody.value) !== '' && formatParagraphs(inputBody.value) !== note.value.data.body) {
      return false;
    } else if (inputVisibility.value !== note.value.data.visibility) {
      return false;
    } else if (inputNotePublishedAtNz.value !== '' && inputNotePublishedAtNz.value !== formatDateTimeNz(note.value.data.publishedAtNz)) {
      return false;
    }

    return true;
  }

  function getError(id: string): string | undefined {
    return props.dataInputErrors.find(inputError => inputError.id === id)?.message;
  }

  async function patchNote() {
    if (note.value.data === undefined) {
      return;
    }

    emit('update-errors', []);

    if (noChange()) {
      emit('update-errors', [{ id: 'article', message: 'Note has not been modified' }]);

      return;
    }
    
    if (tidy(inputBody.value) === '') {
      emit('update-errors', [{ id: 'body', message: 'Content is required' }]);

      return;
    }

    emit('update-status', 'pending');

    const input = `users/${route.params.userId}/notes/${route.params.noteId}`;

    const statusActions: StatusActions = {
      204: () => {
        router.push({ name: 'UserNote', params: { userId: props.dataUserHandle, noteId: note.value.data!.handle } });
      }
    }

    const body = new FormData();

    if (formatParagraphs(inputBody.value) !== note.value.data.body) {
      body.append('body', formatParagraphs(inputBody.value));
    }

    if (inputVisibility.value !== note.value.data.visibility) {
      body.append('visibility', inputVisibility.value);
    }

    if (inputNotePublishedAtNz.value !== '' && inputNotePublishedAtNz.value !== formatDateTimeNz(note.value.data.publishedAtNz)) {
      body.append('publishedAtNz', inputNotePublishedAtNz.value);
    }

    const init = { method: "PATCH", body: body };

    apiFetchAuthenticated({ input, statusActions, init });
  }
</script>

<template>
  <section id="panel-notes" role="tabpanel" aria-labelledby="tab-notes">
    <header>
      <h2>Edit Note</h2>
    </header>

    <template v-if="note === null">
      <p role="status" class="visually-hidden" aria-live="polite">Please wait while the note information is fetched.</p>
                
      <progress max="100">Fetching note...</progress>
    </template>

    <template v-if="note.error !== undefined">
      <p>
        {{ note.error.body }}
      </p>
    </template>

    <template v-if="note.data !== undefined">
      <form @submit.prevent="patchNote" :action="`/api/users/${route.params.userId}/notes/${route.params.noteId}`" method="POST" novalidate>
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

        <InputText
          v-model="inputNotePublishedAtNz"
          type="datetime-local"
          label="Date & Time (NZ)"
          name="publishedAtNz"
          id="publishedAtNz"
          help="Leave blank to publish immediately. Notes set in the future will not be visible until that scheduled time."
          :error="getError('publishedAtNz')" />

        <SubmitButton
          :busy="dataStatus === 'pending'"
          text="Update"
          busy-text="Updating..." />
      </form>
    </template>
  </section>
</template>
