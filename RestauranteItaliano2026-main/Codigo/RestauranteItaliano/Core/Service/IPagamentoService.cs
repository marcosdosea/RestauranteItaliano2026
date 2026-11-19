using Core;
using System.Collections.Generic;

namespace Core.Service
{
    public interface IPagamentoService
    {
        uint Create(Pagamento pagamento);
        IEnumerable<Pagamento> GetByAtendimento(uint idAtendimento);
        decimal GetTotalPago(uint idAtendimento);
    }
}
