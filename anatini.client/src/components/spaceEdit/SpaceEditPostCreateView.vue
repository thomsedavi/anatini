<script setup lang="ts">
  import type { InputError, Status, StatusActions, Visibility } from '@/common/types';
  import { ref } from 'vue';
  import InputTextArea from '@/common/InputTextArea.vue';
  import VisibilitySelect from '@/common/VisibilitySelect.vue';
  import InputText from '@/common/InputText.vue';
  import { formatArticle, tidy } from '@/common/utils';
  import SubmitButton from '@/common/SubmitButton.vue';
  import { apiFetchAuthenticated } from '@/common/apiFetch';

  const props = defineProps<{
    dataStatus: Status,
    dataSpaceId: string,
    dataInputErrors: InputError[],
  }>();

   const emit = defineEmits<{
    'update-status': [newStatus: Status],
    'update-errors': [newInputErrors: InputError[]],
  }>();

  const inputArticle = ref<string>('');
  const inputVisibility = ref<Visibility>('Public');
  const inputPostHandle = ref<string>('');

  function getError(id: string): string | undefined {
    return props.dataInputErrors.find(inputError => inputError.id === id)?.message;
  }

  async function postPost() {
    emit('update-errors', []);

    if (tidy(inputArticle.value) === '') {
      emit('update-errors', [{ id: 'article', message: 'Content is required' }]);

      return;
    }

    emit('update-status', 'pending');

    const input = `spaces/${props.dataSpaceId}/posts`;

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

    if (tidy(inputPostHandle.value) !== '') {
      body.append('handle', tidy(inputPostHandle.value));
    }

    const init = { method: "POST", body: body };

    apiFetchAuthenticated({ input, statusActions, init });
  }
</script>

<template>
  <section id="panel-posts" role="tabpanel" aria-labelledby="tab-posts">
    <header>
      <h2>Create Post</h2>
    </header>

    <form @submit.prevent="postPost" :action="`/api/spaces/${dataSpaceId}/posts`" method="POST" novalidate>
      <fieldset>
        <legend class="visuallyhidden">Create Post</legend>

        <InputTextArea
          v-model="inputArticle"
          label="Content"
          name="article"
          id="article"
          :maxLength="512"
          :error="getError('article')"
          :isArticle="true"
          help="This is your post. Asterisks allow for *emphasis* and **strong text**." />

        <VisibilitySelect v-model="inputVisibility" />

        <InputText
          v-model="inputPostHandle"
          label="Handle"
          name="handle"
          id="handle"
          :maxlength="64"
          help="lower case with hyphens (e.g. 'my-anatini-space'), optional"
          :error="getError('handle')" />
      </fieldset>

      <SubmitButton
        :busy="dataStatus === 'pending'"
        text="Create"
        busy-text="Creating..." />
    </form>
  </section>
</template>
