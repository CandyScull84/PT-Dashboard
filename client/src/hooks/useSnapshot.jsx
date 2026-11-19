
import { useEffect, useState, useRef } from "react";
import { ordersByDept, productionSeries, endOfDay}  from "../data/mock.js"; 

function apiUrl(path) {
    const base = import.meta.env.VITE_API_BASE || '/dashboard';
    return base.replace(/\/+$/, '') + '/' + path.replace(/^\/+/, '');
}

export default function useSnapshot() {
    const [data, setData] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const lastSigRef = useRef(null);
    const intervalMs = 10000;

    useEffect(() => {
        let cancelled = false;
        let t;

        if (import.meta.env.VITE_DEMO_MODE === "true") {
            setData({
                orders: ordersByDept,
                production: productionSeries,
                endOfDay: endOfDay
            });
            setLoading(false);
            return;
        }
        
        async function fetchData() {
            try {
                const res = await fetch(apiUrl('api/snapshot'), { credentials: 'include' });
                if (!res.ok) throw new Error(`HTTP ${res.status}`);
                const json = await res.json();

                if (!cancelled && (!lastSigRef.current || json?.signature !== lastSigRef.current)) {
                    lastSigRef.current = json?.signature ?? null;
                    setData(json);
                }
            } catch (e) {
                if (!cancelled) setError(e.message || 'Fel vid hämtning');
            } finally {
                if (!cancelled) setLoading(false);
            }
        }

        fetchData();
        t = setInterval(fetchData, intervalMs);
        return () => { cancelled = true; clearInterval(t); };
    }, []);

    return { data, loading, error };
}
