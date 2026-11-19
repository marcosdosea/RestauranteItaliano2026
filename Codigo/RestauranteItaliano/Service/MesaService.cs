using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service
{
    /// <summary>
    /// Serviço das mesas do restaurante (CS08 - Manter Mesas do Restaurante)
    /// </summary>
    public class MesaService : IMesaService
    {
        private readonly RestauranteContext context;

        public MesaService(RestauranteContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Cria uma nova mesa no banco de dados, após validar os dados da mesa.
        /// </summary>
        /// <param name="mesa">A mesa a ser criada</param>
        /// <returns>O id da mesa criada</returns>
        /// <exception cref="ServiceException"></exception>
        public int Create(Mesa mesa)
        {
            Validar(mesa);
            context.Add(mesa);
            context.SaveChanges();
            return mesa.Id;
        }

        /// <summary>
        /// Edita uma mesa existente no banco de dados, após validar os dados da mesa.
        /// </summary>
        /// <param name="mesa">A mesa a ser editada</param>
        /// <exception cref="ServiceException"></exception>
        public void Edit(Mesa mesa)
        {
            Validar(mesa);

            // uma mesa que já teve atendimentos não pode mudar de restaurante,
            // senão o histórico dela passaria a pertencer a outra casa
            var idRestauranteAtual = context.Mesas
                .Where(m => m.Id == mesa.Id)
                .Select(m => (uint?)m.IdRestaurante)
                .FirstOrDefault();
            if (idRestauranteAtual != null && idRestauranteAtual != mesa.IdRestaurante &&
                context.Atendimentos.Any(a => a.IdMesa == mesa.Id))
            {
                throw new ServiceException("Não é possível trocar o restaurante de uma mesa que já possui atendimentos.");
            }

            context.Update(mesa);
            context.SaveChanges();
        }

        /// <summary>
        /// Exclui uma mesa do banco de dados, caso ela não tenha atendimentos registrados.
        /// </summary>
        /// <param name="id">O id da mesa</param>
        /// <exception cref="ServiceException"></exception>
        public void Delete(int id)
        {
            var mesa = context.Mesas.Find(id);
            if (mesa == null) return;

            if (context.Atendimentos.Any(a => a.IdMesa == id))
            {
                throw new ServiceException("Não é possível excluir a mesa, pois ela possui atendimentos registrados.");
            }
            context.Remove(mesa);
            context.SaveChanges();
        }

        /// <summary>
        /// Busca uma mesa pelo id, junto com o restaurante ao qual pertence. Retorna null se não encontrar.
        /// </summary>
        /// <param name="id">O id da mesa</param>
        /// <returns>A mesa encontrada</returns>
        public Mesa? Get(int id)
        {
            return context.Mesas.Include(m => m.IdRestauranteNavigation)
                .AsNoTracking()
                .FirstOrDefault(m => m.Id == id);
        }

        /// <summary>
        /// Busca todas as mesas cadastradas, ordenadas pelo nome do restaurante e pela identificação
        /// </summary>
        /// <returns>Uma coleção de mesas</returns>
        public IEnumerable<Mesa> GetAll()
        {
            return context.Mesas.Include(m => m.IdRestauranteNavigation)
                .AsNoTracking()
                .OrderBy(m => m.IdRestauranteNavigation.Nome)
                .ThenBy(m => m.Identificacao);
        }

        private void Validar(Mesa mesa)
        {
            mesa.Identificacao = (mesa.Identificacao ?? string.Empty).Trim();

            if (string.IsNullOrEmpty(mesa.Identificacao))
            {
                throw new ServiceException("A identificação da mesa é obrigatória.");
            }
            if (!context.Restaurantes.Any(r => r.Id == mesa.IdRestaurante))
            {
                throw new ServiceException("O restaurante informado não existe.");
            }
            // a mesma identificação pode existir em restaurantes diferentes, mas não no mesmo
            var identificacao = mesa.Identificacao.ToLower();
            if (context.Mesas.Any(m => m.IdRestaurante == mesa.IdRestaurante &&
                                       m.Identificacao.ToLower() == identificacao &&
                                       m.Id != mesa.Id))
            {
                throw new ServiceException("Já existe uma mesa com essa identificação neste restaurante.");
            }
        }
    }
}
