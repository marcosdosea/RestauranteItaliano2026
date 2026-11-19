using System.ComponentModel.DataAnnotations;

namespace RestauranteWeb.Models
{
    public class TipoRestauranteModel
    {
        [Display(Name = "Id")]
        public uint? Id { get; set; }

        [Display(Name = "Nome")]
        [Required(ErrorMessage = "O campo {0} é de preenchimento obrigatório.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "O {0} deve ter pelo menos {2} caracteres.")]
        public string Nome { get; set; } = null!;
    }
}
