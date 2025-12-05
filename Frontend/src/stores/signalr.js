// stores/signalr.js
import { backendUrl } from '@/config.json'
import { defineStore } from 'pinia'
import * as signalR from '@microsoft/signalr'
import {useRouter} from "vue-router";

const router = useRouter();

export const useSignalRStore = defineStore('signalr', {
    state: () => ({
        connection: null,
        connected: false
    }),

    actions: {
        async start() {
            this.connection = new signalR.HubConnectionBuilder()
                .withUrl(`${backendUrl}/hubs/game`)
                .withAutomaticReconnect()
                .build()

            /*
            this.connection.on("ReceiveMessage", (msg) => {
                console.log("SignalR event:", msg)
            })
            */
            await this.connection.start();
            this.connection.onclose(e => {
                router.push('/');
            })
            this.connected = true
            console.log("SignalR connected")
        },

        async stop() {
            if (this.connection) {
                await this.connection.stop();
                this.connected = false
                this.connection = null
                console.log("SignalR disconnected")
            }
        }
    }
})
