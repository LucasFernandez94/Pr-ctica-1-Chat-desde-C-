using System.Net.Http.Json;
using System.Text.Json;

namespace Practica1.Chat;

// Capa infraestructura: solo sabe hablar HTTP con Ollama. No sabe de consola.
public sealed class OllamaClient : IDisposable
{
    private readonly HttpClient _http;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        WriteIndented = false
    };

    public string Model { get; set; }

    public OllamaClient(string baseUrl = "http://localhost:11434", string model = "llama3.2:3b", TimeSpan? timeout = null)
    {
        _http = new HttpClient
        {
            BaseAddress = new Uri(baseUrl),
            Timeout = timeout ?? TimeSpan.FromMinutes(5)
        };
        Model = model;
    }

    // Envía el historial completo y devuelve el texto del assistant.
    public async Task<string> ChatAsync(List<ChatMessage> history, CancellationToken ct = default)
    {
        var request = new ChatRequest(Model, history, Stream: false);

        HttpResponseMessage response;
        try
        {
            response = await _http.PostAsJsonAsync("/api/chat", request, JsonOptions, ct);
        }
        catch (HttpRequestException ex)
        {
            throw new InvalidOperationException(
                "No pude conectar con Ollama en http://localhost:11434. ¿Está corriendo? (ollama serve / ollama ps)", ex);
        }
        catch (TaskCanceledException ex)
        {
            throw new TimeoutException($"Ollama tardó demasiado con el modelo '{Model}'. Probá con un modelo más chico.", ex);
        }

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(ct);
            throw new InvalidOperationException($"Ollama devolvió {(int)response.StatusCode}: {body}");
        }

        var chat = await response.Content.ReadFromJsonAsync<ChatResponse>(JsonOptions, ct);
        if (chat?.Message?.Content is not { Length: > 0 } content)
            throw new InvalidOperationException("Ollama devolvió una respuesta vacía.");

        return content.Trim();
    }

    public void Dispose() => _http.Dispose();
}
