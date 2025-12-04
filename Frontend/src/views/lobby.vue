<script setup lang="js">

import {onMounted, onUnmounted, ref} from "vue";
import {useUserStore} from "@/stores/user.js";
import {useRouter} from "vue-router";
import {useSignalRStore} from "@/stores/signalr.js";
import {getLobby, getRequests, joinLobby} from "@/requests/game.js";

const userStore = useUserStore();
const signalr = useSignalRStore();
const router = useRouter();

const lobby = ref([]);
const requests = ref([]);

async function fetchLobby() {
  const res = await getLobby();
  if(res.ok) {
    lobby.value = await res.json();
    lobby.value = lobby.value.filter(x => x.id !== userStore.user.id);
  }
}

async function fetchRequests() {
  console.log('Fetching requests');
  // Placeholder for fetching requests
  const res = await getRequests();
  if(res.ok){
    requests.value = await res.json();
  }
}

onMounted(async () => {
  if(userStore.user.id == null) {
    await router.push("/");
    return;
  }

  await joinLobby();

  await fetchLobby();

  await fetchRequests();

  if(signalr.connected) {
    signalr.connection.on("LobbyChange", async () => await fetchLobby());

    signalr.connection.on("ReceiveRequest", async () => await fetchRequests());
    signalr.connection.on("GameStarted", (data) => {
      console.log("Game started with ID: " + data);
      router.push('/game/' + data);
    })
  }

});

onUnmounted(() => {
  if(signalr.connected) {
    signalr.connection.off("LobbyChange");
    signalr.connection.off("ReceiveRequest");
    signalr.connection.off("GameStarted");
  }

})
</script>

<template>
<div class="flex flex-col h-screen">
  <Navbar/>
  <div class="w-full px-5 flex flex-col h-full pt-4">
    <h2 class="mb-2 text-lg font-bold">Players</h2>
    <div class="flex flex-wrap gap-2 h-full overflow-y-auto">
      <LobbyPlayer v-if="lobby.length > 0"  v-for="player in lobby" :player="player"/>
      <h2 v-else>No players in lobby</h2>
    </div>
    <h2 class="mb-2 text-lg font-bold">Requests</h2>
    <div class="flex flex-wrap gap-2 h-full overflow-y-auto">
      <RequestCard v-if="requests.length > 0" v-for="request in requests" :request="request"/>
      <h2 v-else>No requests</h2>
    </div>
  </div>
</div>
</template>

<style scoped>

</style>