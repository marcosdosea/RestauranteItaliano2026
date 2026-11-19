using Core;
using Mapster;
using RestauranteAPI.Models;

namespace RestauranteAPI.Mappers
{
    public class FormaPagamentoMapper
    {
        public static FormaPagamentoViewModel ToViewModel(Formapagamento formaPagamento)
        {
            return formaPagamento.Adapt<FormaPagamentoViewModel>();
        }

        public static Formapagamento ToEntity(FormaPagamentoViewModel formaPagamentoViewModel)
        {
            return formaPagamentoViewModel.Adapt<Formapagamento>();
        }

        /// <summary>
        /// Copia os dados da ViewModel para uma forma de pagamento que já existe (a que o service buscou),
        /// para editar a mesma instância que o Entity Framework já está acompanhando
        /// </summary>
        public static void ToEntity(FormaPagamentoViewModel formaPagamentoViewModel, Formapagamento formaPagamento)
        {
            formaPagamentoViewModel.Adapt(formaPagamento);
        }

        public static IEnumerable<FormaPagamentoViewModel> ToViewModel(IEnumerable<Formapagamento>? formasPagamento)
        {
            return formasPagamento?.Adapt<IEnumerable<FormaPagamentoViewModel>>() ?? new List<FormaPagamentoViewModel>();
        }
    }
}
