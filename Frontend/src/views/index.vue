<script setup lang="js">
import {createPlayer, getMe} from "@/requests/player.js";
import {computed, onMounted, onUnmounted, ref} from "vue";

import {useToast} from "@nuxt/ui/composables";
import {useUserStore} from "@/stores/user.js";
import CurrentUserGames from "@/components/CurrentUserGames.vue";
import {useRouter} from "vue-router";
import {useSignalRStore} from "@/stores/signalr.js";
import {getLobby} from "@/requests/game.js";

let toast;
  const isAuthed = ref(true);
  const newName = ref("");
  const isServerReachable = ref(true);

  const userStore = useUserStore();
  const signalr = useSignalRStore();
  const router = useRouter();

  const lobby = ref([]);

  const testGameStates = ref([
    {
      id:"game1",
      player1: {
        id: "8a476230-5b14-4951-9831-c89ac0a9b705",
        name: "Austin Dean"
      },
      player2: {
        id: "player2",
        name: "Bob"
      },
      currentTurn: {
        id: "8a476230-5b14-4951-9831-c89ac0a9b705",
        name: "Austin Dean"
      },
      player1Score: 0,
      player2Score: 0,
      isGameOver: false,
      isSinglePlayer: false,
      gameStartTime: null
      
    },
    {
      id:"game1",
      player1: {
        id: "8a476230-5b14-4951-9831-c89ac0a9b705",
        name: "Austin Dean"
      },
      player2: {
        id: "player2",
        name: "Bob"
      },
      currentTurn: {
        id: "8a476230-5b14-4951-9831-c89ac0a9b705",
        name: "Austin Dean"
      },
      player1Score: 0,
      player2Score: 0,
      isGameOver: false,
      isSinglePlayer: false,
      gameStartTime: null

    },
    {
      id:"game1",
      player1: {
        id: "8a476230-5b14-4951-9831-c89ac0a9b705",
        name: "Austin Dean"
      },
      player2: {
        id: "player2",
        name: "Jerehmiah Johnson"
      },
      currentTurn: {
        id: "8a476230-5b14-4951-9831-c89ac0a9b705",
        name: "Austin Dean"
      },
      player1Score: 0,
      player2Score: 0,
      isGameOver: false,
      isSinglePlayer: false,
      gameStartTime: null

    },
    {
      id:"game1",
      player1: {
        id: "8a476230-5b14-4951-9831-c89ac0a9b705",
        name: "Austin Dean"
      },
      player2: {
        id: "player2",
        name: "Bob"
      },
      currentTurn: {
        id: "8a476230-5b14-4951-9831-c89ac0a9b705",
        name: "Austin Dean"
      },
      player1Score: 0,
      player2Score: 0,
      isGameOver: false,
      isSinglePlayer: false,
      gameStartTime: null

    },
    {
      id:"game1",
      player1: {
        id: "8a476230-5b14-4951-9831-c89ac0a9b705",
        name: "Austin Dean"
      },
      player2: {
        id: "player2",
        name: "Jerehmiah Johnson"
      },
      currentTurn: {
        id: "8a476230-5b14-4951-9831-c89ac0a9b705",
        name: "Austin Dean"
      },
      player1Score: 4,
      player2Score: 5,
      isGameOver: true,
      isSinglePlayer: false,
      gameStartTime: null

    }
  ]);


  async function signup() {
    try {
      const res = await createPlayer(newName.value);
      if(res.ok) {
        await getUser();
        await signalr.stop();
        await signalr.start();
      } else {
        const data = await res.text();
        toast.add({
          title: res.statusText,
          description: data,
        });
      }

    } catch (error) {
      toast.add({
        title: "Error",
        description: "Server is unreachable: " +  error.message
      });
    }
  }

  async function getUser() {
    const res = await getMe();
    console.log(res);
    if(!res.ok) {
      if(res.status === 401) isAuthed.value = false;
      else res.text().then(data => {
        if(data === 'Player not found.') return
        toast.add({
          title: res.statusText,
          description: data,
        });
      })
      isAuthed.value = false;
      userStore.setUser({
        id: null,
        name: null
      })
    } else {
      const data = await res.json();
      userStore.setUser(data.user);
      userStore.setGames(data.gameStates);
      userStore.setScores(data.scores);
      console.log(data);
      isAuthed.value = true;
    }
  }

  async function fetchLobby() {
    const res = await getLobby();
    if(res.ok) {
      lobby.value = await res.json();
    }
  }

  const isBlock = ref(false);

  const textLimitStyle = computed(() => {
    let toRet = "";
    if(newName.value.length >= 10) toRet += "opacity-100 ";
    if(newName.value.length >= 15) toRet += "text-red-500";
    return toRet;
  })

  onMounted(async () => {

    updateIsBlock();
    window.addEventListener("resize", updateIsBlock);

    toast = useToast();
    try {
      await getUser();
    } catch (error) {
      isServerReachable.value = false;
      console.log(error)
      // toast.add({
      //   title: "Error",
      //   description: "Server is unreachable"
      // });
    }

    if(signalr.connected) {
      signalr.connection.on("LobbyChange", async () => {
        console.log("Lobby changed, fetching new data");
        await fetchLobby();
      });
    }
    await fetchLobby();
  })

  onUnmounted(() => {
    window.removeEventListener("resize", updateIsBlock);

    if(signalr.connected) {
      signalr.connection.off("LobbyChange");
    }
  });

  function updateIsBlock() {
    isBlock.value = window.innerWidth <= 1028;
  }
</script>

<template>
  <div class="flex flex-col min-h-screen" :class="!isAuthed ? 'h-screen' : ''">
    <Navbar/>
    <div class="px-4 pt-4" v-if="isServerReachable && isAuthed">
      <UButton :block="isBlock" @click="router.push('/lobby')">Join Game Lobby ({{lobby.length}} Players)</UButton>
    </div>
    <div class="flex flex-col items-center justify-center h-full p-5">
      <template v-if="isServerReachable">

        <template v-if="!isAuthed">
          <h2 class="text-center text-3xl my-5">Get Started</h2>
          <UCard>
            <div class="grid gap-4">
              <p class="text-center">Please enter your name</p>
              <div>
                <UInput v-model="newName" class="w-full" style="font-size: 16px;" placeholder="Name" maxlength="20"/>
                <p :class="textLimitStyle" class="text-sm mt-1 opacity-0 transition">{{newName.length}}/20</p>
              </div>
              <UButton @click="signup" block>Continue</UButton>
            </div>
          </UCard>
        </template>
        <template v-else>
          <CurrentUserGames :games="userStore.games"/>
        </template>
      </template>
      <template v-else>
        <h1 class="xl:text-3xl">Server is unreachable. This site is likely shutdown.</h1>
      </template>
    </div>
    <Footer/>
  </div>
</template>

<style scoped>

</style>