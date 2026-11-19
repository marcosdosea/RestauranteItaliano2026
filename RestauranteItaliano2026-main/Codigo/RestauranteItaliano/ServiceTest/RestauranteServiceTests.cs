using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service.Tests;

[TestClass]
public class RestauranteServiceTests
{
    private RestauranteContext contexto = null!;
    private IRestauranteService servicoRestaurante = null!;

    [TestInitialize]
    public void Inicializar()
    {
        var nomeBanco = $"Restaurante_{Guid.NewGuid()}";
        var opcoes = new DbContextOptionsBuilder<RestauranteContext>()
            .UseInMemoryDatabase(nomeBanco)
            .Options;
        contexto = new RestauranteContext(opcoes);

        contexto.Tiporestaurantes.AddRange(
            new Tiporestaurante
            {
                Id = 1,
                Nome = "Cantina italiana"
            },
            new Tiporestaurante
            {
                Id = 2,
                Nome = "Churrascaria"
            });

        contexto.Grupocardapios.Add(
            new Grupocardapio
            {
                Id = 1,
                Nome = "Massas"
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
                IdTipoRestaurante = 2,
                Cnpj = "22333444000162",
                Nome = "Boi na Brasa"
            },
            new Restaurante
            {
                Id = 3,
                IdTipoRestaurante = 1,
                Cnpj = "33444555000143",
                Nome = "Antica Cucina"
            },
            new Restaurante
            {
                Id = 4,
                IdTipoRestaurante = 1,
                Cnpj = "55666777000124",
                Nome = "Pizzaria Napoli"
            });

        // o restaurante 1 tem um item de cardápio
        contexto.Itemcardapios.Add(
            new Itemcardapio
            {
                Id = 1,
                Nome = "Lasagna",
                Preco = 49.90m,
                Ativo = 1,
                Disponivel = "1",
                IdRestaurante = 1,
                IdGrupoCardapio = 1
            });

        // o restaurante 2 tem uma mesa
        contexto.Mesas.Add(
            new Mesa
            {
                Id = 1,
                Identificacao = "Mesa 1",
                IdRestaurante = 2
            });

        // o restaurante 3 tem um funcionário
        contexto.Funcionarios.Add(
            new Funcionario
            {
                Id = 1,
                Nome = "Marina Souza",
                Cpf = "12345678909",
                IdRestaurante = 3,
                IdTipoFuncionario = 1
            });
        // o restaurante 4 não tem nenhum vínculo
        contexto.SaveChanges();
        contexto.ChangeTracker.Clear();
        servicoRestaurante = new RestauranteService(contexto);
    }

    [TestCleanup]
    public void Finalizar()
    {
        contexto.Dispose();
    }

    [TestMethod]
    public void Create_QuandoDadosSaoValidos_DeveRetornarIdDoRestauranteCriado()
    {
        // Arrange
        var restaurante = CriarRestauranteNovo();

        // Act
        var idRetornado = servicoRestaurante.Create(restaurante);

        // Assert
        Assert.AreEqual(5u, idRetornado);
    }

    [TestMethod]
    public void Create_QuandoDadosSaoValidos_DeveArmazenarRestaurante()
    {
        // Arrange
        var restauranteEsperado = CriarRestauranteNovo();

        // Act
        servicoRestaurante.Create(restauranteEsperado);

        // Assert
        var restauranteCriado = servicoRestaurante.Get(5);
        Assert.IsNotNull(restauranteCriado);
        Assert.AreEqual(restauranteEsperado.Id, restauranteCriado.Id);
        Assert.AreEqual("Cantina Nova", restauranteCriado.Nome);
        Assert.AreEqual("44555666000133", restauranteCriado.Cnpj);
        Assert.AreEqual(1u, restauranteCriado.IdTipoRestaurante);
        Assert.AreEqual("Cantina italiana", restauranteCriado.IdTipoRestauranteNavigation.Nome);
    }

    [TestMethod]
    public void Create_QuandoDadosTemMascara_DeveGuardarSomenteOsNumeros()
    {
        // Arrange
        var restaurante = new Restaurante
        {
            Id = 5,
            IdTipoRestaurante = 1,
            Nome = "  Cantina Nova  ",
            Cnpj = "44.555.666/0001-33",
            Cep = "49000-123",
            Rua = "Rua das Flores, 10",
            Estado = "se",
            Telefone1 = "(79) 3222-1111",
            Telefone2 = "(79) 99999-9999"
        };

        // Act
        servicoRestaurante.Create(restaurante);

        // Assert
        var restauranteCriado = servicoRestaurante.Get(5);
        Assert.IsNotNull(restauranteCriado);
        Assert.AreEqual("Cantina Nova", restauranteCriado.Nome);
        Assert.AreEqual("44555666000133", restauranteCriado.Cnpj);
        Assert.AreEqual("49000123", restauranteCriado.Cep);
        Assert.AreEqual("SE", restauranteCriado.Estado);
        Assert.AreEqual("7932221111", restauranteCriado.Telefone1);
        Assert.AreEqual("79999999999", restauranteCriado.Telefone2);
    }

