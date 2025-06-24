# C# Redis Key‑Value Store (CLI)

A tiny laboratory project that demonstrates how to build a **key‑value storage** in C# which persists data in **Redis** and offers a colourful text‑user‑interface powered by [Spectre.Console](https://spectreconsole.net/).

---

## 🛠 Prerequisites

| Tool | Required version | Notes |
|------|------------------|-------|
| [.NET SDK](https://visualstudio.microsoft.com/en/downloads/) | 8.0 or newer | installed via Visual Studio Installer or standalone |
| [Docker Desktop](https://docs.docker.com/desktop/) | latest | includes Docker Engine & Compose |

> ⚠️ Docker is only needed to spin‑up the Redis container. The C# application itself runs on your host.

---

## 🚀 To build & run project:


```bash
dotnet build

docker compose up -d
docker attach $(docker compose ps -q kvstore) # attach to the application’s interactive console
```


## 🖥️ CLI Commands

| Command (exact text) | Action |
|----------------------|--------|
| **Show All** | Displays every key / value currently stored |
| **Add** | Prompts for key & value and stores them (overwrites if key exists) |
| **Find** | Look up a single key and print its value |
| **Delete** | Remove a single key (error if key missing) |
| **Remove All** | Wipes the entire store after confirmation |
| **Clear Console** | Clears the terminal buffer |
| **Exit** | Gracefully stops the application |
| *(Ctrl +C)* | Abruptly terminates – not recommended |
