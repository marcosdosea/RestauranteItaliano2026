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
                _service.Create(_mapper.Map<Itemcardapio>(model));
                return RedirectToAction(nameof(Index));
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
                return RedirectToAction(nameof(Index));
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
                _service.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch (ServiceException ex)
            {
                ModelState.AddModelError("", ex.Message);
                var item = _service.Get(id);
                return View(item == null ? model : _mapper.Map<ItemCardapioModel>(item));
            }
        }

        /// <summary>
        /// Preenche as listas de restaurantes e de grupos do cardápio usadas nos formulários
        /// </summary>
        private void CarregarListas(ItemCardapioModel model)
        {
            model.Restaurantes = new SelectList(_restauranteService.GetAll(), "Id", "Nome", model.IdRestaurante);
            model.GruposCardapio = new SelectList(_grupoCardapioService.GetAll(), "Id", "Nome", model.IdGrupoCardapio);
        }
    }
}
