# Anquilosaurios - Backend Core

[![standard-readme compliant](https://img.shields.io/badge/readme%20style-standard-brightgreen.svg?style=flat-square)](https://github.com/RichardLitt/standard-readme)

> Backend Core for a social, multiplayer web game targeting the Latin American market.

This repository contains the backend service for a multiplayer web application built with ASP.NET Core. It provides the business logic, RESTful API endpoints, and database connectivity required to support user authentication, lobby management, data persistence, and analytics.

---

## Table of Contents

- [Background](#background)
- [Install](#install)
- [Usage](#usage)
- [API](#api)
- [Architecture](#architecture)
- [Tech Stack](#tech-stack)
- [Tools](#tools)
- [Maintainers](#maintainers)
- [License](#license)

---

## Background

The project addresses the lack of accessible, free, browser-based games optimized for low-end devices and unstable internet connections.

---

## Install

> Requirements:
>
> - [.NET 8+ SDK](https://dotnet.microsoft.com/)
> - [Git](https://git-scm.com/)
> - Optional: Azure CLI for deployment

Clone the repository and restore dependencies:

```bash
git clone https://github.com/LePeanutButter/anquilosaurios-backend-core.git
cd anquilosaurios-backend-core
dotnet restore
```

To run the backend locally:

```bash
dotnet run
```

The API will be available at **`https://localhost:5000`** (or configured port).

## Usage

This backend is meant to serve:

- Player data logging and analytics
- Feedback collection

- It works together with the Unity WebGL frontend, communicating over a REST API (future versions may incorporate WebSockets or SignalR for real-time communication).

## API

Full API documentation is in progress and will be published via **Swagger**.

## Architecture

- **Backend:** ASP.NET Core REST API
- **Database:** MongoDB
- **Deployment:** Azure App Services
- **Security:** HTTPS, JWT tokens for session auth
- **Tools:** GitHub, Azure DevOps, Miro

Architecture diagrams and business flows are maintained in **Miro** (link available on request).

## Tech Stack

- **Language:** C#
- **Framework:** ASP.NET Core
- **Database:** MongoDB
- **Auth:** JWT
- **CI/CD:** GitHub Actions
- **Cloud:** Azure App Services and Virtual Machines
- **Monitoring:** Azure Health Probes

## Tools

| Tool         | Purpose                                  |
| ------------ | ---------------------------------------- |
| Miro         | Business logic and architecture diagrams |
| Azure        | Cloud deployment and storage             |
| GitHub       | Version control and collaboration        |
| Azure DevOps | CI/CD pipelines and project tracking     |

## Maintainers

- [Lanapequin](https://github.com/Lanapequin) - Laura Natalia Perilla Quintero
- [LePeanutButter](https://github.com/LePeanutButter) - Santiago Botero Garcia
- [shiro](https://github.com/JoseDavidCastillo) - Jose David Castillo Rodriguez

## License

[MIT](/LICENSE) © Anquilosaurios Team

---

This **README** follows the Standard Readme specification.
