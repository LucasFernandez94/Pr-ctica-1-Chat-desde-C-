using System.Diagnostics;
using Practica1.Chat;

const string defaultModel = "llama3.2:3b";

using var client = new OllamaClient(model: defaultModel);
var history = new List<ChatMessage>
{
    ChatMessage.System("Sos un asistente breve y claro. Respondé en español rioplatense, en 3 líneas como máximo salvo que te pidan más detalle.")
};

Console.WriteLine($"=== Práctica 1 · Chat con Ollama ({client.Model}) ===");
Console.WriteLine("Comandos: /salir · /limpiar · /modelo <nombre> (ej: /modelo qwen3.5:9b)");
Console.WriteLine();

while (true)
{
    Console.Write("Vos > ");
    var input = Console.ReadLine()?.Trim();
    if (string.IsNullOrWhiteSpace(input))
        continue;

    // --- comandos locales (no van al modelo) ---
    if (input.Equals("/salir", StringComparison.OrdinalIgnoreCase))
        break;

    if (input.Equals("/limpiar", StringComparison.OrdinalIgnoreCase))
    {
        history.RemoveAll(m => m.Role != "system");
        Console.WriteLine("(historial limpio)\n");
        continue;
    }

    if (input.StartsWith("/modelo ", StringComparison.OrdinalIgnoreCase))
    {
        client.Model = input["/modelo ".Length..].Trim();
        Console.WriteLine($"(modelo -> {client.Model})\n");
        continue;
    }

    history.Add(ChatMessage.User(input));

    try
    {
        var sw = Stopwatch.StartNew();
        Console.WriteLine("IA pensando...");
        var reply = await client.ChatAsync(history);
        sw.Stop();

        history.Add(ChatMessage.Assistant(reply));
        Console.WriteLine($"\nIA [{client.Model} · {sw.Elapsed.TotalSeconds:F1}s] > {reply}\n");
    }
    catch (Exception ex)
    {
        // Sacamos el user message que falló para no contaminar el historial
        history.RemoveAt(history.Count - 1);
        Console.WriteLine($"\n[error] {ex.Message}\n");
    }
}

Console.WriteLine("Chau!");
