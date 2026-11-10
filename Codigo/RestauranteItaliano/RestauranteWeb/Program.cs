using System.Globalization;
using Core;
using Core.Service;
using Mapster;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using RestauranteWeb.Mapper;
using Service;

namespace RestauranteWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            
            builder.Services.AddDbContext<RestauranteContext>(options =>
                options.UseMySQL(builder.Configuration.GetConnectionString("RestauranteConnection") ?? throw new InvalidOperationException("Conexão com o banco de dados não foi possível. Favor entrar em contato com o Administrador.")));
            
            //Serviços de aplicação
            builder.Services.AddScoped<ITipoRestauranteService, TipoRestauranteService>();

            builder.Services.AddScoped<ITipoFuncionario, TipoFuncionarioService>();

            builder.Services.AddScoped<IGrupoCardapioService, GrupoCardapioService>();

            builder.Services.AddScoped<IFormaPagamentoService, FormaPagamentoService>();

            builder.Services.AddScoped<IItemCardapioService, ItemCardapioService>();

            builder.Services.AddScoped<IRestauranteService, RestauranteService>();

            // Configuração do Mapster
            builder.Services.AddMapster();
            ItemCardapioMapper.Registrar(TypeAdapterConfig.GlobalSettings);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            // Cultura brasileira: preço com vírgula (29,90) e moeda em R$
            var culturaBrasil = new CultureInfo("pt-BR");
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(culturaBrasil),
                SupportedCultures = new List<CultureInfo> { culturaBrasil },
                SupportedUICultures = new List<CultureInfo> { culturaBrasil }
            });

            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
