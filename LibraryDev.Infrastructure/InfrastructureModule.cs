using LibraryDev.Domain.Interfaces.Avaliacoes;
using LibraryDev.Domain.Interfaces.Livros;
using LibraryDev.Domain.Interfaces.Relatorios;
using LibraryDev.Domain.Interfaces.Usuarios;
using LibraryDev.Domain.Services;
using LibraryDev.Infrastructure.Repositories.Avaliacoes;
using LibraryDev.Infrastructure.Repositories.Livros;
using LibraryDev.Infrastructure.Repositories.Relatorios;
using LibraryDev.Infrastructure.Repositories.Usuarios;
using LibraryDev.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LibraryDev.Infrastructure;

public static class InfrastructureModule
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddInfrastructure(IConfiguration configuration)
        {
            services
                .AddRepositories();
            return services;
        }

        private IServiceCollection AddRepositories()
        {
            // Livro
            services.AddScoped<ILivroCommandRepository, LivroCommandRepository>();
            services.AddScoped<ILivroQueryRepository, LivroQueryRepository>();

            // Usuario
            services.AddScoped<IUsuarioCommandRepository, UsuarioCommandRepository>();
            services.AddScoped<IUsuarioQueryRepository, UsuarioQueryRepository>();

            // Avaliacao
            services.AddScoped<IAvaliacaoCommandRepository, AvaliacaoCommandRepository>();
            services.AddScoped<IAvaliacaoQueryRepository, AvaliacaoQueryRepository>();

            // Relatorio
            services.AddScoped<IRelatorioQueryRepository, RelatorioQueryRepository>();

            // External API
            services.AddHttpClient<IOpenLibraryService, OpenLibraryService>(client =>
            {
                client.DefaultRequestHeaders.Add("User-Agent", "LibraryDev/1.0");
                client.Timeout = TimeSpan.FromSeconds(10);
            });

            // Email
            services.AddScoped<IEmailService, EmailService>();

            return services;
        }
    }
}
