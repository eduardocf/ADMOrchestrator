# 🧠 ADMOrchestrator

An orchestration suite that analyzes .NET dependency graphs from GitLab repositories and generates live dashboards with SVG visualizations and structured JSON output.

---

## 🚀 Features

- Clone multiple GitLab repositories
- Parse NuGet, DLL, and COM dependencies
- Generate `.dot` graphs styled like GitHub Actions
- Render SVG visualization with Graphviz
- Build and publish HTML reports and `dependencies.json`
- Save snapshots to PostgreSQL
- View results in IIS-hosted dashboard

---

## 🔧 Technologies

- .NET 8 Web App with Razor Pages and Hosted BackgroundService
- PostgreSQL via EF Core
- LibGit2Sharp for Git operations
- Graphviz CLI for rendering `.dot` files
- IIS or Kestrel-compatible deployment

---

## ⚙️ Installation & Deployment

### 1. 📦 Requirements

- Windows Server with **IIS**
- [.NET 8 Hosting Bundle](https://aka.ms/dotnet-hosting)
- PostgreSQL installed locally or on a reachable host
- Graphviz installed and added to system PATH

### 2. 🔐 PostgreSQL Setup

Create database and user:

- Database: `orchestrator_db`
- User: `postgres`
- Password: `yourpassword`
- Port: `5432`

### 3. 🧱 Project Publishing

From your project root:

```bash
dotnet restore
dotnet publish -c Release -r win-x64 --self-contained false -o ./publish
```

This creates a deployable publish/ folder.

### 4. 🌐 IIS Configuration

- Open IIS Manager
- Add new site:
- Name: OrchestratorGraphEngine
- Path: your publish/ folder
- Port: 5000 or any unused
- App Pool:
- .NET CLR: No Managed Code
- Pipeline mode: Integrated

### 5. 🔏 Permissions

Grant read/write access to App Pool identity on:
- /wwwroot/ (for graph.svg and JSON)
- /Repos/ (local clone directory)
- /OutputRepo/ (optional GitLab publisher)
