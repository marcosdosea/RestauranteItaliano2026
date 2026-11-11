using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service.Tests;

[TestClass]
public class ItemCardapioServiceTests
{
    private RestauranteContext contexto = null!;
    private IItemCardapioService servicoItemCardapio = null!;

    [TestInitialize]
    public void Inicializar()
    {
        var nomeBanco = $"Restaurante_{Guid.NewGuid()}";

        var opcoes = new DbContextOptionsBuilder<RestauranteContext>()
            .UseInMemoryDatabase(nomeBanco)
            .Options;

        contexto = new RestauranteContext(opcoes);

        contexto.Tiporestaurantes.Add(
            new Tiporestaurante
            {
                Id = 1,
                Nome = "Cantina italiana"
            });

        contexto.Restaurantes.Add(
            new Restaurante
            {
                Id = 1,
                IdTipoRestaurante = 1,
                Cnpj = "00000000000000",
                Nome = "Restaurante Italiano"
            });

        contexto.Grupocardapios.AddRange(
            new Grupocardapio
            {
                Id = 1,
                Nome = "Entradas"
            },
            new Grupocardapio
            {
                Id = 2,
                Nome = "Massas"
            });

        contexto.Itemcardapios.AddRange(
            new Itemcardapio
            {
                Id = 1,
                Nome = "Bruschetta",
                Preco = 24.90m,
                Detalhes = "Pão italiano, tomate e manjericão",
                Ativo = 1,
                Disponivel = "1",
                IdRestaurante = 1,
                IdGrupoCardapio = 1
            },
            new Itemcardapio
            {
                Id = 2,
                Nome = "Lasagna",
                Preco = 49.90m,
                Ativo = 1,
                Disponivel = "1",
                IdRestaurante = 1,
                IdGrupoCardapio = 2
            },
            new Itemcardapio
            {
                Id = 3,
                Nome = "Gnocchi",
                Preco = 42.00m,
                Ativo = 1,
                Disponivel = "0",
                IdRestaurante = 1,
                IdGrupoCardapio = 2
            });

        // o item 1 já foi usado em um pedido
        contexto.Pedidoitemcardapios.Add(
            new Pedidoitemcardapio
            {
                IdItemCardapio = 1,
                IdPedido = 1,
                Quantidade = 2
            });

        contexto.SaveChanges();
        contexto.ChangeTracker.Clear();

        servicoItemCardapio = new ItemCardapioService(contexto);
    }

    [TestCleanup]
    public void Finalizar()
    {
        contexto.Dispose();
    }

    [TestMethod]
    public void Create_QuandoDadosSaoValidos_DeveRetornarIdDoItemCriado()
    {
        // Arrange
        var itemCardapio = CriarNovoItem();

        // Act
        var idRetornado = servicoItemCardapio.Create(itemCardapio);

        // Assert
        Assert.AreEqual(4u, idRetornado);
    }

    [TestMethod]
    public void Create_QuandoDadosSaoValidos_DeveArmazenarItem()
    {
        // Arrange
        var itemEsperado = CriarNovoItem();

        // Act
        servicoItemCardapio.Create(itemEsperado);

        // Assert
        var itemCriado = servicoItemCardapio.Get(4);

        Assert.IsNotNull(itemCriado);
        Assert.AreEqual(itemEsperado.Id, itemCriado.Id);
        Assert.AreEqual(itemEsperado.Nome, itemCriado.Nome);
        Assert.AreEqual(itemEsperado.Preco, itemCriado.Preco);
        Assert.AreEqual(itemEsperado.Detalhes, itemCriado.Detalhes);
        Assert.AreEqual(itemEsperado.Ativo, itemCriado.Ativo);
        Assert.AreEqual(itemEsperado.Disponivel, itemCriado.Disponivel);
        Assert.AreEqual(itemEsperado.IdRestaurante, itemCriado.IdRestaurante);
        Assert.AreEqual(itemEsperado.IdGrupoCardapio, itemCriado.IdGrupoCardapio);
    }

    [TestMethod]
    public void Create_QuandoGrupoNaoExiste_DeveLancarServiceException()
    {
        // Arrange
        var itemCardapio = CriarNovoItem();
        itemCardapio.IdGrupoCardapio = 999;

        // Act
        var excecao = Assert.ThrowsExactly<ServiceException>(
            () => { servicoItemCardapio.Create(itemCardapio); });

        // Assert
        Assert.AreEqual("O grupo do cardápio informado não existe.", excecao.Message);
        Assert.HasCount(3, servicoItemCardapio.GetAll().ToList());
    }

