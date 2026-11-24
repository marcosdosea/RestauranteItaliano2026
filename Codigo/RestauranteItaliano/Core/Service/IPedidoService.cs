using Core;
using Core.DTO;

namespace Core.Service
{
    public interface IPedidoService
    {
        int Create(Pedido pedido);
        void Update(Pedido pedido);
        void Delete(uint id);
        Pedido? Get(uint id);
        IEnumerable<Pedido> GetAll();
        IEnumerable<Pedido> GetByAtendimento(uint idAtendimento);
        IEnumerable<Pedido> GetByStatus(string status);
        bool AtendimentoPodeReceberPedidos(uint idAtendimento);
        uint RegistrarPedido(uint idAtendimento, uint idGarcom, IReadOnlyCollection<PedidoItemSolicitadoDTO> itens);
    }
}