<script setup lang="js">

  const props = defineProps({
    games: {
      type: Array,
      required: true
    }
  });

  const activeGames = props.games.filter(x => !x.isGameOver);
  const previousGames = props.games.filter(x => x.isGameOver);

</script>

<template>
<div class="w-screen px-5">
  <h2 class="mb-2 text-lg font-bold">Active Games</h2>
  <template v-if="games.length > 0">
    <div v-if="activeGames.length > 0" class="flex flex-wrap gap-2">
      <GameCard v-for="game in activeGames" :game="game"/>
    </div>
    <h2 v-else>No active games</h2>
    <h2 class="mb-2 text-lg font-bold">Previous Games</h2>
    <div>

    </div>
  </template>
  <h2 class="text-center text-xl font-bold mt-5" v-else>You haven't played any games yet</h2>
</div>
</template>

<style scoped>

</style>