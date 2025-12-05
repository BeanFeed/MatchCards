// File: 'src/views/scoreboard.vue'
<script setup lang="js">

import {onMounted, onUnmounted, ref, nextTick} from "vue";
import QrcodeVue from 'qrcode.vue'
import {getActiveGames, getRecentGames} from "@/requests/game.js";
import {useSignalRStore} from "@/stores/signalr.js";

const signalr = useSignalRStore();

const activeGames = ref([]);
const recentGames = ref([]);

const activeList = ref(null);
const recentList = ref(null);

let activeTimer = null;
let recentTimer = null;

const origin = ref(null);

// Slow scroll with custom duration and easing
function scrollTo(elRef, toBottom = true, duration = 3000) {
  const el = elRef?.value;
  if (!el) return;

  const start = el.scrollTop;
  const end = toBottom ? el.scrollHeight - el.clientHeight : 0;
  const change = end - start;
  const startTime = performance.now();

  // easeInOutQuad
  const ease = (t) => (t < 0.5 ? 2 * t * t : -1 + (4 - 2 * t) * t);

  function step(now) {
    const elapsed = now - startTime;
    const progress = Math.min(elapsed / duration, 1);
    const eased = ease(progress);
    el.scrollTop = start + change * eased;

    if (progress < 1) {
      requestAnimationFrame(step);
    }
  }

  requestAnimationFrame(step);
}

function startAutoScroll(elRef, timerRef) {
  let toBottom = true;
  timerRef.value = setInterval(() => {
    scrollTo(elRef, toBottom, 1500);
    toBottom = !toBottom;
  }, 5000);
}

async function fetchActiveGames() {
  const res = await getActiveGames();
  if(res.ok) {
    activeGames.value = await res.json();
    await nextTick();
    if (!activeTimer) {
      activeTimer = { value: null };
      startAutoScroll(activeList, activeTimer);
    }
  }
}

async function fetchRecentGames() {
  const res = await getRecentGames();
  if(res.ok) {
    recentGames.value = await res.json();
    await nextTick();
    if(!recentTimer) {
      recentTimer = { value: null };
      startAutoScroll(recentList, recentTimer);
    }
  }
}

onMounted(async() => {
  origin.value = window.location.origin;
  signalr.connection.on("ScoreboardUpdate", async() => {
    await fetchActiveGames();
    await fetchRecentGames();
    console.log('Scoreboard updated');
  });

  await fetchActiveGames();
  await fetchRecentGames();
})

onUnmounted(() => {
  signalr.connection.off("ScoreboardUpdate");
})

</script>

<template>
  <div class="flex flex-col h-screen overflow-hidden">
    <Navbar/>
    <div class="h-full w-full flex justify-center gap-5">
      <div class="w-full flex justify-end">
        <div ref="activeList" class="flex flex-col w-min items-center gap-4 max-h-full overflow-y-scroll pb-20">
          <h2 class="whitespace-nowrap pt-4">Active Games</h2>
          <ScoreboardGameCard v-if="activeGames" v-for="game in activeGames" :key="game.id" :game="game"/>
        </div>
      </div>
      <div class="w-full flex justify-start">
        <div ref="recentList" class="flex flex-col w-min items-center gap-4 px-1 h-min max-h-full overflow-y-scroll pb-20">
          <h2 class="whitespace-nowrap pt-4">Recent Games</h2>
          <ScoreboardGameCard v-if="recentGames" v-for="game in recentGames" :key="game.id" :game="game"/>
        </div>
      </div>
    </div>
    <Footer/>
    <div class="absolute bottom-0 right-0 p-4">
      <p class="text-center text-3xl font-bold pb-2">Scan to play game</p>
      <QrcodeVue :value="origin" :size="300" level="H" foreground="#c27aff" background="#00000000" />
    </div>
  </div>
</template>

<style scoped>

</style>
