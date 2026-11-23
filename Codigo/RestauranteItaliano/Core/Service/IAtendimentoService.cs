using System;
using System.Collections.Generic;
using System.Text;
using Core.DTO;

namespace Core.Service
{
    internal interface IAtendimentoService
    {
        IEnumerable<MesaAtendimentoDTO> GetMapaMesas();
        uint Iniciar(int idMesa);
    }
}
