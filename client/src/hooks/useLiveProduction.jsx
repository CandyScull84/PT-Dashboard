
import { useEffect, useState } from "react";
import * as signalR from "@microsoft/signalr";
import { productionSeries } from "../data/mock.js";

function apiBase() {
    return (import.meta.env.VITE_API_BASE || '/dashboard').replace(/\/+$/, '');
}
function apiUrl(path) {
    const base = apiBase();
    return base + '/' + path.replace(/^\/+/, '');
}

export default function useLiveProduction() {
    const [series, setSeries] = useState(null);
    const [error, setError] = useState(null);

    useEffect(() => {
        let stopped = false;
        let connection;

         if (import.meta.env.VITE_DEMO_MODE === "true") {
            setSeries(productionSeries);
            return;
        }

        async function init() {
            try {
                // initial snapshot
                const res = await fetch(apiUrl('api/snapshot'), { credentials: 'include' });
                if (!res.ok) throw new Error(`HTTP ${res.status}`);
                const snap = await res.json();
                if (!stopped) setSeries(snap?.production ?? null);

                // SignalR
                connection = new signalR.HubConnectionBuilder()
                    .withUrl(apiUrl('hub'), { withCredentials: true })
                    .withAutomaticReconnect()
                    .build();

                connection.on("productionUpdate", (payload) => {
                    if (!stopped) setSeries(payload);
                });

                await connection.start();
            } catch (e) {
                if (!stopped) setError(e.message || "Live-anslutning misslyckades");
            }
        }

        init();
        return () => { stopped = true; if (connection) connection.stop(); };
    }, []);

    return { series, error };
}
