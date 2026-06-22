<script setup lang="ts">
  import type { APIResponse, InputError, NoteEdit, Status, StatusActions, Visibility } from '@/types';
  import { ref, watch } from 'vue';
  import { formatArticle, parseFromArticleString, parseSource, tidy, type Source } from '../common/utils.ts';
  import { useRoute } from 'vue-router';
  import { apiFetchAuthenticated } from '../common/apiFetch.ts';
  import VisibilitySelect from '../common/VisibilitySelect.vue';
  import InputTextArea from '../common/InputTextArea.vue';
  import SubmitButton from '../common/SubmitButton.vue';

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

  watch([() => route.params.spaceId, () => route.params.noteId], (source: Source) => fetchNote(parseSource(source)), { immediate: true });

  async function fetchNote(params: string[]) {
    const statusActions: StatusActions = {
      200: (response?: Response) => {
        response?.json()
          .then((value: NoteEdit) => {
            note.value = { data: value};
            inputArticle.value = parseFromArticleString(value.article);
            inputVisibility.value = value.visibility;
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

    apiFetchAuthenticated(`spaces/${params[0]}/notes/${params[1]}/edit`, statusActions);
  };

  function noChange(): boolean {
    if (note.value.data === undefined) {
      return true;
    } else if (tidy(inputArticle.value) !== '' && formatArticle(inputArticle.value) !== note.value.data.article) {
      return false;
    } else if (inputVisibility.value !== note.value.data.visibility) {
      return false;
    }

    return true;
  }

  function getError(id: string): string | undefined {
    return props.inputErrors.find(inputError => inputError.id === id)?.message;
  }

  async function patchNote() {
    if (note.value.data === undefined || noChange()) {
      return;
    }

    emit('update-errors', []);

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

    const init = { method: "PATCH", body: body };

    apiFetchAuthenticated(`spaces/${route.params.spaceId}/notes/${route.params.noteId}`, statusActions, init);
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
      <form @submit.prevent="patchNote" :action="`/api/spaces/${route.params.spaceId}/notes/${route.params.noteId}`" method="POST" novalidate>
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
        </fieldset>

        <SubmitButton
          :busy="status === 'pending'"
          :disabled="noChange()"
          text="Update"
          busy-text="Updating..." />
      </form>
    </template>
  </section>
</template>
