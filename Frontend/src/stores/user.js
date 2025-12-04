import {ref, reactive} from 'vue'
import { defineStore } from 'pinia'

export const useUserStore = defineStore('user', () => {
  const user = reactive({
    id: null,
    name: null
  });
  const games = ref([]);
  const scores = ref([]);

  const socket = ref();

  function setUser(newUser) {
    console.log("New user", newUser);
    user.id = newUser.id;
    user.name = newUser.name;
  }

  function setGames(newGames) {
    games.value = newGames;
  }

  function setScores(newScores) {
    scores.value = newScores;
  }
  return { user, games, scores, socket, setUser, setGames, setScores }
},
    {
      persist: true
    })
