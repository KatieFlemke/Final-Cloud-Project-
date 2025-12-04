using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using FinalProjectAPI.Data;
using FinalProjectAPI.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ---------- Services ----------

// Controllers
builder.Services.AddControllers();

// Swagger with security scheme
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Book Management API",
        Version = "v1",
        Description = "Cloud-native Book Management API with persistent storage, automated validation, and Key Vault authentication."
    });

    // API Key security scheme definition
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "x-api-key",
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Description = "API Key required for authentication",
        Scheme = "ApiKeyScheme",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "ApiKey"
        }
    };

    // Add security to Swagger
    c.AddSecurityDefinition("ApiKey", securityScheme);

    // Require API key for all endpoints
    var securityRequirement = new OpenApiSecurityRequirement
    {
        { securityScheme, new string[] { } }
    };
    c.AddSecurityRequirement(securityRequirement);
});

// Application Insights
builder.Services.AddApplicationInsightsTelemetry();

// EF Core + Azure SQL (or local SQL)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// API Key Management
if (builder.Environment.IsDevelopment())
{
    // Local dev key
    builder.Services.AddSingleton<string>("dev-local-api-key-123");
}
else
{
    // Production: retrieve from Key Vault
    var vaultUrl = builder.Configuration["KeyVault:VaultUrl"];
    if (string.IsNullOrWhiteSpace(vaultUrl))
        throw new InvalidOperationException("KeyVault:VaultUrl is not configured.");

    var secretClient = new SecretClient(new Uri(vaultUrl), new DefaultAzureCredential());
    var secret = secretClient.GetSecret("ApiKey");
    var apiKey = secret.Value.Value;

    builder.Services.AddSingleton<string>(apiKey);
}

var app = builder.Build();


// ---------- Middleware Pipeline ----------

// Swagger enabled
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.DocumentTitle = "Book Management API – Swagger UI";
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Book API V1");
    });
}

// Enforce API key on all routes
var apiKeyValue = app.Services.GetRequiredService<string>();
app.UseMiddleware<ApiKeyMiddleware>(apiKeyValue);

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
