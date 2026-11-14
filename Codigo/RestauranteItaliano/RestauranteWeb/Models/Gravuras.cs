using System.Globalization;
using System.Text;
using System.Text.Json;

namespace RestauranteWeb.Models
{
    /// <summary>
    /// Escolhe a gravura (ilustração em linogravura, em wwwroot/images/gravuras) de um grupo do cardápio
    /// pelo nome do grupo. É só visual: os grupos são cadastrados pelo usuário, então o nome é comparado
    /// por palavras-chave, e o que não for reconhecido usa a gravura genérica do prato.
    /// </summary>
    public static class Gravuras
    {
        public const string Pasta = "~/images/gravuras/";
        public const string Generica = "prato";

        /// <summary>Palavra-chave (sem acento, em minúsculas) e o arquivo da gravura, na ordem de prioridade.</summary>
        public static readonly IReadOnlyList<KeyValuePair<string, string>> PalavrasChave = new List<KeyValuePair<string, string>>
        {
            new("antipast", "antipasti"),
            new("entrada", "antipasti"),
            new("petisco", "antipasti"),
            new("pizz", "pizze"),
            new("primi", "primi"),
            new("massa", "primi"),
            new("risot", "primi"),
            new("secondi", "secondi"),
            new("principa", "secondi"),
            new("carne", "secondi"),
            new("peix", "secondi"),
            new("dolci", "dolci"),
            new("doce", "dolci"),
            new("sobremesa", "dolci"),
            new("bevand", "bevande"),
            new("bebida", "bevande"),
            new("vinho", "bevande"),
            new("drink", "bevande"),
        };

        /// <summary>
        /// As mesmas palavras-chave em JSON ([["entrada","antipasti"], ...]), para o atributo data-gravuras:
        /// o JavaScript da tabela e da prévia usa a mesma regra do servidor, escrita num lugar só.
        /// </summary>
        public static readonly string PalavrasChaveJson =
            JsonSerializer.Serialize(PalavrasChave.Select(palavra => new[] { palavra.Key, palavra.Value }));

        /// <summary>Nome do arquivo (sem extensão) da gravura que representa o grupo.</summary>
        public static string DoGrupo(string? nomeGrupo)
        {
            var nome = SemAcento(nomeGrupo ?? string.Empty).ToLowerInvariant();
            foreach (var palavra in PalavrasChave)
            {
                if (nome.Contains(palavra.Key))
                {
                    return palavra.Value;
                }
            }
            return Generica;
        }

        /// <summary>Caminho virtual do SVG da gravura, para usar com Url.Content.</summary>
        public static string CaminhoDoGrupo(string? nomeGrupo) => $"{Pasta}{DoGrupo(nomeGrupo)}.svg";

        private static string SemAcento(string texto)
        {
            var decomposto = texto.Normalize(NormalizationForm.FormD);
            var resultado = new StringBuilder(decomposto.Length);
            foreach (var letra in decomposto)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(letra) != UnicodeCategory.NonSpacingMark)
                {
                    resultado.Append(letra);
                }
            }
            return resultado.ToString();
        }
    }
}
