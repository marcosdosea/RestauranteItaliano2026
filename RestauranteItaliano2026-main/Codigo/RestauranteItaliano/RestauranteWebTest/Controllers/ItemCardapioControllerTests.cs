using Core;
using Core.Datatables;
using Core.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using RestauranteWeb.Models;

namespace RestauranteWeb.Controllers.Tests;

[TestClass]
public class ItemCardapioControllerTests
{
    private Mock<IItemCardapioService> itemCardapioServiceMock = null!;
    private Mock<IGrupoCardapioService> grupoCardapioServiceMock = null!;
    private Mock<IRestauranteService> restauranteServiceMock = null!;
    private ItemCardapioController controller = null!;

    [TestInitialize]
    public void Inicializar()
    {
        itemCardapioServiceMock = new Mock<IItemCardapioService>();
        grupoCardapioServiceMock = new Mock<IGrupoCardapioService>();
        restauranteServiceMock = new Mock<IRestauranteService>();

        itemCardapioServiceMock
            .Setup(service => service.GetAll())
            .Returns(ObterItensDeTeste());

        itemCardapioServiceMock
            .Setup(service => service.Get(1))
            .Returns(ObterItemAlvo());

        itemCardapioServiceMock
            .Setup(service => service.Get(999))
            .Returns((Itemcardapio?)null);

        grupoCardapioServiceMock
            .Setup(service => service.GetAll())
            .Returns(ObterGruposDeTeste());

        restauranteServiceMock
            .Setup(service => service.GetAll())
            .Returns(ObterRestaurantesDeTeste());

        controller = new ItemCardapioController(
            itemCardapioServiceMock.Object,
            grupoCardapioServiceMock.Object,
            restauranteServiceMock.Object);

        // os avisos de sucesso e de erro das telas são guardados no TempData
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
    public void Index_QuandoChamado_DeveRetornarViewComListaDeGrupos()
    {
        // Act
        var resultado = controller.Index();

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        var grupos = Assert.IsInstanceOfType<SelectList>(viewResult.ViewData["GruposCardapio"]);

        Assert.HasCount(2, grupos.ToList());
        Assert.IsNull(viewResult.ViewData.Model);

        // os itens não são carregados aqui: a tabela busca cada página em GetDataPage
        itemCardapioServiceMock.Verify(
            service => service.GetAll(),
            Times.Never);
    }

    [TestMethod]
    public void GetDataPage_QuandoChamado_DeveRetornarJsonComAPaginaDeItens()
    {
        // Arrange
        var request = new DatatableRequest { Draw = 3, Start = 0, Length = 10 };

        itemCardapioServiceMock
            .Setup(service => service.GetDataPage(request))
            .Returns(new DatatableResponse<Itemcardapio>
            {
                Draw = 3,
                RecordsTotal = 9,
                RecordsFiltered = 3,
                Data = ObterItensDeTeste()
            });

        // Act
        var resultado = controller.GetDataPage(request);

        // Assert
        var jsonResult = Assert.IsInstanceOfType<JsonResult>(resultado);
        var resposta = Assert.IsInstanceOfType<DatatableResponse<ItemCardapioViewModel>>(jsonResult.Value);

        Assert.AreEqual(3, resposta.Draw);
        Assert.AreEqual(9, resposta.RecordsTotal);
        Assert.AreEqual(3, resposta.RecordsFiltered);

        Assert.IsNotNull(resposta.Data);
        var itens = resposta.Data.ToList();

        Assert.HasCount(3, itens);
        Assert.AreEqual("Bruschetta", itens[0].Nome);
        Assert.AreEqual(24.90m, itens[0].Preco);
        Assert.AreEqual("Entradas", itens[0].NomeGrupoCardapio);
        Assert.IsTrue(itens[0].Disponivel);

        // o item 3 está em falta: chega na tela como indisponível
        Assert.IsTrue(itens[2].Ativo);
        Assert.IsFalse(itens[2].Disponivel);

        itemCardapioServiceMock.Verify(
            service => service.GetDataPage(request),
            Times.Once);
    }

    [TestMethod]
    public void GetDataPage_QuandoNaoExistemItens_DeveRetornarJsonComPaginaVazia()
    {
        // Arrange
        var request = new DatatableRequest { Draw = 1, Start = 0, Length = 10 };

        itemCardapioServiceMock
            .Setup(service => service.GetDataPage(request))
            .Returns(new DatatableResponse<Itemcardapio>
            {
                Draw = 1,
                RecordsTotal = 0,
                RecordsFiltered = 0,
                Data = Array.Empty<Itemcardapio>()
            });

        // Act
        var resultado = controller.GetDataPage(request);

        // Assert
        var jsonResult = Assert.IsInstanceOfType<JsonResult>(resultado);
        var resposta = Assert.IsInstanceOfType<DatatableResponse<ItemCardapioViewModel>>(jsonResult.Value);

        Assert.AreEqual(0, resposta.RecordsTotal);
        Assert.IsNotNull(resposta.Data);
        Assert.IsEmpty(resposta.Data.ToList());
    }

    [TestMethod]
    public void Details_QuandoItemExiste_DeveRetornarViewComItem()
    {
        // Act
        var resultado = controller.Details(1);

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        var item = Assert.IsInstanceOfType<ItemCardapioViewModel>(
            viewResult.ViewData.Model);

        Assert.AreEqual(1u, item.Id);
        Assert.AreEqual("Lasagna", item.Nome);
        Assert.AreEqual(49.90m, item.Preco);
        Assert.AreEqual("Massas", item.NomeGrupoCardapio);

        itemCardapioServiceMock.Verify(
            service => service.Get(1),
            Times.Once);
    }

    [TestMethod]
    public void Details_QuandoItemNaoExiste_DeveRetornarNotFound()
    {
        // Act
        var resultado = controller.Details(999);

        // Assert
        Assert.IsInstanceOfType<NotFoundResult>(resultado);

        itemCardapioServiceMock.Verify(
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
        var item = Assert.IsInstanceOfType<ItemCardapioViewModel>(
            viewResult.ViewData.Model);

        Assert.IsNull(item.Id);
        Assert.IsTrue(item.Ativo);
        Assert.IsTrue(item.Disponivel);

        // com um restaurante só cadastrado, ele já vem escolhido
        Assert.AreEqual(1u, item.IdRestaurante);

        Assert.IsNotNull(item.Restaurantes);
        Assert.IsNotNull(item.GruposCardapio);
        Assert.HasCount(1, item.Restaurantes.ToList());
        Assert.HasCount(2, item.GruposCardapio.ToList());
    }

    [TestMethod]
    public void Create_QuandoModeloValido_DeveCriarItemERedirecionarParaIndex()
    {
        // Arrange
        var modelo = CriarModeloDeNovoItem();
        Itemcardapio? itemCriado = null;

        itemCardapioServiceMock
            .Setup(service => service.Create(It.IsAny<Itemcardapio>()))
            .Callback<Itemcardapio>(item => itemCriado = item)
            .Returns(4u);

        // Act
        var resultado = controller.Create(modelo);

        // Assert
        var redirectResult = VerificarRedirecionamentoParaIndex(resultado);

        Assert.AreEqual("item-4", redirectResult.Fragment);

        Assert.IsNotNull(itemCriado);
        Assert.AreEqual(modelo.Nome, itemCriado.Nome);
        Assert.AreEqual(modelo.Preco, itemCriado.Preco);
        Assert.AreEqual(modelo.Detalhes, itemCriado.Detalhes);
        Assert.AreEqual((sbyte)1, itemCriado.Ativo);
        Assert.AreEqual("0", itemCriado.Disponivel);
        Assert.AreEqual(1u, itemCriado.IdRestaurante);
        Assert.AreEqual(2u, itemCriado.IdGrupoCardapio);
        Assert.AreEqual(
            "Tiramisù entrou no cardápio.",
            controller.TempData["Mensagem"]);

        itemCardapioServiceMock.Verify(
            service => service.Create(It.IsAny<Itemcardapio>()),
            Times.Once);
    }

    [TestMethod]
    public void Create_QuandoModeloInvalido_NaoDeveCriarItem()
    {
        // Arrange
        var modelo = CriarModeloDeNovoItem();

        controller.ModelState.AddModelError(
            nameof(ItemCardapioViewModel.Nome),
            "O campo Nome é de preenchimento obrigatório.");

        // Act
        var resultado = controller.Create(modelo);

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        var item = Assert.IsInstanceOfType<ItemCardapioViewModel>(
            viewResult.ViewData.Model);

        // as listas voltam preenchidas para o formulário ser mostrado de novo
        Assert.IsNotNull(item.Restaurantes);
        Assert.IsNotNull(item.GruposCardapio);

        itemCardapioServiceMock.Verify(
            service => service.Create(It.IsAny<Itemcardapio>()),
            Times.Never);
    }

    [TestMethod]
    public void Create_QuandoServicoRecusa_DeveRetornarViewComMensagemDeErro()
    {
        // Arrange
        var modelo = CriarModeloDeNovoItem();

        itemCardapioServiceMock
            .Setup(service => service.Create(It.IsAny<Itemcardapio>()))
            .Throws(new ServiceException("Já existe um item do cardápio com o mesmo nome neste restaurante."));

        // Act
        var resultado = controller.Create(modelo);

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);

        Assert.AreSame(modelo, viewResult.ViewData.Model);
        VerificarMensagemDeErro("Já existe um item do cardápio com o mesmo nome neste restaurante.");
        Assert.IsNull(controller.TempData["Mensagem"]);
    }

    [TestMethod]
    public void Edit_QuandoRequisicaoGetEItemExiste_DeveRetornarViewComItem()
    {
        // Act
        var resultado = controller.Edit(1);

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        var item = Assert.IsInstanceOfType<ItemCardapioViewModel>(
            viewResult.ViewData.Model);

        Assert.AreEqual(1u, item.Id);
        Assert.AreEqual("Lasagna", item.Nome);
        Assert.AreEqual(2u, item.IdGrupoCardapio);
        Assert.IsNotNull(item.Restaurantes);
        Assert.IsNotNull(item.GruposCardapio);

        itemCardapioServiceMock.Verify(
            service => service.Get(1),
            Times.Once);
    }

    [TestMethod]
    public void Edit_QuandoRequisicaoGetEItemNaoExiste_DeveRetornarNotFound()
    {
        // Act
        var resultado = controller.Edit(999);

        // Assert
        Assert.IsInstanceOfType<NotFoundResult>(resultado);

        itemCardapioServiceMock.Verify(
            service => service.Get(999),
            Times.Once);
    }

    [TestMethod]
    public void Edit_QuandoModeloValido_DeveAtualizarItemERedirecionarParaIndex()
    {
        // Arrange
        var modelo = CriarModeloDeItemExistente();
        Itemcardapio? itemEditado = null;

        itemCardapioServiceMock
            .Setup(service => service.Edit(It.IsAny<Itemcardapio>()))
            .Callback<Itemcardapio>(item => itemEditado = item);

        // Act
        var resultado = controller.Edit(2, modelo);

        // Assert
        var redirectResult = VerificarRedirecionamentoParaIndex(resultado);

        Assert.AreEqual("item-2", redirectResult.Fragment);

        Assert.IsNotNull(itemEditado);
        Assert.AreEqual(2u, itemEditado.Id);
        Assert.AreEqual(modelo.Nome, itemEditado.Nome);
        Assert.AreEqual(modelo.Preco, itemEditado.Preco);
        Assert.AreEqual((sbyte)0, itemEditado.Ativo);
        Assert.AreEqual("1", itemEditado.Disponivel);
        Assert.AreEqual(
            "Alterações em Gnocchi salvas.",
            controller.TempData["Mensagem"]);

        itemCardapioServiceMock.Verify(
            service => service.Edit(It.IsAny<Itemcardapio>()),
            Times.Once);
    }

    [TestMethod]
    public void Edit_QuandoModeloInvalido_NaoDeveAtualizarItem()
    {
        // Arrange
        var modelo = CriarModeloDeItemExistente();

        controller.ModelState.AddModelError(
            nameof(ItemCardapioViewModel.Preco),
            "O campo Preço deve ser maior que zero.");

        // Act
        var resultado = controller.Edit(2, modelo);

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        var item = Assert.IsInstanceOfType<ItemCardapioViewModel>(
            viewResult.ViewData.Model);

        Assert.IsNotNull(item.Restaurantes);
        Assert.IsNotNull(item.GruposCardapio);

        itemCardapioServiceMock.Verify(
            service => service.Edit(It.IsAny<Itemcardapio>()),
            Times.Never);
    }

    [TestMethod]
    public void Edit_QuandoServicoRecusa_DeveRetornarViewComMensagemDeErro()
    {
        // Arrange
        var modelo = CriarModeloDeItemExistente();

        itemCardapioServiceMock
            .Setup(service => service.Edit(It.IsAny<Itemcardapio>()))
            .Throws(new ServiceException("O grupo do cardápio informado não existe."));

        // Act
        var resultado = controller.Edit(2, modelo);

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);

        Assert.AreSame(modelo, viewResult.ViewData.Model);
        VerificarMensagemDeErro("O grupo do cardápio informado não existe.");
    }

