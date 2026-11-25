using Core.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RestauranteWeb.Mapper;
using RestauranteWeb.Models;

namespace RestauranteWeb.Controllers
{
    public class FuncionarioController : Controller
    {
        private readonly IFuncionarioService funcionarioService;
        private readonly IRestauranteService restauranteService;
        private readonly ITipoFuncionario tipoFuncionarioService;

        public FuncionarioController(
            IFuncionarioService funcionarioService,
            IRestauranteService restauranteService,
            ITipoFuncionario tipoFuncionarioService)
        {
            this.funcionarioService = funcionarioService;
            this.restauranteService = restauranteService;
            this.tipoFuncionarioService = tipoFuncionarioService;
        }

        // GET: Funcionario
        [HttpGet]
        public IActionResult Index()
        {
            var funcionarios = funcionarioService.GetAll();
            var viewModels = FuncionarioMapper.ToViewModel(funcionarios);

            return View(viewModels);
        }

        // GET: Funcionario/Create
        [HttpGet]
        public IActionResult Create()
        {
            var model = new FuncionarioViewModel();

            CarregarListas(model);

            return View(model);
        }

        // POST: Funcionario/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(FuncionarioViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var funcionario =
                        FuncionarioMapper.ToEntity(model);

                    funcionarioService.Create(funcionario);

                    TempData["Mensagem"] =
                        $"{model.Nome} foi cadastrado(a) como funcionário(a).";

                    return RedirectToAction(nameof(Index));
                }
                catch (ServiceException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            CarregarListas(model);

            return View(model);
        }

        private void CarregarListas(FuncionarioViewModel model)
        {
            model.Restaurantes = new SelectList(
                restauranteService.GetAll(),
                "Id",
                "Nome",
                model.IdRestaurante);

            model.TiposFuncionario = new SelectList(
                tipoFuncionarioService.GetAll(),
                "Id",
                "Nome",
                model.IdTipoFuncionario);
        }
    }
}