using Core;
using Core.DTO;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service
{
    public class PedidoService : IPedidoService
    {
        private readonly RestauranteContext context;

        public PedidoService(RestauranteContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Registra um novo pedido para um atendimento específico, feito por um garçom, com os itens solicitados.
        /// </summary>
        /// <param name="idAtendimento"></param>
        /// <param name="idGarcom"></param>
        /// <param name="itens"></param>
        /// <returns></returns>
        /// <exception cref="ServiceException"></exception>
        public uint RegistrarPedido(
            uint idAtendimento,
            uint idGarcom,
            IReadOnlyCollection<PedidoItemSolicitadoDTO> itens)
        {
            if (itens == null || itens.Count == 0)
            {
                throw new ServiceException(
                    "Adicione pelo menos um item ao pedido.");
            }

            var atendimento = context.Atendimentos
                .FirstOrDefault(a => a.Id == idAtendimento);

            if (atendimento == null)
            {
                throw new ServiceException("Atendimento não encontrado.");
            }

            if (atendimento.Status != "I")
            {
                throw new ServiceException("O atendimento não está ativo.");
            }

            var idRestaurante = context.Mesas
                .Where(m => m.Id == atendimento.IdMesa)
                .Select(m => m.IdRestaurante)
                .FirstOrDefault();

            var garcomValido = context.Funcionarios
                .Any(f =>
                    f.Id == idGarcom &&
                    f.IdRestaurante == idRestaurante);

            if (!garcomValido)
            {
                throw new ServiceException(
                    "Garçom não encontrado neste restaurante.");
            }

            var itensRecebidos = new HashSet<uint>();

            foreach (var itemSolicitado in itens)
            {
                if (itemSolicitado.Quantidade <= 0)
                {
                    throw new ServiceException(
                        "A quantidade de cada item deve ser maior que zero.");
                }

                if (!itensRecebidos.Add(itemSolicitado.IdItemCardapio))
                {
                    throw new ServiceException(
                        "O mesmo item foi adicionado mais de uma vez.");
                }

                var itemValido = context.Itemcardapios.Any(i =>
                    i.Id == itemSolicitado.IdItemCardapio &&
                    i.IdRestaurante == idRestaurante &&
                    i.Ativo == 1 &&
                    i.Disponivel == "1");

                if (!itemValido)
                {
                    throw new ServiceException(
                        "Um dos itens não existe, está indisponível ou não pertence ao restaurante.");
                }
            }

            var agora = DateTime.Now;

            var pedido = new Pedido
            {
                IdAtendimento = idAtendimento,
                IdGarcom = idGarcom,
                DataHoraSolicitacao = agora,
                DaaHoraAtendimento = agora,
                Status = "S"
            };

            foreach (var itemSolicitado in itens)
            {
                pedido.Pedidoitemcardapios.Add(new Pedidoitemcardapio
                {
                    IdItemCardapio = itemSolicitado.IdItemCardapio,
                    Quantidade = itemSolicitado.Quantidade,
                    IdPedidoNavigation = pedido
                });
            }

            context.Pedidos.Add(pedido);
            context.SaveChanges();

            return pedido.Id;
        }

        /// <summary>
        /// Atualiza o status de um atendimento para verificar se ele pode receber novos pedidos.
        /// </summary>
        /// <param name="idAtendimento"></param>
        /// <returns></returns>
        public bool AtendimentoPodeReceberPedidos(uint idAtendimento)
        {
            return context.Atendimentos.Any(a =>
                a.Id == idAtendimento &&
                a.Status == "I");
        }

        /// <summary>
        /// Cria um novo pedido no sistema.
        /// </summary>
        /// <param name="pedido"></param>
        /// <returns></returns>
        /// <exception cref="ServiceException"></exception>
        public int Create(Pedido pedido)
        {
            if (pedido == null)
            {
                throw new ServiceException("O pedido não foi informado.");
            }

            context.Pedidos.Add(pedido);
            context.SaveChanges();

            return (int)pedido.Id;
        }

        /// <summary>
        /// Deleta um pedido existente do sistema com base no ID fornecido.
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="ServiceException"></exception>
        public void Delete(uint id)
        {
            var pedido = context.Pedidos
                .FirstOrDefault(p => p.Id == id);

            if (pedido == null)
            {
                throw new ServiceException("Pedido não encontrado.");
            }

            context.Pedidos.Remove(pedido);
            context.SaveChanges();
        }

        /// <summary>
        /// Busca um pedido específico pelo seu ID, incluindo informações do atendimento relacionado.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Pedido? Get(uint id)
        {
            return context.Pedidos
                .Include(p => p.IdAtendimentoNavigation)
                .AsNoTracking()
                .FirstOrDefault(p => p.Id == id);
        }

        /// <summary>
        /// Busca todos os pedidos existentes no sistema, ordenados pela data e hora de solicitação.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Pedido> GetAll()
        {
            return context.Pedidos
                .AsNoTracking()
                .OrderBy(p => p.DataHoraSolicitacao);
        }

        /// <summary>
        /// Busca todos os pedidos relacionados a um atendimento específico, ordenados pela data e hora de solicitação.
        /// </summary>
        /// <param name="idAtendimento"></param>
        /// <returns></returns>
        public IEnumerable<Pedido> GetByAtendimento(uint idAtendimento)
        {
            return context.Pedidos
                .AsNoTracking()
                .Where(p => p.IdAtendimento == idAtendimento)
                .OrderBy(p => p.DataHoraSolicitacao);
        }

        /// <summary>
        /// Busca todos os pedidos com um status específico, ordenados pela data e hora de solicitação.
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public IEnumerable<Pedido> GetByStatus(string status)
        {
            return context.Pedidos
                .AsNoTracking()
                .Where(p => p.Status == status)
                .OrderBy(p => p.DataHoraSolicitacao);
        }

        /// <summary>
        /// Atualiza as informações de um pedido existente no sistema.
        /// </summary>
        /// <param name="pedido"></param>
        /// <exception cref="ServiceException"></exception>
        public void Update(Pedido pedido)
        {
            if (pedido == null)
            {
                throw new ServiceException("O pedido não foi informado.");
            }

            context.Pedidos.Update(pedido);
            context.SaveChanges();
        }
    }
}