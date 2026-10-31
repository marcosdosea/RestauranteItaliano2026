namespace Core.Service
{
    public interface IFormaPagamentoService
    {
        uint Create(Formapagamento formaPagamento);
        void Edit(Formapagamento formaPagamento);
        void Delete(uint id);
        Formapagamento Get(uint id);
        IEnumerable<Formapagamento> GetAll();
    }
}
