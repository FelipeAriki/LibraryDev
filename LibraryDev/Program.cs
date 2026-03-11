using LibraryDev.Application.Interfaces;
using LibraryDev.Application.Interfaces.Livros;
using LibraryDev.Application.Services;
using LibraryDev.Domain.Interfaces.Livros;
using LibraryDev.Domain.Interfaces.Usuarios;
using LibraryDev.Infrastructure.Repositories.Livros;
using LibraryDev.Infrastructure.Repositories.Usuarios;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

//MIGRAR NA REFATORAÇÃO PARA O MÉTODO DE EXTENSÃO
builder.Services.AddScoped<ILivroService, LivroService>();
builder.Services.AddScoped<ILivroCommandRepository, LivroCommandRepository>();
builder.Services.AddScoped<ILivroQueryRepository, LivroQueryRepository>();

builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IUsuarioCommandRepository, UsuarioCommandRepository>();
builder.Services.AddScoped<IUsuarioQueryRepository, UsuarioQueryRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
