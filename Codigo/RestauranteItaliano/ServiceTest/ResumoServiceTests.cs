using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service.Tests;

[TestClass]
public class ResumoServiceTests
{
    private RestauranteContext contexto = null!;
    private IResumoService servicoResumo = null!;

    [TestInitialize]
    public void Inicializar()
    {
        var nomeBanco = $"Restaurante_{Guid.NewGuid()}";

        var opcoes = new DbContextOptionsBuilder<RestauranteContext>()
            .UseInMemoryDatabase(nomeBanco)
            .Options;

        contexto = new RestauranteContext(opcoes);

        servicoResumo = new ResumoService(contexto);
    }

    [TestCleanup]
    public void Finalizar()
    {
        contexto.Dispose();
    }

    [TestMethod]
    public void GetResumo_QuandoExistemItens_DeveContarApenasOsItensAtivos()
    {
        // Arrange
        ArmazenarDadosDeTeste();

        // Act
        var resumo = servicoResumo.GetResumo();

        // Assert
        Assert.AreEqual(3, resumo.ItensNoCardapio);
        Assert.AreEqual(2, resumo.ItensDisponiveis);
        Assert.AreEqual(1, resumo.ItensEmFalta);
    }

    [TestMethod]
    public void GetResumo_QuandoExistemItens_DeveRetornarGruposOrdenadosPelaQuantidadeDeItensAtivos()
    {
        // Arrange
        ArmazenarDadosDeTeste();

        // Act
        var grupos = servicoResumo.GetResumo().Grupos.ToList();

        // Assert
        // "Bebidas" não tem itens e "Sobremesas" só tem item inativo: os dois ficam de fora
        Assert.HasCount(2, grupos);

        Assert.AreEqual("Massas", grupos[0].Nome);
        Assert.AreEqual(2, grupos[0].QuantidadeItens);

        Assert.AreEqual("Entradas", grupos[1].Nome);
        Assert.AreEqual(1, grupos[1].QuantidadeItens);
    }

    [TestMethod]
    public void GetResumo_QuandoExistemCadastros_DeveRetornarAQuantidadeDeCadaCadastro()
    {
        // Arrange
        ArmazenarDadosDeTeste();

        // Act
        var resumo = servicoResumo.GetResumo();

        // Assert
        Assert.AreEqual(4, resumo.QuantidadeGruposCardapio);
        Assert.AreEqual(2, resumo.QuantidadeFormasPagamento);
        Assert.AreEqual(3, resumo.QuantidadeTiposFuncionario);
        Assert.AreEqual(1, resumo.QuantidadeTiposRestaurante);
    }

    [TestMethod]
    public void GetResumo_QuandoNaoHaDados_DeveRetornarResumoZerado()
    {
        // Act
        var resumo = servicoResumo.GetResumo();

        // Assert
        Assert.AreEqual(0, resumo.ItensNoCardapio);
        Assert.AreEqual(0, resumo.ItensDisponiveis);
        Assert.AreEqual(0, resumo.ItensEmFalta);
        Assert.IsEmpty(resumo.Grupos.ToList());
        Assert.AreEqual(0, resumo.QuantidadeGruposCardapio);
        Assert.AreEqual(0, resumo.QuantidadeFormasPagamento);
        Assert.AreEqual(0, resumo.QuantidadeTiposFuncionario);
        Assert.AreEqual(0, resumo.QuantidadeTiposRestaurante);
    }

    private void ArmazenarDadosDeTeste()
    {
        contexto.Tiporestaurantes.Add(
            new Tiporestaurante
            {
                Id = 1,
                Nome = "Cantina italiana"
            });

        contexto.Tipofuncionarios.AddRange(
            new Tipofuncionario
            {
                Id = 1,
                Nome = "Garçom"
            },
            new Tipofuncionario
            {
                Id = 2,
                Nome = "Cozinheiro"
            },
            new Tipofuncionario
            {
                Id = 3,
                Nome = "Gerente"
            });

        contexto.Formapagamentos.AddRange(
            new Formapagamento
            {
                Id = 1,
                Nome = "Dinheiro"
            },
            new Formapagamento
            {
                Id = 2,
                Nome = "Pix"
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
            },
            new Grupocardapio
            {
                Id = 3,
                Nome = "Sobremesas"
            },
            new Grupocardapio
            {
                Id = 4,
                Nome = "Bebidas"
            });

        contexto.Itemcardapios.AddRange(
            new Itemcardapio
            {
                Id = 1,
                Nome = "Bruschetta",
                Preco = 24.90m,
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
            },
            new Itemcardapio
            {
                Id = 4,
                Nome = "Panna cotta",
                Preco = 22.00m,
                Ativo = 0,
                Disponivel = "1",
                IdRestaurante = 1,
                IdGrupoCardapio = 3
            });

        contexto.SaveChanges();
        contexto.ChangeTracker.Clear();
    }
}
