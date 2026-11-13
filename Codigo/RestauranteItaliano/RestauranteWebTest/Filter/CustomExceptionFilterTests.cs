using Core.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace RestauranteWeb.Filter.Tests;

[TestClass]
public class CustomExceptionFilterTests
{
    private CustomExceptionFilter filtro = null!;

    [TestInitialize]
    public void Inicializar()
    {
        filtro = new CustomExceptionFilter(new EmptyModelMetadataProvider());
    }

    [TestMethod]
    public void OnException_QuandoErroInesperado_DeveMostrarTelaDeErroComMensagemGenerica()
    {
        // Arrange
        var excecao = new InvalidOperationException("detalhe interno");
        var contexto = CriarContexto(excecao);

        // Act
        filtro.OnException(contexto);

        // Assert
        Assert.IsTrue(contexto.ExceptionHandled);

        var viewResult = Assert.IsInstanceOfType<ViewResult>(contexto.Result);

        Assert.AreEqual("Error", viewResult.ViewName);
        Assert.AreEqual(
            "Ocorreu um erro inesperado. Por favor entrar em contato com o administrador do sistema.",
            viewResult.ViewData["ErrorMessage"]);
        Assert.AreSame(excecao, viewResult.ViewData["Exception"]);
    }

    [TestMethod]
    public void OnException_QuandoServiceException_DeveMostrarAMensagemDaRegraDeNegocio()
    {
        // Arrange
        var contexto = CriarContexto(new ServiceException("Item do cardápio não encontrado."));

        // Act
        filtro.OnException(contexto);

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(contexto.Result);

        Assert.AreEqual("Item do cardápio não encontrado.", viewResult.ViewData["ErrorMessage"]);
    }

    [TestMethod]
    public void ObterMensagem_QuandoErroDoEntityFrameworkSemMySql_DeveRetornarMensagemGenerica()
    {
        // Arrange
        var excecao = new DbUpdateException("falha ao salvar", new InvalidOperationException());

        // Act
        var mensagem = CustomExceptionFilter.ObterMensagem(excecao);

        // Assert
        Assert.AreEqual(
            "Ocorreu um erro inesperado. Por favor entrar em contato com o administrador do sistema.",
            mensagem);
    }

    [TestMethod]
    public void ObterMensagemBancoDeDados_QuandoRegistroEstaVinculado_DeveExplicarQueNaoPodeExcluir()
    {
        // Act
        var mensagem = CustomExceptionFilter.ObterMensagemBancoDeDados(1451);

        // Assert
        Assert.AreEqual(
            "Não é possível excluir este registro, pois ele está vinculado a outros dados no sistema.",
            mensagem);
    }

    [TestMethod]
    public void ObterMensagemBancoDeDados_QuandoValorRepetido_DeveExplicarAChaveUnica()
    {
        // Act
        var mensagem = CustomExceptionFilter.ObterMensagemBancoDeDados(1062);

        // Assert
        Assert.AreEqual("Violação de chave única: já existe um registro com este valor.", mensagem);
    }

    [TestMethod]
    public void ObterMensagemBancoDeDados_QuandoChaveEstrangeiraInvalida_DeveExplicarOsDadosRelacionados()
    {
        // Act
        var mensagens = new[] { 1216, 1217, 1452 }
            .Select(CustomExceptionFilter.ObterMensagemBancoDeDados)
            .Distinct()
            .ToList();

        // Assert
        Assert.HasCount(1, mensagens);
        Assert.AreEqual(
            "Violação de chave estrangeira: operação inválida devido a dados relacionados.",
            mensagens[0]);
    }

    [TestMethod]
    public void ObterMensagemBancoDeDados_QuandoCampoInvalido_DeveExplicarOProblemaDoCampo()
    {
        // Act e Assert
        Assert.AreEqual("Um campo obrigatório não foi preenchido.", CustomExceptionFilter.ObterMensagemBancoDeDados(1048));
        Assert.AreEqual("Um campo excedeu o tamanho máximo permitido.", CustomExceptionFilter.ObterMensagemBancoDeDados(1406));
        Assert.AreEqual("Tipo de dado inválido fornecido para um campo.", CustomExceptionFilter.ObterMensagemBancoDeDados(1366));
    }

    [TestMethod]
    public void ObterMensagemBancoDeDados_QuandoOutroErro_DeveRetornarMensagemGeralDoBanco()
    {
        // Act
        var mensagem = CustomExceptionFilter.ObterMensagemBancoDeDados(2002);

        // Assert
        Assert.AreEqual(
            "Ocorreu um erro no banco de dados. Por favor entrar em contato com o administrador do sistema.",
            mensagem);
    }

    private static ExceptionContext CriarContexto(Exception excecao)
    {
        var actionContext = new ActionContext(
            new DefaultHttpContext(),
            new RouteData(),
            new ActionDescriptor());

        return new ExceptionContext(actionContext, new List<IFilterMetadata>())
        {
            Exception = excecao
        };
    }
}
