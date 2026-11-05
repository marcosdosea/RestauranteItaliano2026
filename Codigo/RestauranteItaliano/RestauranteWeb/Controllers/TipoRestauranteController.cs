using Core;
using Core.Service;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestauranteWeb.Models;

namespace RestauranteWeb.Controllers
{
    public class TipoRestauranteController : Controller
    {
        private readonly ITipoRestauranteService _service;
        private readonly IMapper _mapper;

        public TipoRestauranteController(ITipoRestauranteService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        // GET: TipoRestauranteController
        public ActionResult Index()
        {
            return View(_mapper.Map<List<TipoRestauranteModel>>(_service.GetAll()));
        }

        // GET: TipoRestauranteController/Details/5
        public ActionResult Details(uint id)
        {
            var tipo = _service.Get(id);
            if (tipo == null) return NotFound();
            return View(_mapper.Map<TipoRestauranteModel>(tipo));
        }

        // GET: TipoRestauranteController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TipoRestauranteController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Tiporestaurante model)
        {
            if (!ModelState.IsValid) return View(model);
            try
            {
                _service.Create(_mapper.Map<Tiporestaurante>(model));
                return RedirectToAction(nameof(Index));
            }
            catch (ServiceException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        // GET: TipoRestauranteController/Edit/5
        public ActionResult Edit(uint id)
        {
            var tipo = _service.Get(id);
            if (tipo == null) return NotFound();
            return View(_mapper.Map<TipoRestauranteModel>(tipo));
        }

        // POST: TipoRestauranteController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(uint id, TipoRestauranteModel model)
        {
            if (!ModelState.IsValid) return View(model);
            try
            {
                model.Id = id;
                _service.Edit(_mapper.Map<Tiporestaurante>(model));
                return RedirectToAction(nameof(Index));
            }
            catch (ServiceException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        // GET: TipoRestauranteController/Delete/5
        public ActionResult Delete(uint id)
        {
            var tipo = _service.Get(id);
            if (tipo == null) return NotFound();
            return View(_mapper.Map<TipoRestauranteModel>(tipo));
        }

        // POST: TipoRestauranteController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(uint id, TipoRestauranteModel model)
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
