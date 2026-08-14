<script setup lang="ts">
  import type { InputError, Note, Status, StatusActions, Visibility } from '@/common/types';
  import { ref } from 'vue';
  import InputText from '@/common/InputText.vue';
  import InputTextArea from '@/common/InputTextArea.vue';
  import { formatArticle, tidy } from '@/common/utils';
  import SubmitButton from '@/common/SubmitButton.vue';
  import { apiFetchAuthenticated } from '@/common/apiFetch';
  import VisibilitySelect from '@/common/VisibilitySelect.vue';
  import { useRouter } from 'vue-router';

  const router = useRouter();

  const props = defineProps<{
    userId: string,
    userHandle: string,
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

    const input = `users/${props.userId}/notes`;

    const statusActions: StatusActions = {
      201: (response?: Response) => {
          response?.json()
            .then((value: Note) => {
              router.push({ name: 'UserNote', params: { userId: props.userHandle, noteId: value.handle } });
            });
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

    apiFetchAuthenticated({ input, statusActions, init });
  }
</script>

<template>
  <section id="panel-posts" role="tabpanel" aria-labelledby="tab-posts">
    <header>
      <h2>Create Note</h2>
    </header>

    <form @submit.prevent="postNote" :action="`/api/users/${userId}/notes`" method="POST" novalidate>
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

      <SubmitButton
        :busy="status === 'pending'"
        text="Create"
        busy-text="Creating..." />
    </form>
  </section>
</template>
