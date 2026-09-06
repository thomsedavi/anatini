<script setup lang="ts">
  import type { APIResponse, InputError, PostEdit, Status, StatusActions, Visibility } from '@/common/types';
  import { ref, watch } from 'vue';
  import { formatArticle, parseFromArticleString, parseSource, tidy, type Source } from '@/common/utils';
  import SubmitButton from '@/common/SubmitButton.vue';
  import InputText from '@/common/InputText.vue';
  import InputTextArea from '@/common/InputTextArea.vue';
  import { useRoute } from 'vue-router';
  import { apiFetchAuthenticated } from '@/common/apiFetch';
  import VisibilitySelect from '@/common/VisibilitySelect.vue';
  import { formatDateTimeNz } from '@/common/dateUtils';

  const route = useRoute();

  const props = defineProps<{
    dataStatus: Status,
    dataInputErrors: InputError[],
  }>();

  const emit = defineEmits<{
    'update-status': [newStatus: Status],
    'update-errors': [newInputErrors: InputError[]],
  }>();

  const post = ref<APIResponse<PostEdit>>({ fetching: true });
  const inputArticle = ref<string>('');
  const inputVisibility = ref<Visibility>('Public');
  const inputPostPublishedAtNz = ref<string>('');

  watch([() => route.params.postId], (source: Source) => fetchPost(parseSource(source)), { immediate: true });

  async function fetchPost(params: string[]) {
    const input = `spaces/${params[0]}/posts/${params[1]}/edit`;

    const statusActions: StatusActions = {
      200: (response?: Response) => {
        response?.json()
          .then((value: PostEdit) => {
            post.value = { data: value };
            inputArticle.value = parseFromArticleString(value.article);
            inputVisibility.value = value.visibility;
            inputPostPublishedAtNz.value = formatDateTimeNz(value.publishedAtNz);
          })
          .catch(() => {
            post.value = { error: { heading: 'Unknown Error', body: 'There was a problem fetching your post, please reload the page' }};
          });
      },
      404: () => {
        post.value = { error: { heading: '404 Not Found', body: 'Post not found' }};
      },
      500: () => {
        post.value = { error: { heading: 'Unknown Error', body: 'There was a problem fetching your post, please reload the page' }};
      }
    };

    apiFetchAuthenticated({ input, statusActions });
  };

  function noChange(): boolean {
    if (post.value.data === undefined) {
      return true;
    } else if (tidy(inputArticle.value) !== '' && formatArticle(inputArticle.value) !== post.value.data.article) {
      return false;
    } else if (inputVisibility.value !== post.value.data.visibility) {
      return false;
    } else if (inputPostPublishedAtNz.value !== '' && inputPostPublishedAtNz.value !== formatDateTimeNz(post.value.data.publishedAtNz)) {
      return false;
    }

    return true;
  }

  function getError(id: string): string | undefined {
    return props.dataInputErrors.find(inputError => inputError.id === id)?.message;
  }

  async function patchPost() {
    if (post.value.data === undefined) {
      return;
    }

    emit('update-errors', []);

    if (noChange()) {
      emit('update-errors', [{ id: 'article', message: 'Post has not been modified' }]);

      return;
    }
    
    if (tidy(inputArticle.value) === '') {
      emit('update-errors', [{ id: 'article', message: 'Content is required' }]);

      return;
    }

    emit('update-status', 'pending');

    const input = `spaces/${route.params.spaceId}/posts/${route.params.postId}`;

    const statusActions: StatusActions = {
      200: () => {
        emit('update-status', 'success');
      }
    }

    const body = new FormData();

    if (formatArticle(inputArticle.value) !== post.value.data.article) {
      body.append('article', formatArticle(inputArticle.value));
    }

    if (inputVisibility.value !== post.value.data.visibility) {
      body.append('visibility', inputVisibility.value);
    }

    if (inputPostPublishedAtNz.value !== '' && inputPostPublishedAtNz.value !== formatDateTimeNz(post.value.data.publishedAtNz)) {
      body.append('publishedAtNz', inputPostPublishedAtNz.value);
    }

    const init = { method: "PATCH", body: body };

    apiFetchAuthenticated({ input, statusActions, init });
  }
</script>

<template>
  <section id="panel-posts" role="tabpanel" aria-labelledby="tab-posts">
    <header>
      <h2>Edit Post</h2>
    </header>

    <template v-if="post === null">
      <p role="status" class="visuallyhidden" aria-live="polite">Please wait while the post information is fetched.</p>
                
      <progress max="100">Fetching post...</progress>
    </template>

    <template v-if="post.error !== undefined">
      <p>
        {{ post.error.body }}
      </p>
    </template>

    <template v-if="post.data !== undefined">
      <form @submit.prevent="patchPost" :action="`/api/spaces/${route.params.spaceId}/posts/${route.params.postId}`" method="POST" novalidate>
        <fieldset>
          <legend class="visuallyhidden">Edit Post</legend>

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
            v-model="inputPostPublishedAtNz"
            type="datetime-local"
            label="Date & Time (NZ)"
            name="publishedAtNz"
            id="publishedAtNz"
            help="Leave blank to publish immediately. Posts set in the future will not be visible until that scheduled time."
            :error="getError('publishedAtNz')" />
        </fieldset>

        <SubmitButton
          :busy="dataStatus === 'pending'"
          text="Update"
          busy-text="Updating..." />
      </form>
    </template>
  </section>
</template>
