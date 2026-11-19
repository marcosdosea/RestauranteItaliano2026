using Core.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MySql.Data.MySqlClient;

namespace RestauranteAPI.Filter
{
    /// <summary>
    /// Transforma as exceções que escapam dos controllers da API em respostas HTTP com uma mensagem clara.
    /// Mesmo modelo do HttpResponseExceptionFilter da BibliotecaAPI do professor.
    /// </summary>
    public class HttpResponseExceptionFilter : IActionFilter, IOrderedFilter
    {
        public int Order { get; } = int.MaxValue - 10;

        public void OnActionExecuting(ActionExecutingContext context) { }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception == null)
            {
                return;
            }

            var resposta = ObterResposta(context.Exception);
            if (resposta != null)
            {
                context.Result = new ObjectResult(resposta.Value) { StatusCode = resposta.Status };
                context.ExceptionHandled = true;
            }
        }

        /// <summary>
        /// Escolhe o código HTTP e a mensagem para a exceção
        /// </summary>
        /// <param name="exception">A exceção lançada</param>
        /// <returns>A resposta, ou nulo quando a exceção não é tratada aqui</returns>
        public static HttpResponseException? ObterResposta(Exception exception)
        {
            if (exception is HttpResponseException httpResponseException)
            {
                return httpResponseException;
            }

            // regra de negócio violada (ex.: nome repetido, item já usado em pedidos)
            if (exception is ServiceException)
            {
                return new HttpResponseException(StatusCodes.Status400BadRequest, exception.Message);
            }

            // o Entity Framework embrulha o erro do MySQL em uma DbUpdateException
            var mySqlException = ProcurarMySqlException(exception);
            if (mySqlException != null)
            {
                return mySqlException.Number switch
                {
                    1451 => new HttpResponseException(StatusCodes.Status409Conflict, "Não é possível excluir este registro, pois ele está vinculado a outros dados no sistema."),
                    1062 => new HttpResponseException(StatusCodes.Status409Conflict, "Violação de chave única: já existe um registro com este valor."),
                    1216 or 1217 or 1452 => new HttpResponseException(StatusCodes.Status400BadRequest, "Violação de chave estrangeira: operação inválida devido a dados relacionados."),
                    _ => new HttpResponseException(StatusCodes.Status500InternalServerError, "Ocorreu um erro no banco de dados. Por favor entrar em contato com o administrador do sistema.")
                };
            }

            return null;
        }

        private static MySqlException? ProcurarMySqlException(Exception? exception)
        {
            while (exception != null)
            {
                if (exception is MySqlException mySqlException)
                {
                    return mySqlException;
                }
                exception = exception.InnerException;
            }
            return null;
        }
    }
}
