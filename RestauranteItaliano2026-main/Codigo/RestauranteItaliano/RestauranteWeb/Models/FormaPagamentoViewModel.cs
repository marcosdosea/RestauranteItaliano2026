using System.ComponentModel.DataAnnotations;

namespace RestauranteWeb.Models
{
    public class FormaPagamentoViewModel
    {
        [Display(Name = "Código")]
        public uint? Id { get; set; }

        [Display(Name = "Nome")]
        [Required(ErrorMessage = "O campo {0} é de preenchimento obrigatório.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "O campo {0} deve ter entre {2} e {1} caracteres.")]
        public string Nome { get; set; } = null!;
    }
}
