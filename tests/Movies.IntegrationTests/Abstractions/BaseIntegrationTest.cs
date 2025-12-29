using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Movies.Infrastructure.Persistence;

namespace Movies.IntegrationTests.Abstractions;

public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestWebAppFactory>, IAsyncLifetime
{
    protected readonly IServiceScopeFactory ScopeFactory;
    protected readonly HttpClient Client;
    private readonly Func<Task> _resetDatabase;

    public BaseIntegrationTest(IntegrationTestWebAppFactory factory)
    {
        ScopeFactory = factory.Services.GetRequiredService<IServiceScopeFactory>();
        Client = factory.CreateClient();

        _resetDatabase = async () =>
        {
            using var scope = ScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await context.Movies.ExecuteDeleteAsync();
        };
    }

    public Task DisposeAsync() => Task.CompletedTask;

    public async Task InitializeAsync() => await _resetDatabase();
}
