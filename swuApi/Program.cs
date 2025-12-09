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

// Deserializar para que los enums no devuelvan el binario al que corresponde su valor sino el string que representa ese binario
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

// Dependencias Repositories
builder.Services.AddScoped<IRepository<Collection>, CollectionRepository>(provider =>
    new CollectionRepository(connectionString!));

builder.Services.AddScoped<IRepository<Pack>, PackRepository>(provider =>
    new PackRepository(connectionString!));

builder.Services.AddScoped<IPackOpeningRepository, CardRepository>(provider =>
    new CardRepository(connectionString));

builder.Services.AddScoped<IUserRepository, UserRepository>(provider =>
    new UserRepository(connectionString!));

builder.Services.AddScoped<IUserCardRepository, UserCardRepository>(provider =>
    new UserCardRepository(connectionString!));

builder.Services.AddScoped<IRepository<Review>, ReviewRepository>(provider =>
    new ReviewRepository(connectionString!));

// Dependencias Services
builder.Services.AddScoped<IService<Card>, CardService>();
builder.Services.AddScoped<IService<Collection>, CollectionService>();
builder.Services.AddScoped<IService<Pack>, PackService>();
builder.Services.AddScoped<IPackOpeningService, PackOpeningService>();
builder.Services.AddScoped<IService<User>, UserService>();
builder.Services.AddScoped<IUserCardService, UserCardService>();
builder.Services.AddScoped<IService<Review>, ReviewService>();


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