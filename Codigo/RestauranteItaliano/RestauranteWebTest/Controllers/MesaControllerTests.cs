using Core;
using Core.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using RestauranteWeb.Models;

namespace RestauranteWeb.Controllers.Tests;

[TestClass]
public class MesaControllerTests
{
    private Mock<IMesaService> mesaServiceMock = null!;
    private Mock<IRestauranteService> restauranteServiceMock = null!;
    private MesaController controller = null!;

    [TestInitialize]
    public void Inicializar()
    {
        mesaServiceMock = new Mock<IMesaService>();
        mesaServiceMock
            .Setup(service => service.GetAll())
            .Returns(ObterMesasDeTeste());
        mesaServiceMock
            .Setup(service => service.Get(1))
            .Returns(ObterMesaAlvo());
        mesaServiceMock
            .Setup(service => service.Get(999))
            .Returns((Mesa?)null);

        restauranteServiceMock = new Mock<IRestauranteService>();
        restauranteServiceMock
            .Setup(service => service.GetAll())
            .Returns(ObterRestaurantesDeTeste());

        controller = new MesaController(
            mesaServiceMock.Object,
            restauranteServiceMock.Object);

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
    public void Index_QuandoChamado_DeveRetornarViewComTodasAsMesas()
    {
        // Act
        var resultado = controller.Index();

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        var mesas = Assert.IsInstanceOfType<IEnumerable<MesaViewModel>>(
            viewResult.ViewData.Model);
        Assert.HasCount(3, mesas);
        var primeiraMesa = mesas.First();
        Assert.AreEqual(1, primeiraMesa.Id);
        Assert.AreEqual("Mesa 1", primeiraMesa.Identificacao);
        Assert.AreEqual("Trattoria Bella", primeiraMesa.NomeRestaurante);
        mesaServiceMock.Verify(
            service => service.GetAll(),
            Times.Once);
    }

    [TestMethod]
    public void Index_QuandoNaoExistemMesas_DeveRetornarViewComColecaoVazia()
    {
        // Arrange
        mesaServiceMock
            .Setup(service => service.GetAll())
            .Returns(Array.Empty<Mesa>());

        // Act
        var resultado = controller.Index();

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        var mesas = Assert.IsInstanceOfType<IEnumerable<MesaViewModel>>(
            viewResult.ViewData.Model);
        Assert.IsEmpty(mesas);
    }

    [TestMethod]
    public void Details_QuandoMesaExiste_DeveRetornarViewComMesa()
    {
        // Act
        var resultado = controller.Details(1);

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        var mesa = Assert.IsInstanceOfType<MesaViewModel>(
            viewResult.ViewData.Model);
        Assert.AreEqual(1, mesa.Id);
        Assert.AreEqual("Mesa 1", mesa.Identificacao);
        Assert.AreEqual(1u, mesa.IdRestaurante);
        Assert.AreEqual("Trattoria Bella", mesa.NomeRestaurante);
        mesaServiceMock.Verify(
            service => service.Get(1),
            Times.Once);
    }

    [TestMethod]
    public void Details_QuandoMesaNaoExiste_DeveRetornarNotFound()
    {
        // Act
        var resultado = controller.Details(999);

        // Assert
        Assert.IsInstanceOfType<NotFoundResult>(resultado);
    }

    [TestMethod]
    public void Create_QuandoRequisicaoGet_DeveRetornarViewComModeloInicialEListaDeRestaurantes()
    {
        // Act
        var resultado = controller.Create();

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        var mesa = Assert.IsInstanceOfType<MesaViewModel>(
            viewResult.ViewData.Model);
        Assert.IsNull(mesa.Id);
        Assert.IsTrue(string.IsNullOrEmpty(mesa.Identificacao));
        Assert.IsNull(mesa.IdRestaurante);
        Assert.IsNotNull(mesa.Restaurantes);
        Assert.HasCount(2, mesa.Restaurantes.ToList());
    }

    [TestMethod]
    public void Create_QuandoSoExisteUmRestaurante_DeveTrazerORestauranteEscolhido()
    {
        // Arrange
        restauranteServiceMock
            .Setup(service => service.GetAll())
            .Returns(new[] { ObterRestauranteTrattoria() });

        // Act
        var resultado = controller.Create();

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        var mesa = Assert.IsInstanceOfType<MesaViewModel>(
            viewResult.ViewData.Model);
        Assert.AreEqual(1u, mesa.IdRestaurante);
    }

    [TestMethod]
    public void Create_QuandoModeloValido_DeveCriarMesaERedirecionarParaIndex()
    {
        // Arrange
        var modelo = CriarModeloDeNovaMesa();
        Mesa? mesaCriada = null;
        mesaServiceMock
            .Setup(service => service.Create(It.IsAny<Mesa>()))
            .Callback<Mesa>(mesa => mesaCriada = mesa)
            .Returns(4);

        // Act
        var resultado = controller.Create(modelo);

        // Assert
        VerificarRedirecionamentoParaIndex(resultado);
        Assert.IsNotNull(mesaCriada);
        Assert.AreEqual(modelo.Identificacao, mesaCriada.Identificacao);
        Assert.AreEqual(1u, mesaCriada.IdRestaurante);
        Assert.AreEqual(
            "Mesa 4 entrou na lista de mesas.",
            controller.TempData["Mensagem"]);
        mesaServiceMock.Verify(
            service => service.Create(It.IsAny<Mesa>()),
            Times.Once);
    }

    [TestMethod]
    public void Create_QuandoModeloInvalido_NaoDeveCriarMesaERecarregarListaDeRestaurantes()
    {
        // Arrange
        var modelo = CriarModeloDeNovaMesa();
        controller.ModelState.AddModelError(
            nameof(MesaViewModel.Identificacao),
            "O campo Identificação é obrigatório.");

        // Act
        var resultado = controller.Create(modelo);

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        Assert.AreSame(modelo, viewResult.ViewData.Model);
        Assert.IsNotNull(modelo.Restaurantes);
        mesaServiceMock.Verify(
            service => service.Create(It.IsAny<Mesa>()),
            Times.Never);
    }

    [TestMethod]
    public void Create_QuandoServicoRecusa_DeveRetornarViewComMensagemDeErro()
    {
        // Arrange
        var modelo = CriarModeloDeNovaMesa();
        mesaServiceMock
            .Setup(service => service.Create(It.IsAny<Mesa>()))
            .Throws(new ServiceException("Já existe uma mesa com essa identificação neste restaurante."));

        // Act
        var resultado = controller.Create(modelo);

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        Assert.AreSame(modelo, viewResult.ViewData.Model);
        Assert.IsNotNull(modelo.Restaurantes);
        VerificarMensagemDeErro("Já existe uma mesa com essa identificação neste restaurante.");
        Assert.IsNull(controller.TempData["Mensagem"]);
    }

    [TestMethod]
    public void Edit_QuandoRequisicaoGetEMesaExiste_DeveRetornarViewComMesaELista()
    {
        // Act
        var resultado = controller.Edit(1);

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        var mesa = Assert.IsInstanceOfType<MesaViewModel>(
            viewResult.ViewData.Model);
        Assert.AreEqual(1, mesa.Id);
        Assert.AreEqual("Mesa 1", mesa.Identificacao);
        Assert.IsNotNull(mesa.Restaurantes);
        Assert.HasCount(2, mesa.Restaurantes.ToList());
    }

    [TestMethod]
    public void Edit_QuandoRequisicaoGetEMesaNaoExiste_DeveRetornarNotFound()
    {
        // Act
        var resultado = controller.Edit(999);

        // Assert
        Assert.IsInstanceOfType<NotFoundResult>(resultado);
    }

    [TestMethod]
    public void Edit_QuandoModeloValido_DeveEditarMesaERedirecionarParaIndex()
    {
        // Arrange
        var modelo = CriarModeloDeMesaExistente();
        modelo.Id = null;
        Mesa? mesaEditada = null;
        mesaServiceMock
            .Setup(service => service.Edit(It.IsAny<Mesa>()))
            .Callback<Mesa>(mesa => mesaEditada = mesa);

        // Act
        var resultado = controller.Edit(1, modelo);

        // Assert
        VerificarRedirecionamentoParaIndex(resultado);
        Assert.IsNotNull(mesaEditada);
        Assert.AreEqual(1, mesaEditada.Id);
        Assert.AreEqual("Mesa 10", mesaEditada.Identificacao);
        Assert.AreEqual(
            "Alterações em Mesa 10 salvas.",
            controller.TempData["Mensagem"]);
        mesaServiceMock.Verify(
            service => service.Edit(It.IsAny<Mesa>()),
            Times.Once);
    }

    [TestMethod]
    public void Edit_QuandoModeloInvalido_NaoDeveEditarMesa()
    {
        // Arrange
        var modelo = CriarModeloDeMesaExistente();
        controller.ModelState.AddModelError(
            nameof(MesaViewModel.IdRestaurante),
            "Selecione o Restaurante.");

        // Act
        var resultado = controller.Edit(1, modelo);

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        Assert.AreSame(modelo, viewResult.ViewData.Model);
        Assert.IsNotNull(modelo.Restaurantes);
        mesaServiceMock.Verify(
            service => service.Edit(It.IsAny<Mesa>()),
            Times.Never);
    }

    [TestMethod]
    public void Edit_QuandoServicoRecusa_DeveRetornarViewComMensagemDeErro()
    {
        // Arrange
        var modelo = CriarModeloDeMesaExistente();
        mesaServiceMock
            .Setup(service => service.Edit(It.IsAny<Mesa>()))
            .Throws(new ServiceException("Não é possível trocar o restaurante de uma mesa que já possui atendimentos."));

        // Act
        var resultado = controller.Edit(1, modelo);

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        Assert.AreSame(modelo, viewResult.ViewData.Model);
        VerificarMensagemDeErro("Não é possível trocar o restaurante de uma mesa que já possui atendimentos.");
        Assert.IsNull(controller.TempData["Mensagem"]);
    }

    [TestMethod]
    public void Delete_QuandoRequisicaoGetEMesaExiste_DeveRetornarViewComMesa()
    {
        // Act
        var resultado = controller.Delete(1);

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        var mesa = Assert.IsInstanceOfType<MesaViewModel>(
            viewResult.ViewData.Model);
        Assert.AreEqual(1, mesa.Id);
        Assert.AreEqual("Mesa 1", mesa.Identificacao);
    }

    [TestMethod]
    public void Delete_QuandoRequisicaoGetEMesaNaoExiste_DeveRetornarNotFound()
    {
        // Act
        var resultado = controller.Delete(999);

        // Assert
        Assert.IsInstanceOfType<NotFoundResult>(resultado);
    }

    [TestMethod]
    public void Delete_QuandoRequisicaoPost_DeveExcluirMesaERedirecionarParaIndex()
    {
        // Arrange
        var modelo = new MesaViewModel { Id = 1, Identificacao = "Mesa 1" };

        // Act
        var resultado = controller.Delete(1, modelo);

        // Assert
        VerificarRedirecionamentoParaIndex(resultado);
        Assert.AreEqual(
            "Mesa 1 saiu da lista de mesas.",
            controller.TempData["Mensagem"]);
        mesaServiceMock.Verify(
            service => service.Delete(1),
            Times.Once);
    }

    [TestMethod]
    public void Delete_QuandoServicoRecusa_DeveRetornarViewComMensagemDeErroEDadosDaMesa()
    {
        // Arrange
        var modelo = new MesaViewModel { Id = 1, Identificacao = "Mesa 1" };
        mesaServiceMock
            .Setup(service => service.Delete(1))
            .Throws(new ServiceException("Não é possível excluir a mesa, pois ela possui atendimentos registrados."));

        // Act
        var resultado = controller.Delete(1, modelo);

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        var mesa = Assert.IsInstanceOfType<MesaViewModel>(
            viewResult.ViewData.Model);
        // a mesa é recarregada, então a tela volta com o nome do restaurante
        Assert.AreEqual("Trattoria Bella", mesa.NomeRestaurante);
        VerificarMensagemDeErro("Não é possível excluir a mesa, pois ela possui atendimentos registrados.");
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

    private static void VerificarRedirecionamentoParaIndex(IActionResult resultado)
    {
        var redirectResult =
            Assert.IsInstanceOfType<RedirectToActionResult>(resultado);
        Assert.IsNull(redirectResult.ControllerName);
        Assert.AreEqual(
            nameof(MesaController.Index),
            redirectResult.ActionName);
    }

    private static MesaViewModel CriarModeloDeNovaMesa()
    {
        return new MesaViewModel
        {
            Identificacao = "Mesa 4",
            IdRestaurante = 1
        };
    }

    private static MesaViewModel CriarModeloDeMesaExistente()
    {
        return new MesaViewModel
        {
            Id = 1,
            Identificacao = "Mesa 10",
            IdRestaurante = 1
        };
    }

    private static Restaurante ObterRestauranteTrattoria()
    {
        return new Restaurante
        {
            Id = 1,
            IdTipoRestaurante = 1,
            Cnpj = "11222333000181",
            Nome = "Trattoria Bella"
        };
    }

    private static Restaurante ObterRestauranteAntica()
    {
        return new Restaurante
        {
            Id = 2,
            IdTipoRestaurante = 1,
            Cnpj = "22333444000162",
            Nome = "Antica Cucina"
        };
    }

    private static Mesa ObterMesaAlvo()
    {
        return new Mesa
        {
            Id = 1,
            Identificacao = "Mesa 1",
            IdRestaurante = 1,
            IdRestauranteNavigation = ObterRestauranteTrattoria()
        };
    }

    private static IEnumerable<Mesa> ObterMesasDeTeste()
    {
        return
        [
            ObterMesaAlvo(),
            new Mesa
            {
                Id = 2,
                Identificacao = "Mesa 2",
                IdRestaurante = 1,
                IdRestauranteNavigation = ObterRestauranteTrattoria()
            },
            new Mesa
            {
                Id = 3,
                Identificacao = "Mesa 1",
                IdRestaurante = 2,
                IdRestauranteNavigation = ObterRestauranteAntica()
            }
        ];
    }

    private static IEnumerable<Restaurante> ObterRestaurantesDeTeste()
    {
        return
        [
            ObterRestauranteTrattoria(),
            ObterRestauranteAntica()
        ];
    }
}
