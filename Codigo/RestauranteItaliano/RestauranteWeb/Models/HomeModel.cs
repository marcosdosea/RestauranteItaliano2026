namespace RestauranteWeb.Models
{
    public class HomeModel
    {
        public string Saudacao { get; set; } = "Buongiorno";
        public string DataLonga { get; set; } = string.Empty;
        public string HoraAgora { get; set; } = string.Empty;

        /// <summary>Falso quando não foi possível consultar o banco de dados.</summary>
        public bool DadosDisponiveis { get; set; }

        public int ItensNoCardapio { get; set; }
        public int ItensDisponiveis { get; set; }
        public int ItensEmFalta { get; set; }
        public List<GrupoResumoModel> Grupos { get; set; } = new();

        public List<CapituloModel> Capitulos { get; set; } = new();

        public int FuncoesProntas => Capitulos.Sum(c => c.Entradas.Count(e => e.Pronta));
        public int FuncoesEmBreve => Capitulos.Sum(c => c.Entradas.Count(e => !e.Pronta));

        public int PercentualDisponivel => ItensNoCardapio > 0
            ? (int)Math.Round(ItensDisponiveis * 100.0 / ItensNoCardapio)
            : 0;
    }

    public class GrupoResumoModel
    {
        public string Nome { get; set; } = string.Empty;
        public int QuantidadeItens { get; set; }
    }

    public class CapituloModel
    {
        public string Romano { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public string Ancora { get; set; } = string.Empty;
        public List<EntradaIndiceModel> Entradas { get; set; } = new();

        public bool TemFuncaoPronta => Entradas.Any(e => e.Pronta);

        /// <summary>Primeira tela pronta do capítulo, usada pelo link do menu.</summary>
        public string? ControllerInicial => Entradas.FirstOrDefault(e => e.Pronta)?.Controller;
    }

    public class EntradaIndiceModel
    {
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string CasoDeUso { get; set; } = string.Empty;

        /// <summary>Controller da funcionalidade; nulo enquanto o caso de uso não foi implementado.</summary>
        public string? Controller { get; set; }

        /// <summary>Quantidade de registros cadastrados (mostrada no lugar do "preço" do cardápio).</summary>
        public int? Quantidade { get; set; }
        public string Singular { get; set; } = "registro";
        public string Plural { get; set; } = "registros";

        public bool Pronta => Controller != null;

        public string Resumo
        {
            get
            {
                if (!Pronta) return "em breve";
                if (Quantidade == null) return "abrir";
                return Quantidade == 1 ? $"1 {Singular}" : $"{Quantidade} {Plural}";
            }
        }
    }
}
