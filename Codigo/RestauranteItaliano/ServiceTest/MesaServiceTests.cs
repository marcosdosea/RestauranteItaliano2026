using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service.Tests;

[TestClass]
public class MesaServiceTests
{
    private RestauranteContext contexto = null!;
    private IMesaService servicoMesa = null!;

    [TestInitialize]
    public void Inicializar()
    {
        var nomeBanco = $"Mesa_{Guid.NewGuid()}";
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

        contexto.Restaurantes.AddRange(
            new Restaurante
            {
                Id = 1,
                IdTipoRestaurante = 1,
                Cnpj = "11222333000181",
                Nome = "Trattoria Bella"
            },
            new Restaurante
            {
                Id = 2,
                IdTipoRestaurante = 1,
                Cnpj = "22333444000162",
                Nome = "Antica Cucina"
            });

        contexto.Mesas.AddRange(
            new Mesa
            {
                Id = 1,
                Identificacao = "Mesa 1",
                IdRestaurante = 1
            },
            new Mesa
            {
                Id = 2,
                Identificacao = "Mesa 2",
                IdRestaurante = 1
            },
            // a mesma identificação em outro restaurante é permitida
            new Mesa
            {
                Id = 3,
                Identificacao = "Mesa 1",
                IdRestaurante = 2
            },
            new Mesa
            {
                Id = 4,
                Identificacao = "Varanda",
                IdRestaurante = 1
            });

        // a mesa 4 já teve um atendimento
        contexto.Atendimentos.Add(
            new Atendimento
            {
                Id = 1,
                IdMesa = 4,
                Status = "F",
                DataHoraInicio = new DateTime(2026, 11, 1, 20, 0, 0)
            });

        contexto.SaveChanges();
        contexto.ChangeTracker.Clear();
        servicoMesa = new MesaService(contexto);
    }

    [TestCleanup]
    public void Finalizar()
    {
        contexto.Dispose();
    }

    [TestMethod]
    public void Create_QuandoDadosSaoValidos_DeveRetornarIdDaMesaCriada()
    {
        // Arrange
        var mesa = CriarMesaNova();

        // Act
        var idRetornado = servicoMesa.Create(mesa);

        // Assert
        Assert.AreEqual(5, idRetornado);
    }

    [TestMethod]
    public void Create_QuandoDadosSaoValidos_DeveArmazenarMesa()
    {
        // Arrange
        var mesa = CriarMesaNova();

        // Act
        servicoMesa.Create(mesa);

        // Assert
        var mesaCriada = servicoMesa.Get(5);
        Assert.IsNotNull(mesaCriada);
        Assert.AreEqual("Mesa 3", mesaCriada.Identificacao);
        Assert.AreEqual(1u, mesaCriada.IdRestaurante);
        Assert.AreEqual("Trattoria Bella", mesaCriada.IdRestauranteNavigation.Nome);
    }

    [TestMethod]
    public void Create_QuandoIdentificacaoTemEspacos_DeveGuardarSemEspacosNasPontas()
    {
        // Arrange
        var mesa = CriarMesaNova();
        mesa.Identificacao = "  Mesa 3  ";

        // Act
        servicoMesa.Create(mesa);

        // Assert
        var mesaCriada = servicoMesa.Get(5);
        Assert.IsNotNull(mesaCriada);
        Assert.AreEqual("Mesa 3", mesaCriada.Identificacao);
    }

    [TestMethod]
    public void Create_QuandoIdentificacaoJaExisteNoMesmoRestaurante_DeveLancarServiceException()
    {
        // Arrange
        var mesa = CriarMesaNova();
        mesa.Identificacao = "mesa 1";

        // Act
        var excecao = Assert.ThrowsExactly<ServiceException>(
            () => { servicoMesa.Create(mesa); });

        // Assert
        Assert.AreEqual(
            "Já existe uma mesa com essa identificação neste restaurante.",
            excecao.Message);
        Assert.HasCount(4, servicoMesa.GetAll().ToList());
    }

