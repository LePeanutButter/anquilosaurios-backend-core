using aquilosaurios_backend_core.Infrastructure.External;
using DotNetEnv;

using System.Text;
using aquilosaurios_backend_core.Application;
using aquilosaurios_backend_core.Infrastructure.External.AuthProviders;
using aquilosaurios_backend_core.Infrastructure.External.JwtAuth;
using aquilosaurios_backend_core.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

// Add services to the container.
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

Env.Load();

// Condicional pago PayPal
if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("PAYMENT_PROVIDERS_PAYPAL_CLIENTID")) &&
    !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("PAYMENT_PROVIDERS_PAYPAL_SECRET")))
{
    var paypalClientId = Environment.GetEnvironmentVariable("PAYMENT_PROVIDERS_PAYPAL_CLIENTID");
    var paypalSecret = Environment.GetEnvironmentVariable("PAYMENT_PROVIDERS_PAYPAL_SECRET");
    var paypalProvider = new PaypalProvider(paypalClientId, paypalSecret);
    builder.Services.AddSingleton<IPaymentProvider>(paypalProvider);
}

// Condicional pago Stripe
if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("PAYMENT_PROVIDERS_STRIPE_APIKEY")))
{
    var stripeApiKey = Environment.GetEnvironmentVariable("PAYMENT_PROVIDERS_STRIPE_APIKEY");
    var stripeProvider = new StripeProvider(stripeApiKey);
    builder.Services.AddSingleton<IPaymentProvider>(stripeProvider);
}


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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();