    [TestMethod]
    public void Create_QuandoCnpjJaExiste_DeveLancarServiceException()
    {
        // Arrange
        var restaurante = CriarRestauranteNovo();
        restaurante.Cnpj = "11.222.333/0001-81";

        // Act
        var excecao = Assert.ThrowsExactly<ServiceException>(
            () => { servicoRestaurante.Create(restaurante); });

        // Assert
        Assert.AreEqual(
            "Já existe um restaurante cadastrado com esse CNPJ.",
            excecao.Message);
        Assert.HasCount(4, servicoRestaurante.GetAll().ToList());
    }

    [TestMethod]
    public void Create_QuandoTipoDeRestauranteNaoExiste_DeveLancarServiceException()
    {
        // Arrange
        var restaurante = CriarRestauranteNovo();
        restaurante.IdTipoRestaurante = 99;

        // Act
        var excecao = Assert.ThrowsExactly<ServiceException>(
            () => { servicoRestaurante.Create(restaurante); });

        // Assert
        Assert.AreEqual(
            "O tipo de restaurante informado não existe.",
            excecao.Message);
        Assert.HasCount(4, servicoRestaurante.GetAll().ToList());
    }

    [TestMethod]
    public void Edit_QuandoRestauranteExiste_DeveAtualizarSeusDados()
    {
        // Arrange
        var restaurante = new Restaurante
        {
            Id = 2,
            IdTipoRestaurante = 1,
            Cnpj = "22333444000162",
            Nome = "Boi na Brasa Grill",
            Cidade = "Aracaju",
            Estado = "SE"
        };

        // Act
        servicoRestaurante.Edit(restaurante);

        // Assert
        var restauranteAtualizado = servicoRestaurante.Get(2);
        Assert.IsNotNull(restauranteAtualizado);
        Assert.AreEqual("Boi na Brasa Grill", restauranteAtualizado.Nome);
        Assert.AreEqual(1u, restauranteAtualizado.IdTipoRestaurante);
        Assert.AreEqual("Aracaju", restauranteAtualizado.Cidade);
        Assert.AreEqual("SE", restauranteAtualizado.Estado);
    }

    [TestMethod]
    public void Edit_QuandoCnpjNaoMuda_DeveSalvarSemErro()
    {
        // Arrange
        var restaurante = new Restaurante
        {
            Id = 1,
            IdTipoRestaurante = 1,
            Cnpj = "11.222.333/0001-81",
            Nome = "Trattoria Bella Napoli"
        };

        // Act
        servicoRestaurante.Edit(restaurante);

        // Assert
        var restauranteAtualizado = servicoRestaurante.Get(1);
        Assert.IsNotNull(restauranteAtualizado);
        Assert.AreEqual("Trattoria Bella Napoli", restauranteAtualizado.Nome);
        Assert.AreEqual("11222333000181", restauranteAtualizado.Cnpj);

    }

    [TestMethod]
    public void Edit_QuandoCnpjJaExisteEmOutroRestaurante_DeveLancarServiceException()
    {
        // Arrange
        var restaurante = new Restaurante
        {
            Id = 2,
            IdTipoRestaurante = 2,
            Cnpj = "11222333000181",
            Nome = "Boi na Brasa"
        };

        // Act
        var excecao = Assert.ThrowsExactly<ServiceException>(
            () => { servicoRestaurante.Edit(restaurante); });

        // Assert
        Assert.AreEqual(
            "Já existe um restaurante cadastrado com esse CNPJ.",
            excecao.Message);
    }

    [TestMethod]
    public void Edit_QuandoTipoDeRestauranteNaoExiste_DeveLancarServiceException()
    {
        // Arrange
        var restaurante = new Restaurante
        {
            Id = 2,
            IdTipoRestaurante = 99,
            Cnpj = "22333444000162",
            Nome = "Boi na Brasa"
        };

        // Act
        var excecao = Assert.ThrowsExactly<ServiceException>(
            () => { servicoRestaurante.Edit(restaurante); });

        // Assert
        Assert.AreEqual(
            "O tipo de restaurante informado não existe.",
            excecao.Message);
    }

    [TestMethod]
    public void Delete_QuandoRestauranteNaoTemVinculos_DeveRemoverRestaurante()
    {
        // Act
        servicoRestaurante.Delete(4);

        // Assert
        var restauranteExcluido = servicoRestaurante.Get(4);
        var restaurantesRestantes = servicoRestaurante.GetAll().ToList();
        Assert.IsNull(restauranteExcluido);
        Assert.HasCount(3, restaurantesRestantes);
        Assert.IsFalse(restaurantesRestantes.Any(restaurante => restaurante.Id == 4));
    }

