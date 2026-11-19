using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;

namespace Util
{
    /// <summary>
    /// Validação customizada para CNPJ
    /// </summary>
    public class CnpjAttribute : ValidationAttribute
    {
        /// <summary>
        /// Construtor da classe CnpjAttribute, define a mensagem de erro padrão para validação de CNPJ inválido.
        /// </summary>
        public CnpjAttribute()
        {
            ErrorMessage = "CNPJ inválido";
        }

        public override bool IsValid(object? value)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
                return true;
            // Remove caracteres não numéricos do valor do CNPJ
            var digitos = Regex.Replace(value.ToString(), @"[^\d]", "");

            //O methods.validarCnpj(digitos) é chamado para validar o CNPJ, e o resultado é retornado.
            if (!Regex.IsMatch(digitos, "^[0-9]+$"))
                return false;

            // 000000000000, 111111111111... passam na conta dos dígitos, mas não são válidos
            if (digitos.Distinct().Count() == 1)
                return false;

            return Methods.ValidarCnpj(digitos);
        }

    }
}
