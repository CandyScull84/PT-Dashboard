export const departments = [
    {id: 1, name: 'Department A', color: '#881A7E' },
    {id: 2, name: 'Department B', color: '#808484' },
    {id: 3, name: 'Department C', color: '#0089C4' },
    {id: 4, name: 'Department D', color: '#002B45' },
    {id: 5, name: 'Department E', color: '#005C84' },
    {id: 9, name: 'Department F', color: '#D1D4D3' },
    {id: 10, name: 'Department G', color: '#F0AB00' }

]
export const deptById = new Map(departments.map(d => [d.id, d]));