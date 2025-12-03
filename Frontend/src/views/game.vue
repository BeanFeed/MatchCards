<script setup lang="js">

  import {useRoute} from "vue-router";
  import {onMounted, ref, watch} from "vue";

  const route = useRoute();

  const gameId = ref(route.params.gameId);

  watch(() => route.params.gameId, (newVal, oldVal) => {
    gameId.value = newVal;
  })

  onMounted(async () => {
    
  })

</script>

<template>

</template>

<style scoped>

</style>