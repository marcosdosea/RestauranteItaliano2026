"use strict";

/* Validação no navegador ajustada para o português do Brasil.
   Este arquivo é carregado pela _ValidationScriptsPartial, então vale para todos os formulários. */

// Mensagens padrão do jQuery Validation em português. Elas só aparecem quando
// o campo não tem mensagem própria vinda dos atributos da ViewModel
// (por exemplo, o limite do atributo maxlength do HTML).
if (window.jQuery && jQuery.validator) {
  jQuery.extend(jQuery.validator.messages, {
    required: "Este campo é de preenchimento obrigatório.",
    remote: "Corrija este campo.",
    email: "Informe um e-mail válido.",
    url: "Informe um endereço (URL) válido.",
    date: "Informe uma data válida.",
    dateISO: "Informe uma data válida (ISO).",
    number: "Informe um número válido, com vírgula nos centavos.",
    digits: "Informe somente dígitos.",
    equalTo: "Informe o mesmo valor novamente.",
    maxlength: jQuery.validator.format("Informe no máximo {0} caracteres."),
    minlength: jQuery.validator.format("Informe pelo menos {0} caracteres."),
    rangelength: jQuery.validator.format("Informe entre {0} e {1} caracteres."),
    range: jQuery.validator.format("Informe um valor entre {0} e {1}."),
    max: jQuery.validator.format("Informe um valor menor ou igual a {0}."),
    min: jQuery.validator.format("Informe um valor maior ou igual a {0}."),
    step: jQuery.validator.format("Informe um múltiplo de {0}."),
  });

  // O sistema usa a cultura pt-BR: os números são digitados com vírgula nos centavos (29,90)
  // e, se houver, ponto nos milhares (1.250,00). Sem isto, o navegador recusaria a vírgula.
  const paraNumero = (valor) => parseFloat(String(valor).replace(/\./g, "").replace(",", "."));

  jQuery.validator.methods.number = function (valor, elemento) {
    return this.optional(elemento) || /^-?\d{1,3}(\.\d{3})*(,\d+)?$|^-?\d+(,\d+)?$/.test(valor);
  };

  jQuery.validator.methods.range = function (valor, elemento, limites) {
    const numero = paraNumero(valor);
    return this.optional(elemento) || (numero >= limites[0] && numero <= limites[1]);
  };
}
