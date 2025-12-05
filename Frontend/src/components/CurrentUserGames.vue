<script setup lang="js">

import {onMounted, onUnmounted} from "vue";
  import {useUserStore} from "@/stores/user.js";
  import {useSignalRStore} from "@/stores/signalr.js";
import {getMe} from "@/requests/player.js";

  const props = defineProps({
    games: {
      type: Array,
      required: true
    }
  });


  const signalr = useSignalRStore();

  const userStore = useUserStore();

  async function fetchMyGames() {
    const res = await getMe();
    if(res.ok) {
      const data = await res.json();
      userStore.setGames(data.gameStates);
      props.games = data.gameStates;
    }
  }

  onMounted(() => {
    signalr.connection.on("GameStateUpdate", async () => {
      await fetchMyGames();
    });
  })

  onUnmounted(() => {
    signalr.connection.off("GameStateUpdate");
  })

</script>

<template>
<div class="w-dvh px-5">
  <h2 class="mb-2 text-lg font-bold">Active Games</h2>
  <template v-if="games.length > 0">
    <div v-if="games.filter(x => !x.isGameOver).length > 0" class="flex flex-wrap gap-2">
      <GameCard v-for="game in games.filter(x => !x.isGameOver)" :game="game"/>
    </div>
    <h2 v-else>No active games</h2>
    <h2 class="mb-2 text-lg font-bold">Previous Games</h2>
    <div v-if="games.filter(x => x.isGameOver).length > 0" class="flex flex-wrap gap-2">
      <GameCard v-for="game in games.filter(x => x.isGameOver)" :game="game"/>
    </div>
    <h2 v-else>No previous games</h2>
  </template>
  <h2 class="text-center text-xl font-bold mt-5" v-else>You haven't played any games yet</h2>
</div>
</template>

<style scoped>

</style>