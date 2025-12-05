<script setup lang="js">
import { useRoute, useRouter } from "vue-router";
import {onMounted, ref, nextTick, onUnmounted} from "vue";
import { getGameState } from "@/requests/game.js";
import { globalToast } from "@/main.js";
import {useUserStore} from "@/stores/user.js";
import {useSignalRStore} from "@/stores/signalr.js";

const route = useRoute();
const router = useRouter();

const user = useUserStore();

const gameId = ref(route.params.gameId);
const gameState = ref(null);

const gridContainer = ref(null);
const gridStyle = ref({});
const gridBox = ref(null);

const flippedCards = ref({});

const cards = ref({});

const signalr = useSignalRStore();
const openGameOver = ref(false);

function setCardRef(cardId) {
  return (el) => {
    if (el) {
      cards.value[cardId] = el;
    }
  };
}


async function fetchGameState() {
  const res = await getGameState(gameId.value);
  if (!res.ok) {
    res.text().then((text) => {
      globalToast.add({ title: "Failed to load game", description: text });
    });
    await router.push("/");
  } else {
    gameState.value = await res.json();
    await nextTick();
    setTimeout(() => {
      if(gameState.value.isGameOver) openGameOver.value = true;
    }, 2000);
  }
}

function calculateGridSize() {
  if(gridContainer.value.clientWidth > gridContainer.value.clientHeight) {
    const containerSize = gridContainer.value.clientHeight;
    const gridSize = gridBox.value.clientHeight;
    gridBox.value.style.transform = `scaleX(${containerSize / gridSize}) scaleY(${containerSize / gridSize})`;

  } else {
    const containerSize = gridContainer.value.clientWidth;
    const gridSize = gridBox.value.clientWidth;
    gridBox.value.style.transform = `scaleX(${containerSize / gridSize}) scaleY(${containerSize / gridSize})`;
  }
}

const lockFlips = ref(false);

async function flipCard(cardId) {
  if(gameState.value.isGameOver) return;
  if(!lockFlips.value) {
    lockFlips.value = true;
  } else return;
  if(gameState.value.currentTurn.id === user.user.id && !flippedCards.value[cardId] && Object.keys(flippedCards.value).length < 2) {
    console.log("Can flip car")
    const cardRef = cards.value[cardId];
    console.log("Card ref", cardRef)

    if(cardRef) {
      cardRef.toggleFlip();
      flippedCards.value[cardId] = true;
      const msg = {
        gameStateId: gameState.value.id,
        cardId: cardId,
        playerId: user.user.id
      }

      signalr.connection.invoke("FlipCard", msg);
    }
  }
  if(Object.keys(flippedCards.value).length == 2) {

    setTimeout(() => {
      let keys = Object.keys(flippedCards.value);
      //get gameState.cards by id
      let card1 = gameState.value.cards.find(c => c.id == keys[0]);
      let card2 = gameState.value.cards.find(c => c.id == keys[1]);
      if(card1.cardIndex !== card2.cardIndex) {
        cards.value[keys[0]].toggleFlip();
        cards.value[keys[1]].toggleFlip();
      }
      flippedCards.value = {};
      lockFlips.value = false;

    }, 1000)


  }
  lockFlips.value = false;
}

onMounted(async () => {
  await fetchGameState();

  window.addEventListener("resize", calculateGridSize);
  calculateGridSize()

  signalr.connection.on("Error", async (data) => {
    console.log("Error from SignalR:", data);
    await fetchGameState();
  });

  signalr.connection.on("CardFlip", async (data) => {
    if(gameState.value.currentTurn.id === user.user.id) return; //ignore own flips
    console.log("Opponent flipped card", data);
    const cardRef = cards.value[data];
    console.log("Card ref", cardRef)

    if(cardRef) {
      cardRef.toggleFlip();
      flippedCards.value[data] = true;
    }
    if(Object.keys(flippedCards.value).length == 2) {
      await new Promise(resolve => setTimeout(resolve, 1000));
      let keys = Object.keys(flippedCards.value);
      //get gameState.cards by id
      let card1 = gameState.value.cards.find(c => c.id == keys[0]);
      let card2 = gameState.value.cards.find(c => c.id == keys[1]);
      if(card1.cardIndex !== card2.cardIndex) {
        cards.value[keys[0]].toggleFlip();
        cards.value[keys[1]].toggleFlip();
      }
      flippedCards.value = {};
    }
  });

  signalr.connection.on("GameStateUpdate", async (data) => {
    console.log("SignalR: Game state updated", data);
    gameState.value = data;
    await nextTick();
    setTimeout(() => {
      if(data.isGameOver) openGameOver.value = true;
    }, 2000);
  });
});

onUnmounted(() => {
  window.removeEventListener("resize", calculateGridSize);
  signalr.connection.off("CardFlip");
  signalr.connection.off("GameStateUpdate");
  signalr.connection.off("Error");
});
</script>

<template>
  <div class="flex flex-col h-dvh overflow-hidden">
    <GameScoreboard class="app" v-if="gameState" :game-state="gameState" />
    <div class="app h-full overflow-hidden flex justify-center items-center my-2" ref="gridContainer">
      <div class="grid grid-cols-5 grid-rows-4 w-max h-max" ref="gridBox" :style="gridStyle">
        <FaceCard
            v-for="card in (gameState?.cards || []).sort((a, b) => a.position - b.position)"
            :key="card.id"
            :card="card"
            :ref="setCardRef(card.id)"
            @flip="flipCard(card.id)"
        />
      </div>
    </div>
    <div class="rotate-warning">
      <p>Please rotate device to landscape</p>
    </div>
    <Footer/>
    <UModal v-model:open="openGameOver">
      <template #header>
        <h2 class="text-3xl font-bold text-center w-full">Game Over</h2>
      </template>
      <template #body>
        <p class="text-center">The game has ended!</p>
        <div class="mt-4">
          <p class="text-center">{{gameState.player1.name}}'s Score: {{gameState.player1Score}}</p>
          <p class="text-center">{{gameState.player2.name}}'s Score: {{gameState.player2Score}}</p>
          <p class="text-center" v-if="gameState.player1Score > gameState.player2Score">{{gameState.player1.name}} Wins!</p>
          <p class="text-center" v-else-if="gameState.player1Score < gameState.player2Score">{{gameState.player2.name}} Wins!</p>
          <p class="text-center" v-else>It's a Tie!</p>
        </div>
      </template>
      <template #footer>
        <UButton @click="router.push('/')" block>Return to Home</UButton>
      </template>
    </UModal>
  </div>
</template>

<style scoped>
/* Show app only in landscape */
@media (orientation: landscape) {
  .app {
  }
  .rotate-warning {
    display: none;
  }
}

/* Block UI in portrait with a rotate message */
@media (orientation: portrait) {
  .app {
    display: none;
  }
  .rotate-warning {
    position: fixed;
    inset: 0;
    display: grid;
    place-items: center;
    background: #0008;
    color: #fff;
    font: 600 1rem system-ui;
    z-index: 9999;
  }
}

</style>
