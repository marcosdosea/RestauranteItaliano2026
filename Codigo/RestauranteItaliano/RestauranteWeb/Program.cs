using System.Globalization;
using Core;
using Core.Service;
using Mapster;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Service;

namespace RestauranteWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews(options =>
            {
                // Mensagens de validação geradas pelo próprio ASP.NET (ex.: número inválido) em português
                var mensagens = options.ModelBindingMessageProvider;
                mensagens.SetValueMustBeANumberAccessor(campo => $"O campo {campo} deve ser um número, com vírgula nos centavos (ex.: 29,90).");
                mensagens.SetValueMustNotBeNullAccessor(campo => $"O campo {campo} é de preenchimento obrigatório.");
                mensagens.SetValueIsInvalidAccessor(valor => $"O valor '{valor}' é inválido.");
                mensagens.SetAttemptedValueIsInvalidAccessor((valor, campo) => $"O valor '{valor}' não é válido para o campo {campo}.");
                mensagens.SetMissingBindRequiredValueAccessor(campo => $"O campo {campo} é de preenchimento obrigatório.");
                mensagens.SetMissingKeyOrValueAccessor(() => "Um valor é obrigatório.");
                mensagens.SetMissingRequestBodyRequiredValueAccessor(() => "O corpo da requisição é obrigatório.");
                mensagens.SetNonPropertyAttemptedValueIsInvalidAccessor(valor => $"O valor '{valor}' é inválido.");
                mensagens.SetNonPropertyUnknownValueIsInvalidAccessor(() => "O valor informado é inválido.");
                mensagens.SetNonPropertyValueMustBeANumberAccessor(() => "O campo deve ser um número.");
                mensagens.SetUnknownValueIsInvalidAccessor(campo => $"O valor informado para o campo {campo} é inválido.");
            });
            
            builder.Services.AddDbContext<RestauranteContext>(options =>
                options.UseMySQL(builder.Configuration.GetConnectionString("RestauranteConnection") ?? throw new InvalidOperationException("Conexão com o banco de dados não foi possível. Favor entrar em contato com o Administrador.")));
            
            // Serviços de aplicação
            builder.Services.AddTransient<ITipoRestauranteService, TipoRestauranteService>();
            builder.Services.AddTransient<ITipoFuncionario, TipoFuncionarioService>();
            builder.Services.AddTransient<IGrupoCardapioService, GrupoCardapioService>();
            builder.Services.AddTransient<IFormaPagamentoService, FormaPagamentoService>();
            builder.Services.AddTransient<IItemCardapioService, ItemCardapioService>();
            builder.Services.AddTransient<IRestauranteService, RestauranteService>();
            builder.Services.AddTransient<IResumoService, ResumoService>();

            // Configuração do Mapster
            builder.Services.AddMapster();

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
