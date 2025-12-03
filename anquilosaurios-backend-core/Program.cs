using DotNetEnv;

using System.Text;
using aquilosaurios_backend_core.Configuration;
using aquilosaurios_backend_core.Application;
using aquilosaurios_backend_core.Infrastructure.External;
using aquilosaurios_backend_core.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

Env.Load();

builder.Configuration.AddEnvironmentVariables();

// Condicional pago PayPal
//if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("PAYMENT_PROVIDERS_PAYPAL_CLIENTID")) &&
//    !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("PAYMENT_PROVIDERS_PAYPAL_SECRET")))
//{
//    var paypalClientId = Environment.GetEnvironmentVariable("PAYMENT_PROVIDERS_PAYPAL_CLIENTID");
//    var paypalSecret = Environment.GetEnvironmentVariable("PAYMENT_PROVIDERS_PAYPAL_SECRET");
//    var paypalProvider = new PaypalProvider(paypalClientId, paypalSecret);
//    builder.Services.AddSingleton<IPaymentProvider>(paypalProvider);
//}

// Condicional pago Stripe
//if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("PAYMENT_PROVIDERS_STRIPE_APIKEY")))
//{
//    var stripeApiKey = Environment.GetEnvironmentVariable("PAYMENT_PROVIDERS_STRIPE_APIKEY");
//    var stripeProvider = new StripeProvider(stripeApiKey);
//    builder.Services.AddSingleton<IPaymentProvider>(stripeProvider);
//}

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthProvider, LocalAuthProvider>();
builder.Services.AddSingleton<IJwtService, JwtService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();


var jwtKey = builder.Configuration["Jwt:Key"]!;
var jwtIssuer = builder.Configuration["Jwt:Issuer"]!;
var jwtAudience = builder.Configuration["Jwt:Audience"]!;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontendApps", policy =>
    {
        policy.WithOrigins(
            "https://anquilosaurios-development-frontend-bwcbgzf6byefdthz.eastus-01.azurewebsites.net",
            "https://anquilosaurios-development-webgl-a3ewf7dehzgugtbn.eastus-01.azurewebsites.net"
            )
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.Configure<MongoSettings>(options =>
{
    options.ConnectionString = builder.Configuration["ConnectionStrings:MongoDB"]!;
    options.DatabaseName = builder.Configuration["MongoDB:DatabaseName"]!;
});

builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var mongo = sp.GetRequiredService<IOptions<MongoSettings>>().Value;
    return new MongoClient(mongo.ConnectionString);
});

builder.Services.AddScoped(sp =>
{
    var mongo = sp.GetRequiredService<IOptions<MongoSettings>>().Value;
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase(mongo.DatabaseName);
});

builder.Services.AddScoped<IUserRepository, UserRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontendApps");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
await app.RunAsync();

public static partial class Program { }