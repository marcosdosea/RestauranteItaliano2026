using Core;
using Core.Service;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RestauranteWeb.Models;

namespace RestauranteWeb.Controllers
{
    public class ItemCardapioController : Controller
    {
        private readonly IItemCardapioService _service;
        private readonly IGrupoCardapioService _grupoCardapioService;
        private readonly IRestauranteService _restauranteService;
        private readonly IMapper _mapper;

        public ItemCardapioController(IItemCardapioService service,
                                      IGrupoCardapioService grupoCardapioService,
                                      IRestauranteService restauranteService,
                                      IMapper mapper)
        {
            _service = service;
            _grupoCardapioService = grupoCardapioService;
            _restauranteService = restauranteService;
            _mapper = mapper;
        }

        // GET: ItemCardapioController
        public ActionResult Index()
        {
            return View(_mapper.Map<List<ItemCardapioModel>>(_service.GetAll()));
        }

        // GET: ItemCardapioController/Details/5
        public ActionResult Details(uint id)
        {
            var item = _service.Get(id);
            if (item == null) return NotFound();
            return View(_mapper.Map<ItemCardapioModel>(item));
        }

        // GET: ItemCardapioController/Create
        public ActionResult Create()
        {
            var model = new ItemCardapioModel { Ativo = true, Disponivel = true };
            CarregarListas(model);
            return View(model);
        }

        // POST: ItemCardapioController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ItemCardapioModel model)
        {
            if (!ModelState.IsValid)
            {
                CarregarListas(model);
                return View(model);
            }
            try
            {
                var id = _service.Create(_mapper.Map<Itemcardapio>(model));
                TempData["Mensagem"] = $"{model.Nome} entrou no cardápio.";
                return RedirectToAction(nameof(Index), null, $"item-{id}");
            }
            catch (ServiceException ex)
            {
                ModelState.AddModelError("", ex.Message);
                CarregarListas(model);
                return View(model);
            }
        }

        // GET: ItemCardapioController/Edit/5
        public ActionResult Edit(uint id)
        {
            var item = _service.Get(id);
            if (item == null) return NotFound();
            var model = _mapper.Map<ItemCardapioModel>(item);
            CarregarListas(model);
            return View(model);
        }

        // POST: ItemCardapioController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(uint id, ItemCardapioModel model)
        {
            if (!ModelState.IsValid)
            {
                CarregarListas(model);
                return View(model);
            }
            try
            {
                model.Id = id;
                _service.Edit(_mapper.Map<Itemcardapio>(model));
                TempData["Mensagem"] = $"Alterações em {model.Nome} salvas.";
                return RedirectToAction(nameof(Index), null, $"item-{id}");
            }
            catch (ServiceException ex)
            {
                ModelState.AddModelError("", ex.Message);
                CarregarListas(model);
                return View(model);
            }
        }

        // GET: ItemCardapioController/Delete/5
        public ActionResult Delete(uint id)
        {
            var item = _service.Get(id);
            if (item == null) return NotFound();
            return View(_mapper.Map<ItemCardapioModel>(item));
        }

        // POST: ItemCardapioController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(uint id, ItemCardapioModel model)
        {
            try
            {
                var nome = _service.Get(id)?.Nome ?? model.Nome;
                _service.Delete(id);
                TempData["Mensagem"] = $"{nome} saiu do cardápio.";
                return RedirectToAction(nameof(Index));
            }
            catch (ServiceException ex)
            {
                ModelState.AddModelError("", ex.Message);
                var item = _service.Get(id);
                return View(item == null ? model : _mapper.Map<ItemCardapioModel>(item));
            }
        }

        // POST: ItemCardapioController/AlternarDisponivel/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AlternarDisponivel(uint id, bool disponivel)
        {
            try
            {
                _service.AlterarDisponibilidade(id, disponivel);
                var nome = _service.Get(id)?.Nome ?? "O item";
                TempData["Mensagem"] = disponivel
                    ? $"{nome} está disponível de novo."
                    : $"{nome} agora aparece como em falta.";
            }
            catch (ServiceException ex)
            {
                TempData["Mensagem"] = ex.Message;
            }
            return RedirectToAction(nameof(Index), null, $"item-{id}");
        }

        /// <summary>
        /// Preenche as listas de restaurantes e de grupos do cardápio usadas nos formulários
        /// </summary>
        private void CarregarListas(ItemCardapioModel model)
        {
            var restaurantes = _restauranteService.GetAll().ToList();

            // com um restaurante só, ele já vem escolhido
            if (model.IdRestaurante == null && restaurantes.Count == 1)
            {
                model.IdRestaurante = restaurantes[0].Id;
            }

            model.Restaurantes = new SelectList(restaurantes, "Id", "Nome", model.IdRestaurante);
            model.GruposCardapio = new SelectList(_grupoCardapioService.GetAll(), "Id", "Nome", model.IdGrupoCardapio);
        }
    }
}