    [TestMethod]
    public void Delete_QuandoRequisicaoGetEItemExiste_DeveRetornarViewComItem()
    {
        // Act
        var resultado = controller.Delete(1);

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        var item = Assert.IsInstanceOfType<ItemCardapioViewModel>(
            viewResult.ViewData.Model);

        Assert.AreEqual(1u, item.Id);
        Assert.AreEqual("Lasagna", item.Nome);

        itemCardapioServiceMock.Verify(
            service => service.Get(1),
            Times.Once);
    }

    [TestMethod]
    public void Delete_QuandoRequisicaoGetEItemNaoExiste_DeveRetornarNotFound()
    {
        // Act
        var resultado = controller.Delete(999);

        // Assert
        Assert.IsInstanceOfType<NotFoundResult>(resultado);

        itemCardapioServiceMock.Verify(
            service => service.Get(999),
            Times.Once);
    }

    [TestMethod]
    public void Delete_QuandoRequisicaoPost_DeveExcluirItemERedirecionarParaIndex()
    {
        // Arrange
        var modelo = CriarModeloDeItemExistente();

        // Act
        var resultado = controller.Delete(2, modelo);

        // Assert
        VerificarRedirecionamentoParaIndex(resultado);

        Assert.AreEqual(
            "Gnocchi saiu do cardápio.",
            controller.TempData["Mensagem"]);

        itemCardapioServiceMock.Verify(
            service => service.Delete(2),
            Times.Once);
    }

