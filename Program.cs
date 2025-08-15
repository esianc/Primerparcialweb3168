using System.Globalization;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapPost("/codigo", (ClientRequest req) =>
{
    var errorMessage = Validar(req);
    if (errorMessage.Count > 0)
    {
        return Results.BadRequest(new { errorMessage });
    }

    //Quitar tildes
    var nombre = RemoveDiacritics(req.Nombre).Trim();
    var apellido = RemoveDiacritics(req.Apellido).Trim();
    var categoria = RemoveDiacritics(req.Categoria).Trim();

    //Generar el código
    string codigo =
        FirstN(nombre.ToUpperInvariant(), 1) +
        FirstN(apellido.ToUpperInvariant(), 2) +
        (req.Anoregistro % 100).ToString("D2", CultureInfo.InvariantCulture) +
        FirstN(categoria.ToUpperInvariant(), 2);

    return Results.Ok(new { codigo });
});

app.Run();

// Función para validar los datos de entrada
public record ClientRequest(
        string Nombre, string Apellido, int Anoregistro, string Categoria
        );

static List<string> Validar(ClientRequest req)
{
    var errorMessage = new List<string>();

    if (string.IsNullOrWhiteSpace(req.Nombre))
        errorMessage.Add("El nombre es obligatorio.");
    if (string.IsNullOrWhiteSpace(req.Apellido))
        errorMessage.Add("El apellido es obligatorio.");
    if (req.Anoregistro <= 0)
        errorMessage.Add("El año de registro debe ser un número positivo.");
    if (string.IsNullOrWhiteSpace(req.Categoria))
        errorMessage.Add("La categoría es obligatoria.");

    return errorMessage;
}

static string RemoveDiacritics(string text)
{
    var normalizedString = text.Normalize(NormalizationForm.FormD);
    var stringBuilder = new StringBuilder(text.Length);

    foreach (var c in normalizedString)
    {
        var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
        if (unicodeCategory != UnicodeCategory.NonSpacingMark)
        {
            stringBuilder.Append(c);
        }
    }

    return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
}

static string FirstN(string text, int n)
{
    if (string.IsNullOrEmpty(text) || n <= 0)
        return string.Empty;

    return text.Length <= n ? text : text.Substring(0, n);
}