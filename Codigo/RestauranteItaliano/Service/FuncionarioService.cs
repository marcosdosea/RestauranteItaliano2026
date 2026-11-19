using Core;
using Core.Datatables;
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
        /// Cria um novo funcionário no banco de dados e retorna seu ID.
        /// </summary>
        public uint Create(Funcionario funcionario)
        {
            context.Add(funcionario);
            context.SaveChanges();
            return funcionario.Id;
        }

        /// <summary>
        /// Exclui um funcionário pelo ID.
        /// </summary>
        public void Delete(uint id)
        {
            var funcionario = context.Funcionarios.Find(id);

            if (funcionario != null)
            {
                context.Remove(funcionario);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Atualiza as informações de um funcionário.
        /// </summary>
        public void Edit(Funcionario funcionario)
        {
            context.Update(funcionario);
            context.SaveChanges();
        }

        /// <summary>
        /// Obtém um funcionário pelo ID.
        /// </summary>
        public Funcionario? Get(uint id)
        {
            return context.Funcionarios
                .Include(funcionario => funcionario.IdTipoFuncionarioNavigation)
                .Include(funcionario => funcionario.IdRestauranteNavigation)
                .FirstOrDefault(funcionario => funcionario.Id == id);
        }

        /// <summary>
        /// Recupera todos os funcionários.
        /// </summary>
        public IEnumerable<Funcionario> GetAll()
        {
            return context.Funcionarios
                .Include(funcionario => funcionario.IdTipoFuncionarioNavigation)
                .Include(funcionario => funcionario.IdRestauranteNavigation)
                .AsNoTracking();
        }

        /// <summary>
        /// Obtém funcionários pelo nome.
        /// </summary>
        public IEnumerable<Funcionario> GetByNome(string nome)
        {
            return context.Funcionarios
                .Where(funcionario => funcionario.Nome.Contains(nome))
                .AsNoTracking();
        }

        /// <summary>
        /// Recupera os funcionários de forma paginada para o DataTables.
        /// </summary>
        public DatatableResponse<Funcionario> GetDataPage(DatatableRequest request)
        {
            var funcionarios = context.Funcionarios
                .Include(f => f.IdTipoFuncionarioNavigation)
                .Include(f => f.IdRestauranteNavigation)
                .AsNoTracking();

            // Total de registros na tabela.
            var totalRecords = funcionarios.Count();

            // Filtro pelos campos de busca.
            if (request.Search != null &&
                !string.IsNullOrWhiteSpace(request.Search.GetValueOrDefault("value")))
            {
                var searchValue = request.Search.GetValueOrDefault("value") ?? string.Empty;

                funcionarios = funcionarios.Where(f =>
                    f.Id.ToString().Contains(searchValue) ||
                    f.Nome.ToLower().Contains(searchValue.ToLower()) ||
                    f.Cpf.Contains(searchValue) ||
                    f.IdTipoFuncionarioNavigation.Nome.ToLower().Contains(searchValue.ToLower()) ||
                    f.IdRestauranteNavigation.Nome.ToLower().Contains(searchValue.ToLower()));
            }

            // Ordenação pelas colunas permitidas.
            if (request.Order != null && request.Order.Count > 0)
            {
                string coluna = request.Order[0].GetValueOrDefault("column") ?? "0";
                string direcao = request.Order[0].GetValueOrDefault("dir") ?? "asc";

                switch (coluna)
                {
                    case "0":
                        funcionarios = direcao == "asc"
                            ? funcionarios.OrderBy(f => f.Id)
                            : funcionarios.OrderByDescending(f => f.Id);
                        break;

                    case "1":
                        funcionarios = direcao == "asc"
                            ? funcionarios.OrderBy(f => f.Nome)
                            : funcionarios.OrderByDescending(f => f.Nome);
                        break;

                    case "2":
                        funcionarios = direcao == "asc"
                            ? funcionarios.OrderBy(f => f.Cpf)
                            : funcionarios.OrderByDescending(f => f.Cpf);
                        break;

                    case "3":
                        funcionarios = direcao == "asc"
                            ? funcionarios.OrderBy(f => f.IdTipoFuncionarioNavigation.Nome)
                            : funcionarios.OrderByDescending(f => f.IdTipoFuncionarioNavigation.Nome);
                        break;

                    case "4":
                        funcionarios = direcao == "asc"
                            ? funcionarios.OrderBy(f => f.IdRestauranteNavigation.Nome)
                            : funcionarios.OrderByDescending(f => f.IdRestauranteNavigation.Nome);
                        break;
                }
            }

            // Total de registros filtrados.
            int countRecordsFiltered = funcionarios.Count();

            // Paginação.
            funcionarios = funcionarios
                .Skip(request.Start)
                .Take(request.Length);

            return new DatatableResponse<Funcionario>()
            {
                Data = funcionarios.ToList(),
                Draw = request.Draw,
                RecordsFiltered = countRecordsFiltered,
                RecordsTotal = totalRecords
            };
        }
    }
}
