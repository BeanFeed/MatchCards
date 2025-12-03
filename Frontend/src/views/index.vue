<script setup lang="js">
  import {createPlayer, getMe} from "@/requests/player.js";
  import {onMounted, onUnmounted, ref} from "vue";

  import {useToast} from "@nuxt/ui/composables";
  import {useUserStore} from "@/stores/user.js";
  import CurrentUserGames from "@/components/CurrentUserGames.vue";

  let toast;
  const isAuthed = ref(true);
  const newName = ref("");
  const isServerReachable = ref(true);

  const userStore = useUserStore();

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
      await getUser();
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
        toast.add({
          title: res.statusText,
          description: data,
        });
      })
      isAuthed.value = false;
    } else {
      const data = await res.json();
      userStore.setUser(data.user);
      userStore.setGames(data.gameStates);
      userStore.setScores(data.scores);
      console.log(data);
      isAuthed.value = true;
    }
  }

  const isBlock = ref(false);

  onMounted(async () => {
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

    updateIsBlock();
    window.addEventListener("resize", updateIsBlock);
  })

  onUnmounted(() => {
    window.removeEventListener("resize", updateIsBlock);
  });

  function updateIsBlock() {
    isBlock.value = window.innerWidth <= 1028;
  }
</script>

<template>
  <div class="flex flex-col min-h-screen" :class="!isAuthed ? 'h-screen' : ''">
    <Navbar/>
    <div class="px-4 pt-4">
      <UButton :block="isBlock">Find Match</UButton>
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