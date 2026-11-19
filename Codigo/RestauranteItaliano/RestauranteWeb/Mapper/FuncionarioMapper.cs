
using RestauranteWeb.Models;
using Core;
using Mapster;
using Util;

namespace RestauranteWeb.Mapper
{
    public class FuncionarioMapper
    {
        public static FuncionarioViewModel ToViewModel(Funcionario funcionario)
        {
            var funcionarioViewModel = funcionario.Adapt<FuncionarioViewModel>();

            funcionarioViewModel.NomeTipoFuncionario =
                funcionario.IdTipoFuncionarioNavigation?.Nome;

            funcionarioViewModel.NomeRestaurante =
                funcionario.IdRestauranteNavigation?.Nome;

            return funcionarioViewModel;
        }

        public static Funcionario ToEntity(FuncionarioViewModel funcionarioViewModel)
        {
            var funcionario = funcionarioViewModel.Adapt<Funcionario>();

            funcionario.Cpf = Methods.RemoveNaoNumericos(funcionarioViewModel.Cpf);

            return funcionario;
        }

        public static IEnumerable<FuncionarioViewModel> ToViewModel(IEnumerable<Funcionario> funcionarios)
        {
            return funcionarios.Select(f => ToViewModel(f));
        }

        public static IEnumerable<Funcionario> ToEntity(IEnumerable<FuncionarioViewModel> funcionariosViewModel)
        {
            return funcionariosViewModel.Adapt<IEnumerable<Funcionario>>();
        }
    }
}
