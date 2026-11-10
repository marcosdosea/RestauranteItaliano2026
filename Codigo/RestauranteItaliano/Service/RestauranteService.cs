using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service
{
    /// <summary>
    /// Versão mínima do serviço de restaurantes, criada para o CS07 (Manter Itens do Cardápio),
    /// que precisa listar os restaurantes. Cadastro, edição e exclusão ficam para o CS05.
    /// </summary>
    public class RestauranteService : IRestauranteService
    {
        private readonly RestauranteContext _context;

        public RestauranteService(RestauranteContext context)
        {
            _context = context;
        }

        public int Create(Restaurante restaurante)
        {
            throw new NotImplementedException("Será implementado no CS05 - Manter Restaurantes.");
        }

        public void Edit(Restaurante restaurante)
        {
            throw new NotImplementedException("Será implementado no CS05 - Manter Restaurantes.");
        }

        public void Delete(int id)
        {
            throw new NotImplementedException("Será implementado no CS05 - Manter Restaurantes.");
        }

        /// <summary>
        /// Busca um restaurante pelo id
        /// </summary>
        /// <param name="id">O id do restaurante</param>
        /// <returns>O restaurante encontrado</returns>
        /// <exception cref="ServiceException"></exception>
        public Restaurante Get(int id)
        {
            return _context.Restaurantes.Find((uint)id)
                ?? throw new ServiceException("Restaurante não encontrado.");
        }

        /// <summary>
        /// Busca todos os restaurantes cadastrados, ordenados pelo nome
        /// </summary>
        /// <returns>Uma coleção de restaurantes</returns>
        public IEnumerable<Restaurante> GetAll()
        {
            return _context.Restaurantes.AsNoTracking().OrderBy(r => r.Nome);
        }
    }
}
