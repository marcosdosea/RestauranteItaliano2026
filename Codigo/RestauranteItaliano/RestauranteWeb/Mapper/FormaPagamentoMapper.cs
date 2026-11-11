using Core;
using Mapster;
using RestauranteWeb.Models;

namespace RestauranteWeb.Mapper
{
    public class FormaPagamentoMapper
    {
        public static FormaPagamentoModel ToViewModel(Formapagamento formaPagamento)
        {
            return formaPagamento.Adapt<FormaPagamentoModel>();
        }

        public static Formapagamento ToEntity(FormaPagamentoModel formaPagamentoModel)
        {
            return formaPagamentoModel.Adapt<Formapagamento>();
        }

        public static IEnumerable<FormaPagamentoModel> ToViewModel(IEnumerable<Formapagamento>? formasPagamento)
        {
            return formasPagamento?.Adapt<IEnumerable<FormaPagamentoModel>>() ?? new List<FormaPagamentoModel>();
        }

        public static IEnumerable<Formapagamento> ToEntity(IEnumerable<FormaPagamentoModel>? formasPagamentoModel)
        {
            return formasPagamentoModel?.Adapt<IEnumerable<Formapagamento>>() ?? new List<Formapagamento>();
        }
    }
}