    [TestMethod]
    public void Delete_QuandoRestauranteTemItensDeCardapio_DeveLancarServiceException()
    {
        // Act
        var excecao = Assert.ThrowsExactly<ServiceException>(
            () => { servicoRestaurante.Delete(1); });

        // Assert
        Assert.AreEqual(
            "Não é possível excluir o restaurante, pois ele possui funcionários, itens de cardápio ou mesas cadastrados.",
            excecao.Message);
        Assert.IsNotNull(servicoRestaurante.Get(1));
    }

    [TestMethod]
    public void Delete_QuandoRestauranteTemMesas_DeveLancarServiceException()
    {
        // Act
        var excecao = Assert.ThrowsExactly<ServiceException>(
            () => { servicoRestaurante.Delete(2); });

        // Assert
        Assert.AreEqual(
            "Não é possível excluir o restaurante, pois ele possui funcionários, itens de cardápio ou mesas cadastrados.",
            excecao.Message);
        Assert.IsNotNull(servicoRestaurante.Get(2));
    }

    [TestMethod]
    public void Delete_QuandoRestauranteTemFuncionarios_DeveLancarServiceException()
    {
        // Act
        var excecao = Assert.ThrowsExactly<ServiceException>(
            () => { servicoRestaurante.Delete(3); });

        // Assert
        Assert.AreEqual(
            "Não é possível excluir o restaurante, pois ele possui funcionários, itens de cardápio ou mesas cadastrados.",
            excecao.Message);
        Assert.IsNotNull(servicoRestaurante.Get(3));
    }

    [TestMethod]
    public void Delete_QuandoRestauranteNaoExiste_NaoDeveAlterarOsDados()
    {
        // Arrange
        var restaurantesIniciais = servicoRestaurante.GetAll().ToList();

        // Act
        servicoRestaurante.Delete(999);

        // Assert
        var restaurantesFinais = servicoRestaurante.GetAll().ToList();
        Assert.HasCount(restaurantesIniciais.Count, restaurantesFinais);
        CollectionAssert.AreEquivalent(
            restaurantesIniciais.Select(restaurante => restaurante.Id).ToList(),
            restaurantesFinais.Select(restaurante => restaurante.Id).ToList());
    }

    [TestMethod]
    public void Get_QuandoIdExiste_DeveRetornarRestauranteComSeuTipo()
    {
        // Act
        var restaurante = servicoRestaurante.Get(1);

        // Assert
        Assert.IsNotNull(restaurante);
        Assert.AreEqual(1u, restaurante.Id);
        Assert.AreEqual("Trattoria Bella", restaurante.Nome);
        Assert.AreEqual("11222333000181", restaurante.Cnpj);
        Assert.AreEqual("Cantina italiana", restaurante.IdTipoRestauranteNavigation.Nome);
    }

    [TestMethod]
    public void Get_QuandoIdNaoExiste_DeveRetornarNulo()
    {
        // Act
        var restaurante = servicoRestaurante.Get(999);

        // Assert
        Assert.IsNull(restaurante);
    }

    [TestMethod]
    public void GetAll_QuandoExistemRestaurantes_DeveRetornarTodosOrdenadosPeloNome()
    {
        // Act
        var restaurantes = servicoRestaurante.GetAll().ToList();

        // Assert
        Assert.IsNotNull(restaurantes);
        Assert.HasCount(4, restaurantes);
        CollectionAssert.AreEqual(
            new[] { "Antica Cucina", "Boi na Brasa", "Pizzaria Napoli", "Trattoria Bella" },
            restaurantes.Select(restaurante => restaurante.Nome).ToList());
        Assert.AreEqual("Cantina italiana", restaurantes.First().IdTipoRestauranteNavigation.Nome);
    }

    [TestMethod]
    public void GetAll_QuandoNaoHaRestaurantes_DeveRetornarColecaoVazia()
    {
        // Arrange
        contexto.Funcionarios.RemoveRange(contexto.Funcionarios);
        contexto.Mesas.RemoveRange(contexto.Mesas);
        contexto.Itemcardapios.RemoveRange(contexto.Itemcardapios);
        contexto.Restaurantes.RemoveRange(contexto.Restaurantes);
        contexto.SaveChanges();

        // Act
        var restaurantes = servicoRestaurante.GetAll().ToList();

        // Assert
        Assert.IsNotNull(restaurantes);
        Assert.IsEmpty(restaurantes);
    }

    private static Restaurante CriarRestauranteNovo()
    {
        return new Restaurante
        {
            Id = 5,
            IdTipoRestaurante = 1,
            Cnpj = "44555666000133",
            Nome = "Cantina Nova"
        };
    }
}


