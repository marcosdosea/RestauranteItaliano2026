using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using Util;

namespace RestauranteWeb.Models
{
    public class FuncionarioViewModel
    {
        [Display(Name = "Código")]
        [BindNever]
        public uint? Id { get; set; }

        [Display(Name = "Nome")]
        [Required(ErrorMessage = "Informe o nome do funcionário.")]
        [StringLength(45, MinimumLength = 3,
            ErrorMessage = "O nome deve ter entre {2} e {1} caracteres.")]

        [BindNever]
        [ValidateNever]
        public string? NomeRestaurante { get; set; }

        [BindNever]
        [ValidateNever]
        public string? NomeTipoFuncionario { get; set; }

        public string Nome { get; set; } = string.Empty;

        [Display(Name = "CPF")]
        [Required(ErrorMessage = "Informe o CPF.")]
        [RegularExpression(@"^\d{11}$",
            ErrorMessage = "Digite o CPF com 11 números, sem pontos ou traço.")]
        [CPF(ErrorMessage = "CPF inválido.")]
        public string Cpf { get; set; } = string.Empty;

        [Display(Name = "CEP")]
        [RegularExpression(@"^\d{8}$",
            ErrorMessage = "Digite o CEP com 8 números, sem hífen.")]
        [Cep(ErrorMessage = "CEP inválido.")]
        public string? Cep { get; set; }

        [Display(Name = "Rua")]
        [StringLength(45,
            ErrorMessage = "A rua deve ter no máximo 45 caracteres.")]
        public string? Rua { get; set; }

        [Display(Name = "Bairro")]
        [StringLength(45,
            ErrorMessage = "O bairro deve ter no máximo 45 caracteres.")]
        public string? Bairro { get; set; }

        [Display(Name = "Cidade")]
        [StringLength(45,
            ErrorMessage = "A cidade deve ter no máximo 45 caracteres.")]
        public string? Cidade { get; set; }

        [Display(Name = "Estado")]
        [RegularExpression(@"^[A-Za-z]{2}$",
            ErrorMessage = "Informe a sigla do estado com duas letras, como SE.")]
        public string? Estado { get; set; }

        [Display(Name = "Telefone fixo")]
        [RegularExpression(@"^\d{10}$",
            ErrorMessage = "Digite o telefone fixo com DDD e 8 números, sem símbolos.")]
        [TelefoneFixo(ErrorMessage = "Telefone fixo inválido.")]
        public string? Telefone1 { get; set; }

        [Display(Name = "Celular")]
        [RegularExpression(@"^\d{11}$",
            ErrorMessage = "Digite o celular com DDD e 9 números, sem símbolos.")]
        [TelefoneCelular(ErrorMessage = "Celular inválido.")]
        public string? Telefone2 { get; set; }

        [Display(Name = "Restaurante")]
        [Required(ErrorMessage = "Selecione um restaurante.")]
        [Range(typeof(uint), "1", "4294967295",
            ErrorMessage = "Selecione um restaurante válido.")]
        public uint? IdRestaurante { get; set; }

        [Display(Name = "Tipo de funcionário")]
        [Required(ErrorMessage = "Selecione um tipo de funcionário.")]
        [Range(typeof(uint), "1", "4294967295",
            ErrorMessage = "Selecione um tipo de funcionário válido.")]
        public uint? IdTipoFuncionario { get; set; }

        [BindNever]
        [ValidateNever]
        public SelectList? Restaurantes { get; set; }

        [BindNever]
        [ValidateNever]
        public SelectList? TiposFuncionario { get; set; }
    }
}