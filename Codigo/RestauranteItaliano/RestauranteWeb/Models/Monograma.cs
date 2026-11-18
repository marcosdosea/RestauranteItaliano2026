namespace RestauranteWeb.Models
{
    /// <summary>
    /// Monta o monograma (as iniciais) que aparece no crachá do tipo de funcionário.
    /// É só visual: nome de uma palavra vira uma letra, e nome composto vira a primeira e a última
    /// inicial, sem as palavras de ligação (de, da, do, das, dos, e).
    /// </summary>
    public static class Monograma
    {
        private static readonly HashSet<string> Ligacoes = new(StringComparer.OrdinalIgnoreCase) { "de", "da", "do", "das", "dos", "e" };

        /// <summary>Iniciais do nome em maiúsculas; "?" quando o nome está vazio.</summary>
        public static string DoNome(string? nome)
        {
            var palavras = (nome ?? string.Empty)
                .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Where(palavra => !Ligacoes.Contains(palavra))
                .ToList();

            if (palavras.Count == 0)
            {
                return "?";
            }

            var iniciais = palavras.Count == 1
                ? palavras[0][..1]
                : $"{palavras[0][0]}{palavras[^1][0]}";
            return iniciais.ToUpperInvariant();
        }
    }
}
