<script setup lang="ts">
  import type { ErrorMessage, InputError, NoteEdit, Status, StatusActions, Visibility } from '@/types';
  import { ref, watch } from 'vue';
  import { formatArticle, parseFromArticleString, parseSource, tidy, type Source } from '../common/utils';
  import SubmitButton from '../common/SubmitButton.vue';
  import InputTextArea from '../common/InputTextArea.vue';
  import { useRoute } from 'vue-router';
  import { apiFetchAuthenticated } from '../common/apiFetch';
  import VisibilitySelect from '../common/VisibilitySelect.vue';

  const route = useRoute();

  const props = defineProps<{
    status: Status,
    inputErrors: InputError[],
  }>();

  const emit = defineEmits<{
    'update-status': [newStatus: Status],
    'update-errors': [newInputErrors: InputError[]],
  }>();

  const note = ref<NoteEdit | ErrorMessage | null>(null);
  const inputArticle = ref<string>('');
  const inputVisibility = ref<Visibility>('Public');

  watch([() => route.params.noteId], (source: Source) => fetchNote(parseSource(source)), { immediate: true });

  async function fetchNote(params: string[]) {
    const statusActions: StatusActions = {
      200: (response?: Response) => {
        response?.json()
          .then((value: NoteEdit) => {
            note.value = value;
            inputArticle.value = parseFromArticleString(value.article);
            inputVisibility.value = value.visibility;
          })
          .catch(() => { note.value = { error: true, heading: 'Unknown Error', body: 'There was a problem fetching your note, please reload the page' }});
      },
      404: () => {
        note.value = { error: true, heading: '404 Not Found', body: 'Note not found' };
      },
      500: () => {
        note.value = { error: true, heading: 'Unknown Error', body: 'There was a problem fetching your note, please reload the page' };
      }
    };

    apiFetchAuthenticated(`account/notes/${params[0]}/edit`, statusActions);
  };

  function getError(id: string): string | undefined {
    return props.inputErrors.find(inputError => inputError.id === id)?.message;
  }

  async function patchNote() {
    if (note.value === null || 'error' in note.value) {
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

    body.append('article', formatArticle(inputArticle.value));

    if (inputVisibility.value !== note.value.visibility) {
      body.append('visibility', inputVisibility.value);
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

    <section v-else-if="'error' in note">
      <p>
        {{ note.body }}
      </p>
    </section>

    <template v-else>
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
        </fieldset>

        <SubmitButton
          :busy="status === 'pending'"
          :disabled="tidy(inputArticle) === '' || formatArticle(inputArticle) === note.article"
          text="Update"
          busy-text="Updating..." />
      </form>
    </template>
  </section>
</template>
