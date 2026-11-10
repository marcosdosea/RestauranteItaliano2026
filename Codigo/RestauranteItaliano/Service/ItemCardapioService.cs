using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service
{
    public class ItemCardapioService : IItemCardapioService
    {
        private readonly RestauranteContext _context;

        public ItemCardapioService(RestauranteContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Cria um novo item do cardápio, caso o grupo e o restaurante existam
        /// e não exista outro item com o mesmo nome no mesmo restaurante
        /// </summary>
        /// <param name="itemCardapio">O item a ser criado</param>
        /// <returns>O id do item criado</returns>
        /// <exception cref="ServiceException"></exception>
        public uint Create(Itemcardapio itemCardapio)
        {
            Validar(itemCardapio);

            _context.Add(itemCardapio);
            _context.SaveChanges();
            return itemCardapio.Id;
        }

        /// <summary>
        /// Edita um item do cardápio, com as mesmas regras do cadastro
        /// </summary>
        /// <param name="itemCardapio">O item a ser editado</param>
        /// <exception cref="ServiceException"></exception>
        public void Edit(Itemcardapio itemCardapio)
        {
            Validar(itemCardapio);

            _context.Update(itemCardapio);
            _context.SaveChanges();
        }

        /// <summary>
        /// Exclui um item do cardápio, caso ele ainda não tenha sido usado em nenhum pedido.
        /// Se o item não for encontrado, nenhuma ação será realizada.
        /// </summary>
        /// <param name="id">O id do item a ser excluído</param>
        /// <exception cref="ServiceException"></exception>
        public void Delete(uint id)
        {
            var item = _context.Itemcardapios.Find(id);
            if (item == null) return;

            if (_context.Pedidoitemcardapios.Any(p => p.IdItemCardapio == id))
            {
                throw new ServiceException("Não é possível excluir o item, pois ele já foi usado em pedidos. Para tirá-lo do cardápio, desmarque a opção Ativo no cardápio.");
            }

            _context.Remove(item);
            _context.SaveChanges();
        }

        /// <summary>
        /// Marca um item do cardápio como disponível ou em falta, sem alterar os demais dados
        /// </summary>
        /// <param name="id">O id do item</param>
        /// <param name="disponivel">Verdadeiro para disponível, falso para em falta</param>
        /// <exception cref="ServiceException"></exception>
        public void AlterarDisponibilidade(uint id, bool disponivel)
        {
            var item = _context.Itemcardapios.Find(id)
                ?? throw new ServiceException("Item do cardápio não encontrado.");

            item.Disponivel = disponivel ? "1" : "0";
            _context.SaveChanges();
        }

        /// <summary>
        /// Busca um item do cardápio pelo id, junto com o grupo e o restaurante, caso exista
        /// </summary>
        /// <param name="id">O id do item</param>
        /// <returns>O item encontrado ou nulo</returns>
        public Itemcardapio? Get(uint id)
        {
            return _context.Itemcardapios
                .Include(i => i.IdGrupoCardapioNavigation)
                .Include(i => i.IdRestauranteNavigation)
                .AsNoTracking()
                .FirstOrDefault(i => i.Id == id);
        }

        /// <summary>
        /// Busca todos os itens do cardápio, na ordem em que os grupos foram cadastrados e depois pelo nome
        /// </summary>
        /// <returns>Uma coleção de itens do cardápio</returns>
        public IEnumerable<Itemcardapio> GetAll()
        {
            return _context.Itemcardapios
                .Include(i => i.IdGrupoCardapioNavigation)
                .Include(i => i.IdRestauranteNavigation)
                .AsNoTracking()
                .OrderBy(i => i.IdGrupoCardapio)
                .ThenBy(i => i.Nome);
        }

        /// <summary>
        /// Regras comuns ao cadastro e à edição de um item do cardápio
        /// </summary>
        /// <param name="itemCardapio">O item a ser validado</param>
        /// <exception cref="ServiceException"></exception>
        private void Validar(Itemcardapio itemCardapio)
        {
            if (!_context.Grupocardapios.Any(g => g.Id == itemCardapio.IdGrupoCardapio))
            {
                throw new ServiceException("O grupo do cardápio informado não existe.");
            }

            if (!_context.Restaurantes.Any(r => r.Id == itemCardapio.IdRestaurante))
            {
                throw new ServiceException("O restaurante informado não existe.");
            }

            if (_context.Itemcardapios.Any(i => i.Nome == itemCardapio.Nome
                                             && i.IdRestaurante == itemCardapio.IdRestaurante
                                             && i.Id != itemCardapio.Id))
            {
                throw new ServiceException("Já existe um item do cardápio com o mesmo nome neste restaurante.");
            }
        }
    }
}
