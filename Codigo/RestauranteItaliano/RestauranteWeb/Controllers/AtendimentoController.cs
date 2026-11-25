using Core.Service;
using Microsoft.AspNetCore.Mvc;
using RestauranteWeb.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RestauranteWeb.Controllers
{
    public class AtendimentoController : Controller
    {
        private readonly IAtendimentoService atendimentoService;
        private readonly IRestauranteService restauranteService;

        public AtendimentoController(IAtendimentoService atendimentoService, IRestauranteService restauranteService)
        {
            this.atendimentoService = atendimentoService;
            this.restauranteService = restauranteService;
        }

        // GET: Atendimento
        [HttpGet]
        public IActionResult Index(uint? idRestaurante)
        {
            var model = new AtendimentoViewModel
            {
                IdRestaurante = idRestaurante,
                Restaurantes = new SelectList(
                    restauranteService.GetAll(),
                    "Id", "Nome", idRestaurante)
            };

            if (idRestaurante.HasValue)
            {
                model.Mesas = atendimentoService.GetMapaMesas(idRestaurante.Value)
                    .Select(m => new MesaAtendimentoViewModel
                    {
                        IdMesa = m.IdMesa,
                        Identificacao = m.Identificacao,
                        Ocupada = m.Ocupada,
                        InicioAtendimento = m.InicioAtendimento,
                        IdAtendimento = m.IdAtendimento
                    })
                    .ToList();
            }

            return View(model);
        }

        // POST: Atendimento/Iniciar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Iniciar(int idMesa, uint idRestaurante)
        {
            try
            {
                atendimentoService.Iniciar(idMesa, idRestaurante);
                TempData["Mensagem"] = "Atendimento iniciado.";
            }
            catch (ServiceException ex)
            {
                TempData["Erro"] = ex.Message;
            }
            return RedirectToAction(nameof(Index), new { idRestaurante });
        }
    }
}
