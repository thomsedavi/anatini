<script setup lang="ts">
</script>

<template>
  <section id="panel-notes" role="tabpanel" aria-labelledby="tab-notes">
    <header>
      <h2>Notes</h2>
      <RouterLink :to="{ name: 'AccountNoteCreate' }">+ Create Note</RouterLink>
    </header>
  </section>
</template>
