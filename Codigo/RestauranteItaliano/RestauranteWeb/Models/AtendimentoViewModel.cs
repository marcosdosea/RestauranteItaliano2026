using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RestauranteWeb.Models;

public class AtendimentoViewModel
{
    [Required(ErrorMessage = "Selecione um restaurante.")]
    public uint? IdRestaurante { get; set; }

    public SelectList? Restaurantes { get; set; }

    public List<MesaAtendimentoViewModel> Mesas { get; set; } = [];
}

public class MesaAtendimentoViewModel
{
    public int IdMesa { get; set; }
    public string Identificacao { get; set; } = "";
    public bool Ocupada { get; set; }
    public DateTime? InicioAtendimento { get; set; }
}