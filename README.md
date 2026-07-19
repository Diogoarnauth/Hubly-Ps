# 🖥️ Hubly

[![.NET](https://img.shields.io/badge/.NET-8.0-purple.svg)](https://dotnet.microsoft.com/)
[![Next.js](https://img.shields.io/badge/Next.js-15.3.0-black.svg)](https://nextjs.org/)
[![React](https://img.shields.io/badge/React-18.3.1-blue.svg)](https://reactjs.org/)
[![Docker](https://img.shields.io/badge/Docker-Ready-2496ED.svg)](https://docker.com/)
[![React Native](https://img.shields.io/badge/React%20Native-0.72.0-blue.svg)](https://reactnative.dev/)

> **Hubly, A network interface/web app that connects influencers with businesses, enabling seamless partnerships and collaborations, developed as part of the Project and Seminar course at ISEL 2025/2026**

## 🌟 Overview

Hubly addresses the growing challenge of managing partnership approaches and negotiation between content creators and small/medium businesses. With the expanding market of digital content, more businesses need to find partners with an established audience to grant visibility to their brand. On the other hand, content creators need to effectively monetize their audience through structured partnerships. With that, Hubly appears as a marketplace where both sides can find a structured, secure, and centralized environment to initiate, negotiate, and manage high-intent bilateral partnerships. By replacing fragmented multi-channel communication with an analytical, profile-anchored CRM ecosystem, the platform eliminates operational fatigue and provides transparent, data-driven matchings that scale collaboration efficiency.



## 🌐 How to access the project
Currently, the project runs in a local development environment. You can set up and run the entire ecosystem by following the detailed steps that can be found in this file below.

## 📂 1. Project Directory Structure & Organization
The project repository is centralized and structured according to the following directory layout to streamline the evaluation and review process:

```text
HUBLY-PS/
├── code/
│   ├── api/                     # Backend Source Code (.NET 8.0)
│   │   ├── Hubly.Api/           # REST API Controllers, Endpoints & Configurations
│   │   ├── Hubly.Domain/        # Domain Entities, Rules & Core Interfaces
│   │   ├── Hubly.Infrastructure/# Data Persistence, EF Core & Database Context
│   │   ├── Hubly.Services/      # Business Logic Implementations & Services
│   │   ├── Dockerfile           # Backend Container Configuration
│   │   └── Hubly.slnx           # XML-based Visual Studio Solution File
│   └── web/                     # Frontend Source Code (Next.js 15)
│       ├── .next/               # Next.js Build Output Directory
│       ├── app/                 # Next.js App Router (Pages, Layouts & Routing)
│       ├── components/          # Reusable UI Components (Shadcn UI, etc.)
│       ├── lib/                 # Utility Functions and Shared Helpers
│       ├── providers/           # React Context Providers (QueryClient, Themes)
│       ├── services/            # Frontend API Client Services & Fetching Logic
│       ├── Dockerfile           # Frontend Container Configuration
│       └── package.json         # Frontend Dependencies & Scripts
├── docker/                      # Specific environment configurations
├── docs/                        # Diagrams and auxiliary documentation
├── notes/                       # Development notes and scratchpads
├── docker-compose.yml           # Production Container Orchestration Blueprint
├── Hubly-Ps.sln                 # Main Visual Studio Solution File
├── Requests.http                # Integration test endpoints script
└── README.md                    # This orientation file
```

## ✨ Features

### 🔐 User Management & Identity Lifecycle
- **Stateless Secure Authentication**: Stateless token-based sessions with a strict device session cap policy (automatically invalidating the oldest token when thresholds are breached).
- **Cryptographic Password Protection**: Industry-standard security using a dedicated `IPasswordEncoder` combining hashing and unique random salts to neutralize pre-computed rainbow table compromises.
- **Asynchronous Verification Pipeline**: Secure registration process tied to an expiration-controlled numeric code system for verified email activation.

### 🔍 Multi-Criteria Discovery & Search Engine
- **Creator Profile Discovery**: Advanced multi-dimensional filtering (`CreatorSearchInputModel`) using social platform IDs, follower brackets, pricing ranges, market sectors, and a peer-reviewed rating system with anti-self-bias math validation.
- **Company Discovery**: Granular search controllers (`CompanySearchInputModel`) allowing creators to filter corporate entities by name, active industrial sectors, workforce size, and geographic countries.
- **Optimized Data Fetching**: Seamless server-side cursor and offset pagination rules applied across all search models to maintain high rendering performance.

### 💬 Messaging & Conversation Architecture
- **Flexible Participant Abstraction**: Decoupled messaging lanes natively supporting dynamic B2B and B2C conversational topologies (Company vs. Creator, Creator vs. Creator, or Company vs. Company).
- **Real-Time SignalR Stream**: Native bidirectional WebSockets integration for instantaneous message delivery, live synchronization, and real-time interface layout updates without manual browser refreshes.
- **Message Lifecycle Controls**: Dynamic unread message badges, background read-state tracking (last-read message matching), alongside full message editing badges and server-backed soft-deletes (`is_deleted`).

### 🏷️ Partnership CRM & Tagging System
- **Predefined System Baselines**: Out-of-the-box operational pipeline tracking tags to map traditional negotiation phases: `Contacted` 🔵, `Negotiating` 🟡, `Accepted` 🟢, and `Rejected` 🔴.
- **Dynamic Customization Layer**: Full user autonomy to construct unique custom tags with specific taxonomy names and hexadecimal color codes matching corporate styles.
- **Asymmetric Counterparty Isolation**: Absolute privacy during negotiation workflows. Tag assignments are entirely isolated to the local account workspace; counterparties remain completely unaware of your secret classifications.

### 🤖 Analytical Tracking & Recommendation Engine
- **Interaction Telemetry Logging**: Background tracking architecture that captures real-time profile views to construct accurate, dynamic user interest maps.
- **Role-Based Predictive Feeds**: Automated dashboard customization that avoids static layouts by dynamically promoting new profiles (Sectors, Countries, and Pricing brackets) mirroring your past search histories.
- **Data-Driven Match Optimization**: Intelligent feed generation designed to bridge high-intent business alignments based on programmatic search footprints.

### ⚡ Architectural Reliability
- **Atomic Transaction Management**: Core database operations are wrapped within a strict unit-of-work wrapper that fires automatic rollbacks on partial failures, ensuring complete data consistency.
- **Functional Error Handling**: Robust integration of the `OneOf` library pattern, eliminating raw unhandled exceptions and forcing predictable, strongly-typed API responses.


## 🧪 5. Testing the Application

### Running the Backend Test Suite
To validate isolated business logic and domain constraints, the project includes a rigorous test suite built with **xUnit** and **Moq**. To execute these tests locally outside of the Docker containers, navigate to the test directory and run the .NET CLI test command:

```bash
# If you are already inside the repository root:
cd code/api/Hubly.Services

# Run the test execution runner:
dotnet test
```

## 🚀 Quick Start

### Prerequisites
- [Docker](https://docs.docker.com/get-docker/) and [Docker Compose](https://docs.docker.com/compose/install/)
- [Node.js](https://nodejs.org/) (v18 or higher)

### ⚡ One-Command Setup

```bash
# Clone the repository
git clone https://github.com/Diogoarnauth/Hubly-Ps.git
cd Hubly-Ps

### 🌐 Web Application

```bash
cd code/web
npm install
```

### 🔧 Configuration

#### Email Service Setup
Create `api/Hubly.Api/appsettings.json`:

```json
{
    "EmailSettings": {
        "Port": 587,
        "EnableSsl": true,
        "FromName": "Hubly App",
        "ConfirmationCodeLength": 6,
        "ConfirmationCodeExpiryHours": 24,
        "FromEmail": "your-email@example.com",
        "Host": "smtp.gmail.com",
        "UserName": "your-username",
        "Password": "your-app-password"
    }
}
```

### 🏭 Production Deployment
On the root directory, run:
```bash
docker compose up --build
```
This will build and create these containers:
- DB (postgres)
- Hubly-Api
- Hubly-Nginx
- Hubly-Web

### 🛠️ Technology Stack

#### Backend
- **Framework**: ASP.NET Core 8.0
- **Language**: C#
- **Database**: PostgreSQL with Entity Framework Core

#### Frontend
- **Web**: Next.js 15.3.0 + React 18.3.1 + TypeScript
- **Styling**: Tailwind CSS + Shadcn UI

#### Infrastructure
- **Containerization**: Docker + Docker Compose
- **Reverse Proxy**: Nginx (Production layout)
- **Database Hosting**: PostgreSQL
