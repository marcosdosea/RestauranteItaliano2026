using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service
{
    public class TipoRestauranteService : ITipoRestauranteService
    {
        private readonly RestauranteContext _context;
        public TipoRestauranteService(RestauranteContext context)
        {
            _context = context;
        }

        /// <summary>
        /// cria um novo tipo de restaurante, caso não exista outro com o mesmo nome
        /// </summary>
        /// <param name="tipoRestaurante"></param>
        /// <returns></returns>
        /// <exception cref="ServiceException"></exception>
        public uint Create(Tiporestaurante tipoRestaurante)
        {
            if(_context.Tiporestaurantes.Any(t => t.Nome == tipoRestaurante.Nome))
            {
                throw new ServiceException("Já existe um tipo de restaurante com o mesmo nome.");
            }

            _context.Add(tipoRestaurante);
            _context.SaveChanges();
            return tipoRestaurante.Id;
        }

        /// <summary>
        /// edita um tipo de restaurante, caso não exista outro com o mesmo nome
        /// </summary>
        /// <param name="tipoRestaurante"></param>
        /// <exception cref="ServiceException"></exception>

        public void Edit(Tiporestaurante tipoRestaurante)
        {
            if (_context.Tiporestaurantes.Any(t => t.Nome == tipoRestaurante.Nome && t.Id != tipoRestaurante.Id))
            {
                throw new ServiceException("Já existe um tipo de restaurante com o mesmo nome.");
            }
            _context.Update(tipoRestaurante);
            _context.SaveChanges();
        }

        /// <summary>
        /// Deleta um tipo de restaurante, caso não exista nenhum restaurante associado a ele
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="ServiceException"></exception>

        public void Delete(uint id)
        {
            var tipo = _context.Tiporestaurantes.Find(id);
            if (tipo == null) return;

            if(_context.Restaurantes.Any(r => r.IdTipoRestaurante == id))
            {
                throw new ServiceException("Tipo de restaurante não encontrado.");
            }
            _context.Remove(tipo);
            _context.SaveChanges();
        }

        /// <summary>
        /// Busca um tipo de restaurante pelo id, caso exista
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        public Tiporestaurante? Get(uint id)
        {
            //return _context.Tiporestaurantes.AsNoTracking().FirstOrDefault(t => t.Id == id);
            return _context.Tiporestaurantes.Find(id);
        }

        /// <summary>
        /// Busca todos os tipos de restaurante cadastrados
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Tiporestaurante> GetAll()
        {
            return _context.Tiporestaurantes.AsNoTracking();
        }

        /// <summary>
        /// Busca todos os tipos de restaurante que começam com o nome informado
        /// </summary>
        /// <param name="nome"></param>
        /// <returns></returns>
        public IEnumerable<Tiporestaurante> GetbyNome(string nome)
        {
            return _context.Tiporestaurantes.Where(t => t.Nome.StartsWith(nome)).AsNoTracking();
        }

    }
}