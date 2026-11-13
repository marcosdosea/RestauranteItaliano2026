using Core.Service;
using Microsoft.AspNetCore.Mvc;
using RestauranteWeb.Mapper;
using RestauranteWeb.Models;

namespace RestauranteWeb.Controllers
{
    public class FormaPagamentoController : Controller
    {
        private readonly IFormaPagamentoService formaPagamentoService;

        public FormaPagamentoController(IFormaPagamentoService formaPagamentoService)
        {
            this.formaPagamentoService = formaPagamentoService;
        }

        // GET: FormaPagamentoController
        public ActionResult Index()
        {
            var formasPagamento = formaPagamentoService.GetAll();
            var formasPagamentoViewModel = FormaPagamentoMapper.ToViewModel(formasPagamento);
            return View(formasPagamentoViewModel);
        }

        // GET: FormaPagamentoController/Details/5
        public ActionResult Details(uint id)
        {
            var formaPagamento = formaPagamentoService.Get(id);
            if (formaPagamento == null) return NotFound();
            var formaPagamentoViewModel = FormaPagamentoMapper.ToViewModel(formaPagamento);
            return View(formaPagamentoViewModel);
        }

        // GET: FormaPagamentoController/Create
        public ActionResult Create()
        {
            var formaPagamentoViewModel = new FormaPagamentoViewModel();
            return View(formaPagamentoViewModel);
        }

        // POST: FormaPagamentoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FormaPagamentoViewModel formaPagamentoViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var formaPagamento = FormaPagamentoMapper.ToEntity(formaPagamentoViewModel);
                    formaPagamentoService.Create(formaPagamento);
                    TempData["Mensagem"] = $"{formaPagamentoViewModel.Nome} entrou nas formas de pagamento.";
                    return RedirectToAction(nameof(Index));
                }
                catch (ServiceException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(formaPagamentoViewModel);
        }

        // GET: FormaPagamentoController/Edit/5
        public ActionResult Edit(uint id)
        {
            var formaPagamento = formaPagamentoService.Get(id);
            if (formaPagamento == null) return NotFound();
            var formaPagamentoViewModel = FormaPagamentoMapper.ToViewModel(formaPagamento);
            return View(formaPagamentoViewModel);
        }

        // POST: FormaPagamentoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(uint id, FormaPagamentoViewModel formaPagamentoViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    formaPagamentoViewModel.Id = id;
                    var formaPagamento = FormaPagamentoMapper.ToEntity(formaPagamentoViewModel);
                    formaPagamentoService.Edit(formaPagamento);
                    TempData["Mensagem"] = $"Alterações em {formaPagamentoViewModel.Nome} salvas.";
                    return RedirectToAction(nameof(Index));
                }
                catch (ServiceException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(formaPagamentoViewModel);
        }

        // GET: FormaPagamentoController/Delete/5
        public ActionResult Delete(uint id)
        {
            var formaPagamento = formaPagamentoService.Get(id);
            if (formaPagamento == null) return NotFound();
            var formaPagamentoViewModel = FormaPagamentoMapper.ToViewModel(formaPagamento);
            return View(formaPagamentoViewModel);
        }

        // POST: FormaPagamentoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(uint id, FormaPagamentoViewModel formaPagamentoViewModel)
        {
            try
            {
                formaPagamentoService.Delete(id);
                TempData["Mensagem"] = $"{formaPagamentoViewModel.Nome} saiu das formas de pagamento.";
                return RedirectToAction(nameof(Index));
            }
            catch (ServiceException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(formaPagamentoViewModel);
            }
        }
    }
}
