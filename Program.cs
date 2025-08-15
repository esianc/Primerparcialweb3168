using System.Globalization;
using System.Text;
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapPost("/codigo", (ClientRequest req) =>
{
    var errorMessage = Validar(req);
    if (errors.Count > 0)
    {
        return Results.BadRequest(new { errors });
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
