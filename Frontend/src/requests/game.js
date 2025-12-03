import {backendUrl} from "@/config.json";

/*

export async function createPlayer(playerName) {
    return await fetch(encodeURI(`${backendUrl}/player/createplayer?name=${playerName}`), {
        credentials: "include",
        method: "POST",
        timeout: 5000
    });
}
 */

export async function topTenScores() {
    return await fetch(`${backendUrl}/game/toptenscores`);
}

export async function getActiveGames() {
    return await fetch(`${backendUrl}/game/getactivegames`);
}

export async function joinLobby() {
    return await fetch(`${backendUrl}/game/joinlobby`, {
        credentials: "include",
        method: "POST"
    });
}

export async function leaveLobby() {
    return await fetch(`${backendUrl}/game/leavelobby`, {
        credentials: "include",
        method: "POST"
    });
}

export async function getLobby() {
    return await fetch(`${backendUrl}/game/getlobby`);
}

export async function sendRequest(opponentId) {
    return await fetch(encodeURI(`${backendUrl}/game/sendrequest?opponentId=${opponentId}`), {
        credentials: "include",
        method: "POST"
    });
}

export async function declineRequest(requesterId) {
    return await fetch(encodeURI(`${backendUrl}/game/declinerequest?requesterId=${requesterId}`), {
        credentials: "include",
        method: "POST"
    });
}

export async function getRequests() {
    return await fetch(`${backendUrl}/game/getrequests`, {
        credentials: "include"
    });
}

export async function acceptRequest(requesterId) {
    return await fetch(encodeURI(`${backendUrl}/game/acceptrequest?requesterId=${requesterId}`), {
        credentials: "include",
        method: "POST"
    });
}