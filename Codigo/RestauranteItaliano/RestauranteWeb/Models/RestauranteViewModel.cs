using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Util;

namespace RestauranteWeb.Models
{
    public class RestauranteViewModel
    {
        [Display(Name = "Código")]
        public uint? Id { get; set; }

        [Display(Name = "Nome")]
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [StringLength(45, MinimumLength = 3, ErrorMessage = "O campo {0} deve ter entre {2} e {1} caracteres.")]
        public string Nome { get; set; } = null!;

        [Display(Name = "CNPJ")]
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [StringLength(18,  ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
        [RegularExpression(@"^[0-9./\- ]*$", ErrorMessage = "O campo {0} deve conter apenas números, pontos, barra, e hífen.")]
        [Cnpj]
        public string Cnpj { get; set; } = null!;

        [Display(Name = "Tipo de Restaurante")]
        [Required(ErrorMessage = "Selecione o {0}")]
        public uint? IdTipoRestaurante { get; set; }

        [Display(Name = "Tipo de Restaurante")]
        public string? NomeTipoRestaurante { get; set; }

        [Display(Name = "CEP")]
        [RegularExpression(@"^\d{5}-?\d{3}$", ErrorMessage = "O campo {0} deve ter 8 números (ex.: 49000-000).")]
        [Cep(ErrorMessage = "CEP inválido.")]
        public string? Cep { get; set; }

        [Display(Name = "Rua")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "O campo {0} deve ter entre {2} e {1} caracteres.")]
        public string? Rua { get; set; }

        [Display(Name = "Bairro")]
        [StringLength(45, MinimumLength = 3, ErrorMessage = "O campo {0} deve ter entre {2} e {1} caracteres.")]
        public string? Bairro { get; set; }

        [Display(Name = "Cidade")]
        [StringLength(45, MinimumLength = 3, ErrorMessage = "O campo {0} deve ter entre {2} e {1} caracteres.")]
        public string? Cidade { get; set; }

        [Display(Name = "Estado")]
        [RegularExpression(@"^[A-Za-z]{2}$", ErrorMessage = "O campo {0} deve ter 2 letras (ex.: SE).")]
        public string? Estado { get; set; }

        [Display(Name = "Telefone fixo")]
        [RegularExpression(@"^[0-9()\- ]*$", ErrorMessage = "O campo {0} aceita apenas números, parênteses e hífen.")]
        [TelefoneFixo(ErrorMessage = "O campo {0} deve ter DDD e 8 números")]
        public string? Telefone1 { get; set; }

        [Display(Name = "Celular")]
        [RegularExpression(@"^[0-9()\- ]*$", ErrorMessage = "O campo {0} aceita apenas números, parênteses e hífen.")]
        [TelefoneCelular(ErrorMessage = "O campo {0} deve ter DDD e 9 números")]
        public string? Telefone2 { get; set; }

        [BindNever]
        [ValidateNever]
        public SelectList? TiposRestaurante { get; set; }
    }
}
