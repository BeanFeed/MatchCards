import {backendUrl} from "@/config.json";
import {fetchWithTimeout} from "@/main.js";

export async function getMe() {

    return await fetchWithTimeout(`${backendUrl}/api/player/me`, {
        credentials: "include",
        timeout: 5000
    });
}

export async function createPlayer(playerName) {
    return await fetch(encodeURI(`${backendUrl}/api/player/createplayer?name=${playerName}`), {
        credentials: "include",
        method: "POST",
        timeout: 5000
    });
}

export async function myGames() {
    return await fetch(`${backendUrl}/api/player/mygames`, {
        credentials: "include",
    });
}