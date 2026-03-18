using LibraryDev.Application.Interfaces;
using LibraryDev.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LibraryDev.Application;

public static class ApplicationModule
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddApplication()
        {
            services
                .AddServices();

            return services;
        }

        private IServiceCollection AddServices()
        {
            services.AddScoped<ILivroService, LivroService>();
            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddScoped<IAvaliacaoService, AvaliacaoService>();
            //services.AddScoped<IRelatorioService, RelatorioService>();
            return services;
        }
    }
}
