<script setup lang="ts">
  import { apiFetchAuthenticated } from '@/common/apiFetch';
  import type { APIResponse, InputError, Status, StatusActions, Work } from '@/common/types';
  import { formatArticle, parseFromArticleString, parseSource, tidy, type Source } from '@/common/utils';
  import SubmitButton from '@/common/SubmitButton.vue';
  import InputText from '@/common/InputText.vue';
  import InputTextArea from '@/common/InputTextArea.vue';
  import { ref, watch } from 'vue';
  import { useRoute, useRouter } from 'vue-router';

  const route = useRoute();
  const router = useRouter();

  const props = defineProps<{
    dataUserHandle: string,
    dataStatus: Status,
    dataInputErrors: InputError[],
  }>();

  const emit = defineEmits<{
    'update-status': [newStatus: Status],
    'update-errors': [newInputErrors: InputError[]],
  }>();

  const website = ref<APIResponse<Work>>({ fetching: true });
  const inputArticle = ref<string>('');
  const inputUrl = ref<string>('');

  watch([() => route.params.userId, () => route.params.websiteId], (source: Source) => fetchWebsite(parseSource(source)), { immediate: true });

  async function fetchWebsite(params: string[]) {
    const input = `users/${params[0]}/websites/${params[1]}`;

    const statusActions: StatusActions = {
      200: (response?: Response) => {
        response?.json()
          .then((value: Work) => {
            website.value = { data: value };
            inputArticle.value = parseFromArticleString(value.article);
            inputUrl.value = value.url;
          });
      },
    }

    apiFetchAuthenticated({ input, statusActions });
  }

  function noChange(): boolean {
    if (website.value.data === undefined) {
      return true;
    } else if (tidy(inputArticle.value) !== '' && formatArticle(inputArticle.value) !== website.value.data.article) {
      return false;
    } else if (tidy(inputUrl.value) !== website.value.data.url) {
      return false;
    }

    return true;
  }

  function getError(id: string): string | undefined {
    return props.dataInputErrors.find(inputError => inputError.id === id)?.message;
  }

  async function patchWebsite() {
    if (website.value.data === undefined) {
      return;
    }

    emit('update-errors', []);

    if (noChange()) {
      emit('update-errors', [{ id: 'article', message: 'Website has not been modified' }]);

      return;
    }

    const tidiedUrl = tidy(inputUrl.value);

    const input = `users/${route.params.userId}/websites/${route.params.websiteId}`;

    const statusActions: StatusActions = {
      200: (response?: Response) => {
          response?.json()
            .then((value: Work) => {
              router.push({ name: 'UserWebsite', params: { userId: props.dataUserHandle, websiteId: value.handle ?? value.id } });
            });
      }
    }
    
    const body = new FormData();

    if (tidiedUrl !== website.value.data.url) {
      body.append('url', tidiedUrl);
    }

    if (formatArticle(inputArticle.value) !== website.value.data.article) {
      body.append('article', formatArticle(inputArticle.value));
    }

    const init = { method: "PATCH", body: body };

    apiFetchAuthenticated({ input, statusActions, init });
  }
</script>

<template>
  <section id="panel-works" role="tabpanel" aria-labelledby="tab-works">
    <header>
      <h2>Edit Website</h2>
    </header>

    <template v-if="website === null">
      <p role="status" class="visuallyhidden" aria-live="polite">Please wait while the website information is fetched.</p>
                
      <progress max="100">Fetching website...</progress>
    </template>

    <template v-if="website.error !== undefined">
      <p>
        {{ website.error.body }}
      </p>
    </template>

    <template v-if="website.data !== undefined">
      <form @submit.prevent="patchWebsite" :action="`/api/users/${route.params.userId}/notes/${route.params.noteId}`" method="POST" novalidate>
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

        <InputTextArea
          v-model="inputArticle"
          label="Content"
          name="article"
          id="article"
          :maxLength="512"
          :error="getError('article')"
          :isArticle="true"
          help="This is your note. Asterisks allow for *emphasis* and **strong text**." />

        <SubmitButton
          :busy="dataStatus === 'pending'"
          text="Update"
          busy-text="Updating..." />
      </form>
    </template>
  </section>
</template>