    [TestMethod]
    public void Create_QuandoIdentificacaoExisteEmOutroRestaurante_DeveCriarMesa()
    {
        // Arrange
        var mesa = CriarMesaNova();
        mesa.Identificacao = "Mesa 2";
        mesa.IdRestaurante = 2;

        // Act
        servicoMesa.Create(mesa);

        // Assert
        Assert.HasCount(5, servicoMesa.GetAll().ToList());
    }

    [TestMethod]
    public void Create_QuandoRestauranteNaoExiste_DeveLancarServiceException()
    {
        // Arrange
        var mesa = CriarMesaNova();
        mesa.IdRestaurante = 999;

        // Act
        var excecao = Assert.ThrowsExactly<ServiceException>(
            () => { servicoMesa.Create(mesa); });

        // Assert
        Assert.AreEqual("O restaurante informado não existe.", excecao.Message);
        Assert.HasCount(4, servicoMesa.GetAll().ToList());
    }

    [TestMethod]
    public void Create_QuandoIdentificacaoEstaEmBranco_DeveLancarServiceException()
    {
        // Arrange
        var mesa = CriarMesaNova();
        mesa.Identificacao = "   ";

        // Act
        var excecao = Assert.ThrowsExactly<ServiceException>(
            () => { servicoMesa.Create(mesa); });

        // Assert
        Assert.AreEqual("A identificação da mesa é obrigatória.", excecao.Message);
    }

    [TestMethod]
    public void Edit_QuandoMesaExiste_DeveAtualizarSeusDados()
    {
        // Arrange
        var mesa = new Mesa { Id = 2, Identificacao = "Mesa 20", IdRestaurante = 2 };

        // Act
        servicoMesa.Edit(mesa);

        // Assert
        var mesaEditada = servicoMesa.Get(2);
        Assert.IsNotNull(mesaEditada);
        Assert.AreEqual("Mesa 20", mesaEditada.Identificacao);
        Assert.AreEqual(2u, mesaEditada.IdRestaurante);
        Assert.AreEqual("Antica Cucina", mesaEditada.IdRestauranteNavigation.Nome);
    }

    [TestMethod]
    public void Edit_QuandoIdentificacaoNaoMuda_DeveSalvarSemErro()
    {
        // Arrange
        var mesa = new Mesa { Id = 1, Identificacao = "Mesa 1", IdRestaurante = 1 };

        // Act
        servicoMesa.Edit(mesa);

        // Assert
        var mesaEditada = servicoMesa.Get(1);
        Assert.IsNotNull(mesaEditada);
        Assert.AreEqual("Mesa 1", mesaEditada.Identificacao);
    }

    [TestMethod]
    public void Edit_QuandoIdentificacaoJaExisteEmOutraMesaDoRestaurante_DeveLancarServiceException()
    {
        // Arrange
        var mesa = new Mesa { Id = 2, Identificacao = "Mesa 1", IdRestaurante = 1 };

        // Act
        var excecao = Assert.ThrowsExactly<ServiceException>(
            () => { servicoMesa.Edit(mesa); });

        // Assert
        Assert.AreEqual(
            "Já existe uma mesa com essa identificação neste restaurante.",
            excecao.Message);
        Assert.AreEqual("Mesa 2", servicoMesa.Get(2)!.Identificacao);
    }

    [TestMethod]
    public void Edit_QuandoRestauranteNaoExiste_DeveLancarServiceException()
    {
        // Arrange
        var mesa = new Mesa { Id = 2, Identificacao = "Mesa 2", IdRestaurante = 999 };

        // Act
        var excecao = Assert.ThrowsExactly<ServiceException>(
            () => { servicoMesa.Edit(mesa); });

        // Assert
        Assert.AreEqual("O restaurante informado não existe.", excecao.Message);
    }

