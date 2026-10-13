# Prácticas LLM + .NET + Ollama local

Repo de aprendizaje: conectar .NET con modelos locales (Ollama) sin depender de APIs pagas.

## Entorno
- .NET 8 (`dotnet --version`)
- Ollama en `http://localhost:11434` (`ollama list`, `ollama ps`)
- Modelos probados: `llama3.2:3b` (default), `granite3.3:2b`, `qwen3.5:9b`

## Estructura
```
PracticasLLM.sln
src/
  Practica1.Chat/   # Chat consola con HttpClient puro
  # Practica2.* (pendiente: structured output)
  # Practica3.* (pendiente: tool calling)
  # Practica4.* (pendiente: embeddings + RAG)
```

## Práctica 1 — Chat desde C# (hecha)
Consola que mantiene historial y habla con `POST /api/chat` (`stream: false`).

```bash
dotnet run --project src/Practica1.Chat
```

Comandos: `/salir` · `/limpiar` · `/modelo <nombre>`

Arquitectura: `Program.cs` (presentación/loop) → `OllamaClient.cs` (infra/HTTP) → `Models.cs` (records).
Sin NuGets a propósito, para aprender el protocolo real.

## Nota de rendimiento
Si la primera respuesta tarda mucho (>60s), es carga en frío del modelo.
Probá de nuevo sin debugger (`dotnet run`) y compará. Ver modelos cargados con `ollama ps`.
