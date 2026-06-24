<script setup lang="ts">
  import { ref, watch } from 'vue';
  import InputText from '../common/InputText.vue';
  import type { InputError, Status, StatusActions } from '@/types';
  import { tidy } from '../common/utils';
  import SubmitButton from '../common/SubmitButton.vue';
  import { apiFetchAuthenticated } from '../common/apiFetch';
  import InputCheckbox from '../common/InputCheckbox.vue';
 
  const props = defineProps<{
    status: Status,
    inputErrors: InputError[],
  }>();

  const emit = defineEmits<{
    'update-status': [newStatus: Status],
    'update-errors': [newInputErrors: InputError[]],
  }>();

  const inputEventName = ref<string>('');
  const inputEventUrl = ref<string>('');
  const inputEventIsFullDay = ref<boolean>(false);
  const inputEventStartDate = ref<string>('');
  const inputEventStartTime = ref<string>('');
  const inputEventEndDate = ref<string>('');
  const inputEventEndTime = ref<string>('');
  const isEventEndDateDirty = ref<boolean>(false);

  watch(inputEventStartDate, (date) => {
    if (!isEventEndDateDirty.value) {
      inputEventEndDate.value = date;
    }
  });

  function onEventEndDateInput(): void {
    isEventEndDateDirty.value = true;
  }

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
    body.append('startDate', inputEventStartDate.value);

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
        <legend>Event Information</legend>

        <InputText
          v-model="inputEventName"
          label="Name"
          name="name"
          id="name"
          :maxlength="256"
          :required="true"
          help="The name of your event"
          :error="getError('name')" />

        <InputText
          v-model="inputEventUrl"
          label="Link"
          name="url"
          id="url"
          type="url"
          placeholder="https://example.com"
          pattern="https://.*"
          :maxlength="256"
          help="The link to your event"
          :error="getError('name')" />
      </fieldset>

      <fieldset>
        <legend>Date and Time</legend>

        <InputCheckbox
          v-model="inputEventIsFullDay"
          label="This is an all-day event"
          id="is-full-day" />

        <InputText
          v-model="inputEventStartDate"
          type="date"
          label="Start Date"
          name="start-date"
          id="start-date"
          :required="true"
          :error="getError('startDate')" />

        <InputText
          v-model="inputEventStartTime"
          type="time"
          label="Start Time"
          name="start-time"
          id="start-time"
          :required="!inputEventIsFullDay"
          :disabled="inputEventIsFullDay"
          :error="getError('startTime')" />

        <InputText
          v-model="inputEventEndDate"
          type="date"
          label="End Date"
          name="end-date"
          id="end-date"
          :required="true"
          :input="onEventEndDateInput"
          :error="getError('endDate')" />

        <InputText
          v-model="inputEventEndTime"
          type="time"
          label="End Time"
          name="end-time"
          id="end-time"
          :required="!inputEventIsFullDay"
          :disabled="inputEventIsFullDay"
          :error="getError('endTime')" />
      </fieldset>

      <SubmitButton
        :busy="status === 'pending'"
        :disabled="tidy(inputEventName) === ''"
        text="Create"
        busy-text="Creating..." />
    </form>
  </section>
</template>
