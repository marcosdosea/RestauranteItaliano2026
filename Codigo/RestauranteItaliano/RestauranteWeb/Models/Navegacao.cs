namespace RestauranteWeb.Models
{
    /// <summary>
    /// Mapa do sistema: os casos de uso agrupados em capítulos. É usado pelo menu
    /// do cabeçalho e pelo índice da página inicial. Quando um caso de uso for
    /// implementado, basta preencher o Controller da entrada.
    /// </summary>
    public static class Navegacao
    {
        private static readonly string[] Romanos = { "i.", "ii.", "iii.", "iv.", "v.", "vi.", "vii.", "viii.", "ix.", "x." };

        /// <summary>
        /// Monta os capítulos com as funções prontas primeiro, já numerados.
        /// Cada chamada devolve uma lista nova, que pode ser alterada à vontade.
        /// </summary>
        public static List<CapituloModel> Capitulos()
        {
            var capitulos = MontarCapitulos()
                .OrderByDescending(c => c.Entradas.Any(e => e.Pronta))
                .ToList();

            for (var i = 0; i < capitulos.Count; i++)
            {
                capitulos[i].Romano = i < Romanos.Length ? Romanos[i] : $"{i + 1}.";
                capitulos[i].Entradas = capitulos[i].Entradas
                    .OrderByDescending(e => e.Pronta)
                    .ToList();
            }

            return capitulos;
        }

        /// <summary>
        /// Capítulo ao qual pertence o controller informado, ou nulo se não pertencer a nenhum
        /// </summary>
        public static CapituloModel? CapituloDo(List<CapituloModel> capitulos, string? controller)
        {
            if (string.IsNullOrEmpty(controller)) return null;
            return capitulos.FirstOrDefault(c => c.Entradas.Any(e =>
                string.Equals(e.Controller, controller, StringComparison.OrdinalIgnoreCase)));
        }

        private static List<CapituloModel> MontarCapitulos()
        {
            return new List<CapituloModel>
            {
                new CapituloModel { Nome = "Salão", Ancora = "salao", Entradas = new List<EntradaIndiceModel>
                {
                    new EntradaIndiceModel { Titulo = "Mapa do salão", Descricao = "iniciar, acompanhar e finalizar atendimentos", CasoDeUso = "CS09 · CS12" },
                    new EntradaIndiceModel { Titulo = "Mesas", Descricao = "as mesas que o restaurante gerencia", CasoDeUso = "CS08" }
                }},
                new CapituloModel { Nome = "Pedidos", Ancora = "pedidos", Entradas = new List<EntradaIndiceModel>
                {
                    new EntradaIndiceModel { Titulo = "Pedidos", Descricao = "o que cada mesa pediu, e quando", CasoDeUso = "CS10" }
                }},
                new CapituloModel { Nome = "Pagamentos", Ancora = "pagamentos", Entradas = new List<EntradaIndiceModel>
                {
                    new EntradaIndiceModel { Titulo = "Pagamentos", Descricao = "parciais ou totais, por atendimento", CasoDeUso = "CS11" }
                }},
                new CapituloModel { Nome = "Cardápio", Ancora = "cardapio", Entradas = new List<EntradaIndiceModel>
                {
                    new EntradaIndiceModel { Titulo = "Itens do cardápio", Descricao = "pratos e bebidas oferecidos pela casa", CasoDeUso = "CS07", Controller = "ItemCardapio", Singular = "item", Plural = "itens" },
                    new EntradaIndiceModel { Titulo = "Grupos do cardápio", Descricao = "bebidas, entradas, petiscos, sobremesas…", CasoDeUso = "CS04", Controller = "GrupoCardapio", Singular = "grupo", Plural = "grupos" }
                }},
                new CapituloModel { Nome = "Equipe", Ancora = "equipe", Entradas = new List<EntradaIndiceModel>
                {
                    new EntradaIndiceModel { Titulo = "Funcionários", Descricao = "quem trabalha em cada restaurante", CasoDeUso = "CS06" },
                    new EntradaIndiceModel { Titulo = "Tipos de funcionário", Descricao = "garçons, cozinheiros, gerentes", CasoDeUso = "CS03", Controller = "TipoFuncionario", Singular = "tipo", Plural = "tipos" }
                }},
                new CapituloModel { Nome = "Configurações", Ancora = "configuracoes", Entradas = new List<EntradaIndiceModel>
                {
                    new EntradaIndiceModel { Titulo = "Restaurantes", Descricao = "as casas que usam a plataforma", CasoDeUso = "CS05" },
                    new EntradaIndiceModel { Titulo = "Tipos de restaurante", Descricao = "pizzarias, churrascarias, comida brasileira", CasoDeUso = "CS02", Controller = "TipoRestaurante", Singular = "tipo", Plural = "tipos" },
                    new EntradaIndiceModel { Titulo = "Formas de pagamento", Descricao = "como a conta pode ser paga", CasoDeUso = "CS01", Controller = "FormaPagamento", Singular = "forma", Plural = "formas" }
                }}
            };
        }
    }
}
