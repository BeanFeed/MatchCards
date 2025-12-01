import {backendUrl} from "@/config.json";
import {fetchWithTimeout} from "@/main.js";

export async function getMe() {

    return await fetchWithTimeout(`${backendUrl}/player/me`, {
        credentials: "include",
        timeout: 5000
    });
}

export async function createPlayer(playerName) {
    return await fetch(encodeURI(`${backendUrl}/player/createplayer?name=${playerName}`), {
        credentials: "include",
        method: "POST",
        timeout: 5000
    });
}