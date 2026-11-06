using RestauranteWeb.Mapper;
using RestauranteWeb.Models;
using Core.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RestauranteWeb.Controllers
{
    public class TipoFuncionarioController : Controller
    {
        private readonly ITipoFuncionario tipoFuncionarioService;

        public TipoFuncionarioController(ITipoFuncionario tipoFuncionarioService)
        {
            this.tipoFuncionarioService = tipoFuncionarioService;
        }

        // GET: TipoFuncionarioController
        public ActionResult Index()
        {
            var tiposFuncionarios = tipoFuncionarioService.GetAll();
            var tiposFuncionariosViewModel = TipoFuncionarioMapper.ToViewModel(tiposFuncionarios);
            return View(tiposFuncionariosViewModel);
        }

        // GET: TipoFuncionarioController/Details/5
        public ActionResult Details(uint id)
        {
            var tipoFuncionario = tipoFuncionarioService.Get(id);
            var tipoFuncionarioViewModel = TipoFuncionarioMapper.ToViewModel(tipoFuncionario);
            return View(tipoFuncionarioViewModel);
        }

        // GET: TipoFuncionarioController/Create
        public ActionResult Create()
        {
            var tipoFuncionarioViewModel = new TipoFuncionarioViewModel();
            return View(tipoFuncionarioViewModel);
        }

        // POST: TipoFuncionarioController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TipoFuncionarioViewModel tipoFuncionarioViewModel)
        {
            if (ModelState.IsValid)
            {
                var tipoFuncionario = TipoFuncionarioMapper.ToEntity(tipoFuncionarioViewModel);
                tipoFuncionarioService.Create(tipoFuncionario);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: TipoFuncionarioController/Edit/5
        public ActionResult Edit(uint id)
        {
            var tipoFuncionario = tipoFuncionarioService.Get(id);
            var tipoFuncionarioViewModel = TipoFuncionarioMapper.ToViewModel(tipoFuncionario);
            return View(tipoFuncionarioViewModel);
        }

        // POST: TipoFuncionarioController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TipoFuncionarioViewModel tipoFuncionarioViewModel)
        {
            if (ModelState.IsValid)
            {
                var tipoFuncionario = TipoFuncionarioMapper.ToEntity(tipoFuncionarioViewModel);
                tipoFuncionarioService.Edit(tipoFuncionario);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: TipoFuncionarioController/Delete/5
        public ActionResult Delete(uint id)
        {
            var tipoFuncionario = tipoFuncionarioService.Get(id);
            var tipoFuncionarioViewModel = TipoFuncionarioMapper.ToViewModel(tipoFuncionario);
            return View(tipoFuncionarioViewModel);
        }

        // POST: TipoFuncionarioController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(TipoFuncionarioViewModel tipoFuncionarioViewModel)
        {
            tipoFuncionarioService.Delete(tipoFuncionarioViewModel.Id);
            return RedirectToAction(nameof(Index));
        }
    }
}
