using Core;
using Core.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using RestauranteWeb.Models;

namespace RestauranteWeb.Controllers.Tests;

[TestClass]
public class RestauranteControllerTests
{
    private Mock<IRestauranteService> restauranteServiceMock = null!;
    private Mock<ITipoRestauranteService> tipoRestauranteServiceMock = null!;
    private RestauranteController controller = null!;

    [TestInitialize]
    public void Inicializar()
    {
        restauranteServiceMock = new Mock<IRestauranteService>();
        restauranteServiceMock
            .Setup(service => service.GetAll())
            .Returns(ObterRestaurantesDeTeste());
        restauranteServiceMock
            .Setup(service => service.Get(1))
            .Returns(ObterRestauranteAlvo());
        restauranteServiceMock
            .Setup(service => service.Get(999))
            .Returns((Restaurante?)null);

        tipoRestauranteServiceMock = new Mock<ITipoRestauranteService>();
        tipoRestauranteServiceMock
            .Setup(service => service.GetAll())
            .Returns(ObterTiposDeRestauranteDeTeste());

        controller = new RestauranteController(
            restauranteServiceMock.Object,
            tipoRestauranteServiceMock.Object);

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
    public void Index_QuandoChamado_DeveRetornarViewComTodosOsRestaurantes()
    {
        // Act
        var resultado = controller.Index();

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        var restaurantes = Assert.IsInstanceOfType<IEnumerable<RestauranteViewModel>>(
            viewResult.ViewData.Model);
        Assert.HasCount(3, restaurantes);
        var primeiroRestaurante = restaurantes.First();
        Assert.AreEqual(1u, primeiroRestaurante.Id);
        Assert.AreEqual("Trattoria Bella", primeiroRestaurante.Nome);
        Assert.AreEqual("Cantina italiana", primeiroRestaurante.NomeTipoRestaurante);
        restauranteServiceMock.Verify(
            service => service.GetAll(),
            Times.Once);
    }

    [TestMethod]
    public void Index_QuandoNaoExistemRestaurantes_DeveRetornarViewComColecaoVazia()
    {
        // Arrange
        restauranteServiceMock
            .Setup(service => service.GetAll())
            .Returns(Array.Empty<Restaurante>());

        // Act
        var resultado = controller.Index();

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        var restaurantes = Assert.IsInstanceOfType<IEnumerable<RestauranteViewModel>>(
            viewResult.ViewData.Model);
        Assert.IsEmpty(restaurantes);
    }

    [TestMethod]
    public void Details_QuandoRestauranteExiste_DeveRetornarViewComRestaurante()
    {
        // Act
        var resultado = controller.Details(1);

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        var restaurante = Assert.IsInstanceOfType<RestauranteViewModel>(
            viewResult.ViewData.Model);
        Assert.AreEqual(1u, restaurante.Id);
        Assert.AreEqual("Trattoria Bella", restaurante.Nome);
        Assert.AreEqual("11222333000181", restaurante.Cnpj);
        Assert.AreEqual("Cantina italiana", restaurante.NomeTipoRestaurante);
        restauranteServiceMock.Verify(
            service => service.Get(1),
            Times.Once);
    }

    [TestMethod]
    public void Details_QuandoRestauranteNaoExiste_DeveRetornarNotFound()
    {
        // Act
        var resultado = controller.Details(999);

        // Assert
        Assert.IsInstanceOfType<NotFoundResult>(resultado);
        restauranteServiceMock.Verify(
            service => service.Get(999),
            Times.Once);
    }

    [TestMethod]
    public void Create_QuandoRequisicaoGet_DeveRetornarViewComModeloInicialEListaDeTipos()
    {
        // Act
        var resultado = controller.Create();

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        var restaurante = Assert.IsInstanceOfType<RestauranteViewModel>(
            viewResult.ViewData.Model);
        Assert.IsNull(restaurante.Id);
        Assert.IsTrue(string.IsNullOrEmpty(restaurante.Nome));
        Assert.IsNotNull(restaurante.TiposRestaurante);
        Assert.HasCount(2, restaurante.TiposRestaurante.ToList());
    }

    [TestMethod]
    public void Create_QuandoModeloValido_DeveCriarRestauranteERedirecionarParaIndex()
    {
        // Arrange
        var modelo = CriarModeloDeNovoRestaurante();
        Restaurante? restauranteCriado = null;
        restauranteServiceMock
            .Setup(service => service.Create(It.IsAny<Restaurante>()))
            .Callback<Restaurante>(restaurante => restauranteCriado = restaurante)
            .Returns(4u);

        // Act
        var resultado = controller.Create(modelo);

        // Assert
        VerificarRedirecionamentoParaIndex(resultado);
        Assert.IsNotNull(restauranteCriado);
        Assert.AreEqual(4u, restauranteCriado.Id);
        Assert.AreEqual(modelo.Nome, restauranteCriado.Nome);
        Assert.AreEqual(modelo.Cnpj, restauranteCriado.Cnpj);
        Assert.AreEqual(1u, restauranteCriado.IdTipoRestaurante);
        Assert.AreEqual(
            "Cantina Nova entrou na lista de restaurantes.",
            controller.TempData["Mensagem"]);
        restauranteServiceMock.Verify(
            service => service.Create(It.IsAny<Restaurante>()),
            Times.Once);
    }

    [TestMethod]
    public void Create_QuandoModeloInvalido_NaoDeveCriarRestauranteERecarregarListaDeTipos()
    {
        // Arrange
        var modelo = CriarModeloDeNovoRestaurante();
        controller.ModelState.AddModelError(
            nameof(RestauranteViewModel.Nome),
            "O campo Nome é de preenchimento obrigatório.");

        // Act
        var resultado = controller.Create(modelo);

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        Assert.AreSame(modelo, viewResult.ViewData.Model);
        Assert.IsNotNull(modelo.TiposRestaurante);
        restauranteServiceMock.Verify(
            service => service.Create(It.IsAny<Restaurante>()),
            Times.Never);
    }

    [TestMethod]
    public void Create_QuandoServicoRecusa_DeveRetornarViewComMensagemDeErro()
    {
        // Arrange
        var modelo = CriarModeloDeNovoRestaurante();
        restauranteServiceMock
            .Setup(service => service.Create(It.IsAny<Restaurante>()))
            .Throws(new ServiceException("Já existe um restaurante cadastrado com esse CNPJ."));

        // Act
        var resultado = controller.Create(modelo);

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        Assert.AreSame(modelo, viewResult.ViewData.Model);
        Assert.IsNotNull(modelo.TiposRestaurante);
        VerificarMensagemDeErro("Já existe um restaurante cadastrado com esse CNPJ.");
        Assert.IsNull(controller.TempData["Mensagem"]);
    }

    [TestMethod]
    public void Edit_QuandoRequisicaoGetERestauranteExiste_DeveRetornarViewComRestauranteELista()
    {
        // Act
        var resultado = controller.Edit(1);

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        var restaurante = Assert.IsInstanceOfType<RestauranteViewModel>(
            viewResult.ViewData.Model);
        Assert.AreEqual(1u, restaurante.Id);
        Assert.AreEqual("Trattoria Bella", restaurante.Nome);
        Assert.IsNotNull(restaurante.TiposRestaurante);
        Assert.HasCount(2, restaurante.TiposRestaurante.ToList());
    }

    [TestMethod]
    public void Edit_QuandoRequisicaoGetERestauranteNaoExiste_DeveRetornarNotFound()
    {
        // Act
        var resultado = controller.Edit(999);

        // Assert
        Assert.IsInstanceOfType<NotFoundResult>(resultado);
    }

    [TestMethod]
    public void Edit_QuandoModeloValido_DeveEditarRestauranteERedirecionarParaIndex()
    {
        // Arrange
        var modelo = CriarModeloDeRestauranteExistente();
        modelo.Id = null;
        Restaurante? restauranteEditado = null;
        restauranteServiceMock
            .Setup(service => service.Edit(It.IsAny<Restaurante>()))
            .Callback<Restaurante>(restaurante => restauranteEditado = restaurante);

        // Act
        var resultado = controller.Edit(1, modelo);

        // Assert
        VerificarRedirecionamentoParaIndex(resultado);
        Assert.IsNotNull(restauranteEditado);
        Assert.AreEqual(1u, restauranteEditado.Id);
        Assert.AreEqual(modelo.Nome, restauranteEditado.Nome);
        Assert.AreEqual(
            "Alterações em Trattoria Bella Napoli salvas.",
            controller.TempData["Mensagem"]);
        restauranteServiceMock.Verify(
            service => service.Edit(It.IsAny<Restaurante>()),
            Times.Once);
    }

    [TestMethod]
    public void Edit_QuandoModeloInvalido_NaoDeveEditarRestaurante()
    {
        // Arrange
        var modelo = CriarModeloDeRestauranteExistente();
        controller.ModelState.AddModelError(
            nameof(RestauranteViewModel.Cnpj),
            "CNPJ inválido.");

        // Act
        var resultado = controller.Edit(1, modelo);

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        Assert.AreSame(modelo, viewResult.ViewData.Model);
        Assert.IsNotNull(modelo.TiposRestaurante);
        restauranteServiceMock.Verify(
            service => service.Edit(It.IsAny<Restaurante>()),
            Times.Never);
    }

    [TestMethod]
    public void Edit_QuandoServicoRecusa_DeveRetornarViewComMensagemDeErro()
    {
        // Arrange
        var modelo = CriarModeloDeRestauranteExistente();
        restauranteServiceMock
            .Setup(service => service.Edit(It.IsAny<Restaurante>()))
            .Throws(new ServiceException("Já existe um restaurante cadastrado com esse CNPJ."));

        // Act
        var resultado = controller.Edit(1, modelo);

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        Assert.AreSame(modelo, viewResult.ViewData.Model);
        VerificarMensagemDeErro("Já existe um restaurante cadastrado com esse CNPJ.");
        Assert.IsNull(controller.TempData["Mensagem"]);
    }

    [TestMethod]
    public void Delete_QuandoRequisicaoGetERestauranteExiste_DeveRetornarViewComRestaurante()
    {
        // Act
        var resultado = controller.Delete(1);

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        var restaurante = Assert.IsInstanceOfType<RestauranteViewModel>(
            viewResult.ViewData.Model);
        Assert.AreEqual(1u, restaurante.Id);
        Assert.AreEqual("Trattoria Bella", restaurante.Nome);
    }

    [TestMethod]
    public void Delete_QuandoRequisicaoGetERestauranteNaoExiste_DeveRetornarNotFound()
    {
        // Act
        var resultado = controller.Delete(999);

        // Assert
        Assert.IsInstanceOfType<NotFoundResult>(resultado);
    }

    [TestMethod]
    public void Delete_QuandoRequisicaoPost_DeveExcluirRestauranteERedirecionarParaIndex()
    {
        // Arrange
        var modelo = CriarModeloDeRestauranteExistente();

        // Act
        var resultado = controller.Delete(1, modelo);

        // Assert
        VerificarRedirecionamentoParaIndex(resultado);
        Assert.AreEqual(
            "Trattoria Bella Napoli saiu da lista de restaurantes.",
            controller.TempData["Mensagem"]);
        restauranteServiceMock.Verify(
            service => service.Delete(1),
            Times.Once);
    }

    [TestMethod]
    public void Delete_QuandoServicoRecusa_DeveRetornarViewComMensagemDeErroEDadosDoRestaurante()
    {
        // Arrange
        var modelo = new RestauranteViewModel { Id = 1, Nome = "Trattoria Bella" };
        restauranteServiceMock
            .Setup(service => service.Delete(1))
            .Throws(new ServiceException("Não é possível excluir o restaurante, pois ele possui funcionários, itens de cardápio ou mesas cadastrados."));

        // Act
        var resultado = controller.Delete(1, modelo);

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        var restaurante = Assert.IsInstanceOfType<RestauranteViewModel>(
            viewResult.ViewData.Model);
        // o restaurante é recarregado, então a tela volta com o tipo e os demais dados
        Assert.AreEqual("Cantina italiana", restaurante.NomeTipoRestaurante);
        Assert.AreEqual("11222333000181", restaurante.Cnpj);
        VerificarMensagemDeErro("Não é possível excluir o restaurante, pois ele possui funcionários, itens de cardápio ou mesas cadastrados.");
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
            nameof(RestauranteController.Index),
            redirectResult.ActionName);
    }

