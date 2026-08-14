<script setup lang="ts">
  import type { InputError, Status, StatusActions, Website } from '@/common/types';
  import InputText from '@/common/InputText.vue';
  import InputTextArea from '@/common/InputTextArea.vue';
  import { ref } from 'vue';
  import { useRouter } from 'vue-router';
  import SubmitButton from '@/common/SubmitButton.vue';
  import { apiFetchAuthenticated } from '@/common/apiFetch';
  import { tidy } from '@/common/utils';

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

  const inputName = ref<string>('');
  const inputArticle = ref<string>('');
  const inputHandle = ref<string>('');
  const inputUrl = ref<string>('');

  function getError(id: string): string | undefined {
    return props.inputErrors.find(inputError => inputError.id === id)?.message;
  }

  async function postWebsite() {
    emit('update-errors', []);

    const tidiedName = tidy(inputName.value);
    const tidiedUrl = tidy(inputUrl.value);

    const inputErrors: InputError[] = [];

    if (tidiedUrl === '') {
      inputErrors.push({ id: 'url', message: 'Link is required' });
    }

    if (tidiedName === '') {
      inputErrors.push({ id: 'name', message: 'Name is required' });
    }

    if (inputErrors.length > 0) {
      emit('update-errors', inputErrors);

      return;
    }

    emit('update-status', 'pending');

    const input = `users/${props.userId}/websites`;

    const statusActions: StatusActions = {
      201: (response?: Response) => {
          response?.json()
            .then((value: Website) => {
              router.push({ name: 'UserWebsite', params: { userId: props.userHandle, websiteId: value.handle } });
            });
      },
      400: () => {
        emit('update-status', 'error');
      }
    }

    const body = new FormData();

    body.append('url', tidiedUrl);
    body.append('name', tidiedName);

    const init = { method: "POST", body: body };

    apiFetchAuthenticated({ input, statusActions, init });
  }
</script>

<template>
  <section id="panel-works" role="tabpanel" aria-labelledby="tab-works">
    <header>
      <h2>Create Website</h2>
    </header>

    <form @submit.prevent="postWebsite" :action="`/api/users/${userId}/websites`" method="POST" novalidate>
      <InputText
        v-model="inputUrl"
        label="Link"
        name="url"
        id="url"
        type="url"
        placeholder="https://example.com"
        pattern="https://.*"
        :maxlength="256"
        help="The link to your website (e.g. a ticket booking site)."
        :error="getError('url')" />

      <InputText
        v-model="inputName"
        label="Name"
        name="name"
        id="name"
        :maxlength="256"
        :required="true"
        help="The name of your website"
        :error="getError('name')" />

      <InputTextArea
        v-model="inputArticle"
        label="Description"
        name="article"
        id="article"
        :maxLength="512"
        :error="getError('article')"
        :isArticle="true"
        help="Describe your link. Asterisks allow for *emphasis* and **strong text**." />

      <InputText
        v-model="inputHandle"
        label="Handle"
        name="handle"
        id="handle"
        :maxlength="64"
        help="lower case with hyphens (e.g. 'my-anatini-website'), optional custom web address"
        :error="getError('handle')" />

      <SubmitButton
        :busy="status === 'pending'"
        text="Create"
        busy-text="Creating..." />
    </form>
  </section>
</template>
