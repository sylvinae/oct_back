using System.Net;
using System.Text;
using API.Data;
using API.Middlewares;
using API.Services;
using API.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddLogging();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register DBContext
builder.Services.AddDbContext<ApiDbContext>();

// Register services
// Is a mess rn, i'll fix soon promise
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ItemService>();
builder.Services.AddScoped<RestockService>();

// Newtonsoft.Json Configuration
builder
    .Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ContractResolver = new ConditionalJsonIgnoreContractResolver();
        // Use your custom resolver
    });

// JWT Configuration
var secret = DotNetEnv.Env.GetString("JWT_SECRET");
if (!builder.Environment.IsDevelopment()) // Check if not in Development
{
    builder
        .Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                ValidIssuer = DotNetEnv.Env.GetString("JWT_ISSUER"),
            };
        });
}

// Cors
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowAllOrigins",
        builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
    );
});

// Kestrel Configuration
var port = DotNetEnv.Env.GetInt("PORT");
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(
        port,
        options =>
        {
            options.UseHttps();
        }
    );
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowAllOrigins"); // Ensure CORS is applied before authentication/authorization
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
