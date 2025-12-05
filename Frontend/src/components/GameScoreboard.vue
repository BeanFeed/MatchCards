<script setup lang="js">

import {computed} from "vue";
import {useUserStore} from "@/stores/user.js";
import {useRouter} from "vue-router";

const props = defineProps({
  gameState: {
    type: Object,
    required: true
  },
});

const router = useRouter();

const userStore = useUserStore();

const isPlayer1 = computed(() => {
  console.log("Game State:", props.gameState)
  return props.gameState.player1.id === userStore.user.id;
});

const playerScore = computed(() => {
  if (isPlayer1.value) {
    return props.gameState.player1Score;
  } else {
    return props.gameState.player2Score;
  }
});

const opponentScore = computed(() => {
  if (isPlayer1.value) {
    return props.gameState.player2Score;
  } else {
    return props.gameState.player1Score;
  }
});

</script>

<template>
  <div class="bg-neutral-900 shadow-glow">
    <div class="px-2 pt-2 grid grid-cols-5 justify-between bar h-min sticky top-0 w-full">
      <div class="w-full">
        <UButton @click="router.push('/')">Home</UButton>
      </div>
      <div class="w-full" v-if="true">
        <div class="flex w-full justify-end">
          <p :class="gameState.currentTurn.id === userStore.user.id ? 'bg-primary-400 text-neutral-900' : 'bg-neutral-800 text-neutral-600'" class="h-min w-max text-center whitespace-nowrap rounded-md px-1">Your Turn</p>
          <p class="text-right whitespace-nowrap pl-1">You &nbsp;&nbsp;{{playerScore}}</p>
        </div>
      </div>
      <div class="w-full">
        <p class="text-center w-full">< Score ></p>
      </div>
      <div class="w-full">
        <div class="flex">
          <p class="w-full text-left whitespace-nowrap pr-1">{{opponentScore}} &nbsp;&nbsp;Opponent</p>
          <p :class="gameState.currentTurn.id !== userStore.user.id ? 'bg-primary-400 text-neutral-900' : 'bg-neutral-800 text-neutral-600'" class="h-min w-max text-center whitespace-nowrap rounded-md px-1">Opponents Turn</p>
        </div>
      </div>
      <div class="w-full">
      </div>
    </div>
    <p class="text-center text-[0.6rem]">Test</p>
  </div>
</template>

<style scoped>
.shadow-glow {
  box-shadow: 0 0 10px 2px rgba(255, 255, 255, 0.2);
}

.bar > div {
  display: flex;
  align-items: center;
}
</style>