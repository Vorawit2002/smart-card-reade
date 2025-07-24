<template>
  <v-app>
    <v-main>
      <v-container>
        <NationalIdCardReader />
      </v-container>
    </v-main>
  </v-app>
</template>

<script setup lang="ts">
import NationalIdCardReader from './components/NationalIdCardReader.vue';
</script>

<style scoped>
/* No specific styles needed for now, Vuetify handles most styling */
</style>
