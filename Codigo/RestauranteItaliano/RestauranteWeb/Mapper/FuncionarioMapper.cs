using Core;
using Mapster;
using RestauranteWeb.Models;

namespace RestauranteWeb.Mapper
{
    public class FuncionarioMapper
    {
        static FuncionarioMapper()
        {
            TypeAdapterConfig<FuncionarioViewModel, Funcionario>.NewConfig()
                .Map(destino => destino.Id, origem => origem.Id ?? 0)
                .Map(
                    destino => destino.IdRestaurante,
                    origem => origem.IdRestaurante ?? 0)
                .Map(
                    destino => destino.IdTipoFuncionario,
                    origem => origem.IdTipoFuncionario ?? 0)
                .Ignore(destino => destino.IdRestauranteNavigation)
                .Ignore(destino => destino.IdTipoFuncionarioNavigation)
                .Ignore(destino => destino.Pedidos);

            TypeAdapterConfig<Funcionario, FuncionarioViewModel>.NewConfig()
                .Map(
                    destino => destino.NomeRestaurante,
                    origem => origem.IdRestauranteNavigation != null
                        ? origem.IdRestauranteNavigation.Nome
                        : null)
                .Map(
                    destino => destino.NomeTipoFuncionario,
                    origem => origem.IdTipoFuncionarioNavigation != null
                        ? origem.IdTipoFuncionarioNavigation.Nome
                        : null)
                .Ignore(destino => destino.Restaurantes)
                .Ignore(destino => destino.TiposFuncionario);
        }

        public static Funcionario ToEntity(
            FuncionarioViewModel funcionarioViewModel)
        {
            return funcionarioViewModel.Adapt<Funcionario>();
        }

        public static FuncionarioViewModel ToViewModel(
            Funcionario funcionario)
        {
            return funcionario.Adapt<FuncionarioViewModel>();
        }

        public static IEnumerable<FuncionarioViewModel> ToViewModel(
            IEnumerable<Funcionario> funcionarios)
        {
            return funcionarios.Adapt<IEnumerable<FuncionarioViewModel>>();
        }
    }
}