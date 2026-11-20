<script setup lang="ts">
  import type { APIResponse, SpaceEdit, InputError, Status, StatusActions } from '@/common/types';
  import { nextTick, ref, watch } from 'vue';
  import { parseParams, tidy, type Params } from '@/common/utils';
  import { apiFetchAuthenticated } from '@/common/apiFetch';
  import { useRoute, useRouter } from 'vue-router';
  import InputText from '@/common/InputText.vue';
  import SubmitButton from '@/common/SubmitButton.vue';

  const route = useRoute();
  const router = useRouter();

  const space = ref<APIResponse<SpaceEdit>>({ fetching: true });
  const inputErrors = ref<InputError[]>([]);
  const inputNoteName = ref<string>('');
  const inputNoteHandle = ref<string>('');
  const status = ref<Status>('idle');
  const errorSectionRef = ref<HTMLElement | null>(null);

  watch([() => route.params.spaceId], (params: Params) => fetchSpace(parseParams(params)), { immediate: true });

  async function fetchSpace(params: string[]) {
    const input = `spaces/${params[0]}/edit`;

    const statusActions: StatusActions = {
      200: (response?: Response) => {
        response?.json()
          .then((value: SpaceEdit) => {
            space.value = { data: value };
          })
          .catch(() => {
            space.value = { error: { status: 'unknown', heading: 'Unknown Error', body: 'There was a problem fetching your account, please reload the page' }};
          });
      },
      401: () => {
        router.replace({ path: '/sign-in', query: { redirect: `/spaces/${params[0]}/notes/create` } });
      },
      403: () => {
        space.value = { error: { status: '403', heading: '403 Forbidden', body: 'No access to space' }};
      },
      404: () => {
        space.value = { error: { status: '404', heading: '404 Not Found', body: 'Space not found' }};
      },
      500: () => {
        space.value = { error: { status: '500', heading: '500 Internal Server Error', body: 'There was a problem fetching your account, please reload the page' }};
      }
    };

    apiFetchAuthenticated({ input, statusActions });
  };

  function getError(id: string): string | undefined {
    return inputErrors.value.find(inputError => inputError.id === id)?.message;
  }

  async function postNote() {
    if (space.value.data === undefined) {
      return;
    }

    inputErrors.value = [];

    const tidiedName = tidy(inputNoteName.value);
    const tidiedHandle = tidy(inputNoteHandle.value);

    if (tidiedName === '') {
      inputErrors.value.push({id: 'name', message: 'Note name is required'});
    }

    if (tidiedHandle === '') {
      inputErrors.value.push({id: 'handle', message: 'Note handle is required'});
    }

    if (inputErrors.value.length > 0) {
      nextTick(() => {
        errorSectionRef.value?.focus();
      });

      return;
    }

    status.value = 'pending';

    const input = `spaces/${space.value.data.id}/notes`;

    const statusActions: StatusActions = {
      201: () => {
        status.value = 'success';

        if (space.value.data !== undefined) {
          router.push({ name: 'NoteEdit', params: { spaceId: space.value.data.id, noteId: tidiedHandle } });
        }
      },
      400: (response?: Response) => {
        status.value = 'error';

        response?.json().then((value) => {
          if (value.errors) {
            if ('Name' in value.errors) {
              inputErrors.value = [{id: 'name', message: value.errors['Name'][0]}]
            }

            if ('Handle' in value.errors) {
              inputErrors.value = [{id: 'handle', message: value.errors['Handle'][0]}]
            }

            nextTick(() => {
              errorSectionRef.value?.focus();
            });
          }
        });
      },
      409: () => {
        status.value = 'error';

        inputErrors.value = [{ id: 'handle', message: 'Handle already in use' }]

        nextTick(() => {
          errorSectionRef.value?.focus();
        });
      }
    }

    const body = new FormData();

    body.append('name', tidiedName);
    body.append('handle', tidiedHandle);

    const init = { method: "POST", body: body };

    apiFetchAuthenticated({ input, statusActions, init });
  }
</script>

<template>
  <main role="main" id="main" tabindex="-1">
    <header>
      <h1>Create Note</h1>
    </header>

    <section v-if="space === null">
      <p role="status" class="visually-hidden" aria-live="polite">Please wait while the space information is fetched.</p>
                
      <progress max="100">Fetching space...</progress>
    </section>

    <section v-if="space.error !== undefined">
      <p>
        {{ space.error.body }}
      </p>
    </section>

    <template v-if="space.data !== undefined">
      <section id="errors" v-if="inputErrors.length > 0" ref="errorSectionRef" tabindex="-1" aria-live="assertive" aria-labelledby="heading-errors">
        <h2 id="heading-errors">There was a problem creating your note</h2>
        <ul role="list">
          <li v-for="error in inputErrors" :key="'error' + error.id">
            <a :href="'#input-' + error.id">{{ error.message }}</a>
          </li>
        </ul>
      </section>

      <section>
        <form @submit.prevent="postNote" :action="`/api/spaces/${space.data.id}/notes`" method="POST" novalidate>
          <fieldset>
            <legend class="visually-hidden">Create Note</legend>

            <InputText
              v-model="inputNoteName"
              label="Name"
              name="name"
              id="name"
              :maxlength="64"
              help="The display name of your note"
              :error="getError('name')" />

            <InputText
              v-model="inputNoteHandle"
              label="Handle"
              name="handle"
              id="handle"
              :maxlength="64"
              help="lower case with hyphens (e.g. 'my-anatini-note')"
              :error="getError('handle')" />
          </fieldset>

          <SubmitButton
            :busy="status === 'pending'"
            text="Create"
            busy-text="Creating..." />
        </form>
      </section>

      <p role="status" class="visually-hidden" aria-live="polite">{{ status === 'success' ? 'Created!' : undefined }}</p>
    </template>
  </main>
</template>
