<script setup lang="ts">
  import { ref, watch } from 'vue';
  import { apiFetchAuthenticated } from '@/common/apiFetch';
  import type { InputError, Status, StatusActions, Visibility } from '@/common/types';
  import { formatParagraphs, tidy } from '@/common/utils';
  import SubmitButton from '@/common/SubmitButton.vue';
  import InputText from '@/common/InputText.vue';
  import InputTextArea from '@/common/InputTextArea.vue';
  import VisibilitySelect from '@/common/VisibilitySelect.vue';

  const props = defineProps<{
    dataSpaceId: string,
    dataSpaceHandle: string,
    dataStatus: Status,
    dataInputErrors: InputError[],
  }>();

  const inputTitle = ref<string>('');
  const inputBody = ref<string>('');
  const inputType = ref<'FixedTime' | 'AllDay'>('FixedTime');
  const inputStartDate = ref<string>('');
  const inputStartTime = ref<string>('');
  const inputEndDate = ref<string>('');
  const inputEndTime = ref<string>('');
  const isEndDateDirty = ref<boolean>(false);
  const inputVisibility = ref<Visibility>('Public');

  watch(inputStartDate, (date) => {
    if (!isEndDateDirty.value) {
      inputEndDate.value = date;
    }
  });

  function onEventEndDateInput(): void {
    isEndDateDirty.value = true;
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

    if (tidy(inputBody.value) === '') {
      inputErrors.push({ id: 'body', message: 'Content is required' });
    }

    if (inputEndDate.value === '') {
      inputErrors.push({ id: 'endDate', message: 'End date is required' });
    }

    if (inputType.value !== 'AllDay' && inputStartTime.value === '') {
      inputErrors.push({ id: 'startTime', message: 'Start time is required' });
    }

    if (inputType.value === 'FixedTime' && inputEndTime.value === '') {
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
    body.append('body', formatParagraphs(inputBody.value));

    let startsAtNz = `${inputStartDate.value}T`;

    if (inputType.value === 'AllDay') {
      startsAtNz += '00:00:00';
    }
    else {
      startsAtNz += `${inputStartTime.value}:00`;
    }

    body.append('startsAtNz', startsAtNz);

    let endsAtNz = `${inputEndDate.value}T`;

    if (inputType.value === 'AllDay') {
      const [y, m, d] = inputEndDate.value.split('-').map(Number);
      const date = new Date(Date.UTC(y, m - 1, d));

      date.setUTCDate(date.getUTCDate() + 1);

      const result = date.toISOString().split('T')[0];

      endsAtNz = `${result}T00:00:00`;
    }
    else {
      endsAtNz = `${inputEndDate.value}T${inputEndTime.value}:00`;
    }

    body.append('endsAtNz', endsAtNz);

    body.append('visibility', inputVisibility.value);

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

      <InputTextArea
        v-model="inputBody"
        label="Content"
        name="body"
        id="body"
        :maxLength="512"
        :error="getError('body')"
        :isArticle="true"
        help="This is your event. Asterisks allow for *emphasis* and **strong text**." />

      <VisibilitySelect v-model="inputVisibility" />

      <input type="radio" id="fixed-time" value="FixedTime" v-model="inputType" />
      <label for="fixed-time">Fixed From/To Time</label>

      <input type="radio" id="all-day" value="AllDay" v-model="inputType" />
      <label for="all-day">All Day</label>

      <InputText
        v-model="inputStartDate"
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
        v-model="inputStartTime"
        type="time"
        label="Start Time"
        name="start-time"
        id="start-time"
        :required="inputType !== 'AllDay'"
        :disabled="inputType === 'AllDay'"
        :hidden="inputType === 'AllDay'"
        :error="getError('startTime')" />

      <InputText
        v-model="inputEndDate"
        type="date"
        label="End Date"
        name="end-date"
        id="end-date"
        :required="true"
        :input="onEventEndDateInput"
        :error="getError('endDate')" />

      <InputText
        v-model="inputEndTime"
        type="time"
        label="End Time"
        name="end-time"
        id="end-time"
        :required="inputType === 'FixedTime'"
        :disabled="inputType !== 'FixedTime'"
        :hidden="inputType !== 'FixedTime'"
        :error="getError('endTime')" />

      <SubmitButton
        :busy="dataStatus === 'pending'"
        text="Create"
        busy-text="Creating..." />
    </form>
  </section>
</template>
