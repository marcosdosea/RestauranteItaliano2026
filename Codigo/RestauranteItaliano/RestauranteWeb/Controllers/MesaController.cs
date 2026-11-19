using Core.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RestauranteWeb.Mapper;
using RestauranteWeb.Models;

namespace RestauranteWeb.Controllers
{
    public class MesaController : Controller
    {
        private readonly IMesaService mesaService;
        private readonly IRestauranteService restauranteService;

        public MesaController(IMesaService mesaService, IRestauranteService restauranteService)
        {
            this.mesaService = mesaService;
            this.restauranteService = restauranteService;
        }

        //GET: MesaController
        public IActionResult Index()
        {
            var mesas = mesaService.GetAll().ToList();
            var mesasViewModel = MesaMapper.ToViewModel(mesas);
            return View(mesasViewModel);
        }

        //GET: MesaController/Details/5
        public IActionResult Details(int id)
        {
            var mesa = mesaService.Get(id);
            if (mesa == null) return NotFound();
            var mesaViewModel = MesaMapper.ToViewModel(mesa);
            return View(mesaViewModel);
        }

        //GET: MesaController/Create
        public IActionResult Create()
        {
            var mesaViewModel = new MesaViewModel();
            CarregarListas(mesaViewModel);
            return View(mesaViewModel);
        }

        //POST: MesaController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(MesaViewModel mesaViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var mesa = MesaMapper.ToEntity(mesaViewModel);
                    mesaService.Create(mesa);
                    TempData["Mensagem"] = $"{mesaViewModel.Identificacao} entrou na lista de mesas.";
                    return RedirectToAction(nameof(Index));
                }
                catch (ServiceException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            CarregarListas(mesaViewModel);
            return View(mesaViewModel);
        }

        //GET: MesaController/Edit/5
        public IActionResult Edit(int id)
        {
            var mesa = mesaService.Get(id);
            if (mesa == null) return NotFound();
            var mesaViewModel = MesaMapper.ToViewModel(mesa);
            CarregarListas(mesaViewModel);
            return View(mesaViewModel);
        }

        //POST: MesaController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, MesaViewModel mesaViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    mesaViewModel.Id = id;
                    var mesa = MesaMapper.ToEntity(mesaViewModel);
                    mesaService.Edit(mesa);
                    TempData["Mensagem"] = $"Alterações em {mesaViewModel.Identificacao} salvas.";
                    return RedirectToAction(nameof(Index));
                }
                catch (ServiceException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            CarregarListas(mesaViewModel);
            return View(mesaViewModel);
        }

        //GET: MesaController/Delete/5
        public IActionResult Delete(int id)
        {
            var mesa = mesaService.Get(id);
            if (mesa == null) return NotFound();
            var mesaViewModel = MesaMapper.ToViewModel(mesa);
            return View(mesaViewModel);
        }

        //POST: MesaController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id, MesaViewModel mesaViewModel)
        {
            try
            {
                mesaService.Delete(id);
                TempData["Mensagem"] = $"{mesaViewModel.Identificacao} saiu da lista de mesas.";
                return RedirectToAction(nameof(Index));
            }
            catch (ServiceException ex)
            {
                // o formulário só envia o código e a identificação (descarta os erros dos campos que não foram enviados)
                ModelState.Clear();
                ModelState.AddModelError("", ex.Message);
                // recarrega a mesa do banco de dados para exibir os dados corretos na view
                var mesa = mesaService.Get(id);
                return View(mesa == null ? mesaViewModel : MesaMapper.ToViewModel(mesa));
            }
        }

        /// <summary>
        /// Preenche a lista de restaurantes usada nos formulários
        /// </summary>
        private void CarregarListas(MesaViewModel mesaViewModel)
        {
            var restaurantes = restauranteService.GetAll().ToList();

            // com um restaurante só, ele já vem escolhido
            if (mesaViewModel.IdRestaurante == null && restaurantes.Count == 1)
            {
                mesaViewModel.IdRestaurante = restaurantes[0].Id;
            }

            mesaViewModel.Restaurantes = new SelectList(restaurantes, "Id", "Nome", mesaViewModel.IdRestaurante);
        }
    }
}
