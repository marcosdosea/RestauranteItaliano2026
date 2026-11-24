using System;
using System.Collections.Generic;
using System.Text;

namespace Core.DTO
{
    public class MesaAtendimentoDTO
    {
        public int IdMesa { get; set; }
        public string Identificacao { get; set; } = string.Empty;
        public bool Ocupada { get; set; }
        public DateTime? InicioAtendimento { get; set; }
    }
}
