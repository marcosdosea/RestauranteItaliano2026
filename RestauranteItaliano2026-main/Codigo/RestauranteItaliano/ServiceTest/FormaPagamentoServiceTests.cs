using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service.Tests;

[TestClass]
public class FormaPagamentoServiceTests
{
    private RestauranteContext contexto = null!;
    private IFormaPagamentoService servicoFormaPagamento = null!;

    [TestInitialize]
    public void Inicializar()
    {
        var nomeBanco = $"Restaurante_{Guid.NewGuid()}";

        var opcoes = new DbContextOptionsBuilder<RestauranteContext>()
            .UseInMemoryDatabase(nomeBanco)
            .Options;

        contexto = new RestauranteContext(opcoes);

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
            },
            new Formapagamento
            {
                Id = 3,
                Nome = "Cartão de crédito"
            });

        // a forma de pagamento 1 já foi usada em um pagamento
        contexto.Pagamentos.Add(
            new Pagamento
            {
                Id = 1,
                DataHora = new DateTime(2026, 5, 10, 20, 30, 0),
                Valor = 120.50m,
                IdAtendimento = 1,
                IdFormaPagamento = 1
            });

        contexto.SaveChanges();
        contexto.ChangeTracker.Clear();

        servicoFormaPagamento = new FormaPagamentoService(contexto);
    }

    [TestCleanup]
    public void Finalizar()
    {
        contexto.Dispose();
    }

    [TestMethod]
    public void Create_QuandoDadosSaoValidos_DeveRetornarIdDaFormaDePagamentoCriada()
    {
        // Arrange
        var formaPagamento = new Formapagamento
        {
            Id = 4,
            Nome = "Cartão de débito"
        };

        // Act
        var idRetornado = servicoFormaPagamento.Create(formaPagamento);

        // Assert
        Assert.AreEqual(4u, idRetornado);
    }

    [TestMethod]
    public void Create_QuandoDadosSaoValidos_DeveArmazenarFormaDePagamento()
    {
        // Arrange
        var formaPagamentoEsperada = new Formapagamento
        {
            Id = 4,
            Nome = "Cartão de débito"
        };

        // Act
        servicoFormaPagamento.Create(formaPagamentoEsperada);

        // Assert
        var formaPagamentoCriada = servicoFormaPagamento.Get(4);

        Assert.IsNotNull(formaPagamentoCriada);
        Assert.AreEqual(formaPagamentoEsperada.Id, formaPagamentoCriada.Id);
        Assert.AreEqual(formaPagamentoEsperada.Nome, formaPagamentoCriada.Nome);
    }

    [TestMethod]
    public void Create_QuandoNomeJaExiste_DeveLancarServiceException()
    {
        // Arrange
        var formaPagamento = new Formapagamento
        {
            Id = 4,
            Nome = "Pix"
        };

        // Act
        var excecao = Assert.ThrowsExactly<ServiceException>(
            () => { servicoFormaPagamento.Create(formaPagamento); });

        // Assert
        Assert.AreEqual(
            "Já existe uma forma de pagamento com o mesmo nome.",
            excecao.Message);
        Assert.HasCount(3, servicoFormaPagamento.GetAll().ToList());
    }

    [TestMethod]
    public void Edit_QuandoFormaDePagamentoExiste_DeveAtualizarSeusDados()
    {
        // Arrange
        var formaPagamento = new Formapagamento
        {
            Id = 3,
            Nome = "Cartão de débito"
        };

        // Act
        servicoFormaPagamento.Edit(formaPagamento);

        // Assert
        var formaPagamentoAtualizada = servicoFormaPagamento.Get(3);

        Assert.IsNotNull(formaPagamentoAtualizada);
        Assert.AreEqual("Cartão de débito", formaPagamentoAtualizada.Nome);
    }

    [TestMethod]
    public void Edit_QuandoNomeNaoMuda_DeveSalvarSemErro()
    {
        // Arrange
        var formaPagamento = new Formapagamento
        {
            Id = 2,
            Nome = "Pix"
        };

        // Act
        servicoFormaPagamento.Edit(formaPagamento);

        // Assert
        var formaPagamentoAtualizada = servicoFormaPagamento.Get(2);

        Assert.IsNotNull(formaPagamentoAtualizada);
        Assert.AreEqual("Pix", formaPagamentoAtualizada.Nome);
    }

    [TestMethod]
    public void Edit_QuandoNomeJaExisteEmOutraFormaDePagamento_DeveLancarServiceException()
    {
        // Arrange
        var formaPagamento = new Formapagamento
        {
            Id = 3,
            Nome = "Pix"
        };

        // Act
        var excecao = Assert.ThrowsExactly<ServiceException>(
            () => { servicoFormaPagamento.Edit(formaPagamento); });

        // Assert
        Assert.AreEqual(
            "Já existe uma forma de pagamento com o mesmo nome.",
            excecao.Message);
    }

    [TestMethod]
    public void Delete_QuandoFormaDePagamentoNaoFoiUsada_DeveRemoverFormaDePagamento()
    {
        // Act
        servicoFormaPagamento.Delete(2);

        // Assert
        var formaPagamentoExcluida = servicoFormaPagamento.Get(2);
        var formasPagamentoRestantes = servicoFormaPagamento.GetAll().ToList();

        Assert.IsNull(formaPagamentoExcluida);
        Assert.HasCount(2, formasPagamentoRestantes);
        Assert.IsFalse(formasPagamentoRestantes.Any(formaPagamento => formaPagamento.Id == 2));
    }

    [TestMethod]
    public void Delete_QuandoExistemPagamentosAssociados_DeveLancarServiceException()
    {
        // Act
        var excecao = Assert.ThrowsExactly<ServiceException>(
            () => { servicoFormaPagamento.Delete(1); });

        // Assert
        Assert.AreEqual(
            "Não é possível excluir a forma de pagamento, pois existem pagamentos associados a ela.",
            excecao.Message);
        Assert.IsNotNull(servicoFormaPagamento.Get(1));
    }

    [TestMethod]
    public void Delete_QuandoFormaDePagamentoNaoExiste_NaoDeveAlterarOsDados()
    {
        // Arrange
        var formasPagamentoIniciais = servicoFormaPagamento.GetAll().ToList();

        // Act
        servicoFormaPagamento.Delete(999);

        // Assert
        var formasPagamentoFinais = servicoFormaPagamento.GetAll().ToList();

        Assert.HasCount(formasPagamentoIniciais.Count, formasPagamentoFinais);
        CollectionAssert.AreEquivalent(
            formasPagamentoIniciais.Select(formaPagamento => formaPagamento.Id).ToList(),
            formasPagamentoFinais.Select(formaPagamento => formaPagamento.Id).ToList());
    }

    [TestMethod]
    public void Get_QuandoIdExiste_DeveRetornarFormaDePagamentoCorrespondente()
    {
        // Act
        var formaPagamento = servicoFormaPagamento.Get(1);

        // Assert
        Assert.IsNotNull(formaPagamento);
        Assert.AreEqual(1u, formaPagamento.Id);
        Assert.AreEqual("Dinheiro", formaPagamento.Nome);
    }

    [TestMethod]
    public void Get_QuandoIdNaoExiste_DeveRetornarNulo()
    {
        // Act
        var formaPagamento = servicoFormaPagamento.Get(999);

        // Assert
        Assert.IsNull(formaPagamento);
    }

    [TestMethod]
    public void GetAll_QuandoExistemFormasDePagamento_DeveRetornarTodasOrdenadasPeloNome()
    {
        // Act
        var formasPagamento = servicoFormaPagamento.GetAll().ToList();

        // Assert
        Assert.IsNotNull(formasPagamento);
        Assert.HasCount(3, formasPagamento);

        CollectionAssert.AreEqual(
            new[] { "Cartão de crédito", "Dinheiro", "Pix" },
            formasPagamento.Select(formaPagamento => formaPagamento.Nome).ToList());
    }

    [TestMethod]
    public void GetAll_QuandoNaoHaFormasDePagamento_DeveRetornarColecaoVazia()
    {
        // Arrange
        contexto.Formapagamentos.RemoveRange(contexto.Formapagamentos);
        contexto.SaveChanges();

        // Act
        var formasPagamento = servicoFormaPagamento.GetAll().ToList();

        // Assert
        Assert.IsNotNull(formasPagamento);
        Assert.IsEmpty(formasPagamento);
    }
}
