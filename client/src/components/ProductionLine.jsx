import React from 'react';
import { Line } from 'react-chartjs-2';
import { departments } from '../data/departments';
import {
Chart as ChartJS,
LineElement,
CategoryScale,
LinearScale,
PointElement,
Tooltip,
Legend
} from 'chart.js';

ChartJS.register(LineElement, CategoryScale, LinearScale, PointElement, Tooltip, Legend);

const ProductionLine = ({ series }) => {
    const chartData = {
        labels: series.labels, // 5-min etiketter (visar bara hel timme i ticks)
        datasets: series.lines.map(line => {
            const dept = departments.find(d => d.id === line.deptId);
            return {
                label: dept ? dept.name : `Dept ${line.deptId}`,
                data: line.values.map(v => (v === undefined ? null : v)),
                borderColor: dept?.color,
                backgroundColor: dept?.color,
                borderWidth: 4,
                tension: 0.5, // lite mjukare men inte för vågig
                cubicInterpolationMode: 'monotone',
                fill: false,
                pointRadius: 0, // inga punkter – ren linje
                pointHoverRadius: 5,
                pointHitRadius: 10,
                HitRadius: 8,
                borderCapStyle: 'round',
                borderJoinStyle: 'round',
                spanGaps: false
            };
        })
};

    const options = {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
            legend: {
                position: 'bottom',
                labels: {
                    font: { size: 12, family: 'Roboto', weight: '300' },
                    color: '#333'
                }
            },
            tooltip: {
                enabled: true,
                mode: 'index',
                intersect: false,
                filter: ctx => ctx.parsed.y !== null,
                callbacks: {
                    title: items => items?.[0]?.label ?? '',
                    label: ctx => `${ctx.dataset.label}: Kvar ${ctx.parsed.y}`
                }
            }
        },
        interaction: {
            mode: 'index',
            intersect: false
        },
        scales: {
            x: {
                title: { display: true, text: 'Tid (h)' },
                ticks: {
                    callback(value) {
                        const lbl = this.getLabelForValue(value); // "HH:mm"
                        const mm = parseInt(lbl.slice(3, 5), 10);
                        return mm === 0 ? lbl : '';
                    },
                    maxRotation: 0,
                    autoSkip: false
                },
                grid: {
                    color: (ctx) => {
                        const lbl = series.labels[ctx.index] || '';
                        const mm = parseInt(lbl.slice(3, 5) || '0', 10);
                        return mm === 0 ? 'rgba(0,0,0,.08)' : 'transparent';
                    }
                }
            },
            y: {
                beginAtZero: true,
                title: { display: true, text: 'Antal ordrar' }
            }
        }
    };

// OBS: Ingen NowLinePlugin här – hjälplinjen är borttagen
return <Line data={chartData} options={options} />;
};

export default ProductionLine;
