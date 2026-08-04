<script setup lang="ts">
  import { ref, watch } from 'vue';
  import InputText from '../common/InputText.vue';
  import type { InputError, SelectOption, Status, StatusActions, Visibility } from '@/types';
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
    'update-page-status': [newPageStatus: string],
    'update-errors': [newInputErrors: InputError[]],
  }>();

  const frequencyOptions = ref([
    { text: 'Daily', value: 'Daily' },
    { text: 'Weekly', value: 'Weekly' },
    { text: 'Monthly', value: 'Monthly' },
    { text: 'Yearly', value: 'Yearly' }
  ]);

  const endsOptions = ref([
    { text: 'Never', value: 'Never' },
    { text: 'After', value: 'After' },
    { text: 'On Date', value: 'OnDate' }
  ]);

  const monthlyRuleOptions = ref<SelectOption[]>([{ value: '', text: 'Please select a date first', disabled: true }]);

  const inputEventName = ref<string>('');
  const inputEventUrl = ref<string>('');
  const inputEventType = ref<'FixedTime' | 'AllDay' | 'Duration'>('FixedTime');
  const inputEventIsRecurring = ref<boolean>(false);
  const inputEventStartDate = ref<string>('');
  const inputEventStartTime = ref<string>('');
  const inputEventEndDate = ref<string>('');
  const inputEventEndTime = ref<string>('');
  const inputEventDays = ref<number>(0);
  const inputEventHours = ref<number>(1);
  const inputEventMinutes = ref<number>(0);
  const inputEventMonday = ref<boolean>(false);
  const inputEventTuesday = ref<boolean>(false);
  const inputEventWednesday = ref<boolean>(false);
  const inputEventThursday = ref<boolean>(false);
  const inputEventFriday = ref<boolean>(false);
  const inputEventSaturday = ref<boolean>(false);
  const inputEventSunday = ref<boolean>(false);
  const inputEventOccurrences = ref<number>(1);
  const inputEventFrequency = ref<'Daily' | 'Weekly' | 'Monthly' | 'Yearly'>('Weekly');
  const inputEventEnds = ref<'Never' | 'After' | 'OnDate'>('Never');
  const inputMonthlyRule = ref<string>('');
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

  function onEventStartDateChange(payload: Event): void {
    const dateValue = (payload.target as HTMLInputElement).value;

    if (dateValue === '') {
      monthlyRuleOptions.value = [{ value: '', text: 'Please select a date first', disabled: true }];
      inputMonthlyRule.value = '';

      emit('update-page-status', 'Date cleared. Monthly recurring rule options disabled.');

      return;
    }

    const newMonthlyOptions: SelectOption[] = [
      { value:'', text: 'Choose a monthly...', disabled: true }
    ];

    const date = new Date(dateValue);
    const dayAbbreviation = ['SU', 'MO', 'TU', 'WE', 'TH', 'FR', 'SA'][date.getDay()];
    const dayDescription = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'][date.getDay()];
    const dayOfMonth = date.getDate();
    const occurrence = Math.ceil(dayOfMonth / 7);
    const monthDaysCount = new Date(date.getFullYear(), date.getMonth() + 1, 0).getDate();

    newMonthlyOptions.push({ value: `BYMONTHDAY=${dayOfMonth}`, text: `On the ${dayOfMonth} of the month` });

    if (occurrence <= 4) {
      const occurrenceDescription = ['first', 'second', 'third', 'fourth'][occurrence - 1];

      newMonthlyOptions.push({ value: `BYDAY=${occurrence}${dayAbbreviation}`, text: `On the ${occurrenceDescription} ${dayDescription} of the month` });
    }

    if ((monthDaysCount - dayOfMonth) <= 6) {
      newMonthlyOptions.push({ value: `BYDAY=-1${dayAbbreviation}`, text: `On the last ${dayDescription} of the month` });
    }

    monthlyRuleOptions.value = newMonthlyOptions;
    inputMonthlyRule.value = '';

    emit('update-page-status', 'Monthly recurring rule options updated based on the selected date.');
  }

  function getError(id: string): string | undefined {
    return props.inputErrors.find(inputError => inputError.id === id)?.message;
  }

  function getByDays(): string[] {
    const byDays: string[] = [];

    if (inputEventMonday.value === true) byDays.push('MO');
    if (inputEventTuesday.value === true) byDays.push('TU');
    if (inputEventWednesday.value === true) byDays.push('WE');
    if (inputEventThursday.value === true) byDays.push('TH');
    if (inputEventFriday.value === true) byDays.push('FR');
    if (inputEventSaturday.value === true) byDays.push('SA');
    if (inputEventSunday.value === true) byDays.push('SU');

    return byDays;
  }

  async function postEvent() {
    emit('update-errors', []);

    const tidiedName = tidy(inputEventName.value);
    const byDays = getByDays();

    const inputErrors: InputError[] = [];

    if (tidiedName === '') {
      inputErrors.push({ id: 'name', message: 'Name is required' });
    }

    if (inputEventStartDate.value === '') {
      inputErrors.push({ id: 'startDate', message: 'Start date is required' });
    }

    if (inputEventType.value !== 'Duration' && inputEventEndDate.value === '') {
      inputErrors.push({ id: 'endDate', message: 'End date is required' });
    }

    if (inputEventType.value !== 'AllDay' && inputEventStartTime.value === '') {
      inputErrors.push({ id: 'startTime', message: 'Start time is required' });
    }

    if (inputEventType.value === 'FixedTime' && inputEventEndTime.value === '') {
      inputErrors.push({ id: 'endTime', message: 'End time is required' });
    }

    if (inputEventIsRecurring.value === true && inputEventFrequency.value === 'Weekly' && byDays.length === 0) {
      inputErrors.push({ id: 'frequency', message: 'Some weekdays are required' });
    }

    if (inputEventIsRecurring.value === true && inputEventFrequency.value === 'Monthly' && inputMonthlyRule.value === '') {
      inputErrors.push({ id: 'monthly', message: 'Some monthly are required' });
    }

    if (inputErrors.length > 0) {
      emit('update-errors', inputErrors);

      return;
    }

    emit('update-status', 'pending');

    const input = 'account/events';

    const statusActions: StatusActions = {
      201: () => {
        emit('update-status', 'success');

        console.log('Handle thing');
      },
      400: () => {
        emit('update-status', 'error');
      },
      500: () => {
        emit('update-status', 'error');
      }
    }

    const body = new FormData();

    body.append('name', tidiedName);

    let startsAtNz = `${inputEventStartDate.value}T`;

    if (inputEventType.value === 'AllDay') {
      startsAtNz += '00:00:00';
    }
    else {
      startsAtNz += `${inputEventStartTime.value}:00`;
    }

    body.append('startsAtNz', startsAtNz);

    if (inputEventType.value !== 'Duration') {
      let endsAtNz = `${inputEventEndDate.value}T`;

      if (inputEventType.value === 'AllDay') {
        const [y, m, d] = inputEventEndDate.value.split('-').map(Number);
        const date = new Date(Date.UTC(y, m - 1, d));

        date.setUTCDate(date.getUTCDate() + 1);

        const result = date.toISOString().split('T')[0];

        endsAtNz = `${result}T00:00:00`;
      }
      else {
        endsAtNz = `${inputEventEndDate.value}T${inputEventEndTime.value}:00`;
      }

      body.append('endsAtNz', endsAtNz);
    } else {
      const duration = `${String(inputEventDays.value).padStart(2, '0')}.${String(inputEventHours.value).padStart(2, '0')}:${String(inputEventMinutes.value).padStart(2, '0')}:00`;

      body.append('duration', duration);
    }

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

    if (inputEventIsRecurring.value) {
      let recurrenceRule = `FREQ=${inputEventFrequency.value.toUpperCase()}`

      const intervalFloored = Math.floor(inputEventInterval.value);

      if (inputEventFrequency.value !== 'Yearly' && intervalFloored !== 1) {
        recurrenceRule += `;INTERVAL=${intervalFloored}`;
      }

      if (inputEventEnds.value === 'After') {
        recurrenceRule += `;COUNT=${Math.floor(inputEventOccurrences.value)}`;
      } else if (inputEventEnds.value === 'OnDate') {
        recurrenceRule += `;UNTIL=${inputEventUntil.value.replace(/-/g, '')}`;
      }

      if (inputEventFrequency.value === 'Weekly') {
        recurrenceRule += `;BYDAY=${byDays.join(',')}`;
      } else if (inputEventFrequency.value === 'Monthly') {
        recurrenceRule += `;${inputMonthlyRule.value}`;
      }

      body.append('recurrenceRule', recurrenceRule);
    }

    const init = { method: "POST", body: body };

    apiFetchAuthenticated({ input, statusActions, init });
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
          :maxlength="255"
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

        <input type="radio" id="fixed-time" value="FixedTime" v-model="inputEventType" />
        <label for="fixed-time">Fixed From/To Time</label>

        <input type="radio" id="all-day" value="AllDay" v-model="inputEventType" />
        <label for="all-day">All Day</label>

        <input type="radio" id="duration" value="Duration" v-model="inputEventType" />
        <label for="duration">Duration</label>

        <InputText
          v-model="inputEventStartDate"
          type="date"
          label="Start Date"
          name="start-date"
          id="start-date"
          helpclass="visuallyhidden"
          help="Selecting a date will update the recurring rule options."
          controls="input-monthly"
          :required="true"
          :change="onEventStartDateChange"
          :error="getError('startDate')" />

        <InputText
          v-model="inputEventStartTime"
          type="time"
          label="Start Time"
          name="start-time"
          id="start-time"
          :required="inputEventType !== 'AllDay'"
          :disabled="inputEventType === 'AllDay'"
          :hidden="inputEventType === 'AllDay'"
          :error="getError('startTime')" />

        <InputText
          v-model="inputEventEndDate"
          type="date"
          label="End Date"
          name="end-date"
          id="end-date"
          :required="inputEventType !== 'Duration'"
          :disabled="inputEventType === 'Duration'"
          :hidden="inputEventType === 'Duration'"
          :input="onEventEndDateInput"
          :error="getError('endDate')" />

        <InputText
          v-model="inputEventEndTime"
          type="time"
          label="End Time"
          name="end-time"
          id="end-time"
          :required="inputEventType === 'FixedTime'"
          :disabled="inputEventType !== 'FixedTime'"
          :hidden="inputEventType !== 'FixedTime'"
          :error="getError('endTime')" />

        <InputText
          v-model="inputEventDays"
          type="number"
          label="Days"
          name="days"
          id="days"
          min="0"
          :required="inputEventType === 'Duration'"
          :disabled="inputEventType !== 'Duration'"
          :hidden="inputEventType !== 'Duration'" />

        <InputText
          v-model="inputEventHours"
          type="number"
          label="Hours"
          name="hours"
          id="hours"
          min="0"
          max="23"
          :required="inputEventType === 'Duration'"
          :disabled="inputEventType !== 'Duration'"
          :hidden="inputEventType !== 'Duration'" />

        <InputText
          v-model="inputEventMinutes"
          type="number"
          label="Minutes"
          name="minutes"
          id="minutes"
          min="0"
          max="59"
          :required="inputEventType === 'Duration'"
          :disabled="inputEventType !== 'Duration'"
          :hidden="inputEventType !== 'Duration'" />
      </fieldset>

      <fieldset>
        <legend>Recurrence</legend>

        <InputCheckbox
          v-model="inputEventIsRecurring"
          label="This event repeats"
          id="is-recurring" />

        <InputSelect
          :options="frequencyOptions"
          label="Frequency"
          name="frequency"
          id="frequency"
          :disabled="!inputEventIsRecurring"
          :hidden="!inputEventIsRecurring"
          v-model="inputEventFrequency" />

        <InputText
          type="number"
          v-model="inputEventInterval"
          label="Repeats Every"
          name="interval"
          id="interval"
          min="1"
          :disabled="!inputEventIsRecurring || inputEventFrequency === 'Yearly'"
          :hidden="!inputEventIsRecurring || inputEventFrequency === 'Yearly'"
          :required="inputEventIsRecurring && inputEventFrequency !== 'Yearly'"
          help="Every X Days/Weeks/Months/Years" />

        <InputCheckbox
          v-model="inputEventMonday"
          id="monday"
          name="monday"
          :disabled="!inputEventIsRecurring || inputEventFrequency !== 'Weekly'"
          :hidden="!inputEventIsRecurring || inputEventFrequency !== 'Weekly'"
          label="Monday" />

        <InputCheckbox
          v-model="inputEventTuesday"
          id="tuesday"
          name="tuesday"
          :disabled="!inputEventIsRecurring || inputEventFrequency !== 'Weekly'"
          :hidden="!inputEventIsRecurring || inputEventFrequency !== 'Weekly'"
          label="Tuesday" />

        <InputCheckbox
          v-model="inputEventWednesday"
          id="wednesday"
          name="wednesday"
          :disabled="!inputEventIsRecurring || inputEventFrequency !== 'Weekly'"
          :hidden="!inputEventIsRecurring || inputEventFrequency !== 'Weekly'"
          label="Wednesday" />

        <InputCheckbox
          v-model="inputEventThursday"
          id="thursday"
          name="thursday"
          :disabled="!inputEventIsRecurring || inputEventFrequency !== 'Weekly'"
          :hidden="!inputEventIsRecurring || inputEventFrequency !== 'Weekly'"
          label="Thursday" />

        <InputCheckbox
          v-model="inputEventFriday"
          id="friday"
          name="friday"
          :disabled="!inputEventIsRecurring || inputEventFrequency !== 'Weekly'"
          :hidden="!inputEventIsRecurring || inputEventFrequency !== 'Weekly'"
          label="Friday" />

        <InputCheckbox
          v-model="inputEventSaturday"
          id="saturday"
          name="saturday"
          :disabled="!inputEventIsRecurring || inputEventFrequency !== 'Weekly'"
          :hidden="!inputEventIsRecurring || inputEventFrequency !== 'Weekly'"
          label="Saturday" />

        <InputCheckbox
          v-model="inputEventSunday"
          id="sunday"
          name="sunday"
          :disabled="!inputEventIsRecurring || inputEventFrequency !== 'Weekly'"
          :hidden="!inputEventIsRecurring || inputEventFrequency !== 'Weekly'"
          label="Sunday" />

        <InputSelect
          :options="monthlyRuleOptions"
          label="Monthly"
          name="monthly"
          id="monthly"
          :disabled="!inputEventIsRecurring || inputEventFrequency !== 'Monthly'"
          :hidden="!inputEventIsRecurring || inputEventFrequency !== 'Monthly'"
          v-model="inputMonthlyRule"
          :error="getError('monthly')" />

        <InputSelect
          :options="endsOptions"
          label="Ends"
          name="ends"
          id="ends"
          :disabled="!inputEventIsRecurring"
          :hidden="!inputEventIsRecurring"
          v-model="inputEventEnds" />

        <InputText
          type="number"
          v-model="inputEventOccurrences"
          label="Occurrences"
          name="occurrences"
          id="occurrences"
          min="1"
          :disabled="inputEventEnds !== 'After'"
          :hidden="inputEventEnds !== 'After'" />

        <InputText
          type="date"
          label="Ends On"
          name="until"
          id="until"
          :disabled="inputEventEnds !== 'OnDate'"
          :hidden="inputEventEnds !== 'OnDate'"
          :required="inputEventEnds === 'OnDate'"
          v-model="inputEventUntil"
          help="Optional, leave blank if event has no end point" />
      </fieldset>

      <SubmitButton
        :busy="status === 'pending'"
        text="Create"
        busy-text="Creating..." />
    </form>
  </section>
</template>
