using Core;
using Core.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using RestauranteWeb.Models;

namespace RestauranteWeb.Controllers.Tests;

[TestClass]
public class FormaPagamentoControllerTests
{
    private Mock<IFormaPagamentoService> formaPagamentoServiceMock = null!;
    private FormaPagamentoController controller = null!;

    [TestInitialize]
    public void Inicializar()
    {
        formaPagamentoServiceMock = new Mock<IFormaPagamentoService>();

        formaPagamentoServiceMock
            .Setup(service => service.GetAll())
            .Returns(ObterFormasDePagamentoDeTeste());

        formaPagamentoServiceMock
            .Setup(service => service.Get(1))
            .Returns(ObterFormaDePagamentoAlvo());

        formaPagamentoServiceMock
            .Setup(service => service.Get(999))
            .Returns((Formapagamento?)null);

        controller = new FormaPagamentoController(formaPagamentoServiceMock.Object);

        // os avisos de sucesso das telas são guardados no TempData
        controller.TempData = new TempDataDictionary(
            new DefaultHttpContext(),
            Mock.Of<ITempDataProvider>());
    }

    [TestCleanup]
    public void Finalizar()
    {
        controller.Dispose();
    }

    [TestMethod]
    public void Index_QuandoChamado_DeveRetornarViewComTodasAsFormasDePagamento()
    {
        // Act
        var resultado = controller.Index();

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        var formasPagamento = Assert.IsInstanceOfType<IEnumerable<FormaPagamentoModel>>(
            viewResult.ViewData.Model);

        Assert.HasCount(3, formasPagamento);

        var primeiraFormaPagamento = formasPagamento.First();

        Assert.AreEqual(1u, primeiraFormaPagamento.Id);
        Assert.AreEqual("Dinheiro", primeiraFormaPagamento.Nome);

        formaPagamentoServiceMock.Verify(
            service => service.GetAll(),
            Times.Once);
    }

    [TestMethod]
    public void Index_QuandoNaoExistemFormasDePagamento_DeveRetornarViewComColecaoVazia()
    {
        // Arrange
        formaPagamentoServiceMock
            .Setup(service => service.GetAll())
            .Returns(Array.Empty<Formapagamento>());

        // Act
        var resultado = controller.Index();

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        var formasPagamento = Assert.IsInstanceOfType<IEnumerable<FormaPagamentoModel>>(
            viewResult.ViewData.Model);

        Assert.IsEmpty(formasPagamento);

        formaPagamentoServiceMock.Verify(
            service => service.GetAll(),
            Times.Once);
    }

    [TestMethod]
    public void Details_QuandoFormaDePagamentoExiste_DeveRetornarViewComFormaDePagamento()
    {
        // Act
        var resultado = controller.Details(1);

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        var formaPagamento = Assert.IsInstanceOfType<FormaPagamentoModel>(
            viewResult.ViewData.Model);

        Assert.AreEqual(1u, formaPagamento.Id);
        Assert.AreEqual("Pix", formaPagamento.Nome);

        formaPagamentoServiceMock.Verify(
            service => service.Get(1),
            Times.Once);
    }

    [TestMethod]
    public void Details_QuandoFormaDePagamentoNaoExiste_DeveRetornarNotFound()
    {
        // Act
        var resultado = controller.Details(999);

        // Assert
        Assert.IsInstanceOfType<NotFoundResult>(resultado);

        formaPagamentoServiceMock.Verify(
            service => service.Get(999),
            Times.Once);
    }

    [TestMethod]
    public void Create_QuandoRequisicaoGet_DeveRetornarViewComModeloInicial()
    {
        // Act
        var resultado = controller.Create();

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        var formaPagamento = Assert.IsInstanceOfType<FormaPagamentoModel>(
            viewResult.ViewData.Model);

        Assert.IsNull(formaPagamento.Id);
        Assert.IsTrue(string.IsNullOrEmpty(formaPagamento.Nome));
    }

    [TestMethod]
    public void Create_QuandoModeloValido_DeveCriarFormaDePagamentoERedirecionarParaIndex()
    {
        // Arrange
        var modelo = CriarModeloDeNovaFormaDePagamento();
        Formapagamento? formaPagamentoCriada = null;

        formaPagamentoServiceMock
            .Setup(service => service.Create(It.IsAny<Formapagamento>()))
            .Callback<Formapagamento>(formaPagamento => formaPagamentoCriada = formaPagamento)
            .Returns(4u);

        // Act
        var resultado = controller.Create(modelo);

        // Assert
        VerificarRedirecionamentoParaIndex(resultado);

        Assert.IsNotNull(formaPagamentoCriada);
        Assert.AreEqual(4u, formaPagamentoCriada.Id);
        Assert.AreEqual(modelo.Nome, formaPagamentoCriada.Nome);
        Assert.AreEqual(
            "Cartão de débito entrou nas formas de pagamento.",
            controller.TempData["Mensagem"]);

        formaPagamentoServiceMock.Verify(
            service => service.Create(It.IsAny<Formapagamento>()),
            Times.Once);
    }

