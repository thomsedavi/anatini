<script setup lang="ts">
  async function postNote() {

  }
</script>

<template>
  <section id="panel-notes" role="tabpanel" aria-labelledby="tab-notes">
    <header>
      <h2>Create Note</h2>
    </header>
    <form @submit.prevent="postNote" :action="`/api/account/notes`" method="POST" novalidate>
    </form>
  </section>
</template>
