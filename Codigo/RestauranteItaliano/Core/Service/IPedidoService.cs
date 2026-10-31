using Core;

namespace Core.Service
{
    public interface IPedidoService
    {
        int Create(Pedido pedido);
        void Update(Pedido pedido);
        void Delete(int id);
        Pedido Get(int id);
        IEnumerable<Pedido> GetAll();
        IEnumerable<Pedido> GetByAtendimento(int idAtendimento);
        IEnumerable<Pedido> GetByStatus(string status);
        bool AtendimentoPodeReceberPedidos(int idAtendimento);
    }
}