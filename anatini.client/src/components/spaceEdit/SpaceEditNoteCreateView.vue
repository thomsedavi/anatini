<script setup lang="ts">
  import type { InputError, Status, StatusActions, Visibility } from '@/types';
  import { ref } from 'vue';
  import InputTextArea from '../common/InputTextArea.vue';
  import VisibilitySelect from '../common/VisibilitySelect.vue';
  import InputText from '../common/InputText.vue';
  import { formatArticle, tidy } from '../common/utils.ts';
  import SubmitButton from '../common/SubmitButton.vue';
  import { apiFetchAuthenticated } from '../common/apiFetch.ts';

  const props = defineProps<{
    status: Status,
    spaceId: string,
    inputErrors: InputError[],
  }>();

   const emit = defineEmits<{
    'update-status': [newStatus: Status],
    'update-errors': [newInputErrors: InputError[]],
  }>();

  const inputArticle = ref<string>('');
  const inputVisibility = ref<Visibility>('Public');
  const inputNoteHandle = ref<string>('');

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

    const input = `spaces/${props.spaceId}/notes`;

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

    const init = { method: "POST", body: body };

    apiFetchAuthenticated({ input, statusActions, init });
  }
</script>

<template>
  <section id="panel-notes" role="tabpanel" aria-labelledby="tab-notes">
    <header>
      <h2>Create Note</h2>
    </header>

    <form @submit.prevent="postNote" :action="`/api/spaces/${spaceId}/notes`" method="POST" novalidate>
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
          help="lower case with hyphens (e.g. 'my-anatini-space'), optional"
          :error="getError('handle')" />
      </fieldset>

      <SubmitButton
        :busy="status === 'pending'"
        text="Create"
        busy-text="Creating..." />
    </form>
  </section>
</template>
