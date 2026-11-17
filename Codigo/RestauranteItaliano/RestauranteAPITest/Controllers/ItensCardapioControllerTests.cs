using Core;
using Core.Service;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RestauranteAPI.Models;

namespace RestauranteAPI.Controllers.Tests;

[TestClass]
public class ItensCardapioControllerTests
{
    private Mock<IItemCardapioService> itemCardapioServiceMock = null!;
    private ItensCardapioController controller = null!;

    [TestInitialize]
    public void Inicializar()
    {
        itemCardapioServiceMock = new Mock<IItemCardapioService>();

        itemCardapioServiceMock
            .Setup(service => service.GetAll())
            .Returns(ObterItensDeTeste());

        itemCardapioServiceMock
            .Setup(service => service.Get(1))
            .Returns(ObterItensDeTeste().First());

        itemCardapioServiceMock
            .Setup(service => service.Get(999))
            .Returns((Itemcardapio?)null);

        controller = new ItensCardapioController(itemCardapioServiceMock.Object);
    }

    [TestMethod]
    public void Get_QuandoChamado_DeveRetornarOkComOsItensConvertidos()
    {
        // Act
        var resultado = controller.Get();

        // Assert
        var okResult = Assert.IsInstanceOfType<OkObjectResult>(resultado);
        var itens = Assert.IsInstanceOfType<IEnumerable<ItemCardapioViewModel>>(okResult.Value).ToList();

        Assert.HasCount(2, itens);

        // ativo (TINYINT) e disponível ('1'/'0') do banco viram booleanos, e o grupo vem pelo nome
        Assert.IsTrue(itens[0].Ativo);
        Assert.IsTrue(itens[0].Disponivel);
        Assert.AreEqual("Massas", itens[0].NomeGrupoCardapio);
        Assert.AreEqual("Trattoria Centro", itens[0].NomeRestaurante);
        Assert.IsFalse(itens[1].Disponivel);
    }

