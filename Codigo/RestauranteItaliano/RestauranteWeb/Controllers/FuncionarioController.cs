
using Core;
using Core.Datatables;
using Core.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RestauranteWeb.Mapper;
using RestauranteWeb.Models;

namespace RestauranteWeb.Controllers
{
    public class FuncionarioController : Controller
    {
        private readonly IFuncionarioService funcionarioService;
        private readonly ITipoFuncionario tipoFuncionarioService;

        public FuncionarioController(IFuncionarioService funcionarioService, ITipoFuncionario tipoFuncionarioService)
        {
            this.funcionarioService = funcionarioService;
            this.tipoFuncionarioService = tipoFuncionarioService;
        }

        private void CarregarTiposFuncionario()
        {
            var tiposFuncionarios = tipoFuncionarioService.GetAll();
            var tiposFuncionariosViewModel = TipoFuncionarioMapper.ToViewModel(tiposFuncionarios);
            ViewBag.TiposFuncionarios = tiposFuncionariosViewModel;
        }

        // GET: FuncionarioController/Create
        public ActionResult Create()
        {
            CarregarTiposFuncionario();
            var funcionarioViewModel = new FuncionarioViewModel();
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
            CarregarTiposFuncionario();
            return View(funcionarioViewModel);
        }

        // GET: FuncionarioController/Edit/5
        public ActionResult Edit(uint id)
        {
            var funcionario = funcionarioService.Get(id);

            if (funcionario == null)
            {
                return NotFound();
            }

            CarregarTiposFuncionario();
            var funcionarioViewModel = FuncionarioMapper.ToViewModel(funcionario);
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

            CarregarTiposFuncionario();

            return View(funcionarioViewModel);
        }

        // GET: FuncionarioController/Delete/5
        public ActionResult Delete(uint id)
        {
            var funcionario = funcionarioService.Get(id);

            if (funcionario == null)
            {
                return NotFound();
            }

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
            var funcionarios = funcionarioService.GetAll();
            var funcionariosViewModel = FuncionarioMapper.ToViewModel(funcionarios);
            return View(funcionariosViewModel);
        }

        // GET: FuncionarioController/Details/5
        public ActionResult Details(uint id)
        {
            var funcionario = funcionarioService.Get(id);

            if (funcionario == null)
            {
                return NotFound();
            }

            var funcionarioViewModel = FuncionarioMapper.ToViewModel(funcionario);
            return View(funcionarioViewModel);
        }
        [HttpPost]
        public IActionResult GetDataPage([FromForm] DatatableRequest request)
        {
            var dados = funcionarioService.GetDataPage(request);
            var dadosViewModel = FuncionarioMapper.ToViewModel(dados.Data ?? Enumerable.Empty<Funcionario>());

            return Json(new DatatableResponse<FuncionarioViewModel>
            {
                Draw = dados.Draw,
                RecordsTotal = dados.RecordsTotal,
                RecordsFiltered = dados.RecordsFiltered,
                Data = dadosViewModel
            });
        }
    }
}
