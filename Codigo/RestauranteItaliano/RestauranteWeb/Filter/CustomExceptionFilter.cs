using Core.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using MySql.Data.MySqlClient;

namespace RestauranteWeb.Filter
{
    /// <summary>
    /// Trata os erros que escapam dos controllers e mostra a tela de erro com uma
    /// mensagem compreensível, no lugar da página genérica. Mesmo modelo do projeto
    /// Biblioteca do professor.
    /// </summary>
    public class CustomExceptionFilter : IExceptionFilter
    {
        private readonly IModelMetadataProvider modelMetadataProvider;

        public CustomExceptionFilter(IModelMetadataProvider modelMetadataProvider)
        {
            this.modelMetadataProvider = modelMetadataProvider;
        }

        public void OnException(ExceptionContext context)
        {
            var exception = context.Exception;
            var result = new ViewResult { ViewName = "Error" };
            result.ViewData = new ViewDataDictionary(modelMetadataProvider, context.ModelState);

            result.ViewData["ErrorMessage"] = ObterMensagem(exception);
            result.ViewData["Exception"] = exception;

            // marca a exceção como tratada
            context.ExceptionHandled = true;
            context.Result = result;
        }

        /// <summary>
        /// Escolhe a mensagem mostrada ao usuário para a exceção
        /// </summary>
        /// <param name="exception">A exceção lançada</param>
        /// <returns>A mensagem em português</returns>
        public static string ObterMensagem(Exception exception)
        {
            if (exception is ServiceException)
            {
                return exception.Message;
            }

            // o Entity Framework embrulha o erro do MySQL em uma DbUpdateException:
            // procura a MySqlException entre as exceções internas
            var mySqlException = ProcurarMySqlException(exception);
            if (mySqlException != null)
            {
                return ObterMensagemBancoDeDados(mySqlException.Number);
            }

            return "Ocorreu um erro inesperado. Por favor entrar em contato com o administrador do sistema.";
        }

        /// <summary>
        /// Traduz o número do erro do MySQL em uma mensagem para o usuário
        /// </summary>
        /// <param name="numero">Número do erro informado pelo MySQL</param>
        /// <returns>A mensagem em português</returns>
        public static string ObterMensagemBancoDeDados(int numero)
        {
            return numero switch
            {
                1451 => "Não é possível excluir este registro, pois ele está vinculado a outros dados no sistema.",
                1062 => "Violação de chave única: já existe um registro com este valor.",
                1048 => "Um campo obrigatório não foi preenchido.",
                1406 => "Um campo excedeu o tamanho máximo permitido.",
                1216 or 1217 or 1452 => "Violação de chave estrangeira: operação inválida devido a dados relacionados.",
                1366 => "Tipo de dado inválido fornecido para um campo.",
                _ => "Ocorreu um erro no banco de dados. Por favor entrar em contato com o administrador do sistema."
            };
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
