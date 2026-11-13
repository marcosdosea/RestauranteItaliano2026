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
            var responseViewModel = new DatatableResponse<ItemCardapioViewModel>()
            {
                Data = ItemCardapioMapper.ToViewModel(response.Data),
                Draw = response.Draw,
                RecordsFiltered = response.RecordsFiltered,
                RecordsTotal = response.RecordsTotal,
                Error = response.Error
            };
            return Json(responseViewModel);
        }

        // GET: ItemCardapioController/Details/5
        public ActionResult Details(uint id)
        {
            var itemCardapio = itemCardapioService.Get(id);
            if (itemCardapio == null) return NotFound();
            var itemCardapioViewModel = ItemCardapioMapper.ToViewModel(itemCardapio);
            return View(itemCardapioViewModel);
        }

        // GET: ItemCardapioController/Create
        public ActionResult Create()
        {
            var itemCardapioViewModel = new ItemCardapioViewModel { Ativo = true, Disponivel = true };
            CarregarListas(itemCardapioViewModel);
            return View(itemCardapioViewModel);
        }

        // POST: ItemCardapioController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ItemCardapioViewModel itemCardapioViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var itemCardapio = ItemCardapioMapper.ToEntity(itemCardapioViewModel);
                    var id = itemCardapioService.Create(itemCardapio);
                    TempData["Mensagem"] = $"{itemCardapioViewModel.Nome} entrou no cardápio.";
                    return RedirectToAction(nameof(Index), null, $"item-{id}");
                }
                catch (ServiceException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            CarregarListas(itemCardapioViewModel);
            return View(itemCardapioViewModel);
        }

        // GET: ItemCardapioController/Edit/5
        public ActionResult Edit(uint id)
        {
            var itemCardapio = itemCardapioService.Get(id);
            if (itemCardapio == null) return NotFound();
            var itemCardapioViewModel = ItemCardapioMapper.ToViewModel(itemCardapio);
            CarregarListas(itemCardapioViewModel);
            return View(itemCardapioViewModel);
        }

        // POST: ItemCardapioController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(uint id, ItemCardapioViewModel itemCardapioViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    itemCardapioViewModel.Id = id;
                    var itemCardapio = ItemCardapioMapper.ToEntity(itemCardapioViewModel);
                    itemCardapioService.Edit(itemCardapio);
                    TempData["Mensagem"] = $"Alterações em {itemCardapioViewModel.Nome} salvas.";
                    return RedirectToAction(nameof(Index), null, $"item-{id}");
                }
                catch (ServiceException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            CarregarListas(itemCardapioViewModel);
            return View(itemCardapioViewModel);
        }

        // GET: ItemCardapioController/Delete/5
        public ActionResult Delete(uint id)
        {
            var itemCardapio = itemCardapioService.Get(id);
            if (itemCardapio == null) return NotFound();
            var itemCardapioViewModel = ItemCardapioMapper.ToViewModel(itemCardapio);
            return View(itemCardapioViewModel);
        }

        // POST: ItemCardapioController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(uint id, ItemCardapioViewModel itemCardapioViewModel)
        {
            try
            {
                itemCardapioService.Delete(id);
                TempData["Mensagem"] = $"{itemCardapioViewModel.Nome} saiu do cardápio.";
                return RedirectToAction(nameof(Index));
            }
            catch (ServiceException ex)
            {
                ModelState.AddModelError("", ex.Message);

                // recarrega o item para a tela mostrar grupo, preço e restaurante de novo
                var itemCardapio = itemCardapioService.Get(id);
                return View(itemCardapio == null ? itemCardapioViewModel : ItemCardapioMapper.ToViewModel(itemCardapio));
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
        private void CarregarListas(ItemCardapioViewModel itemCardapioViewModel)
        {
            var restaurantes = restauranteService.GetAll().ToList();

            // com um restaurante só, ele já vem escolhido
            if (itemCardapioViewModel.IdRestaurante == null && restaurantes.Count == 1)
            {
                itemCardapioViewModel.IdRestaurante = restaurantes[0].Id;
            }

            itemCardapioViewModel.Restaurantes = new SelectList(restaurantes, "Id", "Nome", itemCardapioViewModel.IdRestaurante);
            itemCardapioViewModel.GruposCardapio = new SelectList(grupoCardapioService.GetAll(), "Id", "Nome", itemCardapioViewModel.IdGrupoCardapio);
        }
    }
}
