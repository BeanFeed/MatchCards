<script setup lang="js">
  import {createPlayer, getMe} from "@/requests/player.js";
  import {onMounted, ref} from "vue";

  import {useToast} from "@nuxt/ui/composables";
  import {useUserStore} from "@/stores/user.js";

  let toast;
  const isAuthed = ref(true);
  const newName = ref("");
  const isServerReachable = ref(true);

  const userStore = useUserStore();

  const testGameStates = ref([
    {
      id:"game1",
      player1: {
        id: "d5557310-9a9e-42d3-b8f6-aa188bc1df7a",
        name: "Austin Dean"
      },
      player2: {
        id: "player2",
        name: "Bob"
      },
      currentTurn: {
        id: "d5557310-9a9e-42d3-b8f6-aa188bc1df7a",
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
        id: "d5557310-9a9e-42d3-b8f6-aa188bc1df7a",
        name: "Austin Dean"
      },
      player2: {
        id: "player2",
        name: "Bob"
      },
      currentTurn: {
        id: "d5557310-9a9e-42d3-b8f6-aa188bc1df7a",
        name: "Austin Dean"
      },
      player1Score: 0,
      player2Score: 0,
      isGameOver: false,
      isSinglePlayer: false,
      gameStartTime: null

    },{
      id:"game1",
      player1: {
        id: "d5557310-9a9e-42d3-b8f6-aa188bc1df7a",
        name: "Austin Dean"
      },
      player2: {
        id: "player2",
        name: "Bob"
      },
      currentTurn: {
        id: "d5557310-9a9e-42d3-b8f6-aa188bc1df7a",
        name: "Austin Dean"
      },
      player1Score: 0,
      player2Score: 0,
      isGameOver: false,
      isSinglePlayer: false,
      gameStartTime: null

    },{
      id:"game1",
      player1: {
        id: "d5557310-9a9e-42d3-b8f6-aa188bc1df7a",
        name: "Austin Dean"
      },
      player2: {
        id: "player2",
        name: "Bob"
      },
      currentTurn: {
        id: "d5557310-9a9e-42d3-b8f6-aa188bc1df7a",
        name: "Austin Dean"
      },
      player1Score: 0,
      player2Score: 0,
      isGameOver: false,
      isSinglePlayer: false,
      gameStartTime: null

    },{
      id:"game1",
      player1: {
        id: "d5557310-9a9e-42d3-b8f6-aa188bc1df7a",
        name: "Austin Dean"
      },
      player2: {
        id: "player2",
        name: "Bob"
      },
      currentTurn: {
        id: "d5557310-9a9e-42d3-b8f6-aa188bc1df7a",
        name: "Austin Dean"
      },
      player1Score: 0,
      player2Score: 0,
      isGameOver: false,
      isSinglePlayer: false,
      gameStartTime: null

    }
  ]);


  async function signup() {
    try {
      const res = await createPlayer(newName.value);

    } catch (error) {
      toast.add({
        title: "Error",
        description: "Server is unreachable: " +  error.message
      });
    }
  }

  onMounted(async () => {
    toast = useToast();
    try {
      const res = await getMe();
      console.log(res);
      if(!res.ok) {
        if(res.status === 401) isAuthed.value = false;
        else res.json().then(data => {
          toast.add({
            title: res.statusText,
            description: data,
          });
        })
      } else {
        const data = await res.json();
        userStore.setUser(data.user);
        userStore.setGames(data.gameStates);
        userStore.setScores(data.scores);
        console.log(data);
        isAuthed.value = true;
      }
    } catch (error) {
      isServerReachable.value = false;
      console.log(error)
      // toast.add({
      //   title: "Error",
      //   description: "Server is unreachable"
      // });
    }
  })
</script>

<template>
  <div class="flex flex-col min-h-screen">
    <div class="bg-neutral-900 shadow-glow p-2 grid grid-cols-3 navbar h-min sticky top-0 w-full">
      <div class="w-full p-1">
        <p class="text-xs lg:text-base">Austin's EGR 101 Project</p>
      </div>
      <div class="w-full p-1">
        <h1 class="text-center text-2xl font-bold w-full">Match Cards</h1>
      </div>
      <div class="w-full p-1 flex justify-end gap-2">
        <UButton variant="ghost" class="text-white">Scoreboard</UButton>
      </div>

    </div>
    <div class="flex flex-col items-center justify-center h-full p-5">
      <template v-if="isServerReachable">

        <template v-if="!isAuthed">
          <h2 class="text-center text-3xl my-5">Get Started</h2>
          <UCard>
            <div class="grid gap-4">
              <p class="text-center">Please enter your name</p>
              <UInput v-model="newName" class="w-full" placeholder="Name"/>
              <UButton @click="signup" block>Continue</UButton>
            </div>
          </UCard>
        </template>
        <template v-else>
          <CurrentUserGames :games="testGameStates"/>
        </template>
      </template>
      <template v-else>
        <h1 class="xl:text-3xl">Server is unreachable. This site is likely shutdown.</h1>
      </template>
    </div>
  </div>
</template>

<style scoped>
  .shadow-glow {
    box-shadow: 0 0 10px 2px rgba(255, 255, 255, 0.2);
  }

  .navbar > div {
    display: flex;
    align-items: center;
  }
</style>