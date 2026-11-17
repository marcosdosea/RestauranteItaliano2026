using Core;
using Core.Service;
using Mapster;
using Microsoft.EntityFrameworkCore;
using RestauranteAPI.Filter;
using Service;

namespace RestauranteAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container with Exception handling.
            builder.Services.AddControllers(options => options.Filters.Add(new HttpResponseExceptionFilter()));

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<RestauranteContext>(options =>
                options.UseMySQL(builder.Configuration.GetConnectionString("RestauranteConnection") ?? throw new InvalidOperationException("Conexão com o banco de dados não foi possível. Favor entrar em contato com o Administrador.")));

            // Serviços de aplicação
            builder.Services.AddTransient<IFormaPagamentoService, FormaPagamentoService>();
            builder.Services.AddTransient<IItemCardapioService, ItemCardapioService>();

            // Configuração do Mapster
            builder.Services.AddMapster();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
