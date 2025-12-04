<script setup lang="js">

import {useRoute, useRouter} from "vue-router";
  import {onMounted, ref, watch} from "vue";
  import {getGameState} from "@/requests/game.js";
import {globalToast} from "@/main.js";

  const route = useRoute();
  const router = useRouter();

  const gameId = ref(route.params.gameId);

  const gameState = ref();

  async function fetchGameState() {
    const res = await getGameState(gameId.value);
    if(!res.ok) {
      res.text().then(text => {
        globalToast.add({
          title: "Failed to load game",
          description: text
        });
      })
      await router.push("/");
    } else {
      gameState.value = await res.json();
    }
  }


  onMounted(async () => {
    await fetchGameState();
  })

</script>

<template>
  <div class="flex flex-col h-screen">
    <GameScoreboard v-if="gameState" :game-state="gameState"/>
  </div>
</template>

<style scoped>

</style>