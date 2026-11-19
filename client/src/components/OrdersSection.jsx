
import OrdersDonut from "./OrdersDonut";
import { deptById } from "../data/departments";
// import PropTypes from "prop-types";
import { useMemo } from "react";
import useSnapshot from "../hooks/useSnapshot";

const OrdersSection = () => {
  const {data, loading, error} = useSnapshot();

  const rows = useMemo(() => {
    if (import.meta.env.VITE_DEMO_MODE === "true") {
        return data?.orders ?? [];
    }

    const src = data?.totalsStart ?? [];

    return src.map(totals => ({
        deptId: totals.workgroupId,
        orders: totals.totalOrders
    }));
  }, [data]);

  const merged = useMemo(() => {
    if (!rows || rows.length === 0) return [];

    return rows.map((item) => {
      const dept = deptById?.get(item.deptId);
      return {
        ...item,
        name: dept?.name || `Avdelning ${item.deptId}`,
        color: dept?.color || "#cccccc", // fallback-färg
      };
    });
  }, [rows]);

  const total = useMemo(() => {
    return merged.reduce((sum, m) => sum + (m.orders || 0), 0);
  }, [merged]);

    const HoursByDept = useMemo(() => {
        const dict = new Map();
        const orders = data?.orders || [];
        for (const o of orders) {
            const deptId = import.meta.env.VITE_DEMO_MODE === "true"
              ? o.deptId
              : o.workgroupId;
            const h = (o.totalTime ?? 0);
            dict.set(deptId, (dict.get(deptId) || 0) + h);
        }
        return dict; //Map<deptId, totalHours>
    }, [data]);

    const withHours = useMemo(() => {
        return merged.map(m => ({
            ...m,
            hours: Number((HoursByDept.get(m.deptId) || 0).toFixed(2)) //2 decimaler
        }));
    }, [merged, HoursByDept]);


    const chartData = useMemo(() => ({
        labels: merged.map((m) => m.name),
        datasets: [
            {
                data: merged.map((m) => m.orders),
                backgroundColor: merged.map((m) => m.color),
                borderWidth: 1,
                cutout: "70%",
            },
        ],
    }), [merged]);


  if (!rows || rows.length === 0) {
    return (
      <section className="card orders-card">
        <h4 className="section-title">Dagens utgångsläge</h4>
        <p>Inga orderdata tillgängliga.</p>
      </section>
    );
  }

  return (
    <section className="card orders-card">
      <h4 className="section-title">Dagens utgångsläge</h4>
      <div className="orders-layout">
        <div className="orders-donut">
          <OrdersDonut chartData={chartData} />
        <div className="orders-total" aria-live="polite">
        <div className="orders-total__label">Totalt</div>
        <div className="orders-total__value">{total}</div>
        </div>
        </div>

        <div className="orders-table">
          <table>
            <caption className="sr-only">Orderfördelning per avdelning</caption>
            <thead>
              <tr>
                <th>Avdelning</th>
                <th>Ordrar</th>
                <th>Tid (h)</th>

              </tr>
            </thead>
            <tbody>
              {withHours.map((m) => (
                <tr key={m.deptId}>
                  <td>
                    <span
                      className="color-dot"
                      style={{ backgroundColor: m.color }}
                    ></span>
                    {m.name}
                  </td>
                  <td className="text-right">{m.orders}</td>
                  <td className="text-right">{m.hours}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </section>
  );
};

export default OrdersSection;
