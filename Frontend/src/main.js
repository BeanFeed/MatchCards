import { createApp } from 'vue'
import { createPinia } from 'pinia'
import ui from '@nuxt/ui/vue-plugin'
import './assets/main.css'

import App from './App.vue'
import router from './router'
import {useSignalRStore} from "@/stores/signalr.js";
import piniaPluginPersistedstate from 'pinia-plugin-persistedstate'

const app = createApp(App)

export async function fetchWithTimeout(resource, options = {}) {
    const { timeout = 8000 } = options;

    const controller = new AbortController();
    const id = setTimeout(() => controller.abort(), timeout);

    const response = await fetch(resource, {
        ...options,
        signal: controller.signal
    });
    clearTimeout(id);

    return response;
}
const pinia = createPinia();
pinia.use(piniaPluginPersistedstate)
app.use(pinia)
app.use(router)
app.use(ui)


app.mount('#app')

let signalr = useSignalRStore();
signalr.start();

export const globalToast = useToast();