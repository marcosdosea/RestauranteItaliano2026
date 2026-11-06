using RestauranteWeb.Models;
using Core;
using Mapster;

namespace RestauranteWeb.Mapper
{
    public class TipoFuncionarioMapper
    {
        public static TipoFuncionarioViewModel ToViewModel(Tipofuncionario tipoFuncionario)
        {
            return tipoFuncionario.Adapt<TipoFuncionarioViewModel>();
        }

        public static Tipofuncionario ToEntity(TipoFuncionarioViewModel tipoFuncionarioViewModel)
        {
            return tipoFuncionarioViewModel.Adapt<Tipofuncionario>();
        }

        public static IEnumerable<TipoFuncionarioViewModel> ToViewModel(IEnumerable<Tipofuncionario> tiposFuncionarios)
        {
            return tiposFuncionarios.Adapt<IEnumerable<TipoFuncionarioViewModel>>();
        }

        public static IEnumerable<Tipofuncionario> ToEntity(IEnumerable<TipoFuncionarioViewModel> tiposFuncionariosViewModel)
        {
            return tiposFuncionariosViewModel.Adapt<IEnumerable<Tipofuncionario>>();
        }
    }
}
