"use strict";

/* ---------- Avisos de sucesso e de erro ---------- */

// também vale para avisos que chegam depois (como o do interruptor dos itens, em js/itens-cardapio.js)
const prepararAviso = (aviso) => {
  const fecharAviso = () => {
    aviso.classList.add("aviso--saindo");
    setTimeout(() => aviso.remove(), 300);
  };

  aviso.querySelector("[data-fechar-aviso]")?.addEventListener("click", fecharAviso);

  // o aviso de erro só sai quando a pessoa fecha
  if (aviso.classList.contains("aviso--erro")) return;

  // o de sucesso some sozinho, mas espera se o mouse ou o teclado estiver em cima dele
  let tempo = setTimeout(fecharAviso, 6000);
  aviso.addEventListener("mouseenter", () => clearTimeout(tempo));
  aviso.addEventListener("focusin", () => clearTimeout(tempo));
  aviso.addEventListener("mouseleave", () => {
    tempo = setTimeout(fecharAviso, 3000);
  });
};

document.querySelectorAll(".aviso").forEach(prepararAviso);
window.prepararAviso = prepararAviso;

/* ---------- Formulário do item: prévia e contador ---------- */

const previa = document.querySelector("[data-previa]");

if (previa) {
  const formulario = previa.closest("form");
  const reais = new Intl.NumberFormat("pt-BR", { style: "currency", currency: "BRL" });
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
    const gravura = ver("gravura");
    if (gravura && window.gravuraDoGrupo) {
      const novaGravura = window.gravuraDoGrupo(grupo?.selectedIndex > 0 ? nomeGrupo : "", previa);
      if (!gravura.src.endsWith(novaGravura)) gravura.src = novaGravura;
    }
    ver("nome").textContent = nome?.value.trim() || "Nome do item";
    ver("detalhes").textContent = detalhes?.value.trim() || "detalhes do item";
    ver("preco").textContent = preco?.value.trim() && !Number.isNaN(valor) ? reais.format(valor) : "R$ —";
    ver("selo").hidden = !(ativo?.checked && !disponivel?.checked);
    ver("item").classList.toggle("linha-carta--em-falta", apagado);

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

/* ---------- Cadastros simples (grupos e tipos): prévia e contador ---------- */

// iniciais do crachá, pela mesma regra do servidor (Models/Monograma.cs)
const ligacoes = ["de", "da", "do", "das", "dos", "e"];
const monograma = (nome) => {
  const palavras = nome.split(/\s+/).filter((palavra) => palavra && !ligacoes.includes(palavra.toLowerCase()));
  if (palavras.length === 0) return "?";
  const iniciais = palavras.length === 1 ? palavras[0][0] : palavras[0][0] + palavras[palavras.length - 1][0];
  return iniciais.toUpperCase();
};

document.querySelectorAll("[data-previa-cadastro]").forEach((previaCadastro) => {
  const formulario = previaCadastro.closest("form");
  const campoNome = formulario.querySelector('[name="Nome"]');
  const nome = previaCadastro.querySelector("[data-previa-nome]");
  const iniciais = previaCadastro.querySelector("[data-previa-iniciais]");
  const gravura = previaCadastro.querySelector("[data-previa-gravura]");
  const contador = formulario.querySelector("[data-contador]");

  const atualizar = () => {
    const texto = campoNome.value.trim();
    nome.textContent = texto || nome.dataset.previaVazio;
    if (iniciais) iniciais.textContent = monograma(texto);
    // nos grupos a gravura troca conforme o nome, pela mesma regra do servidor (Models/Gravuras.cs)
    if (gravura && window.gravuraDoGrupo) {
      const novaGravura = window.gravuraDoGrupo(texto, previaCadastro);
      if (!gravura.src.endsWith(novaGravura)) gravura.src = novaGravura;
    }
    if (contador) contador.textContent = `${campoNome.value.length} de ${campoNome.maxLength}`;
  };

  campoNome.addEventListener("input", atualizar);
  atualizar();
});

/* ---------- Busca no livro, no quadro e na rua: filtra os cadastros já carregados ---------- */

const semAcento = (texto) => texto.normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase();

document.querySelectorAll("[data-busca-lista]").forEach((campoBusca) => {
  // o gancho livre do quadro de crachás não é um cadastro e fica sempre à vista
  const linhas = [...document.getElementById(campoBusca.dataset.buscaLista).children].filter((linha) => linha.querySelector("[data-busca-nome]"));
  const semResultado = document.querySelector("[data-sem-resultado]");
  const resumo = document.querySelector("[data-resumo-lista]");
  const resumoOriginal = resumo?.textContent;

  campoBusca.addEventListener("input", () => {
    const busca = semAcento(campoBusca.value.trim());
    let visiveis = 0;
    linhas.forEach((linha) => {
      const aparece = semAcento(linha.querySelector("[data-busca-nome]").textContent).includes(busca);
      linha.hidden = !aparece;
      if (aparece) visiveis++;
    });
    if (semResultado) semResultado.hidden = visiveis > 0;
    if (resumo) resumo.textContent = busca ? `${visiveis} de ${linhas.length} ${resumo.dataset.plural}` : resumoOriginal;
  });
});

/* ---------- Livro do cardápio (grupos): capa, fita marcadora e a página de cada capítulo com os pratos ---------- */

const livro = document.querySelector("[data-livro]");

if (livro) {
  const folha = livro.querySelector("[data-livro-folha]");
  const parte = (nome) => folha.querySelector(`[data-folha-${nome}]`);
  const entradas = [...livro.querySelectorAll(".livro__entrada")];
  const semGiro = window.matchMedia("(prefers-reduced-motion: reduce)").matches;
  const reais = new Intl.NumberFormat("pt-BR", { style: "currency", currency: "BRL" });
  const pratosDoGrupo = new Map();
  let atual = livro.querySelector(".livro__entrada--atual");
  let intencao = null;

  const romano = (numero) => {
    let texto = "";
    for (const [valor, letras] of [[50, "l"], [40, "xl"], [10, "x"], [9, "ix"], [5, "v"], [4, "iv"], [1, "i"]]) {
      while (numero >= valor) {
        texto += letras;
        numero -= valor;
      }
    }
    return texto;
  };

  const criar = (tag, classe, texto) => {
    const elemento = document.createElement(tag);
    if (classe) elemento.className = classe;
    if (texto !== undefined) elemento.textContent = texto;
    return elemento;
  };

  // pratos ativos do grupo, pela mesma consulta da tabela dos itens (filtro por grupo na coluna 1); cada grupo é buscado uma vez
  // e guardado pronto, para a página trocar sem "Carregando" piscando no meio
  const pratosProntos = new Map();
  const buscarPratos = (idGrupo) => {
    if (!pratosDoGrupo.has(idGrupo)) {
      const corpo = new URLSearchParams({
        draw: "1", start: "0", length: "100",
        "columns[0][data]": "nome", "columns[1][data]": "nomeGrupoCardapio", "columns[1][search]": idGrupo,
        "order[0][column]": "0", "order[0][dir]": "asc",
      });
      pratosDoGrupo.set(idGrupo, fetch(livro.dataset.urlItens, { method: "POST", body: corpo })
        .then((resposta) => (resposta.ok ? resposta.json() : Promise.reject(resposta.status)))
        .then((pagina) => pagina.data.filter((item) => item.ativo))
        .catch(() => null)
        .then((pratos) => {
          pratosProntos.set(idGrupo, pratos);
          return pratos;
        }));
    }
    return pratosDoGrupo.get(idGrupo);
  };

  const visiveis = () => entradas.filter((entrada) => !entrada.closest("li").hidden);

  // a fita desce até o fim da entrada aberta no sumário
  const moverFita = () => {
    const topoLivro = livro.getBoundingClientRect().top;
    livro.style.setProperty("--fita-altura", `${Math.round(atual.getBoundingClientRect().bottom - topoLivro + 6)}px`);
  };

  const mostrarPratos = (pratos) => {
    const lista = parte("pratos");
    lista.replaceChildren();
    if (pratos === null) {
      lista.append(criar("li", "folha__prato folha__prato--aviso", "Não foi possível carregar os pratos deste grupo."));
      return;
    }
    if (pratos.length === 0) {
      lista.append(criar("li", "folha__prato folha__prato--aviso", "Nenhum prato neste grupo ainda."));
      return;
    }
    pratos.forEach((prato) => {
      const linha = criar("li", prato.disponivel ? "folha__prato" : "folha__prato folha__prato--em-falta");
      const texto = criar("span", "folha__prato-texto");
      texto.append(criar("span", "folha__prato-nome", prato.nome));
      if (prato.detalhes) texto.append(criar("span", "folha__prato-detalhe", prato.detalhes));
      if (!prato.disponivel) texto.append(criar("span", "selo", "Em falta"));
      const pontos = criar("span", "u-pontos");
      pontos.setAttribute("aria-hidden", "true");
      const preco = criar("data", "folha__prato-preco", prato.preco === null ? "—" : reais.format(prato.preco));
      if (prato.preco !== null) preco.value = Number(prato.preco).toFixed(2);
      linha.append(texto, pontos, preco);
      lista.append(linha);
    });
  };

  // preenche a folha com o capítulo da entrada
  const preencher = (entrada) => {
    const dados = entrada.dataset;
    const ordem = visiveis();
    const posicao = ordem.indexOf(entrada);
    parte("capitulo").textContent = `capítulo ${romano(posicao + 1)}`;
    parte("gravura").src = dados.gravura;
    parte("nome").textContent = dados.nome;
    parte("pagina").textContent = `pág. ${posicao + 1} de ${ordem.length}`;
    parte("anterior").disabled = posicao <= 0;
    parte("proxima").disabled = posicao >= ordem.length - 1;
    for (const [acao, endereco, rotulo] of [["abrir", entrada.href, "Abrir"], ["editar", dados.editar, "Editar"], ["excluir", dados.excluir, "Excluir"]]) {
      parte(acao).href = endereco;
      parte(acao).setAttribute("aria-label", `${rotulo} ${dados.nome}`);
    }
    if (pratosProntos.has(dados.codigo)) {
      mostrarPratos(pratosProntos.get(dados.codigo));
      return;
    }
    parte("pratos").replaceChildren(criar("li", "folha__prato folha__prato--aviso", "Carregando os pratos…"));
    buscarPratos(dados.codigo).then((pratos) => {
      if (entrada === exibida) mostrarPratos(pratos);
    });
  };

  /* A troca de página é um movimento só: o conteúdo sai deslizando de leve para o lado da lombada
     e o novo entra do outro lado (160 + 300 ms: o --tempo-curto e o --tempo-medio do CSS, com a mesma --curva).
     Se o capítulo mudar no meio, a troca termina e segue para o último escolhido, sem cortes. */
  let exibida = atual;
  let trocando = false;
  const SAIR = { duration: 160, easing: "cubic-bezier(0.4, 0, 1, 1)", fill: "forwards" };
  const ENTRAR = { duration: 300, easing: "cubic-bezier(0.22, 1, 0.36, 1)", fill: "forwards" };

  const trocarPagina = async () => {
    if (trocando) return;
    trocando = true;
    while (exibida !== atual) {
      const alvo = atual;
      const ordem = visiveis();
      const sentido = ordem.indexOf(alvo) < ordem.indexOf(exibida) ? -1 : 1;
      if (!semGiro) {
        await folha.animate([{ opacity: 1, transform: "translateX(0)" }, { opacity: 0, transform: `translateX(${-10 * sentido}px)` }], SAIR).finished;
      }
      exibida = alvo;
      preencher(alvo);
      if (!semGiro) {
        await folha.animate([{ opacity: 0, transform: `translateX(${14 * sentido}px)` }, { opacity: 1, transform: "translateX(0)" }], ENTRAR).finished;
      }
    }
    folha.getAnimations().forEach((animacao) => animacao.cancel());
    trocando = false;
  };

  const mostrar = (entrada) => {
    if (!entrada || entrada === atual) return;
    atual?.classList.remove("livro__entrada--atual");
    entrada.classList.add("livro__entrada--atual");
    atual = entrada;
    moverFita();
    trocarPagina();
  };

  const virar = (passo) => {
    const ordem = visiveis();
    mostrar(ordem[ordem.indexOf(atual) + passo]);
  };

  // com o mouse, o livro só folheia quando ele para numa entrada, e não a cada linha que atravessa
  entradas.forEach((entrada) => {
    entrada.addEventListener("pointerenter", () => {
      clearTimeout(intencao);
      intencao = setTimeout(() => mostrar(entrada), 120);
    });
    entrada.addEventListener("pointerleave", () => clearTimeout(intencao));
    entrada.addEventListener("focus", () => mostrar(entrada));
  });
  parte("anterior").addEventListener("click", () => virar(-1));
  parte("proxima").addEventListener("click", () => virar(1));

  // setas do teclado folheiam o livro quando o foco está nele
  livro.addEventListener("keydown", (evento) => {
    if (evento.key === "ArrowRight") virar(1);
    else if (evento.key === "ArrowLeft") virar(-1);
    else return;
    evento.preventDefault();
  });

  // contagem de pratos de cada capítulo no sumário
  entradas.forEach((entrada) => {
    buscarPratos(entrada.dataset.codigo).then((pratos) => {
      if (pratos === null) return;
      entrada.querySelector("[data-livro-quantidade]").textContent = `${pratos.length} ${pratos.length === 1 ? "prato" : "pratos"}`;
    });
  });

  preencher(atual);
  moverFita();
  window.addEventListener("resize", moverFita);
  // quando a busca esconde capítulos, a fita, a página e as setas acompanham
  livro.closest("main")?.querySelector("[data-busca-lista]")?.addEventListener("input", () => {
    const ordem = visiveis();
    if (ordem.length === 0) return;
    if (!ordem.includes(atual)) {
      mostrar(ordem[0]);
      return;
    }
    moverFita();
    if (!trocando) {
      exibida = atual;
      preencher(atual);
    }
  });

  // a capa abre sozinha só na primeira visita da sessão, e só em telas largas
  const capa = livro.querySelector("[data-livro-capa]");
  if (capa && !semGiro && window.matchMedia("(min-width: 768px)").matches) {
    let jaAbriu = false;
    try {
      jaAbriu = sessionStorage.getItem("livro-aberto") === "sim";
      sessionStorage.setItem("livro-aberto", "sim");
    } catch {
      jaAbriu = false;
    }
    if (!jaAbriu) {
      capa.setAttribute("aria-hidden", "true");
      capa.hidden = false;
      capa.addEventListener("transitionend", (evento) => {
        if (evento.target === capa && evento.propertyName === "transform") capa.hidden = true;
      });
      setTimeout(() => capa.classList.add("livro__capa--aberta"), 500);
    }
  }
}

/* ---------- Crachás do quadro: balançam presos ao gancho quando o mouse chega ----------
   O balanço roda inteiro, do começo ao fim, mesmo que o mouse saia no meio (no CSS ele voltaria seco). */

if (window.matchMedia("(hover: hover) and (pointer: fine)").matches && !window.matchMedia("(prefers-reduced-motion: reduce)").matches) {
  document.querySelectorAll(".quadro .cracha").forEach((cracha) => {
    cracha.addEventListener("pointerenter", () => {
      if (cracha.getAnimations().some((animacao) => animacao.id === "balanco")) return;
      cracha.animate(
        [{ rotate: "0deg" }, { rotate: "2.2deg" }, { rotate: "-1.5deg" }, { rotate: "0.7deg" }, { rotate: "-0.25deg" }, { rotate: "0deg" }],
        { duration: 1400, easing: "cubic-bezier(0.65, 0, 0.35, 1)", id: "balanco" },
      );
    });
  });
}

/* ---------- Carimbo "Novo": marca na lista o cadastro que acabou de ser criado ---------- */

const CHAVE_NOVO = "cadastro-novo";

document.querySelectorAll("form[data-lembrar-novo]").forEach((formulario) => {
  formulario.addEventListener("submit", () => {
    try {
      const nome = formulario.querySelector('[name="Nome"]').value.trim();
      sessionStorage.setItem(CHAVE_NOVO, JSON.stringify({ lista: formulario.dataset.lembrarNovo, nome }));
    } catch {
      // sem sessionStorage (modo privado, por exemplo): o cadastro segue sem o carimbo
    }
  });
});

const listaCadastro = document.querySelector("[data-lista-cadastro]");

if (listaCadastro) {
  let novo = null;
  try {
    novo = JSON.parse(sessionStorage.getItem(CHAVE_NOVO));
    sessionStorage.removeItem(CHAVE_NOVO);
  } catch {
    novo = null;
  }

  if (novo?.lista === listaCadastro.dataset.listaCadastro) {
    const nome = [...listaCadastro.querySelectorAll("[data-busca-nome]")].find((elemento) => elemento.textContent.trim() === novo.nome);
    const alvo = nome?.closest("[data-carimbo]");
    if (alvo) {
      const carimbo = document.createElement("span");
      carimbo.className = "carimbo";
      carimbo.setAttribute("aria-hidden", "true");
      carimbo.textContent = "Novo";
      alvo.append(carimbo);
      // no livro, a folha já abre no grupo novo
      if (alvo.classList.contains("livro__entrada")) alvo.dispatchEvent(new Event("pointerenter"));
    }
  }
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
