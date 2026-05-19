<script setup lang="ts">
  const model = defineModel<string>();

  defineProps<{
    legend: string,
    radios: { label: string, value: string, name: string, id: string }[]
  }>();
</script>

<template>
  <fieldset>
    <legend class="visuallyhidden">{{ legend }}</legend>

    <ul role="list">
      <li v-for="radio in radios" :key="`radio-${radio.name}-${radio.value}`">
        <input type="radio" :id="radio.id" :name="radio.name" :value="radio.value" v-model="model" />
        <label :for="radio.id">{{ radio.label }}</label>
      </li>
    </ul>
  </fieldset>
</template>
