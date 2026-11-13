using Core.Service;
using Microsoft.AspNetCore.Mvc;
using RestauranteWeb.Models;
using System.Diagnostics;
using System.Globalization;

namespace RestauranteWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly IResumoService resumoService;
        private readonly ILogger<HomeController> logger;

        public HomeController(IResumoService resumoService, ILogger<HomeController> logger)
        {
            this.resumoService = resumoService;
            this.logger = logger;
        }

        public IActionResult Index()
        {
            var agora = DateTime.Now;
            var cultura = new CultureInfo("pt-BR");
            var data = agora.ToString("dddd, d 'de' MMMM 'de' yyyy", cultura);

            var homeViewModel = new HomeViewModel
            {
                Saudacao = agora.Hour < 12 ? "Buongiorno" : (agora.Hour < 18 ? "Buon pomeriggio" : "Buona sera"),
                DataLonga = char.ToUpper(data[0]) + data[1..],
                HoraAgora = agora.ToString("HH:mm", cultura),
                Capitulos = Navegacao.Capitulos()
            };

            // o cabeçalho da página inicial, no _Layout, mostra a mesma data e a mesma saudação
            ViewData["DataLonga"] = homeViewModel.DataLonga;
            ViewData["Saudacao"] = homeViewModel.Saudacao;

            try
            {
                var resumo = resumoService.GetResumo();

                homeViewModel.ItensNoCardapio = resumo.ItensNoCardapio;
                homeViewModel.ItensDisponiveis = resumo.ItensDisponiveis;
                homeViewModel.ItensEmFalta = resumo.ItensEmFalta;
                homeViewModel.Grupos = resumo.Grupos
                    .Select(grupo => new GrupoResumoViewModel { Nome = grupo.Nome, QuantidadeItens = grupo.QuantidadeItens })
                    .ToList();

                // Quantidade de registros de cada função pronta, mostrada no índice
                var quantidades = new Dictionary<string, int>
                {
                    ["ItemCardapio"] = resumo.ItensNoCardapio,
                    ["GrupoCardapio"] = resumo.QuantidadeGruposCardapio,
                    ["TipoFuncionario"] = resumo.QuantidadeTiposFuncionario,
                    ["TipoRestaurante"] = resumo.QuantidadeTiposRestaurante,
                    ["FormaPagamento"] = resumo.QuantidadeFormasPagamento
                };

                foreach (var entrada in homeViewModel.Capitulos.SelectMany(capitulo => capitulo.Entradas))
                {
                    if (entrada.Controller != null && quantidades.TryGetValue(entrada.Controller, out var quantidade))
                    {
                        entrada.Quantidade = quantidade;
                    }
                }

                homeViewModel.DadosDisponiveis = true;
            }
            catch (Exception ex)
            {
                // A página inicial continua abrindo mesmo se o banco estiver fora do ar.
                logger.LogError(ex, "Não foi possível carregar os dados da página inicial.");
                homeViewModel.DadosDisponiveis = false;
            }

            return View(homeViewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
