using Microsoft.AspNetCore.Mvc;
using Core.Service;
using Microsoft.AspNetCore.Mvc.Rendering;
using RestauranteWeb.Models;
using RestauranteWeb.Mapper;

namespace RestauranteWeb.Controllers
{
    public class RestauranteController : Controller
    {
        private readonly IRestauranteService restauranteService;
        private readonly ITipoRestauranteService tipoRestauranteService;

        public RestauranteController(IRestauranteService restauranteService, ITipoRestauranteService tipoRestauranteService)
        {
            this.restauranteService = restauranteService;
            this.tipoRestauranteService = tipoRestauranteService;
        }

        //GET: RestauranteController
        public IActionResult Index()
        {
            var restaurantes = restauranteService.GetAll();
            var restaurantesViewModel = RestauranteMapper.ToViewModel(restaurantes);
            return View(restaurantesViewModel);
        }

        //GET: RestauranteController/Details/5
        public IActionResult Details(uint id)
        {
            var restaurante = restauranteService.Get(id);
            if (restaurante == null) return NotFound();
            var restauranteViewModel = RestauranteMapper.ToViewModel(restaurante);
            return View(restauranteViewModel);
        }

        //GET: RestauranteController/Create
        public IActionResult Create()
        {
            var restauranteViewModel = new RestauranteViewModel();
            CarregarListas(restauranteViewModel);
            return View(restauranteViewModel);
        }

        //POST: RestauranteController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(RestauranteViewModel restauranteViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var restaurante = RestauranteMapper.ToEntity(restauranteViewModel);
                    restauranteService.Create(restaurante);
                    TempData["Mensagem"] = $"{restauranteViewModel.Nome} entrou na lista de restaurantes.";

                    return RedirectToAction(nameof(Index));
                }
                catch (ServiceException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            CarregarListas(restauranteViewModel);
            return View(restauranteViewModel);
        }

        //GET: RestauranteController/Edit/5
        public IActionResult Edit(uint id)
        {
            var restaurante = restauranteService.Get(id);
            if (restaurante == null) return NotFound();
            var restauranteViewModel = RestauranteMapper.ToViewModel(restaurante);
            CarregarListas(restauranteViewModel);
            return View(restauranteViewModel);
        }

        //POST: RestauranteController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(uint id, RestauranteViewModel restauranteViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    restauranteViewModel.Id = id;
                    var restaurante = RestauranteMapper.ToEntity(restauranteViewModel);
                    restauranteService.Edit(restaurante);
                    TempData["Mensagem"] = $"Alterações em {restauranteViewModel.Nome} salvas.";
                    return RedirectToAction(nameof(Index));
                }
                catch (ServiceException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            CarregarListas(restauranteViewModel);
            return View(restauranteViewModel);
        }

        //GET: RestauranteController/Delete/5
        public IActionResult Delete(uint id)
        {
            var restaurante = restauranteService.Get(id);
            if (restaurante == null) return NotFound();
            var restauranteViewModel = RestauranteMapper.ToViewModel(restaurante);
            return View(restauranteViewModel);
        }

        //POST: RestauranteController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(uint id, RestauranteViewModel restauranteViewModel)
        {
            try
            {
                restauranteService.Delete(id);
                TempData["Mensagem"] = $"{restauranteViewModel.Nome} saiu da lista de restaurantes.";
                return RedirectToAction(nameof(Index));
            }
            catch (ServiceException ex)
            {
                //O formulário só eniva o código e o nome (descarta os erros de campos que não foram enviados)
                ModelState.Clear();
                ModelState.AddModelError("", ex.Message);
                //Recarrega o restaurante do banco de dados para exibir os dados corretos na view
                var restaurante = restauranteService.Get(id);
                return View(restaurante == null ? restauranteViewModel : RestauranteMapper.ToViewModel(restaurante));
            }
        }

        /// <summary>
        /// Preenche a lista de tipos de restaurante usado nos formulários
        /// </summary>

        private void CarregarListas(RestauranteViewModel restauranteViewModel)
        {
            var tiposRestaurante = tipoRestauranteService.GetAll()
                .OrderBy(t => t.Nome)
                .ToList();
            restauranteViewModel.TiposRestaurante = new SelectList(tiposRestaurante, "Id", "Nome", restauranteViewModel.IdTipoRestaurante);
        }
    }
}
