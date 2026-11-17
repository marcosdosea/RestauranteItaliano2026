using Core;
using Core.Service;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RestauranteAPI.Models;

namespace RestauranteAPI.Controllers.Tests;

[TestClass]
public class FormasPagamentoControllerTests
{
    private Mock<IFormaPagamentoService> formaPagamentoServiceMock = null!;
    private FormasPagamentoController controller = null!;

    [TestInitialize]
    public void Inicializar()
    {
        formaPagamentoServiceMock = new Mock<IFormaPagamentoService>();

        formaPagamentoServiceMock
            .Setup(service => service.GetAll())
            .Returns(ObterFormasDePagamentoDeTeste());

        formaPagamentoServiceMock
            .Setup(service => service.Get(1))
            .Returns(new Formapagamento { Id = 1, Nome = "Dinheiro" });

        formaPagamentoServiceMock
            .Setup(service => service.Get(999))
            .Returns((Formapagamento?)null);

        controller = new FormasPagamentoController(formaPagamentoServiceMock.Object);
    }

    [TestMethod]
    public void Get_QuandoChamado_DeveRetornarOkComTodasAsFormasDePagamento()
    {
        // Act
        var resultado = controller.Get();

        // Assert
        var okResult = Assert.IsInstanceOfType<OkObjectResult>(resultado);
        var formasPagamento = Assert.IsInstanceOfType<IEnumerable<FormaPagamentoViewModel>>(okResult.Value).ToList();

        Assert.HasCount(3, formasPagamento);
        Assert.AreEqual(1u, formasPagamento[0].Id);
        Assert.AreEqual("Dinheiro", formasPagamento[0].Nome);
    }

    [TestMethod]
    public void Get_QuandoIdExiste_DeveRetornarOkComAFormaDePagamento()
    {
        // Act
        var resultado = controller.Get(1);

        // Assert
        var okResult = Assert.IsInstanceOfType<OkObjectResult>(resultado);
        var formaPagamento = Assert.IsInstanceOfType<FormaPagamentoViewModel>(okResult.Value);

        Assert.AreEqual(1u, formaPagamento.Id);
        Assert.AreEqual("Dinheiro", formaPagamento.Nome);
    }

    [TestMethod]
    public void Get_QuandoIdNaoExiste_DeveRetornarNotFound()
    {
        // Act
        var resultado = controller.Get(999);

        // Assert
        Assert.IsInstanceOfType<NotFoundObjectResult>(resultado);
    }

    [TestMethod]
    public void Post_QuandoDadosValidos_DeveCriarERetornarCreatedComOId()
    {
        // Arrange
        Formapagamento? formaCriada = null;
        formaPagamentoServiceMock
            .Setup(service => service.Create(It.IsAny<Formapagamento>()))
            .Callback<Formapagamento>(forma => formaCriada = forma)
            .Returns(7);

        // Act
        var resultado = controller.Post(new FormaPagamentoViewModel { Nome = "Vale-refeição" });

        // Assert
        var createdResult = Assert.IsInstanceOfType<CreatedAtActionResult>(resultado);
        var formaPagamento = Assert.IsInstanceOfType<FormaPagamentoViewModel>(createdResult.Value);

        Assert.AreEqual(nameof(FormasPagamentoController.Get), createdResult.ActionName);
        Assert.AreEqual(7u, formaPagamento.Id);
        Assert.IsNotNull(formaCriada);
        Assert.AreEqual("Vale-refeição", formaCriada.Nome);
    }

    [TestMethod]
    public void Post_QuandoDadosInvalidos_DeveRetornarBadRequestSemCriar()
    {
        // Arrange
        controller.ModelState.AddModelError("Nome", "O campo Nome é de preenchimento obrigatório.");

        // Act
        var resultado = controller.Post(new FormaPagamentoViewModel());

        // Assert
        Assert.IsInstanceOfType<BadRequestObjectResult>(resultado);
        formaPagamentoServiceMock.Verify(service => service.Create(It.IsAny<Formapagamento>()), Times.Never);
    }

    [TestMethod]
    public void Put_QuandoIdExiste_DeveEditarAMesmaInstanciaBuscadaPeloService()
    {
        // Arrange
        // o Entity Framework já acompanha a instância que o Get devolve: o Edit tem de receber essa mesma
        var formaBuscada = new Formapagamento { Id = 1, Nome = "Dinheiro" };
        formaPagamentoServiceMock.Setup(service => service.Get(1)).Returns(formaBuscada);

        Formapagamento? formaEditada = null;
        formaPagamentoServiceMock
            .Setup(service => service.Edit(It.IsAny<Formapagamento>()))
            .Callback<Formapagamento>(forma => formaEditada = forma);

        // Act
        var resultado = controller.Put(1, new FormaPagamentoViewModel { Id = 50, Nome = "Dinheiro em espécie" });

        // Assert
        Assert.IsInstanceOfType<OkResult>(resultado);
        Assert.AreSame(formaBuscada, formaEditada);
        Assert.AreEqual(1u, formaEditada!.Id);
        Assert.AreEqual("Dinheiro em espécie", formaEditada.Nome);
    }

    [TestMethod]
    public void Put_QuandoIdNaoExiste_DeveRetornarNotFoundSemEditar()
    {
        // Act
        var resultado = controller.Put(999, new FormaPagamentoViewModel { Nome = "Cheque" });

        // Assert
        Assert.IsInstanceOfType<NotFoundObjectResult>(resultado);
        formaPagamentoServiceMock.Verify(service => service.Edit(It.IsAny<Formapagamento>()), Times.Never);
    }

    [TestMethod]
    public void Delete_QuandoIdExiste_DeveExcluirERetornarOk()
    {
        // Act
        var resultado = controller.Delete(1);

        // Assert
        Assert.IsInstanceOfType<OkResult>(resultado);
        formaPagamentoServiceMock.Verify(service => service.Delete(1), Times.Once);
    }

    [TestMethod]
    public void Delete_QuandoIdNaoExiste_DeveRetornarNotFoundSemExcluir()
    {
        // Act
        var resultado = controller.Delete(999);

        // Assert
        Assert.IsInstanceOfType<NotFoundObjectResult>(resultado);
        formaPagamentoServiceMock.Verify(service => service.Delete(It.IsAny<uint>()), Times.Never);
    }

    private static IEnumerable<Formapagamento> ObterFormasDePagamentoDeTeste()
    {
        return new List<Formapagamento>
        {
            new() { Id = 1, Nome = "Dinheiro" },
            new() { Id = 2, Nome = "Pix" },
            new() { Id = 3, Nome = "Cartão de crédito" },
        };
    }
}
