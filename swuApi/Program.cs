using swuApi.Models;
using swuApi.Repositories;
using swuApi.Services;

var builder = WebApplication.CreateBuilder(args);

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
builder.Services.AddScoped<IPackOpeningRepository, CardRepository>(provider =>
    new CardRepository(connectionString));

builder.Services.AddScoped<IRepository<Collection>, CollectionRepository>(provider =>
    new CollectionRepository(connectionString!));

builder.Services.AddScoped<IRepository<Pack>, PackRepository>(provider =>
    new PackRepository(connectionString!));

builder.Services.AddScoped<IRepository<User>, UserRepository>(provider =>
    new UserRepository(connectionString!));

builder.Services.AddScoped<IUserCardRepository, UserCardRepository>(provider =>
    new UserCardRepository(connectionString!));

// Dependencias Services
builder.Services.AddScoped<IService<Card>, CardService>();
builder.Services.AddScoped<IService<Collection>, CollectionService>();
builder.Services.AddScoped<IService<Pack>, PackService>();
builder.Services.AddScoped<IService<User>, UserService>();
builder.Services.AddScoped<IUserCardService, UserCardService>();


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