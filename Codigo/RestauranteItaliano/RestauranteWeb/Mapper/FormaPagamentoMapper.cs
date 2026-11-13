using Core;
using Mapster;
using RestauranteWeb.Models;

namespace RestauranteWeb.Mapper
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

        public static IEnumerable<FormaPagamentoViewModel> ToViewModel(IEnumerable<Formapagamento>? formasPagamento)
        {
            return formasPagamento?.Adapt<IEnumerable<FormaPagamentoViewModel>>() ?? new List<FormaPagamentoViewModel>();
        }

        public static IEnumerable<Formapagamento> ToEntity(IEnumerable<FormaPagamentoViewModel>? formasPagamentoViewModel)
        {
            return formasPagamentoViewModel?.Adapt<IEnumerable<Formapagamento>>() ?? new List<Formapagamento>();
        }
    }
}
