using swuApi.Repositories;
using swuApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Configuración de servicio CORS global (muy importante)
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Connection string:
var connectionString = builder.Configuration.GetConnectionString("SWUPersonalApi");

builder.Services.AddScoped<IRepository<Card>, CardRepository>(provider =>
new CardRepository(connectionString));

// builder.Services.AddScoped<IColeccionRepository, ColeccionRepository>(provider =>
// new ColeccionRepository(connectionString));

// Otros servicios
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Pipeline de la app
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

// ¡CORS debe ir aquí, antes de endpoints y autorización!
app.UseCors();

app.UseAuthorization();

app.MapGet("/", context =>
{
    context.Response.Redirect("/swagger");
    return Task.CompletedTask;
});

app.MapControllers();
app.Run();