using Core.DTO;
using Core.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RestauranteWeb.Models;

namespace RestauranteWeb.Controllers
{
    public class PedidoController : Controller
    {
        private readonly IPedidoService pedidoService;
        private readonly IItemCardapioService itemCardapioService;
        private readonly IFuncionarioService funcionarioService;

        public PedidoController(
            IPedidoService pedidoService,
            IItemCardapioService itemCardapioService,
            IFuncionarioService funcionarioService)
        {
            this.pedidoService = pedidoService;
            this.itemCardapioService = itemCardapioService;
            this.funcionarioService = funcionarioService;
        }

        [HttpGet]
        public IActionResult Create(uint idAtendimento, uint idRestaurante)
        {
            if (idAtendimento == 0 ||
                !pedidoService.AtendimentoPodeReceberPedidos(idAtendimento))
            {
                TempData["Erro"] = "O atendimento não está ativo.";

                return RedirectToAction(
                    "Index",
                    "Atendimento",
                    new { idRestaurante });
            }

            var model = new RegistrarPedidoViewModel
            {
                IdAtendimento = idAtendimento,
                IdRestaurante = idRestaurante
            };

            CarregarDadosDaTela(model);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(RegistrarPedidoViewModel model)
        {
            model.Itens ??= [];

            if (ModelState.IsValid &&
                !pedidoService.AtendimentoPodeReceberPedidos(
                    model.IdAtendimento))
            {
                ModelState.AddModelError(
                    string.Empty,
                    "O atendimento não está ativo.");
            }

            var itensSelecionados = model.Itens
                .Where(item => item.Quantidade > 0)
                .Select(item => new PedidoItemSolicitadoDTO
                {
                    IdItemCardapio = item.IdItemCardapio,
                    Quantidade = item.Quantidade
                })
                .ToList();

            if (itensSelecionados.Count == 0)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Informe a quantidade de pelo menos um item.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var idPedido = pedidoService.RegistrarPedido(
                        model.IdAtendimento,
                        model.IdGarcom!.Value,
                        itensSelecionados);

                    TempData["Mensagem"] =
                        $"Pedido {idPedido} registrado.";

                    return RedirectToAction(
                        nameof(Create),
                        new
                        {
                            idAtendimento = model.IdAtendimento,
                            idRestaurante = model.IdRestaurante
                        });
                }
                catch (ServiceException ex)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        ex.Message);
                }
            }

            CarregarDadosDaTela(model);

            return View(model);
        }

        private void CarregarDadosDaTela(
            RegistrarPedidoViewModel model)
        {
            CarregarGarcons(model);
            CarregarItens(model);
        }

        private void CarregarGarcons(
            RegistrarPedidoViewModel model)
        {
            model.Garcons = new SelectList(
                funcionarioService.GetGarconsPorRestaurante(
                    model.IdRestaurante),
                "Id",
                "Nome",
                model.IdGarcom);
        }

        private void CarregarItens(
            RegistrarPedidoViewModel model)
        {
            var quantidadesInformadas = model.Itens
                .GroupBy(item => item.IdItemCardapio)
                .ToDictionary(
                    grupo => grupo.Key,
                    grupo => grupo.First().Quantidade);

            model.Itens = itemCardapioService.GetAll()
                .Where(item =>
                    item.IdRestaurante == model.IdRestaurante &&
                    item.Ativo == 1 &&
                    item.Disponivel == "1")
                .OrderBy(item => item.Nome)
                .Select(item => new ItemPedidoViewModel
                {
                    IdItemCardapio = item.Id,
                    Nome = item.Nome,
                    Preco = item.Preco,
                    Quantidade = quantidadesInformadas
                        .GetValueOrDefault(item.Id)
                })
                .ToList();
        }
    }
}