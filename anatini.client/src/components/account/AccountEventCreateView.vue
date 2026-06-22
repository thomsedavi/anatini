<script setup lang="ts">
  import { ref } from 'vue';
  import InputText from '../common/InputText.vue';
  import type { InputError, Status, StatusActions } from '@/types';
  import { tidy } from '../common/utils';
  import SubmitButton from '../common/SubmitButton.vue';
  import { apiFetchAuthenticated } from '../common/apiFetch';
 
  const props = defineProps<{
    status: Status,
    inputErrors: InputError[],
  }>();

  const emit = defineEmits<{
    'update-status': [newStatus: Status],
    'update-errors': [newInputErrors: InputError[]],
  }>();

  const inputEventName = ref<string>('');
  const inputEventStartsAtNz = ref<string>('');

  function getError(id: string): string | undefined {
    return props.inputErrors.find(inputError => inputError.id === id)?.message;
  }

  async function postEvent() {
    emit('update-errors', []);

    const tidiedName = tidy(inputEventName.value);

    emit('update-status', 'pending');

    const statusActions: StatusActions = {
    }

    const body = new FormData();

    body.append('name', tidiedName);
    body.append('startsAtNz', inputEventStartsAtNz.value);

    const init = { method: "POST", body: body };

    apiFetchAuthenticated('account/events', statusActions, init);
  }
</script>

<template>
  <section id="panel-calendar" role="tabpanel" aria-labelledby="tab-calendar">
    <header>
      <h2>Create Event</h2>
    </header>

    <form @submit.prevent="postEvent" :action="`/api/account/events`" method="POST" novalidate>
      <fieldset>
        <legend class="visuallyhidden">Create Event</legend>

        <InputText
          v-model="inputEventName"
          label="Name"
          name="name"
          id="name"
          :maxlength="64"
          help="The name of your event"
          :error="getError('name')" />

        <InputText
          v-model="inputEventStartsAtNz"
          type="datetime-local"
          label="Date & Time (NZ)"
          name="startsAtNz"
          id="startsAtNz"
          help="Event starts here"
          :error="getError('startsAtNz')" />
      </fieldset>

      <SubmitButton
        :busy="status === 'pending'"
        :disabled="tidy(inputEventName) === ''"
        text="Create"
        busy-text="Creating..." />
    </form>
  </section>
</template>
