using RestauranteWeb.Mapper;
using RestauranteWeb.Models;
using Core.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Core.Datatables;

namespace RestauranteWeb.Controllers
{
    public class FuncionarioController : Controller
    {
        private readonly IFuncionarioService funcionarioService;
        private readonly ITipoFuncionario tipoFuncionarioService;
        private readonly IRestauranteService restauranteService;

        public FuncionarioController(
            IFuncionarioService funcionarioService,
            ITipoFuncionario tipoFuncionarioService,
            IRestauranteService restauranteService)
        {
            this.funcionarioService = funcionarioService;
            this.tipoFuncionarioService = tipoFuncionarioService;
            this.restauranteService = restauranteService;
        }

        /// <summary>
        /// Carrega as listas de tipos de funcionário e restaurantes.
        /// </summary>
        private void CarregarListas(FuncionarioViewModel funcionarioViewModel)
        {
            var tiposFuncionarios = tipoFuncionarioService.GetAll()
                .OrderBy(t => t.Nome)
                .ToList();

            funcionarioViewModel.TiposFuncionario = new SelectList(
                tiposFuncionarios,
                "Id",
                "Nome",
                funcionarioViewModel.IdTipoFuncionario
            );

            var restaurantes = restauranteService.GetAll()
                .OrderBy(r => r.Nome)
                .ToList();

            funcionarioViewModel.Restaurantes = new SelectList(
                restaurantes,
                "Id",
                "Nome",
                funcionarioViewModel.IdRestaurante
            );
        }

        // GET: FuncionarioController/Create
        public ActionResult Create()
        {
            var funcionarioViewModel = new FuncionarioViewModel();

            CarregarListas(funcionarioViewModel);

            return View(funcionarioViewModel);
        }

        // POST: FuncionarioController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FuncionarioViewModel funcionarioViewModel)
        {
            if (ModelState.IsValid)
            {
                var funcionario = FuncionarioMapper.ToEntity(funcionarioViewModel);

                funcionarioService.Create(funcionario);

                return RedirectToAction(nameof(Index));
            }

            CarregarListas(funcionarioViewModel);

            return View(funcionarioViewModel);
        }

        // GET: FuncionarioController/Edit/5
        public ActionResult Edit(uint id)
        {
            var funcionario = funcionarioService.Get(id);

            if (funcionario == null)
                return NotFound();

            var funcionarioViewModel = FuncionarioMapper.ToViewModel(funcionario);

            CarregarListas(funcionarioViewModel);

            return View(funcionarioViewModel);
        }

        // POST: FuncionarioController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(FuncionarioViewModel funcionarioViewModel)
        {
            if (ModelState.IsValid)
            {
                var funcionario = FuncionarioMapper.ToEntity(funcionarioViewModel);

                funcionarioService.Edit(funcionario);

                return RedirectToAction(nameof(Index));
            }

            CarregarListas(funcionarioViewModel);

            return View(funcionarioViewModel);
        }

        // GET: FuncionarioController/Delete/5
        public ActionResult Delete(uint id)
        {
            var funcionario = funcionarioService.Get(id);

            if (funcionario == null)
                return NotFound();

            var funcionarioViewModel = FuncionarioMapper.ToViewModel(funcionario);

            return View(funcionarioViewModel);
        }

        // POST: FuncionarioController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(uint id)
        {
            funcionarioService.Delete(id);

            return RedirectToAction(nameof(Index));
        }

        // GET: FuncionarioController
        public ActionResult Index()
        {
            return View();
        }

        // GET: FuncionarioController/GetDataPage
        [HttpPost]
        public IActionResult GetDataPage([FromForm] DatatableRequest request)
        {
            var resultado = funcionarioService.GetDataPage(request);

            var funcionariosViewModel = resultado.Data != null ? FuncionarioMapper.ToViewModel(resultado.Data) : Enumerable.Empty<FuncionarioViewModel>();

            return Json(new
            {
                draw = resultado.Draw,
                recordsTotal = resultado.RecordsTotal,
                recordsFiltered = resultado.RecordsFiltered,
                data = funcionariosViewModel
            });
        }
        // GET: FuncionarioController/Details/5
        public ActionResult Details(uint id)
        {
            var funcionario = funcionarioService.Get(id);

            if (funcionario == null)
                return NotFound();

            var funcionarioViewModel = FuncionarioMapper.ToViewModel(funcionario);

            return View(funcionarioViewModel);
        }
    }
}