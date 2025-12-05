<script setup lang="js">

const props = defineProps({
  game: {
    type: Object,
    required: true
  }
})

</script>

<template>
  <div class="w-100">
    <UCard>
      <p>Players</p>
      <p>{{game.player1.name}}</p>
      <p>{{game.player2.name}}</p>
      <p>Score</p>
      <p>{{game.player1Score}} - {{game.player2Score}}</p>
      <template v-if="!game.isGameOver">
        <div v-if="!game.isSinglePlayer" class="flex flex-col gap-2">
          <p :class="game.currentTurn.id === game.player1.id ? 'bg-primary-400 text-neutral-900' : 'bg-neutral-800 text-neutral-600'" class="min-w-max text-center whitespace-nowrap rounded-md px-1">
            {{ game.player1.name }}'s Turn</p>
          <p :class="game.currentTurn.id === game.player2.id ? 'bg-primary-400 text-neutral-900' : 'bg-neutral-800 text-neutral-600'" class="min-w-max text-center whitespace-nowrap rounded-md px-1">
            {{ game.player2.name }}'s Turn</p>
        </div>
      </template>
      <div v-else-if="!game.isSinglePlayer" class="flex gap-2">
        <p :class="(game.player1Score > game.player2Score) ? 'bg-primary-400 text-neutral-900' : 'bg-neutral-800 text-neutral-600'" class="w-[50%] min-w-min text-center rounded-md px-1">{{ game.player1.name }} Won</p>
        <p :class="(game.player1Score < game.player2Score) ? 'bg-primary-400 text-neutral-900' : 'bg-neutral-800 text-neutral-600'" class="w-[50%] min-w-min text-center rounded-md px-1">{{ game.player2.name }} Won</p>
      </div>
    </UCard>
  </div>
</template>

<style scoped>

</style>