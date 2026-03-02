<script setup lang="ts">
  import { ref } from 'vue';
  import { apiFetchAuthenticated } from '../common/apiFetch';
  import { tidy } from '../common/utils';
  import type { StatusActions } from '@/types';

  const props = defineProps<{
    channelId: string,
    postId: string,
    pageStatus: string,
    name: string,
    dateNZ: string,
    eTag: string | null,
  }>();

  const emit = defineEmits<{
    'update-etag': [eTag: string | null],
    'update-page-status': [newPageStatus: string],
    'update-name': [newName: string],
    'update-date-nz': [newDateNZ: string],
  }>();

  const inputDateNZ = ref<string>(props.dateNZ);
  const inputName = ref<string>(props.name);

  function patchPostDetail(): void {
    if (props.eTag === null) {
      return;
    }

    emit('update-page-status', 'Updating...');

    const body = new FormData();

    if (inputName.value !== props.name) {
      body.append('name', tidy(inputName.value));
    }

    if (inputDateNZ.value !== props.dateNZ) {
      body.append('dateNZ', inputDateNZ.value);
    }

    const init = { method: "PATCH", headers: { "If-Match": props.eTag }, body: body };

    const statusActions: StatusActions = {
      204: (response?: Response) => {
        emit('update-etag', response?.headers.get("ETag") ?? null);
        emit('update-name', tidy(inputName.value));
        emit('update-date-nz', inputDateNZ.value);
        emit('update-page-status', 'Ready');
      }
    }

    apiFetchAuthenticated(`channels/${props.channelId}/posts/${props.postId}`, statusActions, init);
  }

  function detailChanged(): boolean {
    if (tidy(inputName.value) !== props.name) {
      return true;
    } else if (inputDateNZ.value !== props.dateNZ) {
      return true;
    }

    return false;
  }
</script>

<template>
  <section id="panel-details" role="tabpanel" aria-labelledby="tab-details">
    <header>
      <h2>Details</h2>
    </header>

    <form @submit.prevent="patchPostDetail" :action="`/api/channels/${channelId}/posts/${postId}`" method="POST" novalidate>
      <fieldset>
        <legend class="visuallyhidden">Post Details</legend>

        <template v-if="inputName !== null">
          <InputText
            v-model="inputName"
            label="Name"
            name="name"
            id="name-post"
            :maxlength="64"
            :error="undefined"
            help="Article name"
            :required="true" />
        </template>

        <template v-if="inputDateNZ !== null">
          <label for="input-date-post">Publication Date</label>
          <input 
            type="date" 
            v-model="inputDateNZ"
            id="input-date-post" 
            name="date"
            aria-describedby="help-date-post"
            :aria-disabled="pageStatus !== 'Ready' ? true : undefined"
            required >

          <small id="help-date-post">
            Articles dated in the future will not be visible until that date is reached
          </small>
        </template>
      </fieldset>

      <SubmitButton
        :busy="pageStatus === 'Updating...'"
        :disabled="detailChanged() === false"
        text="Update"
        busy-text="Updating..." />
    </form>
  </section>
</template>
