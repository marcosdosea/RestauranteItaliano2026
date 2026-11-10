"use strict";

const reais = new Intl.NumberFormat("pt-BR", {
  style: "currency",
  currency: "BRL",
});

/* ---------- Utilidades ---------- */

// Compara textos sem diferenciar maiúsculas nem acentos ("Tiramisu" acha "Tiramisù").
const normalizar = (texto) =>
  texto
    .normalize("NFD")
    .replace(/[̀-ͯ]/g, "")
    .toLowerCase()
    .trim();

/* ---------- Aviso de sucesso ---------- */

const aviso = document.querySelector(".aviso");

if (aviso) {
  const fecharAviso = () => {
    aviso.classList.add("aviso--saindo");
    setTimeout(() => aviso.remove(), 400);
  };

  aviso.querySelector("[data-fechar-aviso]")?.addEventListener("click", fecharAviso);

  // some sozinho, mas espera se o mouse ou o teclado estiver em cima dele
  let tempo = setTimeout(fecharAviso, 6000);
  aviso.addEventListener("mouseenter", () => clearTimeout(tempo));
  aviso.addEventListener("focusin", () => clearTimeout(tempo));
  aviso.addEventListener("mouseleave", () => {
    tempo = setTimeout(fecharAviso, 3000);
  });
}

/* ---------- Cardápio: busca e filtro por grupo ---------- */

const carta = document.querySelector("[data-carta]");

if (carta) {
  const campoBusca = document.querySelector("[data-busca]");
  const botoesFiltro = [...document.querySelectorAll("[data-filtro]")];
  const grupos = [...carta.querySelectorAll("[data-grupo]")];
  const semResultado = document.querySelector("[data-sem-resultado]");
  const contagem = document.querySelector("[data-contagem]");
  const total = carta.querySelectorAll("[data-prato]").length;
  let grupoEscolhido = "";

  const aplicarFiltros = () => {
    const termo = normalizar(campoBusca?.value ?? "");
    let visiveis = 0;

    grupos.forEach((grupo) => {
      const doGrupo = !grupoEscolhido || grupo.dataset.grupo === grupoEscolhido;
      let visiveisNoGrupo = 0;

      grupo.querySelectorAll("[data-prato]").forEach((prato) => {
        const mostrar = doGrupo && normalizar(prato.dataset.prato).includes(termo);
        prato.hidden = !mostrar;
        if (mostrar) visiveisNoGrupo++;
      });

      grupo.hidden = visiveisNoGrupo === 0;
      visiveis += visiveisNoGrupo;
    });

    if (semResultado) semResultado.hidden = visiveis > 0;
    if (contagem) contagem.textContent = `Mostrando ${visiveis} de ${total} ${total === 1 ? "item" : "itens"}`;
  };

  botoesFiltro.forEach((botao) => {
    botao.addEventListener("click", () => {
      grupoEscolhido = botao.dataset.filtro;
      botoesFiltro.forEach((outro) => outro.setAttribute("aria-pressed", String(outro === botao)));
      aplicarFiltros();
    });
  });

  campoBusca?.addEventListener("input", aplicarFiltros);

  document.querySelector("[data-limpar-filtros]")?.addEventListener("click", () => {
    if (campoBusca) campoBusca.value = "";
    botoesFiltro[0]?.click();
    campoBusca?.focus();
  });

  // atalho "/" leva para a busca, como no ItaPets
  document.addEventListener("keydown", (evento) => {
    const digitando = evento.target.closest("input, textarea, select");
    if (evento.key === "/" && !digitando && campoBusca) {
      evento.preventDefault();
      campoBusca.focus();
    }
  });
}

/* ---------- Formulário do item: prévia e contador ---------- */

const previa = document.querySelector("[data-previa]");

if (previa) {
  const formulario = previa.closest("form") ?? document.querySelector("[data-formulario-item]");
  const campo = (nome) => formulario.querySelector(`[name="${nome}"]`);
  const ver = (nome) => previa.querySelector(`[data-previa-${nome}]`);

  const nome = campo("Nome");
  const grupo = campo("IdGrupoCardapio");
  const preco = campo("Preco");
  const detalhes = campo("Detalhes");
  const ativo = formulario.querySelector('input[type="checkbox"][name="Ativo"]');
  const disponivel = formulario.querySelector('input[type="checkbox"][name="Disponivel"]');
  const contador = formulario.querySelector("[data-contador]");

  const paraNumero = (valor) => Number(String(valor).replace(/\./g, "").replace(",", "."));

  const atualizarPrevia = () => {
    const nomeGrupo = grupo?.selectedIndex > 0 ? grupo.options[grupo.selectedIndex].text : "Grupo";
    const valor = paraNumero(preco?.value ?? "");
    const apagado = !ativo?.checked || !disponivel?.checked;

    ver("grupo").textContent = nomeGrupo;
    ver("nome").textContent = nome?.value.trim() || "Nome do item";
    ver("detalhes").textContent = detalhes?.value.trim() || "detalhes do item";
    ver("preco").textContent = preco?.value.trim() && !Number.isNaN(valor) ? reais.format(valor) : "R$ —";
    ver("selo").hidden = !(ativo?.checked && !disponivel?.checked);
    ver("item").classList.toggle("prato--em-falta", apagado);

    if (!ativo?.checked) {
      ver("nota").textContent = "Item inativo: fica guardado, mas não aparece no cardápio.";
    } else if (!disponivel?.checked) {
      ver("nota").textContent = "Aparece no cardápio, marcado como em falta.";
    } else {
      ver("nota").textContent = "É assim que o item aparece na lista e no cardápio.";
    }

    if (contador && detalhes) {
      contador.textContent = `${detalhes.value.length} de ${detalhes.maxLength > 0 ? detalhes.maxLength : 500}`;
    }
  };

  formulario.addEventListener("input", atualizarPrevia);
  formulario.addEventListener("change", atualizarPrevia);
  atualizarPrevia();
}

/* ---------- Botão de salvar: evita clique duplo ---------- */

document.querySelectorAll("form[data-travar-envio]").forEach((formulario) => {
  formulario.addEventListener("submit", () => {
    // o jQuery Validation cancela o envio quando há erro; só trava se for mesmo enviar
    setTimeout(() => {
      const valido = !window.jQuery || !jQuery(formulario).valid || jQuery(formulario).valid();
      if (!valido) return;
      formulario.querySelectorAll('button[type="submit"]').forEach((botao) => {
        botao.disabled = true;
        botao.setAttribute("aria-busy", "true");
      });
    }, 0);
  });
});
