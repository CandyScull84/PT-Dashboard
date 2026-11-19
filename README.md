# PT Dashboard

A fullstack production monitoring dashboard built with React, C# and ASP.NET Core.

The application provides an overview of production orders, workgroup statistics and daily production progress through interactive data visualizations.

## Features

- **Order overview** – Displays production orders and statistics for different workgroups.
- **Production tracking** – Visualizes production progress throughout the working day.
- **End-of-day statistics** – Shows completion percentages for individual workgroups.
- **Real-time updates** – Uses SignalR for live production updates.
- **Data visualization** – Presents production information through charts and visual indicators.

## Tech Stack

**Frontend**
- React
- JavaScript
- Vite
- SCSS
- Chart.js

**Backend**
- C#
- ASP.NET Core (.NET 9)
- Dapper
- Microsoft SQL Server
- SignalR

## Project Structure

```text
PT-Dashboard/
├── client/     # React frontend
└── server/     # ASP.NET Core backend
```

## Demo Mode

The frontend includes mock production data, allowing the dashboard to run without a database connection or backend service.

Demo mode is enabled for production builds through `client/.env.production`.

## Getting Started

**Frontend**

```bash
cd client
npm install
npm run dev
```

**Backend**

```bash
dotnet run --project server/server.csproj
```

The backend requires a compatible SQL Server database and appropriate connection settings.

## About the Project

PT Dashboard demonstrates fullstack development with a focus on production monitoring, database integration, real-time communication and data visualization.

The repository contains a portfolio version with anonymized configuration and mock data.