using Core;
using Core.Service;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using RestauranteWeb.Models;

namespace RestauranteWeb.Controllers
{
    public class GrupoCardapioController : Controller
    {
        private readonly IGrupoCardapioService _service;
        private readonly IMapper _mapper;

        public GrupoCardapioController(IGrupoCardapioService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        // GET: GrupoCardapioController
        public ActionResult Index()
        {
            return View(_mapper.Map<List<GrupoCardapioModel>>(_service.GetAll()));
        }

        // GET: GrupoCardapioController/Details/5
        public ActionResult Details(uint id)
        {
            var grupo = _service.Get(id);
            if (grupo == null) return NotFound();
            return View(_mapper.Map<GrupoCardapioModel>(grupo));
        }

        // GET: GrupoCardapioController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: GrupoCardapioController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(GrupoCardapioModel model)
        {
            if (!ModelState.IsValid) return View(model);
            try
            {
                _service.Create(_mapper.Map<Grupocardapio>(model));
                return RedirectToAction(nameof(Index));
            }
            catch (ServiceException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        // GET: GrupoCardapioController/Edit/5
        public ActionResult Edit(uint id)
        {
            var grupo = _service.Get(id);
            if (grupo == null) return NotFound();
            return View(_mapper.Map<GrupoCardapioModel>(grupo));
        }

        // POST: GrupoCardapioController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(uint id, GrupoCardapioModel model)
        {
            if (!ModelState.IsValid) return View(model);
            try
            {
                model.Id = id;
                _service.Edit(_mapper.Map<Grupocardapio>(model));
                return RedirectToAction(nameof(Index));
            }
            catch (ServiceException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        // GET: GrupoCardapioController/Delete/5
        public ActionResult Delete(uint id)
        {
            var grupo = _service.Get(id);
            if (grupo == null) return NotFound();
            return View(_mapper.Map<GrupoCardapioModel>(grupo));
        }

        // POST: GrupoCardapioController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(uint id, GrupoCardapioModel model)
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
