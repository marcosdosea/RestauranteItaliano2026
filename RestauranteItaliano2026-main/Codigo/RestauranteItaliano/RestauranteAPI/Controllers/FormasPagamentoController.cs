using Core.Service;
using Microsoft.AspNetCore.Mvc;
using RestauranteAPI.Mappers;
using RestauranteAPI.Models;

namespace RestauranteAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FormasPagamentoController : ControllerBase
    {
        private readonly IFormaPagamentoService formaPagamentoService;

        public FormasPagamentoController(IFormaPagamentoService formaPagamentoService)
        {
            this.formaPagamentoService = formaPagamentoService;
        }

        // GET: api/<FormasPagamentoController>
        [HttpGet]
        public ActionResult Get()
        {
            var listaFormasPagamento = formaPagamentoService.GetAll();
            return Ok(FormaPagamentoMapper.ToViewModel(listaFormasPagamento));
        }

        // GET api/<FormasPagamentoController>/5
        [HttpGet("{id}")]
        public ActionResult Get(uint id)
        {
            var formaPagamento = formaPagamentoService.Get(id);
            if (formaPagamento == null)
                return NotFound("Forma de pagamento não encontrada.");
            return Ok(FormaPagamentoMapper.ToViewModel(formaPagamento));
        }

        // POST api/<FormasPagamentoController>
        [HttpPost]
        public ActionResult Post([FromBody] FormaPagamentoViewModel formaPagamentoViewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest("Dados inválidos.");

            var formaPagamento = FormaPagamentoMapper.ToEntity(formaPagamentoViewModel);
            var id = formaPagamentoService.Create(formaPagamento);

            formaPagamentoViewModel.Id = id;
            return CreatedAtAction(nameof(Get), new { id }, formaPagamentoViewModel);
        }

        // PUT api/<FormasPagamentoController>/5
        [HttpPut("{id}")]
        public ActionResult Put(uint id, [FromBody] FormaPagamentoViewModel formaPagamentoViewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest("Dados inválidos.");

            var formaPagamento = formaPagamentoService.Get(id);
            if (formaPagamento == null)
                return NotFound("Forma de pagamento não encontrada.");

            formaPagamentoViewModel.Id = id;
            FormaPagamentoMapper.ToEntity(formaPagamentoViewModel, formaPagamento);
            formaPagamentoService.Edit(formaPagamento);

            return Ok();
        }

        // DELETE api/<FormasPagamentoController>/5
        [HttpDelete("{id}")]
        public ActionResult Delete(uint id)
        {
            if (formaPagamentoService.Get(id) == null)
                return NotFound("Forma de pagamento não encontrada.");

            formaPagamentoService.Delete(id);
            return Ok();
        }
    }
}
