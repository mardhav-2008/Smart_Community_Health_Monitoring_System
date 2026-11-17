# Smart Community Health Monitoring and Early Warning System

<p align="center">
  <strong>An integrated platform for community-level water quality monitoring and health surveillance in rural Northeast India.</strong>
</p>

<p align="center">
  <img src="https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet&logoColor=white" />
  <img src="https://img.shields.io/badge/ASP.NET_Core-MVC-512BD4?logo=dotnet&logoColor=white" />
  <img src="https://img.shields.io/badge/Database-SQLite-003B57?logo=sqlite&logoColor=white" />
  <img src="https://img.shields.io/badge/ORM-Entity_Framework_Core-512BD4" />
  <img src="https://img.shields.io/badge/Status-Prototype-8FA68C" />
</p>

---

## Overview

The **Smart Community Health Monitoring and Early Warning System** is a web-based platform designed to organize community information, monitor local water sources, collect aggregated health reports, and identify potential patterns that may warrant public-health investigation.

The system brings environmental monitoring and community-level health reporting together through a centralized application.

It aims to support earlier identification of potential water-related public-health risks, particularly in rural communities across Northeast India.

## Problem Statement

Rural communities can face challenges in maintaining consistent water-quality records and identifying emerging patterns in community health reports.

Information about water sources, environmental observations, and reported illnesses may be fragmented across different records and reporting processes.

This project addresses the need for a centralized system that can:

- Maintain structured community information.
- Register and monitor local water sources.
- Record water-quality observations.
- Collect aggregated community health reports.
- Identify potential reporting patterns.
- Generate alerts to support further investigation.

## Key Features

### 1. Community Registry

A centralized directory for managing community information.

- Register and edit communities.
- Store district and state information.
- Record geographic coordinates.
- Maintain approximate population information and reference year.
- Search and filter community records.
- View detailed community information.
- Archive and restore communities without permanently deleting records.
- Display community locations using an interactive map.

### 2. Water Source Monitoring

Manage water infrastructure associated with registered communities.

- Register wells, hand pumps, springs, taps, and storage tanks.
- Associate water sources with communities.
- Store geographic coordinates.
- Activate or deactivate water sources.
- Record water-quality observations.
- Maintain observation timestamps and validation status.
- View historical observations for individual sources.

Observation records include parameters, measured values, units, observation timestamps, recording timestamps, and validation status.

### 3. Community Health Reporting

A structured mechanism for collecting aggregated community-level health information.

- Associate reports with registered communities.
- Record health-report categories.
- Submit aggregated report counts.
- Maintain reporting dates and source information.
- Support administrator verification.

The system focuses on community-level reporting rather than individual patient medical records.

### 4. Early-Warning Alerts

A rule-based mechanism for identifying potential patterns in verified community health reports.

- Generate alerts based on recent reporting patterns.
- Associate alerts with affected communities.
- Assign alert severity.
- Track alert review and resolution.
- Support administrator-controlled status updates.

Available severity levels:

| Severity | Description |
|---|---|
| Informational | Informational notification |
| Low | Low-priority indication |
| Moderate | Pattern requiring attention |
| High | Higher-priority indication |
| Critical | Highest-priority classification |

Available alert statuses:

- New
- Under Review
- Confirmed
- Dismissed
- Resolved

**Important:** The current alert-generation mechanism is a demonstration rule-based prototype. It is not a clinically validated prediction model, trained AI system, or diagnostic tool.

### 5. Monitoring Dashboard

A centralized overview of monitoring information, including:

- Registered water sources.
- Health-report summaries.
- Active alerts.
- Recent water observations.
- Community monitoring information.

### 6. Authentication and User Management

The application uses ASP.NET Core Identity with role-based authorization.

**Administrator**
- Manage application users.
- Assign user roles.
- Activate and deactivate accounts.
- Archive and restore communities.
- Verify health reports.
- Review and update alerts.

**Field Worker**
- View and manage community information.
- Register water sources.
- Record water-quality observations.
- Submit aggregated health reports.
- Access monitoring information.

Administrative operations are protected through authorization policies.

---

## Technology Stack

| Category | Technology |
|---|---|
| Programming language | C# |
| Framework | ASP.NET Core MVC |
| Runtime | .NET 10 |
| ORM | Entity Framework Core 10 |
| Database | SQLite |
| Authentication | ASP.NET Core Identity |
| Frontend | HTML, CSS, JavaScript |
| UI framework | Bootstrap |
| Mapping library | Leaflet.js |
| Map data | OpenStreetMap |

## System Architecture

The application follows a layered ASP.NET Core MVC architecture.

```text
                    USER INTERFACE
                           |
             ASP.NET Core MVC Views
                           |
                           v
                  APPLICATION LAYER
                           |
                  MVC Controllers
                           |
          +----------------+----------------+
          |                |                |
     Community         Monitoring        Identity
     Management        Workflows         & Security
          |                |                |
          +----------------+----------------+
                           |
                           v
                    DATA ACCESS
                           |
                  Entity Framework Core
                           |
                           v
                     SQLite Database
                           |
          +----------------+----------------+
          |                |                |
      Communities     Water Sources     Health Reports
                           |
                    Water Observations
                           |
                    Early-Warning Alerts
```

