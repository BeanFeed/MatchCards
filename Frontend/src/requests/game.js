import {backendUrl} from "@/config.json";

/*

export async function createPlayer(playerName) {
    return await fetch(encodeURI(`${backendUrl}/api/player/createplayer?name=${playerName}`), {
        credentials: "include",
        method: "POST",
        timeout: 5000
    });
}
 */

export async function topTenScores() {
    return await fetch(`${backendUrl}/api/game/toptenscores`);
}

export async function getActiveGames() {
    return await fetch(`${backendUrl}/api/game/getactivegames`);
}

export async function getRecentGames() {
    return await fetch(`${backendUrl}/api/game/getrecentgames`);
}

export async function joinLobby() {
    return await fetch(`${backendUrl}/api/game/joinlobby`, {
        credentials: "include",
        method: "POST"
    });
}

export async function leaveLobby() {
    return await fetch(`${backendUrl}/api/game/leavelobby`, {
        credentials: "include",
        method: "POST"
    });
}

export async function getLobby() {
    return await fetch(`${backendUrl}/api/game/getlobby`);
}

export async function sendRequest(opponentId) {
    return await fetch(encodeURI(`${backendUrl}/api/game/sendrequest?opponentId=${opponentId}`), {
        credentials: "include",
        method: "POST"
    });
}

export async function declineRequest(requesterId) {
    return await fetch(encodeURI(`${backendUrl}/api/game/declinerequest?requesterId=${requesterId}`), {
        credentials: "include",
        method: "POST"
    });
}

export async function getRequests() {
    return await fetch(`${backendUrl}/api/game/getrequests`, {
        credentials: "include"
    });
}

export async function acceptRequest(requesterId) {
    return await fetch(encodeURI(`${backendUrl}/api/game/acceptrequest?requesterId=${requesterId}`), {
        credentials: "include",
        method: "POST"
    });
}

export async function getGameState(gameId) {
    return await fetch(encodeURI(`${backendUrl}/api/game/getgamestate?gameStateId=${gameId}`), {
        credentials: "include"
    });
}