    [TestMethod]
    public void Create_QuandoModeloInvalido_NaoDeveCriarFormaDePagamento()
    {
        // Arrange
        var modelo = CriarModeloDeNovaFormaDePagamento();

        controller.ModelState.AddModelError(
            nameof(FormaPagamentoModel.Nome),
            "O campo Nome é de preenchimento obrigatório.");

        // Act
        var resultado = controller.Create(modelo);

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);

        Assert.AreSame(modelo, viewResult.ViewData.Model);

        formaPagamentoServiceMock.Verify(
            service => service.Create(It.IsAny<Formapagamento>()),
            Times.Never);
    }

    [TestMethod]
    public void Create_QuandoServicoRecusa_DeveRetornarViewComMensagemDeErro()
    {
        // Arrange
        var modelo = CriarModeloDeNovaFormaDePagamento();

        formaPagamentoServiceMock
            .Setup(service => service.Create(It.IsAny<Formapagamento>()))
            .Throws(new ServiceException("Já existe uma forma de pagamento com o mesmo nome."));

        // Act
        var resultado = controller.Create(modelo);

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);

        Assert.AreSame(modelo, viewResult.ViewData.Model);
        VerificarMensagemDeErro("Já existe uma forma de pagamento com o mesmo nome.");
        Assert.IsNull(controller.TempData["Mensagem"]);
    }

    [TestMethod]
    public void Edit_QuandoRequisicaoGetEFormaDePagamentoExiste_DeveRetornarViewComFormaDePagamento()
    {
        // Act
        var resultado = controller.Edit(1);

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        var formaPagamento = Assert.IsInstanceOfType<FormaPagamentoModel>(
            viewResult.ViewData.Model);

        Assert.AreEqual(1u, formaPagamento.Id);
        Assert.AreEqual("Pix", formaPagamento.Nome);

        formaPagamentoServiceMock.Verify(
            service => service.Get(1),
            Times.Once);
    }

    [TestMethod]
    public void Edit_QuandoRequisicaoGetEFormaDePagamentoNaoExiste_DeveRetornarNotFound()
    {
        // Act
        var resultado = controller.Edit(999);

        // Assert
        Assert.IsInstanceOfType<NotFoundResult>(resultado);

        formaPagamentoServiceMock.Verify(
            service => service.Get(999),
            Times.Once);
    }

    [TestMethod]
    public void Edit_QuandoModeloValido_DeveAtualizarFormaDePagamentoERedirecionarParaIndex()
    {
        // Arrange
        var modelo = CriarModeloDeFormaDePagamentoExistente();
        Formapagamento? formaPagamentoEditada = null;

        formaPagamentoServiceMock
            .Setup(service => service.Edit(It.IsAny<Formapagamento>()))
            .Callback<Formapagamento>(formaPagamento => formaPagamentoEditada = formaPagamento);

        // Act
        var resultado = controller.Edit(2, modelo);

        // Assert
        VerificarRedirecionamentoParaIndex(resultado);

        Assert.IsNotNull(formaPagamentoEditada);
        Assert.AreEqual(2u, formaPagamentoEditada.Id);
        Assert.AreEqual(modelo.Nome, formaPagamentoEditada.Nome);
        Assert.AreEqual(
            "Alterações em Cartão de crédito salvas.",
            controller.TempData["Mensagem"]);

        formaPagamentoServiceMock.Verify(
            service => service.Edit(It.IsAny<Formapagamento>()),
            Times.Once);
    }

    [TestMethod]
    public void Edit_QuandoModeloInvalido_NaoDeveAtualizarFormaDePagamento()
    {
        // Arrange
        var modelo = CriarModeloDeFormaDePagamentoExistente();

        controller.ModelState.AddModelError(
            nameof(FormaPagamentoModel.Nome),
            "O campo Nome é de preenchimento obrigatório.");

        // Act
        var resultado = controller.Edit(2, modelo);

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);

        Assert.AreSame(modelo, viewResult.ViewData.Model);

        formaPagamentoServiceMock.Verify(
            service => service.Edit(It.IsAny<Formapagamento>()),
            Times.Never);
    }

    [TestMethod]
    public void Edit_QuandoServicoRecusa_DeveRetornarViewComMensagemDeErro()
    {
        // Arrange
        var modelo = CriarModeloDeFormaDePagamentoExistente();

        formaPagamentoServiceMock
            .Setup(service => service.Edit(It.IsAny<Formapagamento>()))
            .Throws(new ServiceException("Já existe uma forma de pagamento com o mesmo nome."));

        // Act
        var resultado = controller.Edit(2, modelo);

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);

        Assert.AreSame(modelo, viewResult.ViewData.Model);
        VerificarMensagemDeErro("Já existe uma forma de pagamento com o mesmo nome.");
    }

    [TestMethod]
    public void Delete_QuandoRequisicaoGetEFormaDePagamentoExiste_DeveRetornarViewComFormaDePagamento()
    {
        // Act
        var resultado = controller.Delete(1);

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        var formaPagamento = Assert.IsInstanceOfType<FormaPagamentoModel>(
            viewResult.ViewData.Model);

        Assert.AreEqual(1u, formaPagamento.Id);
        Assert.AreEqual("Pix", formaPagamento.Nome);

        formaPagamentoServiceMock.Verify(
            service => service.Get(1),
            Times.Once);
    }

    [TestMethod]
    public void Delete_QuandoRequisicaoGetEFormaDePagamentoNaoExiste_DeveRetornarNotFound()
    {
        // Act
        var resultado = controller.Delete(999);

        // Assert
        Assert.IsInstanceOfType<NotFoundResult>(resultado);

        formaPagamentoServiceMock.Verify(
            service => service.Get(999),
            Times.Once);
    }

    [TestMethod]
    public void Delete_QuandoRequisicaoPost_DeveExcluirFormaDePagamentoERedirecionarParaIndex()
    {
        // Arrange
        var modelo = CriarModeloDeFormaDePagamentoExistente();

        // Act
        var resultado = controller.Delete(2, modelo);

        // Assert
        VerificarRedirecionamentoParaIndex(resultado);

        Assert.AreEqual(
            "Cartão de crédito saiu das formas de pagamento.",
            controller.TempData["Mensagem"]);

        formaPagamentoServiceMock.Verify(
            service => service.Delete(2),
            Times.Once);
    }

    [TestMethod]
    public void Delete_QuandoServicoRecusa_DeveRetornarViewComMensagemDeErro()
    {
        // Arrange
        var modelo = CriarModeloDeFormaDePagamentoExistente();

        formaPagamentoServiceMock
            .Setup(service => service.Delete(2))
            .Throws(new ServiceException("Não é possível excluir a forma de pagamento, pois existem pagamentos associados a ela."));

        // Act
        var resultado = controller.Delete(2, modelo);

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);

        Assert.AreSame(modelo, viewResult.ViewData.Model);
        VerificarMensagemDeErro("Não é possível excluir a forma de pagamento, pois existem pagamentos associados a ela.");
        Assert.IsNull(controller.TempData["Mensagem"]);
    }

    private void VerificarMensagemDeErro(string mensagemEsperada)
    {
        Assert.IsFalse(controller.ModelState.IsValid);

        var mensagens = controller.ModelState.Values
            .SelectMany(entrada => entrada.Errors)
            .Select(erro => erro.ErrorMessage)
            .ToList();

        CollectionAssert.Contains(mensagens, mensagemEsperada);
    }

    private static void VerificarRedirecionamentoParaIndex(ActionResult resultado)
    {
        var redirectResult =
            Assert.IsInstanceOfType<RedirectToActionResult>(resultado);

        Assert.IsNull(redirectResult.ControllerName);
        Assert.AreEqual(
            nameof(FormaPagamentoController.Index),
            redirectResult.ActionName);
    }

    private static FormaPagamentoModel CriarModeloDeNovaFormaDePagamento()
    {
        return new FormaPagamentoModel
        {
            Id = 4,
            Nome = "Cartão de débito"
        };
    }

    private static FormaPagamentoModel CriarModeloDeFormaDePagamentoExistente()
    {
        return new FormaPagamentoModel
        {
            Id = 2,
            Nome = "Cartão de crédito"
        };
    }

    private static Formapagamento ObterFormaDePagamentoAlvo()
    {
        return new Formapagamento
        {
            Id = 1,
            Nome = "Pix"
        };
    }

    private static IEnumerable<Formapagamento> ObterFormasDePagamentoDeTeste()
    {
        return
        [
            new Formapagamento
            {
                Id = 1,
                Nome = "Dinheiro"
            },
            new Formapagamento
            {
                Id = 2,
                Nome = "Pix"
            },
            new Formapagamento
            {
                Id = 3,
                Nome = "Cartão de crédito"
            }
        ];
    }
}