### Core Data Entities

The database contains the following primary entities:

- `ApplicationUser`
- `Community`
- `WaterSource`
- `WaterObservation`
- `HealthReport`
- `EarlyWarningAlert`

Entity relationships are managed through Entity Framework Core and configured using the application's database context.

---

## Project Structure

```text
Smart_Community_Health_Monitoring_System/
│
├── data/
│   └── community-health.db
│
├── src/
│   └── Smart_Community_Health_Monitoring_System/
│       │
│       ├── Areas/
│       │   ├── Admin/
│       │   │   ├── Controllers/
│       │   │   ├── ViewModels/
│       │   │   └── Views/
│       │   │
│       │   ├── Communities/
│       │   │   ├── Controllers/
│       │   │   ├── ViewModels/
│       │   │   └── Views/
│       │   │
│       │   ├── Identity/
│       │   │   └── Pages/
│       │   │
│       │   └── Monitoring/
│       │       ├── Controllers/
│       │       └── Views/
│       │
│       ├── Controllers/
│       ├── Data/
│       ├── Migrations/
│       ├── Models/
│       │   ├── Entities/
│       │   └── Identity/
│       │
│       ├── Views/
│       │   ├── Home/
│       │   └── Shared/
│       │
│       ├── wwwroot/
│       │   ├── css/
│       │   ├── js/
│       │   └── lib/
│       │
│       ├── Program.cs
│       └── Smart_Community_Health_Monitoring_System.csproj
│
├── tests/
│
└── Smart_Community_Health_Monitoring_System.slnx
```

---

## Getting Started

### Prerequisites

Install the following:

- .NET 10 SDK
- Git
- SQLite (optional, for direct database inspection)

Verify the .NET installation:

```bash
dotnet --version
```

### 1. Clone the repository

```bash
git clone https://github.com/mardhav-2008/Smart_Community_Health_Monitoring_System.git
```

Navigate into the repository:

```bash
cd Smart_Community_Health_Monitoring_System
```

### 2. Restore dependencies

```bash
dotnet restore
```

### 3. Apply database migrations

Navigate to the application project:

```bash
cd src/Smart_Community_Health_Monitoring_System
```

Apply the existing Entity Framework migrations:

```bash
dotnet ef database update
```

If the Entity Framework CLI is not installed:

```bash
dotnet tool install --global dotnet-ef
```

### 4. Configure the initial administrator

The application supports initial administrator creation through a CLI setup workflow.

From the repository root, run:

```bash
dotnet run --project src/Smart_Community_Health_Monitoring_System/Smart_Community_Health_Monitoring_System.csproj -- --setup-admin
```

Follow the interactive prompts to configure the initial administrator account.

### 5. Run the application

From the repository root:

```bash
dotnet run --project src/Smart_Community_Health_Monitoring_System/Smart_Community_Health_Monitoring_System.csproj --urls http://localhost:5042
```

Open the application:

```text
http://localhost:5042
```

Sign in using the administrator account configured during initial setup.

---

## Security

The application incorporates several security mechanisms:

- ASP.NET Core Identity authentication.
- Role-based authorization.
- Administrator-only operations.
- Password complexity requirements.
- Account activation and deactivation.
- Security stamp validation.
- Protected POST operations using anti-forgery validation.
- Entity Framework Core parameterized data access.
- Foreign-key constraints and database relationships.

Public self-registration is not enabled.

## Mapping

Community locations are displayed using:

- **Leaflet.js** for interactive map rendering.
- **OpenStreetMap** for map tiles and geographic visualization.

Geographic coordinates are stored with community and water-source information where available.

---

## Current Limitations

This project is an academic prototype. Its current implementation has the following limitations:

- Water-quality observations are manually entered.
- Health information is based on submitted aggregated reports.
- Alert generation uses demonstration rules.
- Alert thresholds have not undergone clinical validation.
- The system does not independently establish a causal relationship between water quality and reported illnesses.
- No automated IoT sensor integration is currently implemented.
- The system does not perform clinical diagnosis.

## Future Scope

Potential enhancements include:

- Integration with IoT-based water-quality sensors.
- Automated periodic sensor-data collection.
- Improved statistical and machine-learning-based risk analysis.
- GIS-based spatial risk visualization.
- SMS and mobile notification support.
- Offline-first data collection for field workers.
- Integration with verified public-health datasets.
- More rigorous validation of alert-generation thresholds.
- Expanded monitoring and reporting capabilities.

---

## Team

**Lovely Professional University**

| Team Member | Registration Number |
|---|---|
| Mardhav Rayapati | 12513484 |
| Ruhani Sharma | 12514000 |
| Nitish Raj | 12522052 |

## Academic Project

Developed as an academic software engineering project focused on community-level environmental monitoring and public-health information management.

---

<p align="center">
  <strong>Water · Health · Community</strong>
  <br />
  <em>Better information. Earlier awareness. Stronger communities.</em>
</p>
