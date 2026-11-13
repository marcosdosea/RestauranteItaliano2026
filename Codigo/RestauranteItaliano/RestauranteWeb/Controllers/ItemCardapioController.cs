using Core.Datatables;
using Core.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RestauranteWeb.Mapper;
using RestauranteWeb.Models;

namespace RestauranteWeb.Controllers
{
    public class ItemCardapioController : Controller
    {
        private readonly IItemCardapioService itemCardapioService;
        private readonly IGrupoCardapioService grupoCardapioService;
        private readonly IRestauranteService restauranteService;

        public ItemCardapioController(IItemCardapioService itemCardapioService,
                                      IGrupoCardapioService grupoCardapioService,
                                      IRestauranteService restauranteService)
        {
            this.itemCardapioService = itemCardapioService;
            this.grupoCardapioService = grupoCardapioService;
            this.restauranteService = restauranteService;
        }

        // GET: ItemCardapioController
        // os itens não vêm aqui: a tabela busca cada página em GetDataPage (DataTables)
        public ActionResult Index()
        {
            var gruposCardapio = grupoCardapioService.GetAll();
            ViewData["GruposCardapio"] = new SelectList(gruposCardapio, "Id", "Nome");
            return View();
        }

        // POST: ItemCardapioController/GetDataPage
        [HttpPost]
        public IActionResult GetDataPage(DatatableRequest request)
        {
            var response = itemCardapioService.GetDataPage(request);
            var responseModel = new DatatableResponse<ItemCardapioModel>()
            {
                Data = ItemCardapioMapper.ToViewModel(response.Data),
                Draw = response.Draw,
                RecordsFiltered = response.RecordsFiltered,
                RecordsTotal = response.RecordsTotal,
                Error = response.Error
            };
            return Json(responseModel);
        }

        // GET: ItemCardapioController/Details/5
        public ActionResult Details(uint id)
        {
            var itemCardapio = itemCardapioService.Get(id);
            if (itemCardapio == null) return NotFound();
            var itemCardapioModel = ItemCardapioMapper.ToViewModel(itemCardapio);
            return View(itemCardapioModel);
        }

        // GET: ItemCardapioController/Create
        public ActionResult Create()
        {
            var itemCardapioModel = new ItemCardapioModel { Ativo = true, Disponivel = true };
            CarregarListas(itemCardapioModel);
            return View(itemCardapioModel);
        }

        // POST: ItemCardapioController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ItemCardapioModel itemCardapioModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var itemCardapio = ItemCardapioMapper.ToEntity(itemCardapioModel);
                    var id = itemCardapioService.Create(itemCardapio);
                    TempData["Mensagem"] = $"{itemCardapioModel.Nome} entrou no cardápio.";
                    return RedirectToAction(nameof(Index), null, $"item-{id}");
                }
                catch (ServiceException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            CarregarListas(itemCardapioModel);
            return View(itemCardapioModel);
        }

        // GET: ItemCardapioController/Edit/5
        public ActionResult Edit(uint id)
        {
            var itemCardapio = itemCardapioService.Get(id);
            if (itemCardapio == null) return NotFound();
            var itemCardapioModel = ItemCardapioMapper.ToViewModel(itemCardapio);
            CarregarListas(itemCardapioModel);
            return View(itemCardapioModel);
        }

        // POST: ItemCardapioController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(uint id, ItemCardapioModel itemCardapioModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    itemCardapioModel.Id = id;
                    var itemCardapio = ItemCardapioMapper.ToEntity(itemCardapioModel);
                    itemCardapioService.Edit(itemCardapio);
                    TempData["Mensagem"] = $"Alterações em {itemCardapioModel.Nome} salvas.";
                    return RedirectToAction(nameof(Index), null, $"item-{id}");
                }
                catch (ServiceException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            CarregarListas(itemCardapioModel);
            return View(itemCardapioModel);
        }

        // GET: ItemCardapioController/Delete/5
        public ActionResult Delete(uint id)
        {
            var itemCardapio = itemCardapioService.Get(id);
            if (itemCardapio == null) return NotFound();
            var itemCardapioModel = ItemCardapioMapper.ToViewModel(itemCardapio);
            return View(itemCardapioModel);
        }

        // POST: ItemCardapioController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(uint id, ItemCardapioModel itemCardapioModel)
        {
            try
            {
                itemCardapioService.Delete(id);
                TempData["Mensagem"] = $"{itemCardapioModel.Nome} saiu do cardápio.";
                return RedirectToAction(nameof(Index));
            }
            catch (ServiceException ex)
            {
                ModelState.AddModelError("", ex.Message);

                // recarrega o item para a tela mostrar grupo, preço e restaurante de novo
                var itemCardapio = itemCardapioService.Get(id);
                return View(itemCardapio == null ? itemCardapioModel : ItemCardapioMapper.ToViewModel(itemCardapio));
            }
        }

        // POST: ItemCardapioController/AlternarDisponivel/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AlternarDisponivel(uint id, bool disponivel)
        {
            try
            {
                itemCardapioService.AlterarDisponibilidade(id, disponivel);
                var nome = itemCardapioService.Get(id)?.Nome ?? "O item";
                TempData["Mensagem"] = disponivel
                    ? $"{nome} está disponível de novo."
                    : $"{nome} agora aparece como em falta.";
            }
            catch (ServiceException ex)
            {
                TempData["Erro"] = ex.Message;
            }
            return RedirectToAction(nameof(Index), null, $"item-{id}");
        }

        /// <summary>
        /// Preenche as listas de restaurantes e de grupos do cardápio usadas nos formulários
        /// </summary>
        private void CarregarListas(ItemCardapioModel itemCardapioModel)
        {
            var restaurantes = restauranteService.GetAll().ToList();

            // com um restaurante só, ele já vem escolhido
            if (itemCardapioModel.IdRestaurante == null && restaurantes.Count == 1)
            {
                itemCardapioModel.IdRestaurante = restaurantes[0].Id;
            }

            itemCardapioModel.Restaurantes = new SelectList(restaurantes, "Id", "Nome", itemCardapioModel.IdRestaurante);
            itemCardapioModel.GruposCardapio = new SelectList(grupoCardapioService.GetAll(), "Id", "Nome", itemCardapioModel.IdGrupoCardapio);
        }
    }
}
