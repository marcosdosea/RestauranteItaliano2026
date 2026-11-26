using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service
{
    public class FuncionarioService : IFuncionarioService
    {
        private readonly RestauranteContext context;

        public FuncionarioService(RestauranteContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Cria um novo funcionário, caso o restaurante e o tipo de funcionário existam
        /// </summary>
        /// <param name="funcionario"></param>
        /// <returns></returns>
        /// <exception cref="ServiceException"></exception>
        public int Create(Funcionario funcionario)
        {
            if (funcionario == null)
            {
                throw new ServiceException(
                    "Os dados do funcionário não foram informados.");
            }

            ValidarReferencias(funcionario);

            context.Funcionarios.Add(funcionario);
            context.SaveChanges();

            return (int)funcionario.Id;
        }

        /// <summary>
        /// Edita um funcionário, caso ele exista e o restaurante e o tipo de funcionário existam
        /// </summary>
        /// <param name="funcionario"></param>
        /// <exception cref="ServiceException"></exception>
        public void Edit(Funcionario funcionario)
        {
            if (funcionario == null)
            {
                throw new ServiceException(
                    "Os dados do funcionário não foram informados.");
            }

            var funcionarioExiste = context.Funcionarios
                .Any(f => f.Id == funcionario.Id);

            if (!funcionarioExiste)
            {
                throw new ServiceException("Funcionário não encontrado.");
            }

            ValidarReferencias(funcionario);

            context.Funcionarios.Update(funcionario);
            context.SaveChanges();
        }

        /// <summary>
        /// Deleta um funcionário, caso ele exista e não esteja associado a nenhum pedido
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="ServiceException"></exception>
        public void Delete(int id)
        {
            if (id <= 0)
            {
                throw new ServiceException(
                    "Identificador de funcionário inválido.");
            }

            var funcionario = context.Funcionarios
                .FirstOrDefault(f => f.Id == (uint)id);

            if (funcionario == null)
            {
                throw new ServiceException("Funcionário não encontrado.");
            }

            var possuiPedidos = context.Pedidos
                .Any(p => p.IdGarcom == funcionario.Id);

            if (possuiPedidos)
            {
                throw new ServiceException(
                    "Não é possível excluir o funcionário porque ele está associado a pedidos.");
            }

            context.Funcionarios.Remove(funcionario);
            context.SaveChanges();
        }

        /// <summary>
        /// Busca um funcionário pelo id, caso ele exista
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="ServiceException"></exception>
        public Funcionario Get(int id)
        {
            if (id <= 0)
            {
                throw new ServiceException(
                    "Identificador de funcionário inválido.");
            }

            return context.Funcionarios
                .AsNoTracking()
                .FirstOrDefault(f => f.Id == (uint)id)
                ?? throw new ServiceException("Funcionário não encontrado.");
        }

        /// <summary>
        /// Busca todos os funcionários, incluindo informações do restaurante e do tipo de funcionário
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Funcionario> GetAll()
        {
            return context.Funcionarios
                .Include(f => f.IdRestauranteNavigation)
                .Include(f => f.IdTipoFuncionarioNavigation)
                .AsNoTracking()
                .OrderBy(f => f.Nome);
        }

        /// <summary>
        /// Busca funcionários pelo nome, caso informado, incluindo informações do restaurante e do tipo de funcionário
        /// </summary>
        /// <param name="nome"></param>
        /// <returns></returns>
        public IEnumerable<Funcionario> GetByNome(string nome)
        {
            var funcionarios = context.Funcionarios
                .AsNoTracking();

            if (string.IsNullOrWhiteSpace(nome))
            {
                return funcionarios.OrderBy(f => f.Nome);
            }

            var termo = nome.Trim();

            return funcionarios
                .Where(f => f.Nome.Contains(termo))
                .OrderBy(f => f.Nome);
        }

        /// <summary>
        /// Busca todos os garçons de um restaurante específico, caso o restaurante exista
        /// </summary>
        /// <param name="idRestaurante"></param>
        /// <returns></returns>
        public IEnumerable<Funcionario> GetGarconsPorRestaurante(
            uint idRestaurante)
        {
            return context.Funcionarios
                .AsNoTracking()
                .Where(f =>
                    f.IdRestaurante == idRestaurante &&
                    f.IdTipoFuncionarioNavigation.Nome == "Garçom")
                .OrderBy(f => f.Nome);
        }

        /// <summary>
        /// Valida se o restaurante e o tipo de funcionário existem, caso contrário lança uma exceção
        /// </summary>
        /// <param name="funcionario"></param>
        /// <exception cref="ServiceException"></exception>
        private void ValidarReferencias(Funcionario funcionario)
        {
            var restauranteExiste = context.Restaurantes
                .Any(r => r.Id == funcionario.IdRestaurante);

            if (!restauranteExiste)
            {
                throw new ServiceException("Restaurante não encontrado.");
            }

            var tipoExiste = context.Tipofuncionarios
                .Any(t => t.Id == funcionario.IdTipoFuncionario);

            if (!tipoExiste)
            {
                throw new ServiceException(
                    "Tipo de funcionário não encontrado.");
            }
        }
    }
}