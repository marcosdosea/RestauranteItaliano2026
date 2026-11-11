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
            var formasPagamentoModel = FormaPagamentoMapper.ToViewModel(formasPagamento);
            return View(formasPagamentoModel);
        }

        // GET: FormaPagamentoController/Details/5
        public ActionResult Details(uint id)
        {
            var formaPagamento = formaPagamentoService.Get(id);
            if (formaPagamento == null) return NotFound();
            var formaPagamentoModel = FormaPagamentoMapper.ToViewModel(formaPagamento);
            return View(formaPagamentoModel);
        }

        // GET: FormaPagamentoController/Create
        public ActionResult Create()
        {
            var formaPagamentoModel = new FormaPagamentoModel();
            return View(formaPagamentoModel);
        }

        // POST: FormaPagamentoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FormaPagamentoModel formaPagamentoModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var formaPagamento = FormaPagamentoMapper.ToEntity(formaPagamentoModel);
                    formaPagamentoService.Create(formaPagamento);
                    TempData["Mensagem"] = $"{formaPagamentoModel.Nome} entrou nas formas de pagamento.";
                    return RedirectToAction(nameof(Index));
                }
                catch (ServiceException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(formaPagamentoModel);
        }

        // GET: FormaPagamentoController/Edit/5
        public ActionResult Edit(uint id)
        {
            var formaPagamento = formaPagamentoService.Get(id);
            if (formaPagamento == null) return NotFound();
            var formaPagamentoModel = FormaPagamentoMapper.ToViewModel(formaPagamento);
            return View(formaPagamentoModel);
        }

        // POST: FormaPagamentoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(uint id, FormaPagamentoModel formaPagamentoModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    formaPagamentoModel.Id = id;
                    var formaPagamento = FormaPagamentoMapper.ToEntity(formaPagamentoModel);
                    formaPagamentoService.Edit(formaPagamento);
                    TempData["Mensagem"] = $"Alterações em {formaPagamentoModel.Nome} salvas.";
                    return RedirectToAction(nameof(Index));
                }
                catch (ServiceException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(formaPagamentoModel);
        }

        // GET: FormaPagamentoController/Delete/5
        public ActionResult Delete(uint id)
        {
            var formaPagamento = formaPagamentoService.Get(id);
            if (formaPagamento == null) return NotFound();
            var formaPagamentoModel = FormaPagamentoMapper.ToViewModel(formaPagamento);
            return View(formaPagamentoModel);
        }

        // POST: FormaPagamentoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(uint id, FormaPagamentoModel formaPagamentoModel)
        {
            try
            {
                formaPagamentoService.Delete(id);
                TempData["Mensagem"] = $"{formaPagamentoModel.Nome} saiu das formas de pagamento.";
                return RedirectToAction(nameof(Index));
            }
            catch (ServiceException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(formaPagamentoModel);
            }
        }
    }
}
