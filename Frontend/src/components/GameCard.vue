<script setup lang="js">

  import {computed} from "vue";
  import {useUserStore} from "@/stores/user.js";

  const props = defineProps({
    game: {
      type: Object,
      required: true
    }
  });

  const userStore = useUserStore();

  const isPlayer1 = computed(() => {
    return props.game.player1.id === userStore.user.id;
  });

</script>

<template>
  <div class="w-max">
    <UCard>
      <p v-if="!game.isSinglePlayer">Opponent: {{computed(() => {
        if(isPlayer1) return game.player2.name;
        else return game.player1.name;
      })}}</p>
      <p v-if="!game.isSinglePlayer">Score: {{computed(() => {
        if(isPlayer1) return "(You) " + game.player1Score + " - " + game.player2Score + " (" + game.player2.name + ")";
        else return "(You) " + game.player2Score + " - " + game.player1Score + " (" + game.player1.name + ")";
      })}}</p>
      <p v-else>Score: {{game.player1Score}}</p>
      <div v-if="!game.isSinglePlayer" class="flex gap-2">
        <p :class="game.currentTurn.id === userStore.user.id ? 'bg-primary-400 text-neutral-900' : 'bg-neutral-800 text-neutral-600'" class="rounded-md px-1">Your Turn</p>
        <p :class="game.currentTurn.id !== userStore.user.id ? 'bg-primary-400 text-neutral-900' : 'bg-neutral-800 text-neutral-600'" class="rounded-md px-1">Opponents Turn</p>
      </div>
      <UButton class="mt-1" block>Resume</UButton>
    </UCard>
  </div>
</template>

<style scoped>

</style>