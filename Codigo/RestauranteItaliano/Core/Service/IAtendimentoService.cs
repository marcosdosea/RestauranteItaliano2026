
using Core.DTO;

namespace Core.Service
{
    public interface IAtendimentoService
    {
        IEnumerable<MesaAtendimentoDTO> GetMapaMesas();
        uint Iniciar(int idMesa);
    }

}