    [TestMethod]
    public void Edit_QuandoMesaComAtendimentosMudaDeRestaurante_DeveLancarServiceException()
    {
        // Arrange
        var mesa = new Mesa { Id = 4, Identificacao = "Varanda", IdRestaurante = 2 };

        // Act
        var excecao = Assert.ThrowsExactly<ServiceException>(
            () => { servicoMesa.Edit(mesa); });

        // Assert
        Assert.AreEqual(
            "Não é possível trocar o restaurante de uma mesa que já possui atendimentos.",
            excecao.Message);
        Assert.AreEqual(1u, servicoMesa.Get(4)!.IdRestaurante);
    }

    [TestMethod]
    public void Edit_QuandoMesaComAtendimentosMudaSoAIdentificacao_DeveSalvarSemErro()
    {
        // Arrange
        var mesa = new Mesa { Id = 4, Identificacao = "Varanda 1", IdRestaurante = 1 };

        // Act
        servicoMesa.Edit(mesa);

        // Assert
        Assert.AreEqual("Varanda 1", servicoMesa.Get(4)!.Identificacao);
    }

    [TestMethod]
    public void Delete_QuandoMesaNaoTemAtendimentos_DeveRemoverMesa()
    {
        // Act
        servicoMesa.Delete(2);

        // Assert
        Assert.IsNull(servicoMesa.Get(2));
        Assert.HasCount(3, servicoMesa.GetAll().ToList());
    }

    [TestMethod]
    public void Delete_QuandoMesaTemAtendimentos_DeveLancarServiceException()
    {
        // Act
        var excecao = Assert.ThrowsExactly<ServiceException>(
            () => { servicoMesa.Delete(4); });

        // Assert
        Assert.AreEqual(
            "Não é possível excluir a mesa, pois ela possui atendimentos registrados.",
            excecao.Message);
        Assert.IsNotNull(servicoMesa.Get(4));
    }

    [TestMethod]
    public void Delete_QuandoMesaNaoExiste_NaoDeveAlterarOsDados()
    {
        // Act
        servicoMesa.Delete(999);

        // Assert
        Assert.HasCount(4, servicoMesa.GetAll().ToList());
    }

    [TestMethod]
    public void Get_QuandoIdExiste_DeveRetornarMesaComSeuRestaurante()
    {
        // Act
        var mesa = servicoMesa.Get(3);

        // Assert
        Assert.IsNotNull(mesa);
        Assert.AreEqual(3, mesa.Id);
        Assert.AreEqual("Mesa 1", mesa.Identificacao);
        Assert.AreEqual("Antica Cucina", mesa.IdRestauranteNavigation.Nome);
    }

    [TestMethod]
    public void Get_QuandoIdNaoExiste_DeveRetornarNulo()
    {
        // Act
        var mesa = servicoMesa.Get(999);

        // Assert
        Assert.IsNull(mesa);
    }

    [TestMethod]
    public void GetAll_QuandoExistemMesas_DeveRetornarOrdenadasPorRestauranteEIdentificacao()
    {
        // Act
        var mesas = servicoMesa.GetAll().ToList();

        // Assert
        Assert.HasCount(4, mesas);
        CollectionAssert.AreEqual(
            new[] { 3, 1, 2, 4 },
            mesas.Select(mesa => mesa.Id).ToList());
        Assert.AreEqual("Antica Cucina", mesas.First().IdRestauranteNavigation.Nome);
    }

    [TestMethod]
    public void GetAll_QuandoNaoHaMesas_DeveRetornarColecaoVazia()
    {
        // Arrange
        contexto.Atendimentos.RemoveRange(contexto.Atendimentos);
        contexto.Mesas.RemoveRange(contexto.Mesas);
        contexto.SaveChanges();

        // Act
        var mesas = servicoMesa.GetAll().ToList();

        // Assert
        Assert.IsEmpty(mesas);
    }

    private static Mesa CriarMesaNova()
    {
        return new Mesa
        {
            Id = 5,
            Identificacao = "Mesa 3",
            IdRestaurante = 1
        };
    }
}
