<script setup lang="ts">
  import { apiFetchAuthenticated } from '@/common/apiFetch';
  import type { APIResponse, InputError, Status, StatusActions, Work } from '@/common/types';
  import { formatParagraphs, parseFromArticleString, parseSource, tidy, type Source } from '@/common/utils';
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

  const work = ref<APIResponse<Work>>({ fetching: true });
  const inputArticle = ref<string>('');
  const inputUrl = ref<string>('');

  watch([() => route.params.userId, () => route.params.workId], (source: Source) => fetchWork(parseSource(source)), { immediate: true });

  async function fetchWork(params: string[]) {
    const input = `users/${params[0]}/works/${params[1]}`;

    const statusActions: StatusActions = {
      200: (response?: Response) => {
        response?.json()
          .then((value: Work) => {
            work.value = { data: value };
            inputArticle.value = parseFromArticleString(value.article);
            inputUrl.value = value.url;
          });
      },
    }

    apiFetchAuthenticated({ input, statusActions });
  }

  function noChange(): boolean {
    if (work.value.data === undefined) {
      return true;
    } else if (tidy(inputArticle.value) !== '' && formatParagraphs(inputArticle.value) !== work.value.data.article) {
      return false;
    } else if (tidy(inputUrl.value) !== work.value.data.url) {
      return false;
    }

    return true;
  }

  function getError(id: string): string | undefined {
    return props.dataInputErrors.find(inputError => inputError.id === id)?.message;
  }

  async function patchWork() {
    if (work.value.data === undefined) {
      return;
    }

    emit('update-errors', []);

    if (noChange()) {
      emit('update-errors', [{ id: 'article', message: 'Work has not been modified' }]);

      return;
    }

    const tidiedUrl = tidy(inputUrl.value);

    const input = `users/${route.params.userId}/works/${route.params.workId}`;

    const statusActions: StatusActions = {
      200: (response?: Response) => {
          response?.json()
            .then((value: Work) => {
              router.push({ name: 'UserWork', params: { userId: props.dataUserHandle, workId: value.handle ?? value.id } });
            });
      }
    }
    
    const body = new FormData();

    if (tidiedUrl !== work.value.data.url) {
      body.append('url', tidiedUrl);
    }

    if (formatParagraphs(inputArticle.value) !== work.value.data.article) {
      body.append('article', formatParagraphs(inputArticle.value));
    }

    const init = { method: "PATCH", body: body };

    apiFetchAuthenticated({ input, statusActions, init });
  }
</script>

<template>
  <section id="panel-works" role="tabpanel" aria-labelledby="tab-works">
    <header>
      <h2>Edit Work</h2>
    </header>

    <template v-if="work === null">
      <p role="status" class="visuallyhidden" aria-live="polite">Please wait while the work information is fetched.</p>
                
      <progress max="100">Fetching work...</progress>
    </template>

    <template v-if="work.error !== undefined">
      <p>
        {{ work.error.body }}
      </p>
    </template>

    <template v-if="work.data !== undefined">
      <form @submit.prevent="patchWork" :action="`/api/users/${route.params.userId}/posts/${route.params.postId}`" method="POST" novalidate>
        <InputText
          v-model="inputUrl"
          label="Link"
          name="url"
          id="url"
          type="url"
          placeholder="https://example.com"
          pattern="https://.*"
          :maxlength="256"
          help="The link to your work (e.g. a ticket booking site)."
          :error="getError('url')" />

        <InputTextArea
          v-model="inputArticle"
          label="Content"
          name="article"
          id="article"
          :maxLength="512"
          :error="getError('article')"
          :isArticle="true"
          help="This is your post. Asterisks allow for *emphasis* and **strong text**." />

        <SubmitButton
          :busy="dataStatus === 'pending'"
          text="Update"
          busy-text="Updating..." />
      </form>
    </template>
  </section>
</template>
