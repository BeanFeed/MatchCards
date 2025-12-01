<script setup lang="js">
import {createPlayer, getMe} from "@/requests/player.js";
  import {ref} from "vue";

  const toast = useToast();
  const isAuthed = ref(true);
  const newName = ref("");
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
    }
  } catch (error) {
    toast.add({
      title: "Error",
      description: "Server is unreachable"
    });
  }


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

</script>

<template>
  <div class="flex flex-col h-screen">
    <div class="bg-neutral-900 shadow-glow p-2 grid grid-cols-3 navbar h-min">
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
        <h2 class="text-center text-3xl mt-5">A Singleplayer or Multiplayer card matching game</h2>
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