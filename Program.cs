var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Método GET: http://localhost:5000/hola

// Método POST: http://localhost:5000/codigo
app.MapPost("/hola", async (HttpRequest request) =>
{
using var reader = new StreamReader(request.Body);
var body = await reader.ReadToEndAsync();

return Results.Ok($"¡Hola Mundo desde POST! Recibido: {body}");
});

app.Run();
