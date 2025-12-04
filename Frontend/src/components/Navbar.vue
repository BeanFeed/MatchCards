<script setup lang="js">

</script>

<template>
  <div class="bg-neutral-900 shadow-glow p-2 grid grid-cols-3 navbar h-min sticky top-0 w-full">
    <div class="w-full p-1">
      <p class="text-xs lg:text-base">Austin's EGR 101 Project</p>
    </div>
    <div class="w-full p-1">
      <h1 class="text-center text-2xl font-bold w-full">Match Cards</h1>
    </div>
    <div class="w-full p-1 justify-end gap-2">
      <UButton variant="ghost" class="text-white">Scoreboard</UButton>
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