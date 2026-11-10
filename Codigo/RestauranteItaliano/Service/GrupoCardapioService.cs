using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service
{
    public class GrupoCardapioService : IGrupoCardapioService
    {
        private readonly RestauranteContext _context;

        public GrupoCardapioService(RestauranteContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Cria um novo grupo do cardápio, caso não exista outro com o mesmo nome
        /// </summary>
        /// <param name="grupoCardapio">O grupo a ser criado</param>
        /// <returns>O id do grupo criado</returns>
        /// <exception cref="ServiceException"></exception>
        public uint Create(Grupocardapio grupoCardapio)
        {
            if (_context.Grupocardapios.Any(g => g.Nome == grupoCardapio.Nome))
            {
                throw new ServiceException("Já existe um grupo do cardápio com o mesmo nome.");
            }

            _context.Add(grupoCardapio);
            _context.SaveChanges();
            return grupoCardapio.Id;
        }

        /// <summary>
        /// Edita um grupo do cardápio, caso não exista outro com o mesmo nome
        /// </summary>
        /// <param name="grupoCardapio">O grupo a ser editado</param>
        /// <exception cref="ServiceException"></exception>
        public void Edit(Grupocardapio grupoCardapio)
        {
            if (_context.Grupocardapios.Any(g => g.Nome == grupoCardapio.Nome && g.Id != grupoCardapio.Id))
            {
                throw new ServiceException("Já existe um grupo do cardápio com o mesmo nome.");
            }

            _context.Update(grupoCardapio);
            _context.SaveChanges();
        }

        /// <summary>
        /// Exclui um grupo do cardápio, caso não exista nenhum item do cardápio associado a ele.
        /// Se o grupo não for encontrado, nenhuma ação será realizada.
        /// </summary>
        /// <param name="id">O id do grupo a ser excluído</param>
        /// <exception cref="ServiceException"></exception>
        public void Delete(uint id)
        {
            var grupo = _context.Grupocardapios.Find(id);
            if (grupo == null) return;

            if (_context.Itemcardapios.Any(i => i.IdGrupoCardapio == id))
            {
                throw new ServiceException("Não é possível excluir o grupo, pois existem itens do cardápio associados a ele.");
            }

            _context.Remove(grupo);
            _context.SaveChanges();
        }

        /// <summary>
        /// Busca um grupo do cardápio pelo id, caso exista
        /// </summary>
        /// <param name="id">O id do grupo</param>
        /// <returns>O grupo encontrado ou nulo</returns>
        public Grupocardapio? Get(uint id)
        {
            return _context.Grupocardapios.Find(id);
        }

        /// <summary>
        /// Busca todos os grupos do cardápio cadastrados, ordenados pelo nome
        /// </summary>
        /// <returns>Uma coleção de grupos do cardápio</returns>
        public IEnumerable<Grupocardapio> GetAll()
        {
            return _context.Grupocardapios.AsNoTracking().OrderBy(g => g.Nome).ToList();
        }
    }
}
