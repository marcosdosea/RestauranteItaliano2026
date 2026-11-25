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

        public IEnumerable<Funcionario> GetAll()
        {
            return context.Funcionarios
                .Include(f => f.IdRestauranteNavigation)
                .Include(f => f.IdTipoFuncionarioNavigation)
                .AsNoTracking()
                .OrderBy(f => f.Nome);
        }

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