using RestauranteWeb.Models;

namespace RestauranteWeb.Models.Tests;

[TestClass]
public class MonogramaTests
{
    [TestMethod]
    [DataRow("Garçom", "G")]
    [DataRow("gerente", "G")]
    [DataRow("Auxiliar de cozinha", "AC")]
    [DataRow("Chefe de cozinha e confeitaria", "CC")]
    [DataRow("  maître   d'hôtel ", "MD")]
    public void DoNome_QuandoONomeTemPalavras_DeveRetornarAsIniciais(string nome, string esperado)
    {
        Assert.AreEqual(esperado, Monograma.DoNome(nome));
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("   ")]
    [DataRow("de da")]
    [DataRow(null)]
    public void DoNome_QuandoNaoHaPalavras_DeveRetornarInterrogacao(string? nome)
    {
        Assert.AreEqual("?", Monograma.DoNome(nome));
    }
}
