using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service
{
    public class FormaPagamentoService : IFormaPagamentoService
    {
        private readonly RestauranteContext context;

        public FormaPagamentoService(RestauranteContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Cria uma nova forma de pagamento, caso não exista outra com o mesmo nome
        /// </summary>
        /// <param name="formaPagamento">A forma de pagamento a ser criada</param>
        /// <returns>O id da forma de pagamento criada</returns>
        /// <exception cref="ServiceException"></exception>
        public uint Create(Formapagamento formaPagamento)
        {
            if (context.Formapagamentos.Any(f => f.Nome == formaPagamento.Nome))
            {
                throw new ServiceException("Já existe uma forma de pagamento com o mesmo nome.");
            }

            context.Add(formaPagamento);
            context.SaveChanges();
            return formaPagamento.Id;
        }

        /// <summary>
        /// Edita uma forma de pagamento, caso não exista outra com o mesmo nome
        /// </summary>
        /// <param name="formaPagamento">A forma de pagamento a ser editada</param>
        /// <exception cref="ServiceException"></exception>
        public void Edit(Formapagamento formaPagamento)
        {
            if (context.Formapagamentos.Any(f => f.Nome == formaPagamento.Nome && f.Id != formaPagamento.Id))
            {
                throw new ServiceException("Já existe uma forma de pagamento com o mesmo nome.");
            }

            context.Update(formaPagamento);
            context.SaveChanges();
        }

        /// <summary>
        /// Exclui uma forma de pagamento, caso não exista nenhum pagamento associado a ela.
        /// Se a forma de pagamento não for encontrada, nenhuma ação será realizada.
        /// </summary>
        /// <param name="id">O id da forma de pagamento a ser excluída</param>
        /// <exception cref="ServiceException"></exception>
        public void Delete(uint id)
        {
            var formaPagamento = context.Formapagamentos.Find(id);
            if (formaPagamento == null) return;

            if (context.Pagamentos.Any(p => p.IdFormaPagamento == id))
            {
                throw new ServiceException("Não é possível excluir a forma de pagamento, pois existem pagamentos associados a ela.");
            }

            context.Remove(formaPagamento);
            context.SaveChanges();
        }

        /// <summary>
        /// Busca uma forma de pagamento pelo id, caso exista
        /// </summary>
        /// <param name="id">O id da forma de pagamento</param>
        /// <returns>A forma de pagamento encontrada ou nulo</returns>
        public Formapagamento? Get(uint id)
        {
            return context.Formapagamentos.Find(id);
        }

        /// <summary>
        /// Busca todas as formas de pagamento cadastradas, ordenadas pelo nome
        /// </summary>
        /// <returns>Uma coleção de formas de pagamento</returns>
        public IEnumerable<Formapagamento> GetAll()
        {
            return context.Formapagamentos.AsNoTracking().OrderBy(f => f.Nome);
        }
    }
}
