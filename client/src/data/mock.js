import { departments } from "./departments"

export const ordersByDept = [
    { deptId: 1, orders: 15, totalTime: 42.5 },
    { deptId: 2, orders: 22, totalTime: 68.0 },
    { deptId: 3, orders: 18, totalTime: 51.5 },
    { deptId: 4, orders: 10, totalTime: 28.0 },
    { deptId: 5, orders: 14, totalTime: 39.5 },
    { deptId: 9, orders: 20, totalTime: 57.0 },
    { deptId: 10, orders: 12, totalTime: 34.5 }
];

export const productionSeries = {
    labels: ['07', '08', '09', '10', '11', '12', '13', '14', '15', '16', '17'],
    lines: [
        { deptId: 1, values: [47, 44, 42, 40, 34, 30, 25, 20, 12, 5] },
        { deptId: 2, values: [45, 41, 38, 36, 32, 28, 20, 15, 8, 3] },
        { deptId: 3, values: [30, 27, 25, 22, 18, 16, 12, 8, 3, 0] },
        { deptId: 4, values: [40, 37, 32, 28, 23, 19, 14, 9, 4, 1] },
        { deptId: 5, values: [29, 26, 24, 22, 18, 15, 11, 7, 4, 0] },
        { deptId: 9, values: [33, 31, 28, 25, 21, 17, 13, 10, 6, 2] },
        { deptId: 10, values: [15, 12, 10, 8, 6, 5, 3, 2, 1, 0] },
    ]
}

export const endOfDay = [
    { deptId: 1, completedPercent: 95 },
    { deptId: 2, completedPercent: 100},  
    { deptId: 3, completedPercent: 100}, 
    { deptId: 4, completedPercent: 55},
    { deptId: 5, completedPercent: 85},
    { deptId: 9, completedPercent: 75 },
    { deptId: 10, completed: 100}
]