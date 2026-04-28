<script setup lang="ts">
  import { nextTick, ref, watch } from 'vue';
  import { useRoute } from 'vue-router';
  import { formatArticle, parseFromArticleString, parseSource, tidy, type Source } from '../common/utils';
  import type { ErrorMessage, InputError, NoteEdit, Status, StatusActions } from '@/types';
  import { apiFetchAuthenticated } from '../common/apiFetch';
  import InputTextArea from '../common/InputTextArea.vue';
  import SubmitButton from '../common/SubmitButton.vue';

  const route = useRoute();

  const note = ref<NoteEdit | ErrorMessage | null>(null);
  const inputErrors = ref<InputError[]>([]);
  const inputArticle = ref<string>('');
  const status = ref<Status>('idle');
  const errorSectionRef = ref<HTMLElement | null>(null);

  watch([() => route.params.channelId, () => route.params.noteId], (source: Source) => fetchNote(parseSource(source)), { immediate: true });

  async function fetchNote(params: string[]) {
    const statusActions: StatusActions = {
      200: (response?: Response) => {
        response?.json()
          .then((value: NoteEdit) => {
            note.value = value;
            inputArticle.value = parseFromArticleString(value.article);
          })
          .catch(() => { note.value = { error: true, heading: 'Unknown Error', body: 'There was a problem fetching your account, please reload the page' }});
      },
      404: () => {
        note.value = { error: true, heading: '404 Not Found', body: 'Channel not found' };
      },
      500: () => {
        note.value = { error: true, heading: 'Unknown Error', body: 'There was a problem fetching your account, please reload the page' };
      }
    };

    apiFetchAuthenticated(`channels/${params[0]}/notes/${params[1]}/edit`, statusActions);
  };

  function getError(id: string): string | undefined {
    return inputErrors.value.find(inputError => inputError.id === id)?.message;
  }

  async function patchNote() {
    inputErrors.value = [];

    if (tidy(inputArticle.value) === '') {
      inputErrors.value.push({id: 'article', message: 'Content is required'});
    }

    const formattedArticle = formatArticle(inputArticle.value);

    if (inputErrors.value.length > 0) {
      nextTick(() => {
        errorSectionRef.value?.focus();
      });

      return;
    }

    status.value = 'pending';

    const statusActions: StatusActions = {
      200: () => {
        status.value = 'success';
      }
    }

    const body = new FormData();

    body.append('article', formattedArticle);

    const init = { method: "PATCH", body: body };

    apiFetchAuthenticated(`channels/${route.params.channelId}/notes/${route.params.noteId}`, statusActions, init);
  }
</script>

<template>
  <main id="main" tabindex="-1">
    <header>
      <h1>Edit Note</h1>
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
      <section id="errors" v-if="inputErrors.length > 0" ref="errorSectionRef" tabindex="-1" aria-live="assertive" aria-labelledby="heading-errors">
        <h2 id="heading-errors">There was a problem editing your note</h2>
        <ul>
          <li v-for="error in inputErrors" :key="'error' + error.id">
            <a :href="'#input-' + error.id">{{ error.message }}</a>
          </li>
        </ul>
      </section>

      <form @submit.prevent="patchNote" :action="`/api/channels/${route.params.channelId}/notes/${route.params.noteId}`" method="POST" novalidate>
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
  </main>
</template>
