<script setup lang="js">
import {ref, watch} from "vue";

const props = defineProps({
  card: {
    type: Object,
    required: true,
  },
});

const emits = defineEmits(["flip"]);

defineExpose({toggleFlip})

// This will toggle the flip
const flipped = ref(true);
const showImage = ref(false);

flipped.value = !props.card.isFaceUp;
if(props.card.isFaceUp) {
  flipped.value = false;
  showImage.value = true;
}

function toggleFlip() {
  if(flipped.value) {
    showImage.value = flipped.value;
    flipped.value = !flipped.value;
  } else {
    flipped.value = !flipped.value;
    setTimeout(() => {
      showImage.value = !flipped.value;
    }, 250);
  }

}

watch(props.card, (newCard) => {
  if(props.card.isFaceUp) {
    flipped.value = false;
    showImage.value = true;
  } else {
    flipped.value = true;
    showImage.value = false;
  }
})
</script>


<template>
  <div class="p-1 flex justify-center items-center">
    <div
        class="w-[100px] h-[100px] perspective-1000"
        @click="emits('flip')"
    >
      <div
          class="w-full h-full relative duration-500 transform-style-preserve-3d"
          :class="{ 'rotate-y-180': flipped }"
      >
        <!-- Front side -->
        <img v-if="showImage"
            :src="`/cards/${card.cardIndex}.jpg`"
            class="absolute w-full h-full object-cover border-3 rounded-lg backface-hidden"
        />
        <!-- Back side -->
        <div
            class="absolute w-full h-full bg-neutral-600 border-3 rounded-lg flex items-center justify-center rotate-y-180 backface-hidden"
        >

        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.perspective-1000 {
  perspective: 1000px;
}

.transform-style-preserve-3d {
  transform-style: preserve-3d;
}

.rotate-y-180 {
  transform: rotateY(180deg);
}

.backface-hidden {
  backface-visibility: hidden;
}
</style>
