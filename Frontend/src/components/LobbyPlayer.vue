<script setup lang="js">

import {ref} from "vue";
import {sendRequest} from "@/requests/game.js";

const props = defineProps({
  player: {
    type: Object,
    required: true
  }
});

const requestSent = ref(false);
const toast = useToast();
async function sendRequestToPlayer() {
  // Here you would add the logic to send a game request to the player
  const res = await sendRequest(props.player.id);
  if(!res.ok) {
    const data = await res.text().then(data => {
      toast.add({
        title: "Failed to send request",
        description: data,
      });
    })

  } else {
    requestSent.value = true;
  }
}

</script>

<template>
<div class="w-max">
  <UCard>
    <p class="text-center font-bold">{{player.name}}</p>
    <UButton @click="sendRequestToPlayer" block :disabled="requestSent">{{requestSent ? 'Request sent' : 'Request to play'}}</UButton>
  </UCard>
</div>
</template>

<style scoped>

</style>