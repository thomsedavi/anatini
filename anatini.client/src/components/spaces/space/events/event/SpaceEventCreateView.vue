<script setup lang="ts">
  import { ref, watch } from 'vue';
  import { apiFetchAuthenticated } from '@/common/apiFetch';
  import type { InputError, Status, StatusActions, Visibility } from '@/common/types';
  import { tidy } from '@/common/utils';
  import SubmitButton from '@/common/SubmitButton.vue';
  import InputText from '@/common/InputText.vue';
  import VisibilitySelect from '@/common/VisibilitySelect.vue';

  const props = defineProps<{
    dataSpaceId: string,
    dataSpaceHandle: string,
    dataStatus: Status,
    dataInputErrors: InputError[],
  }>();

  const inputTitle = ref<string>('');
  const inputEventType = ref<'FixedTime' | 'AllDay'>('FixedTime');
  const inputEventStartDate = ref<string>('');
  const inputEventStartTime = ref<string>('');
  const inputEventEndDate = ref<string>('');
  const inputEventEndTime = ref<string>('');
  const isEventEndDateDirty = ref<boolean>(false);
  const inputEventVisibility = ref<Visibility>('Public');

  watch(inputEventStartDate, (date) => {
    if (!isEventEndDateDirty.value) {
      inputEventEndDate.value = date;
    }
  });

  function onEventEndDateInput(): void {
    isEventEndDateDirty.value = true;
  }

  const emit = defineEmits<{
    'update-status': [newStatus: Status],
    'update-errors': [newInputErrors: InputError[]],
  }>();

  function getError(id: string): string | undefined {
    return props.dataInputErrors.find(inputError => inputError.id === id)?.message;
  }

  async function postEvent() {
    emit('update-errors', []);

    const tidiedTitle = tidy(inputTitle.value);

    const inputErrors: InputError[] = [];

    if (tidiedTitle === '') {
      inputErrors.push({ id: 'title', message: 'Title is required' });
    }

    if (inputEventEndDate.value === '') {
      inputErrors.push({ id: 'endDate', message: 'End date is required' });
    }

    if (inputEventType.value !== 'AllDay' && inputEventStartTime.value === '') {
      inputErrors.push({ id: 'startTime', message: 'Start time is required' });
    }

    if (inputEventType.value === 'FixedTime' && inputEventEndTime.value === '') {
      inputErrors.push({ id: 'endTime', message: 'End time is required' });
    }

    if (inputErrors.length > 0) {
      emit('update-errors', inputErrors);

      return;
    }

    emit('update-status', 'pending');

    const input = `spaces/${props.dataSpaceId}/events`;

    const statusActions: StatusActions = {
    }

    const body = new FormData();

    body.append('title', tidiedTitle);

        let startsAtNz = `${inputEventStartDate.value}T`;

    if (inputEventType.value === 'AllDay') {
      startsAtNz += '00:00:00';
    }
    else {
      startsAtNz += `${inputEventStartTime.value}:00`;
    }

    body.append('startsAtNz', startsAtNz);

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

    body.append('visibility', inputEventVisibility.value);

    const init = { method: "POST", body: body };

    apiFetchAuthenticated({ input, statusActions, init });
  }
</script>

<template>
  <section id="panel-events" role="tabpanel" aria-labelledby="tab-events">
    <header>
      <h2>Create Event</h2>
    </header>

    <form @submit.prevent="postEvent" :action="`/api/spaces/${dataSpaceId}/events`" method="POST" novalidate>
      <InputText
        v-model="inputTitle"
        label="Title"
        name="title"
        id="title"
        :maxlength="256"
        :required="true"
        help="The title of your work"
        :error="getError('title')" />

      <VisibilitySelect v-model="inputEventVisibility" />

      <input type="radio" id="fixed-time" value="FixedTime" v-model="inputEventType" />
      <label for="fixed-time">Fixed From/To Time</label>

      <input type="radio" id="all-day" value="AllDay" v-model="inputEventType" />
      <label for="all-day">All Day</label>

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
        :required="true"
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

      <SubmitButton
        :busy="dataStatus === 'pending'"
        text="Create"
        busy-text="Creating..." />
    </form>
  </section>
</template>
