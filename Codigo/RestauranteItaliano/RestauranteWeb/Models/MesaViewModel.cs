using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RestauranteWeb.Models
{
    public class MesaViewModel
    {
        [Display(Name = "Código")]
        public int? Id { get; set; }

        [Display(Name = "Identificação")]
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [StringLength(45, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
        public string Identificacao { get; set; } = null!;

        [Display(Name = "Restaurante")]
        [Required(ErrorMessage = "Selecione o {0}.")]
        public uint? IdRestaurante { get; set; }

        [Display(Name = "Restaurante")]
        public string? NomeRestaurante { get; set; }

        [BindNever]
        [ValidateNever]
        public SelectList? Restaurantes { get; set; }
    }
}
