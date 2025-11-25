import {backendUrl} from "@/config.json";

export async function getMe() {

    return await fetch(`${backendUrl}/player/me`, {
        credentials: "include"
    });
}
