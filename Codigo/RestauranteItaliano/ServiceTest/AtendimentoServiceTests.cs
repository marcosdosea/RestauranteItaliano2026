using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using Service;

namespace Service.Tests;

[TestClass]
public class AtendimentoServiceTests
{
    private RestauranteContext contexto = null!;
    private DbContextOptions<RestauranteContext> opcoes = null!;
    private IAtendimentoService servico = null!;

    private readonly DateTime inicioAtendimentoAtivo =
        new(2026, 11, 23, 12, 0, 0);

    [TestInitialize]
    public void Inicializar()
    {
        // Um banco diferente para cada teste
        var nomeBanco = $"Atendimento_{Guid.NewGuid()}";

        opcoes = new DbContextOptionsBuilder<RestauranteContext>()
            .UseInMemoryDatabase(nomeBanco)
            .Options;

        contexto = new RestauranteContext(opcoes);

        // Mesa 1: livre
        // Mesa 2: atendimento iniciado
        // Mesa 3: atendimento cancelado
        // Mesa 4: atendimento finalizado
        contexto.Mesas.AddRange(
            new Mesa { Id = 1, Identificacao = "Mesa 1", IdRestaurante = 1 },
            new Mesa { Id = 2, Identificacao = "Mesa 2", IdRestaurante = 1 },
            new Mesa { Id = 3, Identificacao = "Mesa 3", IdRestaurante = 1 },
            new Mesa { Id = 4, Identificacao = "Mesa 4", IdRestaurante = 1 }
        );

        contexto.Atendimentos.AddRange(
            new Atendimento
            {
                IdMesa = 2,
                DataHoraInicio = inicioAtendimentoAtivo,
                Status = "I",
                TotalConta = 0m,
                TotalServico = 0m,
                TotalDesconto = 0m,
                Total = 0m
            },
            new Atendimento
            {
                IdMesa = 3,
                DataHoraInicio = inicioAtendimentoAtivo.AddHours(-2),
                DataHoraFim = inicioAtendimentoAtivo.AddHours(-1),
                Status = "C",
                TotalConta = 0m,
                TotalServico = 0m,
                TotalDesconto = 0m,
                Total = 0m
            },
            new Atendimento
            {
                IdMesa = 4,
                DataHoraInicio = inicioAtendimentoAtivo.AddHours(-4),
                DataHoraFim = inicioAtendimentoAtivo.AddHours(-3),
                Status = "F",
                TotalConta = 0m,
                TotalServico = 0m,
                TotalDesconto = 0m,
                Total = 0m
            }
        );

        contexto.SaveChanges();
        contexto.ChangeTracker.Clear();

        servico = new AtendimentoService(contexto);
    }

    [TestCleanup]
    public void Finalizar()
    {
        contexto.Dispose();
    }

    [TestMethod]
    public void Iniciar_QuandoMesaEstaLivre_DeveSalvarAtendimentoComValoresIniciais()
    {
        var antesDeIniciar = DateTime.Now;

        var idAtendimento = servico.Iniciar(1, 1u);

        var depoisDeIniciar = DateTime.Now;

        // Usa outro contexto para conferir o que foi salvo no banco InMemory
        using var leitura = new RestauranteContext(opcoes);
        var atendimento = leitura.Atendimentos
            .Single(a => a.Id == idAtendimento);

        Assert.AreEqual(1, atendimento.IdMesa);
        Assert.AreEqual("I", atendimento.Status);
        Assert.IsNull(atendimento.DataHoraFim);
        Assert.IsTrue(
            atendimento.DataHoraInicio >= antesDeIniciar &&
            atendimento.DataHoraInicio <= depoisDeIniciar);
        Assert.AreEqual(0m, atendimento.TotalConta);
        Assert.AreEqual(0m, atendimento.TotalServico);
        Assert.AreEqual(0m, atendimento.TotalDesconto);
        Assert.AreEqual(0m, atendimento.Total);
    }

    [TestMethod]
    public void Iniciar_QuandoMesaNaoExiste_DeveLancarServiceException()
    {
        var excecao = Assert.ThrowsExactly<ServiceException>(
            () => servico.Iniciar(999, 1u));

        Assert.AreEqual("Mesa não encontrada neste restaurante.", excecao.Message);

        using var leitura = new RestauranteContext(opcoes);
        Assert.AreEqual(3, leitura.Atendimentos.Count());
    }

    [TestMethod]
    public void Iniciar_QuandoMesaJaTemAtendimentoAtivo_DeveLancarServiceException()
    {
        var excecao = Assert.ThrowsExactly<ServiceException>(
            () => servico.Iniciar(2, 1u));

        Assert.AreEqual(
            "Esta mesa já possui um atendimento iniciado.",
            excecao.Message);

        using var leitura = new RestauranteContext(opcoes);
        Assert.AreEqual(3, leitura.Atendimentos.Count());
    }

    [TestMethod]
    public void Iniciar_QuandoAtendimentoAnteriorFoiCancelado_DevePermitirNovo()
    {
        var idAtendimento = servico.Iniciar(3, 1u);

        using var leitura = new RestauranteContext(opcoes);
        var novoAtendimento = leitura.Atendimentos
            .Single(a => a.Id == idAtendimento);

        Assert.AreEqual(3, novoAtendimento.IdMesa);
        Assert.AreEqual("I", novoAtendimento.Status);
    }

    [TestMethod]
    public void Iniciar_QuandoAtendimentoAnteriorFoiFinalizado_DevePermitirNovo()
    {
        var idAtendimento = servico.Iniciar(4, 1u);

        using var leitura = new RestauranteContext(opcoes);
        var novoAtendimento = leitura.Atendimentos
            .Single(a => a.Id == idAtendimento);

        Assert.AreEqual(4, novoAtendimento.IdMesa);
        Assert.AreEqual("I", novoAtendimento.Status);
    }

    [TestMethod]
    public void GetMapaMesas_DeveIndicarMesasLivresEOcupadas()
    {
        // ToList materializa a consulta no teste, para podermos verificar os resultados
        var mapa = servico.GetMapaMesas(1u).ToList();

        var mesaLivre = mapa.Single(m => m.IdMesa == 1);
        var mesaOcupada = mapa.Single(m => m.IdMesa == 2);
        var mesaCancelada = mapa.Single(m => m.IdMesa == 3);
        var mesaFinalizada = mapa.Single(m => m.IdMesa == 4);

        Assert.IsFalse(mesaLivre.Ocupada);
        Assert.IsNull(mesaLivre.InicioAtendimento);

        Assert.IsTrue(mesaOcupada.Ocupada);
        Assert.AreEqual(
            inicioAtendimentoAtivo,
            mesaOcupada.InicioAtendimento);

        Assert.IsFalse(mesaCancelada.Ocupada);
        Assert.IsFalse(mesaFinalizada.Ocupada);
    }
}