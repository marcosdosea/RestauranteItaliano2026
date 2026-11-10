using Core.Service;
using Microsoft.AspNetCore.Mvc;
using RestauranteWeb.Models;
using System.Diagnostics;
using System.Globalization;

namespace RestauranteWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly IItemCardapioService _itemCardapioService;
        private readonly IGrupoCardapioService _grupoCardapioService;
        private readonly ITipoFuncionario _tipoFuncionarioService;
        private readonly ITipoRestauranteService _tipoRestauranteService;
        private readonly IFormaPagamentoService _formaPagamentoService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IItemCardapioService itemCardapioService,
                              IGrupoCardapioService grupoCardapioService,
                              ITipoFuncionario tipoFuncionarioService,
                              ITipoRestauranteService tipoRestauranteService,
                              IFormaPagamentoService formaPagamentoService,
                              ILogger<HomeController> logger)
        {
            _itemCardapioService = itemCardapioService;
            _grupoCardapioService = grupoCardapioService;
            _tipoFuncionarioService = tipoFuncionarioService;
            _tipoRestauranteService = tipoRestauranteService;
            _formaPagamentoService = formaPagamentoService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var agora = DateTime.Now;
            var cultura = new CultureInfo("pt-BR");
            var data = agora.ToString("dddd, d 'de' MMMM 'de' yyyy", cultura);

            var model = new HomeModel
            {
                Saudacao = agora.Hour < 12 ? "Buongiorno" : (agora.Hour < 18 ? "Buon pomeriggio" : "Buona sera"),
                DataLonga = char.ToUpper(data[0]) + data[1..],
                HoraAgora = agora.ToString("HH:mm", cultura),
                Capitulos = Navegacao.Capitulos()
            };

            try
            {
                var itensAtivos = _itemCardapioService.GetAll().Where(i => i.Ativo == 1).ToList();
                model.ItensNoCardapio = itensAtivos.Count;
                model.ItensDisponiveis = itensAtivos.Count(i => i.Disponivel == "1");
                model.ItensEmFalta = model.ItensNoCardapio - model.ItensDisponiveis;
                model.Grupos = itensAtivos
                    .GroupBy(i => i.IdGrupoCardapioNavigation?.Nome ?? "Sem grupo")
                    .Select(g => new GrupoResumoModel { Nome = g.Key, QuantidadeItens = g.Count() })
                    .OrderByDescending(g => g.QuantidadeItens)
                    .ThenBy(g => g.Nome)
                    .Take(5)
                    .ToList();

                // Quantidade de registros de cada função pronta, mostrada no índice
                var quantidades = new Dictionary<string, int>
                {
                    ["ItemCardapio"] = itensAtivos.Count,
                    ["GrupoCardapio"] = _grupoCardapioService.GetAll().Count(),
                    ["TipoFuncionario"] = _tipoFuncionarioService.GetAll().Count(),
                    ["TipoRestaurante"] = _tipoRestauranteService.GetAll().Count(),
                    ["FormaPagamento"] = _formaPagamentoService.GetAll().Count()
                };

                foreach (var entrada in model.Capitulos.SelectMany(c => c.Entradas))
                {
                    if (entrada.Controller != null && quantidades.TryGetValue(entrada.Controller, out var quantidade))
                    {
                        entrada.Quantidade = quantidade;
                    }
                }

                model.DadosDisponiveis = true;
            }
            catch (Exception ex)
            {
                // A página inicial continua abrindo mesmo se o banco estiver fora do ar.
                _logger.LogError(ex, "Não foi possível carregar os dados da página inicial.");
                model.DadosDisponiveis = false;
            }

            return View(model);
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
