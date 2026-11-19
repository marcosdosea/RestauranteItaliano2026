using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service
{
    public class TipoFuncionarioService : ITipoFuncionario
    {
        private readonly RestauranteContext context;

        public TipoFuncionarioService(RestauranteContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Cria um novo tipo funcionario no banco de dados e retorna o id do tipo funcionario criado.
        /// </summary>
        /// <param name="tipoFuncionario"> O tipo funcionario a ser criado. </param>
        /// <returns> O id do tipo funcionario criado. </returns>
        public uint Create(Tipofuncionario tipoFuncionario)
        {
            context.Add(tipoFuncionario);
            context.SaveChanges();
            return tipoFuncionario.Id;
        }

        /// <summary>
        /// Exclui um tipo funcionario do banco de dados com base no id fornecido. 
        ///Se o tipo funcionario não for encontrado, nenhuma ação será realizada.
        /// </summary>
        /// <param name="id"> O id do tipo funcionario a ser excluído. </param>
        public void Delete(uint id)
        {
            var tipoFuncionario = context.Tipofuncionarios.Find(id);
            if (tipoFuncionario != null)
            {
                context.Remove(tipoFuncionario);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Atualiza as informações de um tipo funcionario existente no banco de dados. 
        ///Se o tipo funcionario não for encontrado, nenhuma ação será realizada.    
        /// </summary>
        /// <param name="tipoFuncionario"> O tipo funcionario a ser atualizado. </param>
        public void Edit(Tipofuncionario tipoFuncionario)
        {
            context.Update(tipoFuncionario);
            context.SaveChanges();
        }

        /// <summary>
        /// Obtém um tipo funcionario pelo id.
        /// </summary>
        /// <param name="id">id do tipo funcionario</param>
        /// <returns></returns>
        public Tipofuncionario? Get(uint id)
        {
            return context.Tipofuncionarios.Find(id);
        }

        /// <summary>
        /// Recupera todos os tipos funcionarios do banco de dados.
        /// </summary>
        /// <returns>uma coleção de tipos funcionarios</returns>
        public IEnumerable<Tipofuncionario> GetAll()
        {
            return context.Tipofuncionarios.AsNoTracking();
        }
    }
}