    [TestMethod]
    public void Delete_QuandoServicoRecusa_DeveRetornarViewComMensagemDeErro()
    {
        // Arrange
        itemCardapioServiceMock
            .Setup(service => service.Delete(1))
            .Throws(new ServiceException("Não é possível excluir o item, pois ele já foi usado em pedidos."));

        // Act
        var resultado = controller.Delete(1, new ItemCardapioViewModel { Id = 1, Nome = "Lasagna" });

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        var item = Assert.IsInstanceOfType<ItemCardapioViewModel>(
            viewResult.ViewData.Model);

        // o item é buscado de novo para a tela mostrar grupo, preço e restaurante
        Assert.AreEqual("Massas", item.NomeGrupoCardapio);
        VerificarMensagemDeErro("Não é possível excluir o item, pois ele já foi usado em pedidos.");
        Assert.IsNull(controller.TempData["Mensagem"]);
    }

    [TestMethod]
    public void AlternarDisponivel_QuandoMarcaEmFalta_DeveAlterarERedirecionarParaIndex()
    {
        // Act
        var resultado = controller.AlternarDisponivel(1, false);

        // Assert
        var redirectResult = VerificarRedirecionamentoParaIndex(resultado);

        Assert.AreEqual("item-1", redirectResult.Fragment);
        Assert.AreEqual(
            "Lasagna agora aparece como em falta.",
            controller.TempData["Mensagem"]);
        Assert.IsNull(controller.TempData["Erro"]);

        itemCardapioServiceMock.Verify(
            service => service.AlterarDisponibilidade(1, false),
            Times.Once);
    }

