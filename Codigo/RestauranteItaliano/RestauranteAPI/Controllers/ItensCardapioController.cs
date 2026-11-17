using Core.Service;
using Microsoft.AspNetCore.Mvc;
using RestauranteAPI.Mappers;
using RestauranteAPI.Models;

namespace RestauranteAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItensCardapioController : ControllerBase
    {
        private readonly IItemCardapioService itemCardapioService;

        public ItensCardapioController(IItemCardapioService itemCardapioService)
        {
            this.itemCardapioService = itemCardapioService;
        }

        // GET: api/<ItensCardapioController>
        [HttpGet]
        public ActionResult Get()
        {
            var listaItensCardapio = itemCardapioService.GetAll();
            return Ok(ItemCardapioMapper.ToViewModel(listaItensCardapio));
        }

        // GET api/<ItensCardapioController>/5
        [HttpGet("{id}")]
        public ActionResult Get(uint id)
        {
            var itemCardapio = itemCardapioService.Get(id);
            if (itemCardapio == null)
                return NotFound("Item do cardápio não encontrado.");
            return Ok(ItemCardapioMapper.ToViewModel(itemCardapio));
        }

        // POST api/<ItensCardapioController>
        [HttpPost]
        public ActionResult Post([FromBody] ItemCardapioViewModel itemCardapioViewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest("Dados inválidos.");

            var itemCardapio = ItemCardapioMapper.ToEntity(itemCardapioViewModel);
            var id = itemCardapioService.Create(itemCardapio);

            itemCardapioViewModel.Id = id;
            return CreatedAtAction(nameof(Get), new { id }, itemCardapioViewModel);
        }

        // PUT api/<ItensCardapioController>/5
        [HttpPut("{id}")]
        public ActionResult Put(uint id, [FromBody] ItemCardapioViewModel itemCardapioViewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest("Dados inválidos.");

            if (itemCardapioService.Get(id) == null)
                return NotFound("Item do cardápio não encontrado.");

            itemCardapioViewModel.Id = id;
            var itemCardapio = ItemCardapioMapper.ToEntity(itemCardapioViewModel);
            itemCardapioService.Edit(itemCardapio);

            return Ok();
        }

        // PUT api/<ItensCardapioController>/5/disponibilidade
        // marca o item como disponível (true) ou em falta (false), por exemplo pelo aplicativo da cozinha
        [HttpPut("{id}/disponibilidade")]
        public ActionResult PutDisponibilidade(uint id, [FromBody] bool disponivel)
        {
            if (itemCardapioService.Get(id) == null)
                return NotFound("Item do cardápio não encontrado.");

            itemCardapioService.AlterarDisponibilidade(id, disponivel);
            return Ok();
        }

        // DELETE api/<ItensCardapioController>/5
        [HttpDelete("{id}")]
        public ActionResult Delete(uint id)
        {
            if (itemCardapioService.Get(id) == null)
                return NotFound("Item do cardápio não encontrado.");

            itemCardapioService.Delete(id);
            return Ok();
        }
    }
}
