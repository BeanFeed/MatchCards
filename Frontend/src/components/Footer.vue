<script setup lang="js">

import {prompts} from "@/config.json";

import {useRoute} from "vue-router";
import {computed} from "vue";

const route = useRoute();

const page = route.name;

const prompt = computed(() => {
  switch (page){
    case 'index':
      return 0;
    case 'lobby':
      return 1;
    case 'game':
      return 2;
    case 'scoreboard':
      return 3;
    default:
      return 0;
  }
})

</script>

<template>
<div class="z-[-1] absolute bottom-0 left-0 w-full h-min px-4 pb-4 ">
  <p v-if="prompt == 0" class="text-xs">This probably still has bugs. If it breaks, try refreshing :)</p>
  <p class="font-bold">Prompt {{prompt + 1}}</p>
  <p class="text-xs">{{prompts[prompt]}}</p>
</div>
</template>

<style scoped>

</style>