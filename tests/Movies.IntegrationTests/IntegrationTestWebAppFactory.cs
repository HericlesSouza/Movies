using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Movies.Infrastructure.Persistence;

using Testcontainers.PostgreSql;

namespace Movies.IntegrationTests;

public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    // 1. Definição do Contentor Postgres
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithDatabase("movies")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    // 2. Configuração da API para usar o Contentor
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            // Encontra e remove a configuração antiga do DbContext (do appsettings)
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

            if (descriptor != null)
                services.Remove(descriptor);

            // Adiciona a nova configuração apontando para o Testcontainer
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(_dbContainer.GetConnectionString());
            });
        });
    }

    // 3. Ciclo de Vida do Contentor
    public async Task InitializeAsync()
    {
        // 1. Inicia o contentor
        await _dbContainer.StartAsync();

        // 2. Cria as opções de conexão apontando para o contentor que acabou de subir
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(_dbContainer.GetConnectionString())
            .Options;

        // 3. Instancia o contexto manualmente e aplica as migrations
        using var context = new AppDbContext(options);
        await context.Database.MigrateAsync();
    }

    public new Task DisposeAsync() => _dbContainer.StopAsync();
}
