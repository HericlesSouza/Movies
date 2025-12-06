using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Movies.Application.Abstractions.Persistence;
using Movies.Infrastructure.Persistence;
using Movies.Infrastructure.Persistence.Repositories;

namespace Movies.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        var connectionString = config.GetConnectionString("DefaultConnection");

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });

        services.AddScoped<IMovieRepository, MovieRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
