namespace Core.DTO
{
    /// <summary>
    /// Números mostrados na página inicial: situação do cardápio e
    /// quantidade de registros de cada cadastro
    /// </summary>
    public class ResumoDTO
    {
        public int ItensNoCardapio { get; set; }

        public int ItensDisponiveis { get; set; }

        public int ItensEmFalta { get; set; }

        public IEnumerable<GrupoResumoDTO> Grupos { get; set; } = new List<GrupoResumoDTO>();

        public int QuantidadeGruposCardapio { get; set; }

        public int QuantidadeTiposFuncionario { get; set; }

        public int QuantidadeTiposRestaurante { get; set; }

        public int QuantidadeFormasPagamento { get; set; }
    }
}
