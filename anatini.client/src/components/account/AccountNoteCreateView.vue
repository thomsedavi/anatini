<script setup lang="ts">
  import type { InputError, Status, StatusActions, Visibility } from '@/types';
  import { ref } from 'vue';
  import InputText from '../common/InputText.vue';
  import InputTextArea from '../common/InputTextArea.vue';
  import { formatArticle, tidy } from '../common/utils';
  import SubmitButton from '../common/SubmitButton.vue';
  import { apiFetchAuthenticated } from '../common/apiFetch';
  import VisibilitySelect from '../common/VisibilitySelect.vue';

  const props = defineProps<{
    status: Status,
    inputErrors: InputError[],
  }>();

  const emit = defineEmits<{
    'update-status': [newStatus: Status],
    'update-errors': [newInputErrors: InputError[]],
  }>();

  const inputArticle = ref<string>('');
  const inputVisibility = ref<Visibility>('Public');
  const inputNoteHandle = ref<string>('');
  const inputNotePublishedAtNz = ref<string>('');

  function getError(id: string): string | undefined {
    return props.inputErrors.find(inputError => inputError.id === id)?.message;
  }

  async function postNote() {
    emit('update-errors', []);

    if (tidy(inputArticle.value) === '') {
      emit('update-errors', [{ id: 'article', message: 'Content is required' }]);

      return;
    }

    emit('update-status', 'pending');

    const statusActions: StatusActions = {
      201: () => {
        emit('update-status', 'success');

        console.log('Handle thing');
      },
      400: () => {
        emit('update-status', 'error');
      }
    }

    const body = new FormData();

    body.append('article', formatArticle(inputArticle.value));
    body.append('visibility', inputVisibility.value);

    if (tidy(inputNoteHandle.value) !== '') {
      body.append('handle', tidy(inputNoteHandle.value));
    }

    if (inputNotePublishedAtNz.value !== '') {
      body.append('publishedAtNz', inputNotePublishedAtNz.value);
    }

    const init = { method: "POST", body: body };

    apiFetchAuthenticated('account/notes', statusActions, init);
  }
</script>

<template>
  <section id="panel-notes" role="tabpanel" aria-labelledby="tab-notes">
    <header>
      <h2>Create Note</h2>
    </header>

    <form @submit.prevent="postNote" :action="`/api/account/notes`" method="POST" novalidate>
      <fieldset>
        <legend class="visuallyhidden">Create Note</legend>

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
          v-model="inputNoteHandle"
          label="Handle"
          name="handle"
          id="handle"
          :maxlength="64"
          help="lower case with hyphens (e.g. 'my-anatini-space'), optional custom web address"
          :error="getError('handle')" />

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
        :disabled="tidy(inputArticle) === ''"
        text="Create"
        busy-text="Creating..." />
    </form>
  </section>
</template>
