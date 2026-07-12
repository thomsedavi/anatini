<script setup lang="ts">
  import type { InputError, Status, StatusActions, Visibility } from '@/types';
  import { nextTick, ref } from 'vue';
  import InputText from './common/InputText.vue';
  import SubmitButton from './common/SubmitButton.vue';
  import { tidy } from './common/utils.ts';
  import { apiFetchAuthenticated } from './common/apiFetch.ts';
  import { useRouter } from 'vue-router';
  import VisibilitySelect from './common/VisibilitySelect.vue';

  const router = useRouter();

  const inputErrors = ref<InputError[]>([]);
  const inputSpaceName = ref<string>('');
  const inputSpaceHandle = ref<string>('');
  const inputVisibility = ref<Visibility>('Public');
  const status = ref<Status>('idle');
  const errorSectionRef = ref<HTMLElement | null>(null);

  function getError(id: string): string | undefined {
    return inputErrors.value.find(inputError => inputError.id === id)?.message;
  }

  async function postSpace() {
    inputErrors.value = [];

    const tidiedName = tidy(inputSpaceName.value);
    const tidiedHandle = tidy(inputSpaceHandle.value);

    if (tidiedName === '') {
      inputErrors.value.push({id: 'name', message: 'Space name is required'});
    }

    if (tidiedHandle === '') {
      inputErrors.value.push({id: 'handle', message: 'Space handle is required'});
    }

    if (inputErrors.value.length > 0) {
      nextTick(() => {
        errorSectionRef.value?.focus();
      });

      return;
    }

    status.value = 'pending';

    const statusActions: StatusActions = {
      201: () => {
        status.value = 'success';

        router.push({ name: 'SpaceEdit', params: { spaceId: tidiedHandle } });
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
    body.append('visibility', inputVisibility.value);

    const init = { method: "POST", body: body };

    apiFetchAuthenticated('spaces', statusActions, init);
  }
</script>

<template>
  <main id="main" tabindex="-1">
    <header>
      <h1>Create Space</h1>
    </header>

    <section id="errors" v-if="inputErrors.length > 0" ref="errorSectionRef" tabindex="-1" aria-live="assertive" aria-labelledby="heading-errors">
      <h2 id="heading-errors">There was a problem creating your space</h2>
      <ul role="list">
        <li v-for="error in inputErrors" :key="'error' + error.id">
          <a :href="'#input-' + error.id">{{ error.message }}</a>
        </li>
      </ul>
    </section>

    <form @submit.prevent="postSpace" action="/api/spaces" method="POST" novalidate>
      <fieldset>
        <legend class="visuallyhidden">Create Space</legend>

        <InputText
          v-model="inputSpaceName"
          label="Name"
          name="name"
          id="name"
          :maxlength="64"
          help="The display name of your space"
          :error="getError('name')" />

        <InputText
          v-model="inputSpaceHandle"
          label="Handle"
          name="handle"
          id="handle"
          :maxlength="64"
          help="lower case with hyphens (e.g. 'my-anatini-space')"
          :error="getError('handle')" />

        <VisibilitySelect v-model="inputVisibility" />
      </fieldset>

      <SubmitButton
        :busy="status === 'pending'"
        text="Create"
        busy-text="Creating..." />
    </form>

    <p role="status" class="visuallyhidden" aria-live="polite">{{ status === 'success' ? 'Created!' : undefined }}</p>
  </main>
</template>
