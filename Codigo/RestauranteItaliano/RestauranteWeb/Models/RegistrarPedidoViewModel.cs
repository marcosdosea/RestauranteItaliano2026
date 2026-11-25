using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RestauranteWeb.Models
{
    public class RegistrarPedidoViewModel
    {
        public uint IdAtendimento { get; set; }
        public uint IdRestaurante { get; set; }

        [Required(ErrorMessage = "Selecione um garçom.")]
        [Range(typeof(uint), "1", "4294967295",
            ErrorMessage = "Selecione um garçom válido.")]
        public uint? IdGarcom { get; set; }

        public SelectList? Garcons { get; set; }

        public List<ItemPedidoViewModel> Itens { get; set; } = [];
    }

    public class ItemPedidoViewModel
    {
        public uint IdItemCardapio { get; set; }
        public string Nome { get; set; } = string.Empty;
        public decimal? Preco { get; set; }
        public decimal Quantidade { get; set; }
    }
}