    [TestMethod]
    public void Create_QuandoRestauranteNaoExiste_DeveLancarServiceException()
    {
        // Arrange
        var itemCardapio = CriarNovoItem();
        itemCardapio.IdRestaurante = 999;

        // Act
        var excecao = Assert.ThrowsExactly<ServiceException>(
            () => { servicoItemCardapio.Create(itemCardapio); });

        // Assert
        Assert.AreEqual("O restaurante informado não existe.", excecao.Message);
        Assert.HasCount(3, servicoItemCardapio.GetAll().ToList());
    }

    [TestMethod]
    public void Create_QuandoNomeJaExisteNoRestaurante_DeveLancarServiceException()
    {
        // Arrange
        var itemCardapio = CriarNovoItem();
        itemCardapio.Nome = "Lasagna";

        // Act
        var excecao = Assert.ThrowsExactly<ServiceException>(
            () => { servicoItemCardapio.Create(itemCardapio); });

        // Assert
        Assert.AreEqual(
            "Já existe um item do cardápio com o mesmo nome neste restaurante.",
            excecao.Message);
        Assert.HasCount(3, servicoItemCardapio.GetAll().ToList());
    }

    [TestMethod]
    public void Edit_QuandoItemExiste_DeveAtualizarSeusDados()
    {
        // Arrange
        var itemCardapio = new Itemcardapio
        {
            Id = 2,
            Nome = "Lasagna alla bolognese",
            Preco = 54.90m,
            Detalhes = "Massa fresca, ragù e queijo gratinado",
            Ativo = 0,
            Disponivel = "0",
            IdRestaurante = 1,
            IdGrupoCardapio = 1
        };

        // Act
        servicoItemCardapio.Edit(itemCardapio);

        // Assert
        var itemAtualizado = servicoItemCardapio.Get(2);

        Assert.IsNotNull(itemAtualizado);
        Assert.AreEqual("Lasagna alla bolognese", itemAtualizado.Nome);
        Assert.AreEqual(54.90m, itemAtualizado.Preco);
        Assert.AreEqual("Massa fresca, ragù e queijo gratinado", itemAtualizado.Detalhes);
        Assert.AreEqual((sbyte)0, itemAtualizado.Ativo);
        Assert.AreEqual("0", itemAtualizado.Disponivel);
        Assert.AreEqual(1u, itemAtualizado.IdGrupoCardapio);
    }

    [TestMethod]
    public void Edit_QuandoNomeJaExisteEmOutroItem_DeveLancarServiceException()
    {
        // Arrange
        var itemCardapio = new Itemcardapio
        {
            Id = 2,
            Nome = "Gnocchi",
            Preco = 49.90m,
            Ativo = 1,
            Disponivel = "1",
            IdRestaurante = 1,
            IdGrupoCardapio = 2
        };

        // Act
        var excecao = Assert.ThrowsExactly<ServiceException>(
            () => { servicoItemCardapio.Edit(itemCardapio); });

        // Assert
        Assert.AreEqual(
            "Já existe um item do cardápio com o mesmo nome neste restaurante.",
            excecao.Message);

        var itemSemAlteracao = servicoItemCardapio.Get(2);

        Assert.IsNotNull(itemSemAlteracao);
        Assert.AreEqual("Lasagna", itemSemAlteracao.Nome);
    }

    [TestMethod]
    public void Delete_QuandoItemNaoFoiPedido_DeveRemoverItem()
    {
        // Act
        servicoItemCardapio.Delete(2);

        // Assert
        var itemExcluido = servicoItemCardapio.Get(2);
        var itensRestantes = servicoItemCardapio.GetAll().ToList();

        Assert.IsNull(itemExcluido);
        Assert.HasCount(2, itensRestantes);
        Assert.IsFalse(itensRestantes.Any(item => item.Id == 2));
    }

    [TestMethod]
    public void Delete_QuandoItemJaFoiPedido_DeveLancarServiceException()
    {
        // Act
        var excecao = Assert.ThrowsExactly<ServiceException>(
            () => { servicoItemCardapio.Delete(1); });

        // Assert
        Assert.AreEqual(
            "Não é possível excluir o item, pois ele já foi usado em pedidos. Para tirá-lo do cardápio, desmarque a opção Ativo no cardápio.",
            excecao.Message);
        Assert.IsNotNull(servicoItemCardapio.Get(1));
    }

