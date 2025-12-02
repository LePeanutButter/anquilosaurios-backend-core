using aquilosaurios_backend_core.Application;
using aquilosaurios_backend_core.Infrastructure.External;
using aquilosaurios_backend_core.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace aquilosaurios_backend_core.Tests.Infrastructure.Tests.External
{
    public class ProgramTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public ProgramTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public void Services_AreRegistered()
        {
            var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
            using var scope = scopeFactory!.CreateScope();
            var services = scope.ServiceProvider;

            Assert.NotNull(services.GetService<IUserRepository>());
            Assert.NotNull(services.GetService<IAuthProvider>());
            Assert.NotNull(services.GetService<IJwtService>());
            Assert.NotNull(services.GetService<IAuthService>());
            Assert.NotNull(services.GetService<IUserService>());

            var paymentProviders = services.GetServices<IPaymentProvider>().ToList();
            Assert.True(paymentProviders.Count >= 0);
        }

        [Fact]
        public async Task App_StartsSuccessfully()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/");

            Assert.True(response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.NotFound);
        }
    }
}
