using System.Text.Json.Serialization;

namespace Practica1.Chat;

// Mensaje en formato Ollama: { "role": "system|user|assistant", "content": "..." }
public sealed record ChatMessage(
    [property: JsonPropertyName("role")] string Role,
    [property: JsonPropertyName("content")] string Content)
{
    public static ChatMessage System(string content) => new("system", content);
    public static ChatMessage User(string content) => new("user", content);
    public static ChatMessage Assistant(string content) => new("assistant", content);
}

// Request POST /api/chat
public sealed record ChatRequest(
    [property: JsonPropertyName("model")] string Model,
    [property: JsonPropertyName("messages")] List<ChatMessage> Messages,
    [property: JsonPropertyName("stream")] bool Stream = false);

// Response (stream:false): { "model": "...", "message": {...}, "done": true, ... }
// Ignoramos el resto de campos (created_at, timings, etc.)
public sealed record ChatResponse(
    [property: JsonPropertyName("model")] string Model,
    [property: JsonPropertyName("message")] ChatMessage Message,
    [property: JsonPropertyName("done")] bool Done);
