using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Util;

namespace RestauranteWeb.Models
{
    public class FuncionarioViewModel
    {
        [Display(Name = "Código")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public uint Id { get; set; }

        [Display(Name = "Nome")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [StringLength(100, MinimumLength = 3,
            ErrorMessage = "O campo {0} deve ter entre {2} e {1} caracteres")]
        public string Nome { get; set; } = null!;

        [Display(Name = "CPF")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [CPF(ErrorMessage = "O campo {0} é inválido")]
        public string Cpf { get; set; } = null!;

        [Display(Name = "CEP")]
        [Cep(ErrorMessage = "O campo {0} é inválido")]
        public string? Cep { get; set; }

        [Display(Name = "Rua")]
        [StringLength(50, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres")]
        public string? Rua { get; set; }

        [Display(Name = "Bairro")]
        [StringLength(50, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres")]
        public string? Bairro { get; set; }

        [Display(Name = "Cidade")]
        [StringLength(50, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres")]
        public string? Cidade { get; set; }

        [Display(Name = "Estado")]
        [StringLength(2, MinimumLength = 2,
            ErrorMessage = "O campo {0} deve possuir 2 caracteres")]
        public string? Estado { get; set; }

        [Display(Name = "Telefone 1")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [Phone(ErrorMessage = "Informe um telefone válido")]
        public string? Telefone1 { get; set; }

        [Display(Name = "Telefone 2")]
        [Phone(ErrorMessage = "Informe um telefone válido")]
        public string? Telefone2 { get; set; }


        [Display(Name = "Restaurante")]
        [Range(1, uint.MaxValue, ErrorMessage = "Selecione um restaurante")]
        public uint IdRestaurante { get; set; }

        [Display(Name = "Nome Tipo de Funcionário")]
        public string? NomeTipoFuncionario { get; set; }

        [Display(Name = "Id Tipo de Funcionário")]
        [Range(1, uint.MaxValue, ErrorMessage = "Selecione um tipo de funcionário")]
        public uint IdTipoFuncionario { get; set; }

        public IEnumerable<SelectListItem> TiposFuncionario { get; set; }
            = new List<SelectListItem>();

        public IEnumerable<SelectListItem> Restaurantes { get; set; }
            = new List<SelectListItem>();
    }
}