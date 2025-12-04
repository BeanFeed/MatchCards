<script setup lang="js">

import {acceptRequest, declineRequest} from "@/requests/game.js";

const props = defineProps({
  request: {
    type: Object,
    required: true
  }
})

const toast = useToast();

async function acceptPlayerRequest() {
  // Placeholder for accepting request logic
  const res = await acceptRequest(props.request.requester.id);
  if(!res.ok) {
    res.text().then(text => {
      toast.add({
        title: "Failed to accept request",
        description: text
      });
    })
  }
}

async function declinePlayerRequest() {
  // Placeholder for declining request logic
  await declineRequest(props.request.requester.id);
}

</script>

<template>
<div class="w-max">
  <UCard>
    <p class="text-center font-bold">{{request.requester.name}}</p>
    <div class="flex gap-2">
      <UButton @click="acceptPlayerRequest" block>Accept</UButton>
      <UButton @click="declinePlayerRequest" color="neutral" block>Decline</UButton>
    </div>
  </UCard>
</div>
</template>

<style scoped>

</style>