using Microsoft.OpenApi.Models;
using MinApi_WordSearch.Endpoints;
using MinApi_WordSearch.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "MinApi_Sopa de Letras",
        Version = "v1",
        Description = "API que busca palabras en una sopa de letras (N x N)."
    });
});

builder.Services.AddCors(options =>{options.AddPolicy("DevAllowAll", policy =>{policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();});});
builder.Services.AddScoped<IWordSearchService, WordSearchService>();        /// Registrar servicio ID Define el contrato; clases dependen de la interfaz.
builder.WebHost.UseUrls("http://localhost:5000", "https://localhost:5001");/// Forzar puertos 

WebApplication app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("DevAllowAll");/// Aplicar CORS antes de mapear endpoints
app.UseHttpsRedirection();
app.MapEndpoints();

Console.WriteLine("API en:");
Console.WriteLine("Swagger UI (HTTP):  http://localhost:5000/swagger");
Console.WriteLine("Swagger UI (HTTPS): https://localhost:5001/swagger");

app.Run();




/*
crea builder config  injeccion de dep y los servicios del api 
registramos el swa
app.MapEndpoints(); mapeo de todas las ruttas del api 
*/