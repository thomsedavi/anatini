<script setup lang="ts">
  import type { ErrorMessage, InputError, NoteEdit, Status, StatusActions } from '@/types';
  import { ref, watch } from 'vue';
  import { formatArticle, parseFromArticleString, parseSource, tidy, type Source } from '../common/utils';
  import SubmitButton from '../common/SubmitButton.vue';
  import InputTextArea from '../common/InputTextArea.vue';
  import { useRoute } from 'vue-router';
  import { apiFetchAuthenticated } from '../common/apiFetch';

  const route = useRoute();

  defineProps<{
    status: Status,
  }>();

  const note = ref<NoteEdit | ErrorMessage | null>(null);
  const inputErrors = ref<InputError[]>([]);
  const inputArticle = ref<string>('');

  watch([() => route.params.noteId], (source: Source) => fetchNote(parseSource(source)), { immediate: true });

  async function fetchNote(params: string[]) {
    const statusActions: StatusActions = {
      200: (response?: Response) => {
        response?.json()
          .then((value: NoteEdit) => {
            note.value = value;
            inputArticle.value = parseFromArticleString(value.article);
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
    return inputErrors.value.find(inputError => inputError.id === id)?.message;
  }

  async function patchNote() {

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
