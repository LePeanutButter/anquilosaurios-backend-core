using aquilosaurios_backend_core.Infrastructure.External;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Load environment variables from .env file
Env.Load();

// Conditionally register PayPal and Stripe providers based on the existence of configuration values
if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("PAYMENT_PROVIDERS_PAYPAL_CLIENTID")) &&
    !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("PAYMENT_PROVIDERS_PAYPAL_SECRET")))
{
    // Register PayPal provider only if both ClientId and Secret are provided
    var paypalClientId = Environment.GetEnvironmentVariable("PAYMENT_PROVIDERS_PAYPAL_CLIENTID");
    var paypalSecret = Environment.GetEnvironmentVariable("PAYMENT_PROVIDERS_PAYPAL_SECRET");
    var paypalProvider = new PaypalProvider(paypalClientId, paypalSecret);
    builder.Services.AddSingleton<IPaymentProvider>(paypalProvider);
}

if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("PAYMENT_PROVIDERS_STRIPE_APIKEY")))
{
    // Register Stripe provider only if ApiKey is provided
    var stripeApiKey = Environment.GetEnvironmentVariable("PAYMENT_PROVIDERS_STRIPE_APIKEY");
    var stripeProvider = new StripeProvider(stripeApiKey);
    builder.Services.AddSingleton<IPaymentProvider>(stripeProvider);
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
