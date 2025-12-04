import { createRouter, createWebHistory } from 'vue-router'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
        name: 'index',
        component: () => import('@/views/index.vue')
    },
    {
      path: '/lobby',
        name: 'lobby',
        component: () => import('@/views/lobby.vue')
    },
    {
      path: '/game/:gameId',
        name: 'game',
        component: () => import('@/views/game.vue')
    }
  ],
})

export default router
