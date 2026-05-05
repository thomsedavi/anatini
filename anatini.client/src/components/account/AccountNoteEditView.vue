<script setup lang="ts">
  import type { ErrorMessage, InputError, NoteEdit, Status } from '@/types';
  import { ref } from 'vue';
  import { formatArticle, tidy } from '../common/utils';
  import SubmitButton from '../common/SubmitButton.vue';
  import InputTextArea from '../common/InputTextArea.vue';
  import { useRoute } from 'vue-router';

  const route = useRoute();

  defineProps<{
    status: Status,
  }>();

  const note = ref<NoteEdit | ErrorMessage | null>(null);
  const inputErrors = ref<InputError[]>([]);
  const inputArticle = ref<string>('');

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