    [TestMethod]
    public void AlternarDisponivel_QuandoMarcaDisponivel_DeveAlterarERedirecionarParaIndex()
    {
        // Act
        var resultado = controller.AlternarDisponivel(1, true);

        // Assert
        VerificarRedirecionamentoParaIndex(resultado);

        Assert.AreEqual(
            "Lasagna está disponível de novo.",
            controller.TempData["Mensagem"]);

        itemCardapioServiceMock.Verify(
            service => service.AlterarDisponibilidade(1, true),
            Times.Once);
    }

    [TestMethod]
    public void AlternarDisponivel_QuandoServicoRecusa_DeveGuardarMensagemDeErro()
    {
        // Arrange
        itemCardapioServiceMock
            .Setup(service => service.AlterarDisponibilidade(999, false))
            .Throws(new ServiceException("Item do cardápio não encontrado."));

        // Act
        var resultado = controller.AlternarDisponivel(999, false);

        // Assert
        VerificarRedirecionamentoParaIndex(resultado);

        // o erro vai para o aviso vermelho, e não para o aviso verde de sucesso
        Assert.AreEqual(
            "Item do cardápio não encontrado.",
            controller.TempData["Erro"]);
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

    private static RedirectToActionResult VerificarRedirecionamentoParaIndex(ActionResult resultado)
    {
        var redirectResult =
            Assert.IsInstanceOfType<RedirectToActionResult>(resultado);

        Assert.IsNull(redirectResult.ControllerName);
        Assert.AreEqual(
            nameof(ItemCardapioController.Index),
            redirectResult.ActionName);

        return redirectResult;
    }

    private static ItemCardapioViewModel CriarModeloDeNovoItem()
    {
        return new ItemCardapioViewModel
        {
            Nome = "Tiramisù",
            Preco = 28.50m,
            Detalhes = "Mascarpone, café e cacau",
            Ativo = true,
            Disponivel = false,
            IdRestaurante = 1,
            IdGrupoCardapio = 2
        };
    }

    private static ItemCardapioViewModel CriarModeloDeItemExistente()
    {
        return new ItemCardapioViewModel
        {
            Id = 2,
            Nome = "Gnocchi",
            Preco = 42.00m,
            Ativo = false,
            Disponivel = true,
            IdRestaurante = 1,
            IdGrupoCardapio = 2
        };
    }

    private static Itemcardapio ObterItemAlvo()
    {
        return new Itemcardapio
        {
            Id = 1,
            Nome = "Lasagna",
            Preco = 49.90m,
            Ativo = 1,
            Disponivel = "1",
            IdRestaurante = 1,
            IdGrupoCardapio = 2,
            IdGrupoCardapioNavigation = new Grupocardapio { Id = 2, Nome = "Massas" },
            IdRestauranteNavigation = ObterRestauranteDeTeste()
        };
    }

    private static IEnumerable<Itemcardapio> ObterItensDeTeste()
    {
        return
        [
            new Itemcardapio
            {
                Id = 1,
                Nome = "Bruschetta",
                Preco = 24.90m,
                Detalhes = "Pão italiano, tomate e manjericão",
                Ativo = 1,
                Disponivel = "1",
                IdRestaurante = 1,
                IdGrupoCardapio = 1,
                IdGrupoCardapioNavigation = new Grupocardapio { Id = 1, Nome = "Entradas" },
                IdRestauranteNavigation = ObterRestauranteDeTeste()
            },
            new Itemcardapio
            {
                Id = 2,
                Nome = "Lasagna",
                Preco = 49.90m,
                Ativo = 1,
                Disponivel = "1",
                IdRestaurante = 1,
                IdGrupoCardapio = 2,
                IdGrupoCardapioNavigation = new Grupocardapio { Id = 2, Nome = "Massas" },
                IdRestauranteNavigation = ObterRestauranteDeTeste()
            },
            new Itemcardapio
            {
                Id = 3,
                Nome = "Gnocchi",
                Preco = 42.00m,
                Ativo = 1,
                Disponivel = "0",
                IdRestaurante = 1,
                IdGrupoCardapio = 2,
                IdGrupoCardapioNavigation = new Grupocardapio { Id = 2, Nome = "Massas" },
                IdRestauranteNavigation = ObterRestauranteDeTeste()
            }
        ];
    }

    private static IEnumerable<Grupocardapio> ObterGruposDeTeste()
    {
        return
        [
            new Grupocardapio
            {
                Id = 1,
                Nome = "Entradas"
            },
            new Grupocardapio
            {
                Id = 2,
                Nome = "Massas"
            }
        ];
    }

    private static Restaurante ObterRestauranteDeTeste()
    {
        return new Restaurante
        {
            Id = 1,
            IdTipoRestaurante = 1,
            Cnpj = "00000000000000",
            Nome = "Restaurante Italiano"
        };
    }

    private static IEnumerable<Restaurante> ObterRestaurantesDeTeste()
    {
        return
        [
            ObterRestauranteDeTeste()
        ];
    }
}
