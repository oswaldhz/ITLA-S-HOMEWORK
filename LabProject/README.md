# LabProject - Gestión de Laboratorio de Computación (Reservas)

Este repositorio incluye:

- **Backend API (ASP.NET Core)**: `LabProject.Api`
- **Capa de Aplicación**: `LabProject.Application`
- **Dominio**: `LabProject.Domain`
- **Infraestructura (EF Core + SQL Server)**: `LabProject.Infrastructure`
- **Frontend (React + Chakra UI)**: `frontend/`
- **Frontend (Blazor Server)**: proyecto `LabProject.Web` (incluido en la solución)

## Requisitos

- .NET SDK 8
- SQL Server (o SQL Express) + SSMS
- Node.js 18+ (solo para el frontend React)

## Ejecutar el backend (API)

```bash
cd LabProject
dotnet restore
dotnet run --project LabProject.Api
```

La API expone endpoints en `/api/*` y usa JWT.

> Nota: por defecto se aplican migraciones al iniciar (`Database.Migrate()`).

## Ejecutar el frontend React

```bash
cd LabProject/frontend
npm install
npm run dev
```

Por defecto apunta a `https://localhost:7095/api`. Puedes sobreescribirlo creando un `.env` basado en `.env.example`.

## Ejecutar el frontend Blazor Server

```bash
cd LabProject
dotnet run
```

El proyecto Blazor consume la misma API.

## Credenciales de prueba (seed)

- Admin: `admin@lab.com` / `Admin123!`
- Asistente: `asistente@lab.com` / `Asistente123!`
- Estudiante: `estudiante@lab.com` / `Estudiante123!`
