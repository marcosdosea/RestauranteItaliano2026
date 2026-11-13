using Core.DTO;
using Core.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using RestauranteWeb.Models;

namespace RestauranteWeb.Controllers.Tests;

[TestClass]
public class HomeControllerTests
{
    private Mock<IResumoService> resumoServiceMock = null!;
    private HomeController controller = null!;

    [TestInitialize]
    public void Inicializar()
    {
        resumoServiceMock = new Mock<IResumoService>();

        resumoServiceMock
            .Setup(service => service.GetResumo())
            .Returns(ObterResumoDeTeste());

        controller = new HomeController(
            resumoServiceMock.Object,
            Mock.Of<ILogger<HomeController>>());
    }

    [TestCleanup]
    public void Finalizar()
    {
        controller.Dispose();
    }

    [TestMethod]
    public void Index_QuandoChamado_DeveRetornarViewComOResumoDoCardapio()
    {
        // Act
        var resultado = controller.Index();

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        var home = Assert.IsInstanceOfType<HomeViewModel>(
            viewResult.ViewData.Model);

        // a data e a saudação vão também para o cabeçalho, no _Layout
        Assert.AreEqual(home.DataLonga, viewResult.ViewData["DataLonga"]);
        Assert.AreEqual(home.Saudacao, viewResult.ViewData["Saudacao"]);

        Assert.IsTrue(home.DadosDisponiveis);
        Assert.AreEqual(9, home.ItensNoCardapio);
        Assert.AreEqual(7, home.ItensDisponiveis);
        Assert.AreEqual(2, home.ItensEmFalta);
        Assert.AreEqual(78, home.PercentualDisponivel);

        Assert.HasCount(2, home.Grupos);
        Assert.AreEqual("Massas", home.Grupos[0].Nome);
        Assert.AreEqual(3, home.Grupos[0].QuantidadeItens);

        resumoServiceMock.Verify(
            service => service.GetResumo(),
            Times.Once);
    }

    [TestMethod]
    public void Index_QuandoChamado_DevePreencherAQuantidadeDeCadaCadastroNoIndice()
    {
        // Act
        var resultado = controller.Index();

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        var home = Assert.IsInstanceOfType<HomeViewModel>(
            viewResult.ViewData.Model);

        var entradas = home.Capitulos
            .SelectMany(capitulo => capitulo.Entradas)
            .ToList();

        Assert.AreEqual(9, entradas.Single(entrada => entrada.Controller == "ItemCardapio").Quantidade);
        Assert.AreEqual(5, entradas.Single(entrada => entrada.Controller == "GrupoCardapio").Quantidade);
        Assert.AreEqual(3, entradas.Single(entrada => entrada.Controller == "TipoFuncionario").Quantidade);
        Assert.AreEqual(1, entradas.Single(entrada => entrada.Controller == "TipoRestaurante").Quantidade);
        Assert.AreEqual(4, entradas.Single(entrada => entrada.Controller == "FormaPagamento").Quantidade);

        // funções que ainda não foram implementadas ficam sem quantidade
        Assert.IsTrue(entradas
            .Where(entrada => !entrada.Pronta)
            .All(entrada => entrada.Quantidade == null));
    }

    [TestMethod]
    public void Index_QuandoBancoEstaForaDoAr_DeveRetornarViewSemDados()
    {
        // Arrange
        resumoServiceMock
            .Setup(service => service.GetResumo())
            .Throws(new InvalidOperationException("Não foi possível conectar ao banco de dados."));

        // Act
        var resultado = controller.Index();

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        var home = Assert.IsInstanceOfType<HomeViewModel>(
            viewResult.ViewData.Model);

        Assert.IsFalse(home.DadosDisponiveis);
        Assert.AreEqual(0, home.ItensNoCardapio);
        Assert.IsEmpty(home.Grupos);
        Assert.IsNotEmpty(home.Capitulos);
    }

    private static ResumoDTO ObterResumoDeTeste()
    {
        return new ResumoDTO
        {
            ItensNoCardapio = 9,
            ItensDisponiveis = 7,
            ItensEmFalta = 2,
            Grupos =
            [
                new GrupoResumoDTO
                {
                    Nome = "Massas",
                    QuantidadeItens = 3
                },
                new GrupoResumoDTO
                {
                    Nome = "Entradas",
                    QuantidadeItens = 2
                }
            ],
            QuantidadeGruposCardapio = 5,
            QuantidadeTiposFuncionario = 3,
            QuantidadeTiposRestaurante = 1,
            QuantidadeFormasPagamento = 4
        };
    }
}
