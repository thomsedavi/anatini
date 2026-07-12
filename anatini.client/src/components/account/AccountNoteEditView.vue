<script setup lang="ts">
  import type { APIResponse, InputError, NoteEdit, Status, StatusActions, Visibility } from '@/types';
  import { ref, watch } from 'vue';
  import { formatArticle, parseFromArticleString, parseSource, tidy, type Source } from '../common/utils';
  import SubmitButton from '../common/SubmitButton.vue';
  import InputText from '../common/InputText.vue';
  import InputTextArea from '../common/InputTextArea.vue';
  import { useRoute } from 'vue-router';
  import { apiFetchAuthenticated } from '../common/apiFetch';
  import VisibilitySelect from '../common/VisibilitySelect.vue';
  import { formatDateTimeNz } from '../common/dateUtils.ts';

  const route = useRoute();

  const props = defineProps<{
    status: Status,
    inputErrors: InputError[],
  }>();

  const emit = defineEmits<{
    'update-status': [newStatus: Status],
    'update-errors': [newInputErrors: InputError[]],
  }>();

  const note = ref<APIResponse<NoteEdit>>({ fetching: true });
  const inputArticle = ref<string>('');
  const inputVisibility = ref<Visibility>('Public');
  const inputNotePublishedAtNz = ref<string>('');

  watch([() => route.params.noteId], (source: Source) => fetchNote(parseSource(source)), { immediate: true });

  async function fetchNote(params: string[]) {
    const statusActions: StatusActions = {
      200: (response?: Response) => {
        response?.json()
          .then((value: NoteEdit) => {
            note.value = { data: value };
            inputArticle.value = parseFromArticleString(value.article);
            inputVisibility.value = value.visibility;
            inputNotePublishedAtNz.value = formatDateTimeNz(value.publishedAtUtc);
          })
          .catch(() => { note.value = { error: { heading: 'Unknown Error', body: 'There was a problem fetching your note, please reload the page' }}});
      },
      404: () => {
        note.value = { error: { heading: '404 Not Found', body: 'Note not found' }};
      },
      500: () => {
        note.value = { error: { heading: 'Unknown Error', body: 'There was a problem fetching your note, please reload the page' }};
      }
    };

    apiFetchAuthenticated(`account/notes/${params[0]}/edit`, statusActions);
  };

  function noChange(): boolean {
    if (note.value.data === undefined) {
      return true;
    } else if (tidy(inputArticle.value) !== '' && formatArticle(inputArticle.value) !== note.value.data.article) {
      return false;
    } else if (inputVisibility.value !== note.value.data.visibility) {
      return false;
    } else if (inputNotePublishedAtNz.value !== '' && inputNotePublishedAtNz.value !== formatDateTimeNz(note.value.data.publishedAtUtc)) {
      return false;
    }

    return true;
  }

  function getError(id: string): string | undefined {
    return props.inputErrors.find(inputError => inputError.id === id)?.message;
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
    
    if (tidy(inputArticle.value) === '') {
      emit('update-errors', [{ id: 'article', message: 'Content is required' }]);

      return;
    }

    emit('update-status', 'pending');

    const body = new FormData();

    const statusActions: StatusActions = {
      200: () => {
        emit('update-status', 'success');
      }
    }

    if (formatArticle(inputArticle.value) !== note.value.data.article) {
      body.append('article', formatArticle(inputArticle.value));
    }

    if (inputVisibility.value !== note.value.data.visibility) {
      body.append('visibility', inputVisibility.value);
    }

    if (inputNotePublishedAtNz.value !== '' && inputNotePublishedAtNz.value !== formatDateTimeNz(note.value.data.publishedAtUtc)) {
      body.append('publishedAtNz', inputNotePublishedAtNz.value);
    }

    const init = { method: "PATCH", body: body };

    apiFetchAuthenticated(`account/notes/${route.params.noteId}`, statusActions, init);
  }
</script>

<template>
  <section id="panel-notes" role="tabpanel" aria-labelledby="tab-notes">
    <header>
      <h2>Edit Note</h2>
    </header>

    <section v-if="note === null">
      <p role="status" class="visuallyhidden" aria-live="polite">Please wait while the note information is fetched.</p>
                
      <progress max="100">Fetching note...</progress>
    </section>

    <section v-if="note.error !== undefined">
      <p>
        {{ note.error.body }}
      </p>
    </section>

    <template v-if="note.data !== undefined">
      <form @submit.prevent="patchNote" :action="`/api/account/notes/${route.params.noteId}`" method="POST" novalidate>
        <fieldset>
          <legend class="visuallyhidden">Edit Note</legend>

          <InputTextArea
            v-model="inputArticle"
            label="Content"
            name="article"
            id="article"
            :maxLength="512"
            :error="getError('article')"
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
        </fieldset>

        <SubmitButton
          :busy="status === 'pending'"
          text="Update"
          busy-text="Updating..." />
      </form>
    </template>
  </section>
</template>
