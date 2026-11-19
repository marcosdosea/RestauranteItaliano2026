using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MySqlX.XDevAPI.Common;
using System.Text.RegularExpressions;

namespace Service
{
    /// <summary>
    /// Versão mínima do serviço de restaurantes, criada para o CS07 (Manter Itens do Cardápio),
    /// que precisa listar os restaurantes. Cadastro, edição e exclusão ficam para o CS05.
    /// </summary>
    public class RestauranteService : IRestauranteService
    {
        private readonly RestauranteContext context;

        public RestauranteService(RestauranteContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Cria um novo restaurante no banco de dados, após validar os dados do restaurante.
        /// </summary>
        /// <param name="restaurante"></param>
        /// <returns>O id do restaurante criado</returns>
        /// <exception cref="ServiceException"></exception>

        public uint Create(Restaurante restaurante)
        {
            Validar(restaurante);
            context.Add(restaurante);
            context.SaveChanges();
            return restaurante.Id;  
        }

        /// <summary>
        /// Edita um restaurante existente no banco de dados, após validar os dados do restaurante.
        /// </summary>
        /// <param name="restaurante">O restaurante a ser editado</param>
        /// <exception cref="ServiceException"></exception>
        public void Edit(Restaurante restaurante)
        {
            Validar(restaurante);
            context.Update(restaurante);
            context.SaveChanges();
        }

        /// <summary>
        /// Exclui um restaurante do banco de dados, caso não haja dados associados a ele (funcionários, itens do cardápio ou mesas).
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="ServiceException"></exception>
        public void Delete(uint id)
        {
            var restaurante = context.Restaurantes.Find(id);
            if (restaurante == null) return;
                
            if (context.Funcionarios.Any(f => f.IdRestaurante == id) ||
                context.Itemcardapios.Any(i => i.IdRestaurante == id) ||
                context.Mesas.Any(m => m.IdRestaurante == id))
            {
                throw new ServiceException("Não é possível excluir o restaurante, pois ele possui funcionários, itens de cardápio ou mesas cadastrados.");
            }
            context.Remove(restaurante);
            context.SaveChanges();
        }

        /// <summary>
        /// Busca um restaurante pelo id, junto com o tipo de restaurante associado, caso exista. Retorna null se não encontrar.
        /// </summary>
        /// <param name="id">O id do restaurante</param>
        /// <returns>O restaurante encontrado</returns>
        /// <exception cref="ServiceException"></exception>
        public Restaurante? Get(uint id)
        {
           return context.Restaurantes.Include(r => r.IdTipoRestauranteNavigation)
                .AsNoTracking()
                .FirstOrDefault(r => r.Id == id);
        }

        /// <summary>
        /// Busca todos os restaurantes cadastrados, ordenados pelo nome
        /// </summary>
        /// <returns>Uma coleção de restaurantes</returns>
        public IEnumerable<Restaurante> GetAll()
        {
            return context.Restaurantes.Include(r => r.IdTipoRestauranteNavigation)
                .AsNoTracking()
                .OrderBy(r => r.Nome);
        }

        private void Validar(Restaurante restaurante)
        {
            restaurante.Nome = restaurante.Nome.Trim();
            restaurante.Cnpj = SoNumero(restaurante.Cnpj) ?? string.Empty;
            restaurante.Cep = SoNumero(restaurante.Cep);
            restaurante.Telefone1 = SoNumero(restaurante.Telefone1);
            restaurante.Telefone2 = SoNumero(restaurante.Telefone2);
            restaurante.Estado = string.IsNullOrWhiteSpace(restaurante.Estado) 
                ? null : restaurante.Estado.Trim().ToUpper();

            if (!context.Tiporestaurantes.Any(t => t.Id == restaurante.IdTipoRestaurante))
            {
                throw new ServiceException("O tipo de restaurante informado não existe.");
            }
            if (context.Restaurantes.Any(r => r.Cnpj == restaurante.Cnpj && r.Id != restaurante.Id))
            {
                throw new ServiceException("Já existe um restaurante cadastrado com esse CNPJ.");
            }
        }

        private static string? SoNumero(string? valor)
        {
            return string.IsNullOrWhiteSpace(valor) ? null : Regex.Replace(valor, "[^0-9]", "");
        }
    }
}
