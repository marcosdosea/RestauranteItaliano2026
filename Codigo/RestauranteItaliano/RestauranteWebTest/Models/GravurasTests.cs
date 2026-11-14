using RestauranteWeb.Models;

namespace RestauranteWeb.Models.Tests;

[TestClass]
public class GravurasTests
{
    [TestMethod]
    [DataRow("Antipasti", "antipasti")]
    [DataRow("Entradas", "antipasti")]
    [DataRow("Primi piatti", "primi")]
    [DataRow("Massas", "primi")]
    [DataRow("Secondi", "secondi")]
    [DataRow("Pratos principais", "secondi")]
    [DataRow("Pizze", "pizze")]
    [DataRow("Pizzas doces", "pizze")]
    [DataRow("Dolci", "dolci")]
    [DataRow("Sobremesas", "dolci")]
    [DataRow("Bevande", "bevande")]
    [DataRow("BEBIDAS", "bevande")]
    public void DoGrupo_QuandoONomeTemPalavraChave_DeveRetornarAGravuraDoGrupo(string nomeGrupo, string esperado)
    {
        Assert.AreEqual(esperado, Gravuras.DoGrupo(nomeGrupo));
    }

    [TestMethod]
    [DataRow("Pães e acompanhamentos")]
    [DataRow("")]
    [DataRow(null)]
    public void DoGrupo_QuandoONomeNaoEReconhecido_DeveRetornarAGravuraGenerica(string? nomeGrupo)
    {
        Assert.AreEqual(Gravuras.Generica, Gravuras.DoGrupo(nomeGrupo));
    }

    [TestMethod]
    public void DoGrupo_QuandoONomeTemAcento_DeveIgnorarOAcento()
    {
        Assert.AreEqual("primi", Gravuras.DoGrupo("Risotos e massas da casa"));
        Assert.AreEqual("bevande", Gravuras.DoGrupo("Bebídas"));
    }

    [TestMethod]
    public void PalavrasChaveJson_QuandoLido_DeveTerOsMesmosParesDaListaNaMesmaOrdem()
    {
        var pares = System.Text.Json.JsonSerializer.Deserialize<string[][]>(Gravuras.PalavrasChaveJson)!;

        Assert.AreEqual(Gravuras.PalavrasChave.Count, pares.Length);
        Assert.AreEqual("antipast", pares[0][0]);
        Assert.AreEqual("antipasti", pares[0][1]);
        Assert.AreEqual("bevande", pares[^1][1]);
    }

    [TestMethod]
    public void CaminhoDoGrupo_QuandoChamado_DeveApontarParaOSvgNaPastaDeGravuras()
    {
        Assert.AreEqual("~/images/gravuras/pizze.svg", Gravuras.CaminhoDoGrupo("Pizze"));
    }
}
