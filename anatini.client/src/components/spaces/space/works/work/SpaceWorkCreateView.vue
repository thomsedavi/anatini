<script setup lang="ts">
  import type { InputError, Status, StatusActions, Visibility, Work } from '@/common/types';
  import InputText from '@/common/InputText.vue';
  import InputTextArea from '@/common/InputTextArea.vue';
  import { ref } from 'vue';
  import { useRouter } from 'vue-router';
  import SubmitButton from '@/common/SubmitButton.vue';
  import { apiFetchAuthenticated } from '@/common/apiFetch';
  import VisibilitySelect from '@/common/VisibilitySelect.vue';
  import { formatParagraphs, tidy } from '@/common/utils';

  const router = useRouter();

  const props = defineProps<{
    dataSpaceId: string,
    dataSpaceHandle: string,
    dataStatus: Status,
    dataInputErrors: InputError[],
  }>();

  const emit = defineEmits<{
    'update-status': [newStatus: Status],
    'update-errors': [newInputErrors: InputError[]],
  }>();

  const inputTitle = ref<string>('');
  const inputBody = ref<string>('');
  const inputVisibility = ref<Visibility>('Public');
  const inputHandle = ref<string>('');

  function getError(id: string): string | undefined {
    return props.dataInputErrors.find(inputError => inputError.id === id)?.message;
  }

  async function postWork() {
    emit('update-errors', []);

    const tidiedTitle = tidy(inputTitle.value);

    const inputErrors: InputError[] = [];

    if (tidiedTitle === '') {
      inputErrors.push({ id: 'title', message: 'Title is required' });
    }

    if (tidy(inputBody.value) === '') {
      inputErrors.push({ id: 'body', message: 'Content is required' });
    }

    if (inputErrors.length > 0) {
      emit('update-errors', inputErrors);

      return;
    }

    emit('update-status', 'pending');

    const input = `spaces/${props.dataSpaceId}/works`;

    const statusActions: StatusActions = {
      201: (response?: Response) => {
          response?.json()
            .then((value: Work) => {
              router.push({ name: 'SpaceWork', params: { spaceId: props.dataSpaceHandle, workId: value.handle ?? value.id } });
            });
      },
      400: () => {
        emit('update-status', 'error');
      }
    }

    const body = new FormData();

    body.append('title', tidiedTitle);
    body.append('body', formatParagraphs(inputBody.value));
    body.append('visibility', inputVisibility.value);

    if (tidy(inputHandle.value) !== '') {
      body.append('handle', tidy(inputHandle.value));
    }

    const init = { method: "POST", body: body };

    apiFetchAuthenticated({ input, statusActions, init });
  }
</script>

<template>
  <section id="panel-works" role="tabpanel" aria-labelledby="tab-works">
    <header>
      <h2>Create Work</h2>
    </header>

    <form @submit.prevent="postWork" :action="`/api/spaces/${dataSpaceId}/works`" method="POST" novalidate>
      <InputText
        v-model="inputTitle"
        label="Title"
        name="title"
        id="title"
        :maxlength="256"
        :required="true"
        help="The title of your work"
        :error="getError('title')" />

      <InputTextArea
        v-model="inputBody"
        label="Description"
        name="body"
        id="body"
        :maxLength="512"
        :error="getError('body')"
        :isArticle="true"
        help="Describe your link. Asterisks allow for *emphasis* and **strong text**." />

      <VisibilitySelect v-model="inputVisibility" />

      <InputText
        v-model="inputHandle"
        label="Handle"
        name="handle"
        id="handle"
        :maxlength="64"
        help="lower case with hyphens (e.g. 'my-anatini-work'), optional custom web address"
        :error="getError('handle')" />

      <SubmitButton
        :busy="dataStatus === 'pending'"
        text="Create"
        busy-text="Creating..." />
    </form>
  </section>
</template>