    [TestMethod]
    public void Get_QuandoIdExiste_DeveRetornarOkComOItem()
    {
        // Act
        var resultado = controller.Get(1);

        // Assert
        var okResult = Assert.IsInstanceOfType<OkObjectResult>(resultado);
        var item = Assert.IsInstanceOfType<ItemCardapioViewModel>(okResult.Value);

        Assert.AreEqual(1u, item.Id);
        Assert.AreEqual("Spaghetti alla carbonara", item.Nome);
        Assert.AreEqual(62m, item.Preco);
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
    public void Post_QuandoDadosValidos_DeveCriarComOsValoresDoBancoERetornarCreated()
    {
        // Arrange
        Itemcardapio? itemCriado = null;
        itemCardapioServiceMock
            .Setup(service => service.Create(It.IsAny<Itemcardapio>()))
            .Callback<Itemcardapio>(item => itemCriado = item)
            .Returns(10);

        var novoItem = new ItemCardapioViewModel
        {
            Nome = "Risotto ai funghi",
            Preco = 64.90m,
            Ativo = true,
            Disponivel = false,
            IdRestaurante = 1,
            IdGrupoCardapio = 2,
        };

        // Act
        var resultado = controller.Post(novoItem);

        // Assert
        var createdResult = Assert.IsInstanceOfType<CreatedAtActionResult>(resultado);
        var item = Assert.IsInstanceOfType<ItemCardapioViewModel>(createdResult.Value);

        Assert.AreEqual(10u, item.Id);
        Assert.IsNotNull(itemCriado);
        Assert.AreEqual((sbyte)1, itemCriado.Ativo);
        Assert.AreEqual("0", itemCriado.Disponivel);
        Assert.AreEqual(2u, itemCriado.IdGrupoCardapio);
    }

    [TestMethod]
    public void Post_QuandoDadosInvalidos_DeveRetornarBadRequestSemCriar()
    {
        // Arrange
        controller.ModelState.AddModelError("Nome", "O campo Nome é de preenchimento obrigatório.");

        // Act
        var resultado = controller.Post(new ItemCardapioViewModel());

        // Assert
        Assert.IsInstanceOfType<BadRequestObjectResult>(resultado);
        itemCardapioServiceMock.Verify(service => service.Create(It.IsAny<Itemcardapio>()), Times.Never);
    }

    [TestMethod]
    public void Put_QuandoIdExiste_DeveEditarComOIdDaRota()
    {
        // Arrange
        Itemcardapio? itemEditado = null;
        itemCardapioServiceMock
            .Setup(service => service.Edit(It.IsAny<Itemcardapio>()))
            .Callback<Itemcardapio>(item => itemEditado = item);

        var alteracao = new ItemCardapioViewModel
        {
            Id = 50,
            Nome = "Spaghetti alla carbonara",
            Preco = 65m,
            IdRestaurante = 1,
            IdGrupoCardapio = 1,
        };

        // Act
        var resultado = controller.Put(1, alteracao);

        // Assert
        Assert.IsInstanceOfType<OkResult>(resultado);
        Assert.IsNotNull(itemEditado);
        Assert.AreEqual(1u, itemEditado.Id);
        Assert.AreEqual(65m, itemEditado.Preco);
    }

    [TestMethod]
    public void Put_QuandoIdNaoExiste_DeveRetornarNotFoundSemEditar()
    {
        // Act
        var resultado = controller.Put(999, new ItemCardapioViewModel { Nome = "Bruschetta", IdRestaurante = 1, IdGrupoCardapio = 1 });

        // Assert
        Assert.IsInstanceOfType<NotFoundObjectResult>(resultado);
        itemCardapioServiceMock.Verify(service => service.Edit(It.IsAny<Itemcardapio>()), Times.Never);
    }

    [TestMethod]
    public void PutDisponibilidade_QuandoIdExiste_DeveAlterarERetornarOk()
    {
        // Act
        var resultado = controller.PutDisponibilidade(1, false);

        // Assert
        Assert.IsInstanceOfType<OkResult>(resultado);
        itemCardapioServiceMock.Verify(service => service.AlterarDisponibilidade(1, false), Times.Once);
    }

    [TestMethod]
    public void PutDisponibilidade_QuandoIdNaoExiste_DeveRetornarNotFoundSemAlterar()
    {
        // Act
        var resultado = controller.PutDisponibilidade(999, true);

        // Assert
        Assert.IsInstanceOfType<NotFoundObjectResult>(resultado);
        itemCardapioServiceMock.Verify(service => service.AlterarDisponibilidade(It.IsAny<uint>(), It.IsAny<bool>()), Times.Never);
    }

    [TestMethod]
    public void Delete_QuandoIdExiste_DeveExcluirERetornarOk()
    {
        // Act
        var resultado = controller.Delete(1);

        // Assert
        Assert.IsInstanceOfType<OkResult>(resultado);
        itemCardapioServiceMock.Verify(service => service.Delete(1), Times.Once);
    }

    [TestMethod]
    public void Delete_QuandoIdNaoExiste_DeveRetornarNotFoundSemExcluir()
    {
        // Act
        var resultado = controller.Delete(999);

        // Assert
        Assert.IsInstanceOfType<NotFoundObjectResult>(resultado);
        itemCardapioServiceMock.Verify(service => service.Delete(It.IsAny<uint>()), Times.Never);
    }

    private static List<Itemcardapio> ObterItensDeTeste()
    {
        var massas = new Grupocardapio { Id = 1, Nome = "Massas" };
        var restaurante = new Restaurante { Id = 1, Nome = "Trattoria Centro" };

        return new List<Itemcardapio>
        {
            new()
            {
                Id = 1, Nome = "Spaghetti alla carbonara", Preco = 62m, Ativo = 1, Disponivel = "1",
                IdGrupoCardapio = 1, IdGrupoCardapioNavigation = massas,
                IdRestaurante = 1, IdRestauranteNavigation = restaurante,
            },
            new()
            {
                Id = 2, Nome = "Gnocchi al pesto", Preco = 54m, Ativo = 1, Disponivel = "0",
                IdGrupoCardapio = 1, IdGrupoCardapioNavigation = massas,
                IdRestaurante = 1, IdRestauranteNavigation = restaurante,
            },
        };
    }
}