    [TestMethod]
    public void Delete_QuandoItemNaoExiste_NaoDeveAlterarOsDados()
    {
        // Arrange
        var itensIniciais = servicoItemCardapio.GetAll().ToList();

        // Act
        servicoItemCardapio.Delete(999);

        // Assert
        var itensFinais = servicoItemCardapio.GetAll().ToList();

        Assert.HasCount(itensIniciais.Count, itensFinais);
        CollectionAssert.AreEquivalent(
            itensIniciais.Select(item => item.Id).ToList(),
            itensFinais.Select(item => item.Id).ToList());
    }

    [TestMethod]
    public void AlterarDisponibilidade_QuandoItemEstaDisponivel_DeveMarcarComoEmFalta()
    {
        // Act
        servicoItemCardapio.AlterarDisponibilidade(1, false);

        // Assert
        var item = servicoItemCardapio.Get(1);

        Assert.IsNotNull(item);
        Assert.AreEqual("0", item.Disponivel);
        Assert.AreEqual("Bruschetta", item.Nome);
        Assert.AreEqual(24.90m, item.Preco);
    }

    [TestMethod]
    public void AlterarDisponibilidade_QuandoItemEstaEmFalta_DeveMarcarComoDisponivel()
    {
        // Act
        servicoItemCardapio.AlterarDisponibilidade(3, true);

        // Assert
        var item = servicoItemCardapio.Get(3);

        Assert.IsNotNull(item);
        Assert.AreEqual("1", item.Disponivel);
    }

    [TestMethod]
    public void AlterarDisponibilidade_QuandoItemNaoExiste_DeveLancarServiceException()
    {
        // Act
        var excecao = Assert.ThrowsExactly<ServiceException>(
            () => { servicoItemCardapio.AlterarDisponibilidade(999, false); });

        // Assert
        Assert.AreEqual("Item do cardápio não encontrado.", excecao.Message);
    }

    [TestMethod]
    public void Get_QuandoIdExiste_DeveRetornarItemComGrupoERestaurante()
    {
        // Act
        var item = servicoItemCardapio.Get(1);

        // Assert
        Assert.IsNotNull(item);
        Assert.AreEqual(1u, item.Id);
        Assert.AreEqual("Bruschetta", item.Nome);
        Assert.AreEqual(24.90m, item.Preco);
        Assert.AreEqual("Entradas", item.IdGrupoCardapioNavigation.Nome);
        Assert.AreEqual("Restaurante Italiano", item.IdRestauranteNavigation.Nome);
    }

    [TestMethod]
    public void Get_QuandoIdNaoExiste_DeveRetornarNulo()
    {
        // Act
        var item = servicoItemCardapio.Get(999);

        // Assert
        Assert.IsNull(item);
    }

    [TestMethod]
    public void GetAll_QuandoExistemItens_DeveRetornarOrdenadosPorGrupoEDepoisPeloNome()
    {
        // Act
        var itens = servicoItemCardapio.GetAll().ToList();

        // Assert
        Assert.IsNotNull(itens);
        Assert.HasCount(3, itens);

        // Entradas (Bruschetta) e depois Massas (Gnocchi, Lasagna)
        CollectionAssert.AreEqual(
            new uint[] { 1, 3, 2 },
            itens.Select(item => item.Id).ToList());
    }

    [TestMethod]
    public void GetAll_QuandoNaoHaItens_DeveRetornarColecaoVazia()
    {
        // Arrange
        contexto.Itemcardapios.RemoveRange(contexto.Itemcardapios);
        contexto.SaveChanges();

        // Act
        var itens = servicoItemCardapio.GetAll().ToList();

        // Assert
        Assert.IsNotNull(itens);
        Assert.IsEmpty(itens);
    }

    private static Itemcardapio CriarNovoItem()
    {
        return new Itemcardapio
        {
            Id = 4,
            Nome = "Tiramisù",
            Preco = 28.50m,
            Detalhes = "Mascarpone, café e cacau",
            Ativo = 1,
            Disponivel = "1",
            IdRestaurante = 1,
            IdGrupoCardapio = 2
        };
    }
}
