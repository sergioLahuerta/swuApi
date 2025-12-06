using swuApi.Repositories;
using swuApi.Models;
using swuApi.Services; // Necesario para registrar los Servicios

var builder = WebApplication.CreateBuilder(args);

// --- 1. CONFIGURACIÓN DE SERVICIOS ---

// Configuración de servicio CORS global
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Connection string, mismo que en appsettings
var connectionString = builder.Configuration.GetConnectionString("SWUPersonalApi");

// Dependencias Repositories
builder.Services.AddScoped<IRepository<Card>, CardRepository>(provider =>
    new CardRepository(connectionString!));

builder.Services.AddScoped<IRepository<Colection>, ColectionRepository>(provider =>
    new ColectionRepository(connectionString!));

// Dependencias Services
builder.Services.AddScoped<IService<Card>, CardService>();
builder.Services.AddScoped<IService<Colection>, ColectionService>();


// Otros servicios del Framework
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseStaticFiles();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MyAPI");
        c.InjectStylesheet("/swagger/SwaggerDark.css");
    });
}

app.UseCors();
app.UseAuthorization();

app.MapGet("/", context =>
{
    context.Response.Redirect("/swagger");
    return Task.CompletedTask;
});

app.MapControllers();
app.Run();