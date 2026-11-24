
using Core.DTO;

namespace Core.Service
{
    public interface IAtendimentoService
    {
        IEnumerable<MesaAtendimentoDTO> GetMapaMesas(uint idRestaurante);
        uint Iniciar(int idMesa, uint idRestaurante);
    }

}