    private static RestauranteViewModel CriarModeloDeNovoRestaurante()
    {
        return new RestauranteViewModel
        {
            Id = 4,
            Nome = "Cantina Nova",
            Cnpj = "44555666000133",
            IdTipoRestaurante = 1
        };
    }

    private static RestauranteViewModel CriarModeloDeRestauranteExistente()
    {
        return new RestauranteViewModel
        {
            Id = 1,
            Nome = "Trattoria Bella Napoli",
            Cnpj = "11222333000181",
            IdTipoRestaurante = 1
        };
    }

    private static Tiporestaurante ObterTipoCantina()
    {
        return new Tiporestaurante
        {
            Id = 1,
            Nome = "Cantina italiana"
        };
    }

    private static Restaurante ObterRestauranteAlvo()
    {
        return new Restaurante
        {
            Id = 1,
            IdTipoRestaurante = 1,
            Cnpj = "11222333000181",
            Nome = "Trattoria Bella",
            IdTipoRestauranteNavigation = ObterTipoCantina()
        };
    }

    private static IEnumerable<Restaurante> ObterRestaurantesDeTeste()
    {
        return
        [
            ObterRestauranteAlvo(),
            new Restaurante
            {
                Id = 2,
                IdTipoRestaurante = 2,
                Cnpj = "22333444000162",
                Nome = "Boi na Brasa",
                IdTipoRestauranteNavigation = new Tiporestaurante { Id = 2, Nome = "Churrascaria" }
            },
            new Restaurante
            {
                Id = 3,
                IdTipoRestaurante = 1,
                Cnpj = "33444555000143",
                Nome = "Antica Cucina",
                IdTipoRestauranteNavigation = ObterTipoCantina()
            }
        ];
    }

    private static IEnumerable<Tiporestaurante> ObterTiposDeRestauranteDeTeste()
    {
        return
        [
            ObterTipoCantina(),
            new Tiporestaurante
            {
                Id = 2,
                Nome = "Churrascaria"
            }
        ];
    }
}
