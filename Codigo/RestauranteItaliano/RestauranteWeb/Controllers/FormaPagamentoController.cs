using Core;
using Core.Service;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using RestauranteWeb.Models;

namespace RestauranteWeb.Controllers
{
    public class FormaPagamentoController : Controller
    {
        private readonly IFormaPagamentoService _service;
        private readonly IMapper _mapper;

        public FormaPagamentoController(IFormaPagamentoService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        // GET: FormaPagamentoController
        public ActionResult Index()
        {
            return View(_mapper.Map<List<FormaPagamentoModel>>(_service.GetAll()));
        }

        // GET: FormaPagamentoController/Details/5
        public ActionResult Details(uint id)
        {
            var formaPagamento = _service.Get(id);
            if (formaPagamento == null) return NotFound();
            return View(_mapper.Map<FormaPagamentoModel>(formaPagamento));
        }

        // GET: FormaPagamentoController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: FormaPagamentoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FormaPagamentoModel model)
        {
            if (!ModelState.IsValid) return View(model);
            try
            {
                _service.Create(_mapper.Map<Formapagamento>(model));
                return RedirectToAction(nameof(Index));
            }
            catch (ServiceException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        // GET: FormaPagamentoController/Edit/5
        public ActionResult Edit(uint id)
        {
            var formaPagamento = _service.Get(id);
            if (formaPagamento == null) return NotFound();
            return View(_mapper.Map<FormaPagamentoModel>(formaPagamento));
        }

        // POST: FormaPagamentoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(uint id, FormaPagamentoModel model)
        {
            if (!ModelState.IsValid) return View(model);
            try
            {
                model.Id = id;
                _service.Edit(_mapper.Map<Formapagamento>(model));
                return RedirectToAction(nameof(Index));
            }
            catch (ServiceException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        // GET: FormaPagamentoController/Delete/5
        public ActionResult Delete(uint id)
        {
            var formaPagamento = _service.Get(id);
            if (formaPagamento == null) return NotFound();
            return View(_mapper.Map<FormaPagamentoModel>(formaPagamento));
        }

        // POST: FormaPagamentoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(uint id, FormaPagamentoModel model)
        {
            try
            {
                _service.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch (ServiceException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }
    }
}
