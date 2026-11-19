using Core.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

namespace RestauranteAPI.Filter.Tests;

[TestClass]
public class HttpResponseExceptionFilterTests
{
    private HttpResponseExceptionFilter filtro = null!;

    [TestInitialize]
    public void Inicializar()
    {
        filtro = new HttpResponseExceptionFilter();
    }

    [TestMethod]
    public void OnActionExecuted_QuandoServiceException_DeveRetornarBadRequestComAMensagem()
    {
        // Arrange
        var contexto = CriarContexto(new ServiceException("Já existe uma forma de pagamento com o mesmo nome."));

        // Act
        filtro.OnActionExecuted(contexto);

        // Assert
        Assert.IsTrue(contexto.ExceptionHandled);
        var resultado = Assert.IsInstanceOfType<ObjectResult>(contexto.Result);
        Assert.AreEqual(StatusCodes.Status400BadRequest, resultado.StatusCode);
        Assert.AreEqual("Já existe uma forma de pagamento com o mesmo nome.", resultado.Value);
    }

    [TestMethod]
    public void OnActionExecuted_QuandoHttpResponseException_DeveUsarOStatusDaExcecao()
    {
        // Arrange
        var contexto = CriarContexto(new HttpResponseException(StatusCodes.Status409Conflict, "Conflito."));

        // Act
        filtro.OnActionExecuted(contexto);

        // Assert
        Assert.IsTrue(contexto.ExceptionHandled);
        var resultado = Assert.IsInstanceOfType<ObjectResult>(contexto.Result);
        Assert.AreEqual(StatusCodes.Status409Conflict, resultado.StatusCode);
        Assert.AreEqual("Conflito.", resultado.Value);
    }

    [TestMethod]
    public void OnActionExecuted_QuandoErroInesperado_NaoDeveTratar()
    {
        // Arrange
        var contexto = CriarContexto(new InvalidOperationException("detalhe interno"));

        // Act
        filtro.OnActionExecuted(contexto);

        // Assert
        Assert.IsFalse(contexto.ExceptionHandled);
        Assert.IsNull(contexto.Result);
    }

    [TestMethod]
    public void OnActionExecuted_QuandoNaoHaExcecao_NaoDeveAlterarAResposta()
    {
        // Arrange
        var contexto = CriarContexto(null);

        // Act
        filtro.OnActionExecuted(contexto);

        // Assert
        Assert.IsFalse(contexto.ExceptionHandled);
        Assert.IsNull(contexto.Result);
    }

    private static ActionExecutedContext CriarContexto(Exception? excecao)
    {
        var actionContext = new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor());
        return new ActionExecutedContext(actionContext, new List<IFilterMetadata>(), controller: new object())
        {
            Exception = excecao,
        };
    }
}
