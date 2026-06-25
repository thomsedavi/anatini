<script setup lang="ts">
  import { ref, watch } from 'vue';
  import InputText from '../common/InputText.vue';
  import type { InputError, Status, StatusActions, Visibility } from '@/types';
  import { tidy } from '../common/utils';
  import SubmitButton from '../common/SubmitButton.vue';
  import { apiFetchAuthenticated } from '../common/apiFetch';
  import InputCheckbox from '../common/InputCheckbox.vue';
  import InputSelect from '../common/InputSelect.vue';
  import VisibilitySelect from '../common/VisibilitySelect.vue';
 
  const props = defineProps<{
    status: Status,
    inputErrors: InputError[],
  }>();

  const emit = defineEmits<{
    'update-status': [newStatus: Status],
    'update-errors': [newInputErrors: InputError[]],
  }>();

  const frequencyOptions = ref([
    { text: 'Daily', value: 'Daily' },
    { text: 'Weekly', value: 'Weekly' },
    { text: 'Monthly', value: 'Monthly' },
    { text: 'Yearly', value: 'Yearly' }
  ]);

  const inputEventName = ref<string>('');
  const inputEventUrl = ref<string>('');
  const inputEventIsFullDay = ref<boolean>(false);
  const inputEventIsRecurring = ref<boolean>(false);
  const inputEventStartDate = ref<string>('');
  const inputEventStartTime = ref<string>('');
  const inputEventEndDate = ref<string>('');
  const inputEventEndTime = ref<string>('');
  const inputEventFrequency = ref<string>('Weekly');
  const inputEventInterval = ref<number>(1);
  const inputEventUntil = ref<string>('');
  const inputEventHandle = ref<string>('');
  const isEventEndDateDirty = ref<boolean>(false);
  const inputEventIsDraft = ref<boolean>(false);
  const inputEventVisibility = ref<Visibility>('Public');

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

    const inputErrors: InputError[] = [];

    if (tidiedName === '') {
      inputErrors.push({ id: 'name', message: 'Name is required' });
    }

    if (inputEventStartDate.value === '') {
      inputErrors.push({ id: 'startDate', message: 'Start date is required' });
    }

    if (inputEventEndDate.value === '') {
      inputErrors.push({ id: 'endDate', message: 'End date is required' });
    }

    if (!inputEventIsFullDay.value) {
      if (inputEventStartTime.value === '') {
        inputErrors.push({ id: 'startTime', message: 'Start time is required' });
      }

      if (inputEventEndTime.value === '') {
        inputErrors.push({ id: 'endTime', message: 'End time is required' });
      }
    }

    if (inputErrors.length > 0) {
      emit('update-errors', inputErrors);

      return;
    }

    emit('update-status', 'pending');

    const statusActions: StatusActions = {
    }

    const body = new FormData();

    body.append('name', tidiedName);
    body.append('startDate', inputEventStartDate.value);
    body.append('endDate', inputEventEndDate.value);
    body.append('visibility', inputEventVisibility.value);

    if (tidy(inputEventHandle.value) !== '') {
      body.append('handle', tidy(inputEventHandle.value));
    }

    if (tidy(inputEventUrl.value) !== '') {
      body.append('url', tidy(inputEventUrl.value));
    }

    if (inputEventIsDraft.value === true) {
      body.append('isDraft', 'true');
    }

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
          help="The external link to your event (e.g. a ticket booking site). For recurring events, this link will be applied to each event but can be modified per event."
          :error="getError('url')" />

        <InputText
          v-model="inputEventHandle"
          label="Handle"
          name="handle"
          id="handle"
          :maxlength="64"
          help="lower case with hyphens (e.g. 'my-anatini-event'), optional custom web address"
          :error="getError('handle')" />

        <VisibilitySelect v-model="inputEventVisibility" />

        <InputCheckbox
          v-model="inputEventIsDraft"
          label="Save As Draft"
          id="is-draft"
          name="is-draft"
          help="If event is recurring, this enables you to modify individual events before publishing event." />
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

      <fieldset>
        <legend>Recurrence</legend>

        <InputCheckbox
          v-model="inputEventIsRecurring"
          label="This event repeats"
          id="is-recurring" />

        <!-- TODO handle requied?-->
        <InputSelect
          :options="frequencyOptions"
          label="Frequency"
          name="frequency"
          id="frequency"
          :disabled="!inputEventIsRecurring"
          v-model="inputEventFrequency" />

        <InputText
          type="number"
          v-model="inputEventInterval"
          label="Repeats every"
          name="interval"
          id="interval"
          min="1"
          :disabled="!inputEventIsRecurring"
          :required="inputEventIsRecurring"
          help="Every X Days/Weeks/Months/Years" />

        <InputText
          type="date"
          label="Ends On"
          name="until"
          id="until"
          :disabled="!inputEventIsRecurring"
          :required="inputEventIsRecurring"
          v-model="inputEventUntil"
          help="Optional, leave blank if event has no end point" />
      </fieldset>

      <SubmitButton
        :busy="status === 'pending'"
        :disabled="tidy(inputEventName) === ''"
        text="Create"
        busy-text="Creating..." />
    </form>
  </section>
</template>
