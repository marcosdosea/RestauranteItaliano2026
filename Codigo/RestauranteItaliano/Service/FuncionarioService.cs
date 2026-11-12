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
        /// Cria um novo funcionario no banco de dados e retorna o id do funcionario criado.
        /// </summary>
        /// <param name="funcionario"> O funcionario a ser criado. </param>
        /// <returns> O id do funcionario criado. </returns>
        public uint Create(Funcionario funcionario)
        {
            context.Add(funcionario);
            context.SaveChanges();
            return funcionario.Id;
        }

        /// <summary>
        /// Exclui um funcionario do banco de dados com base no id fornecido. 
        ///Se o funcionario não for encontrado, nenhuma ação será realizada.
        /// </summary>
        /// <param name="id"> O id do funcionario a ser excluído. </param>
        /// 
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
        /// Atualiza as informações de um funcionario existente no banco de dados. 
        ///Se o funcionario não for encontrado, nenhuma ação será realizada.    
        /// </summary>
        /// <param name="funcionario"> O funcionario a ser atualizado. </param>
        public void Edit(Funcionario funcionario)
        {
            context.Update(funcionario);
            context.SaveChanges();
        }

        /// <summary>
        /// Obtém um funcionario pelo id.
        /// </summary>
        /// <param name="id">id do funcionario</param>
        /// <returns></returns>
        public Funcionario? Get(uint id)
        {
            return context.Funcionarios
                .Include(funcionario => funcionario.IdTipoFuncionarioNavigation)
                .Include(funcionario => funcionario.IdRestauranteNavigation)
                .FirstOrDefault(funcionario => funcionario.Id == id);
        }

        /// <summary>
        /// Recupera todos os funcionarios do banco de dados.
        /// </summary>
        /// <returns>uma coleção de funcionarios</returns>

        public IEnumerable<Funcionario> GetAll()
        {
            return context.Funcionarios
                .Include(funcionario => funcionario.IdTipoFuncionarioNavigation)
                .Include(funcionario => funcionario.IdRestauranteNavigation)
                .AsNoTracking();
        }

        /// <summary>
        /// Obtém um funcionario pelo nome.
        /// </summary>
        /// <param name="nome">nome do funcionario</param>
        /// <returns></returns>
        public IEnumerable<Funcionario> GetByNome(string nome)
        {
            return context.Funcionarios
                .Where(funcionario => funcionario.Nome.Contains(nome))
                .AsNoTracking();
        }

        public DatatableResponse<Funcionario> GetDataPage(DatatableRequest request)
        {
            var funcionarios = context.Funcionarios
                .Include(funcionario => funcionario.IdTipoFuncionarioNavigation)
                .Include(funcionario => funcionario.IdRestauranteNavigation)
                .AsNoTracking();

            // Total de registros
            var totalRecords = funcionarios.Count();

            // Pesquisa
            if (request.Search != null &&
                request.Search.GetValueOrDefault("value") != null)
            {
                var searchValue = request.Search.GetValueOrDefault("value") ?? string.Empty;
                funcionarios = funcionarios.Where(funcionario =>
                funcionario.Id.ToString().Contains(searchValue) ||
                funcionario.Nome.ToLower().Contains(searchValue.ToLower()) ||
                funcionario.Cpf.Contains(searchValue) ||
                 funcionario.IdTipoFuncionarioNavigation.Nome
                    .ToLower().Contains(searchValue.ToLower()));
            }

            // Ordenação pelas colunas permitidas
            if (request.Order != null && request.Order.Count > 0 &&
                request.Order[0].GetValueOrDefault("column").Equals("0"))
            {
                if (request.Order[0].GetValueOrDefault("dir").Equals("asc"))
                    funcionarios = funcionarios.OrderBy(funcionario => funcionario.Nome);
                else
                    funcionarios = funcionarios.OrderByDescending(funcionario => funcionario.Nome);
            }
            else if (request.Order != null && request.Order.Count > 0 &&
                     request.Order[0].GetValueOrDefault("column").Equals("1"))
            {
                if (request.Order[0].GetValueOrDefault("dir").Equals("asc"))
                    funcionarios = funcionarios.OrderBy(funcionario => funcionario.Cpf);
                else
                    funcionarios = funcionarios.OrderByDescending(funcionario => funcionario.Cpf);
            }
            else if (request.Order != null && request.Order.Count > 0 &&
                     request.Order[0].GetValueOrDefault("column").Equals("2"))
            {
                if (request.Order[0].GetValueOrDefault("dir").Equals("asc"))
                    funcionarios = funcionarios.OrderBy(funcionario => funcionario.IdTipoFuncionarioNavigation.Nome);
                else
                    funcionarios = funcionarios.OrderByDescending(funcionario => funcionario.IdTipoFuncionarioNavigation.Nome);
            }
            else if (request.Order != null && request.Order.Count > 0 &&
                     request.Order[0].GetValueOrDefault("column").Equals("3"))
            {
                if (request.Order[0].GetValueOrDefault("dir").Equals("asc"))
                    funcionarios = funcionarios.OrderBy(funcionario => funcionario.Id);
                else
                    funcionarios = funcionarios.OrderByDescending(funcionario => funcionario.Id);
            }

            // Total de registros após a pesquisa
            int countRecordsFiltered = funcionarios.Count();

            // Paginação
            funcionarios = funcionarios.Skip(request.Start).Take(request.Length);
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
