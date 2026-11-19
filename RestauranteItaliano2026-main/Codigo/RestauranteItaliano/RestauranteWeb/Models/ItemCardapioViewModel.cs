using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RestauranteWeb.Models
{
    public class ItemCardapioViewModel
    {
        [Display(Name = "Código")]
        public uint? Id { get; set; }

        [Display(Name = "Nome")]
        [Required(ErrorMessage = "O campo {0} é de preenchimento obrigatório.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "O campo {0} deve ter entre {2} e {1} caracteres.")]
        public string Nome { get; set; } = null!;

        [Display(Name = "Preço")]
        [Range(0.01, 99999999.99, ErrorMessage = "O campo {0} deve ser maior que zero.")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        public decimal? Preco { get; set; }

        [Display(Name = "Detalhes")]
        [StringLength(500, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
        [DataType(DataType.MultilineText)]
        public string? Detalhes { get; set; }

        [Display(Name = "Ativo no cardápio")]
        public bool Ativo { get; set; } = true;

        [Display(Name = "Disponível para pedido")]
        public bool Disponivel { get; set; } = true;

        [Display(Name = "Restaurante")]
        [Required(ErrorMessage = "Selecione o {0}.")]
        public uint? IdRestaurante { get; set; }

        [Display(Name = "Grupo do cardápio")]
        [Required(ErrorMessage = "Selecione o {0}.")]
        public uint? IdGrupoCardapio { get; set; }

        [Display(Name = "Restaurante")]
        public string? NomeRestaurante { get; set; }

        [Display(Name = "Grupo do cardápio")]
        public string? NomeGrupoCardapio { get; set; }

        [BindNever]
        [ValidateNever]
        public SelectList? Restaurantes { get; set; }

        [BindNever]
        [ValidateNever]
        public SelectList? GruposCardapio { get; set; }
    }
}
