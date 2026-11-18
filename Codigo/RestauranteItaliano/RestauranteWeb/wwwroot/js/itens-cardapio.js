"use strict";

/* ---------- Itens do cardápio: tabela com DataTables ----------
   A busca, o filtro por grupo, a ordenação e a paginação são feitos no servidor
   (ItemCardapioController.GetDataPage), como no projeto Biblioteca do professor. */

const tabelaItens = document.querySelector("#tabela-itens");

if (tabelaItens && window.jQuery) {
  const campoBusca = document.querySelector("[data-busca-itens]");
  const camposGrupo = [...document.querySelectorAll("[data-grupo-itens]")];
  const grupoEscolhido = () => camposGrupo.find((campo) => campo.checked)?.value ?? "";
  const resumo = document.querySelector("[data-resumo-itens]");
  const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value ?? "";
  const reais = new Intl.NumberFormat("pt-BR", { style: "currency", currency: "BRL" });

  // endereços das ações, vindos da view (Url.Action)
  const url = {
    pagina: tabelaItens.dataset.url,
    detalhes: tabelaItens.dataset.urlDetalhes,
    editar: tabelaItens.dataset.urlEditar,
    excluir: tabelaItens.dataset.urlExcluir,
    disponibilidade: tabelaItens.dataset.urlDisponibilidade,
  };

  // os textos vêm do banco: escapa antes de montar o HTML de cada linha
  const escapar = (texto) =>
    String(texto ?? "")
      .replaceAll("&", "&amp;")
      .replaceAll("<", "&lt;")
      .replaceAll(">", "&gt;")
      .replaceAll('"', "&quot;")
      .replaceAll("'", "&#39;");

  const plural = (quantidade, singular, varios) => `${quantidade} ${quantidade === 1 ? singular : varios}`;

  // marca o trecho buscado; cada pedaço é escapado separadamente, então o <mark> é o único HTML que entra
  const realcar = (texto) => {
    const termo = campoBusca?.value.trim() ?? "";
    const original = String(texto ?? "");
    const posicao = termo ? original.toLowerCase().indexOf(termo.toLowerCase()) : -1;
    if (posicao < 0) return escapar(original);
    return escapar(original.slice(0, posicao)) +
      `<mark class="realce">${escapar(original.slice(posicao, posicao + termo.length))}</mark>` +
      escapar(original.slice(posicao + termo.length));
  };

  const colunaNome = (dados, tipo, item) => {
    const detalhes = item.detalhes
      ? `<span class="tabela-itens__detalhe">${realcar(item.detalhes)}</span>`
      : "";
    return `<a class="tabela-itens__link" href="${url.detalhes}/${item.id}">
      <span class="tabela-itens__nome">${realcar(item.nome)}</span>${detalhes}</a>`;
  };

  // grupo com a gravura dele; a gravura repete o nome, por isso é decorativa (alt vazio)
  const colunaGrupo = (grupo) =>
    `<span class="tabela-itens__grupo"><img class="tabela-itens__gravura" src="${window.gravuraDoGrupo?.(grupo, tabelaItens) ?? ""}" alt="">${escapar(grupo ?? "—")}</span>`;

  // <data>: o preço formatado para quem lê e o número puro no value
  const colunaPreco = (preco) =>
    preco === null || preco === undefined ? "—" : `<data value="${Number(preco).toFixed(2)}">${reais.format(preco)}</data>`;

  // situação: um interruptor que liga e desliga a disponibilidade (o mesmo formulário de AlternarDisponivel)
  const colunaSituacao = (dados, tipo, item) => {
    if (!item.ativo) return '<span class="selo selo--inativo">Fora do cardápio</span>';
    return `<form class="tabela-itens__formulario" action="${url.disponibilidade}/${item.id}" method="post" data-alternar>
        <input type="hidden" name="__RequestVerificationToken" value="${escapar(token)}">
        <input type="hidden" name="disponivel" value="${item.disponivel ? "false" : "true"}">
        <button class="interruptor-item" type="button" role="switch" aria-checked="${item.disponivel}" aria-label="Disponível: ${escapar(item.nome)}">
          <span class="interruptor-item__trilho" aria-hidden="true"></span>
          <span class="interruptor-item__texto" aria-hidden="true">${item.disponivel ? "Disponível" : "Em falta"}</span>
        </button>
      </form>`;
  };

  const colunaAcoes = (dados, tipo, item) => {
    const nome = escapar(item.nome);
    // <menu>: a lista de comandos que se pode fazer com o item
    return `<menu class="tabela-itens__acoes u-lista-limpa">
        <li><a class="acao" href="${url.editar}/${item.id}" aria-label="Editar ${nome}">Editar</a></li>
        <li><a class="acao acao--perigo" href="${url.excluir}/${item.id}" aria-label="Excluir ${nome}">Excluir</a></li>
      </menu>`;
  };

  const tabela = jQuery(tabelaItens).DataTable({
    processing: true, // mostra "Carregando..." enquanto busca a página
    serverSide: true, // busca, ordenação e paginação no servidor
    orderMulti: false, // ordena por uma coluna de cada vez
    pageLength: 10,
    lengthMenu: [
      [10, 25, 50, -1],
      [10, 25, 50, "Todos"],
    ],
    order: [[1, "asc"]], // começa na ordem do cardápio, grupo a grupo
    autoWidth: false,
    // a busca e o filtro por grupo ficam fora da tabela, no visual do sistema
    layout: {
      topStart: null,
      topEnd: null,
      bottomStart: ["info", "pageLength"],
      bottomEnd: "paging",
    },
    ajax: {
      url: url.pagina,
      type: "POST",
      datatype: "json",
      // envia a busca de cada coluna como texto simples, para o ASP.NET ler em DatatableRequest.Columns
      data: (dados) => {
        dados.columns = dados.columns.map((coluna) => ({
          data: coluna.data ?? "",
          searchable: coluna.searchable,
          orderable: coluna.orderable,
          search: coluna.search?.value ?? "",
        }));
        return dados;
      },
    },
    rowId: (item) => `item-${item.id}`,
    createdRow: (linha, item) => {
      if (!item.ativo) linha.classList.add("tabela-itens__linha--inativa");
      else if (!item.disponivel) linha.classList.add("tabela-itens__linha--em-falta");
    },
    columns: [
      // o nome identifica a linha: vira o cabeçalho dela (<th scope="row">), como pede a tabela acessível
      {
        data: "nome",
        name: "Nome",
        cellType: "th",
        createdCell: (celula) => celula.setAttribute("scope", "row"),
        render: colunaNome,
      },
      { data: "nomeGrupoCardapio", name: "Grupo", render: colunaGrupo },
      { data: "preco", name: "Preço", className: "dt-body-right dt-head-right tabela-itens__preco", render: colunaPreco },
      { data: "disponivel", name: "Situação", render: colunaSituacao },
      { data: null, name: "Ações", orderable: false, searchable: false, className: "tabela-itens__coluna-acoes", render: colunaAcoes },
    ],
    // tradução do DataTables, com os textos da tabela falando em "itens" em vez de "registros"
    language: {
      url: tabelaItens.dataset.idioma,
      info: "Mostrando de _START_ até _END_ de _TOTAL_ itens",
      infoEmpty: "Nenhum item para mostrar",
      infoFiltered: "(filtrados de _MAX_ itens)",
      lengthMenu: "_MENU_ itens por página",
      emptyTable: "Nenhum item cadastrado ainda.",
      zeroRecords: "Nenhum item encontrado. Confira a busca ou escolha outro grupo.",
      // o DataTables junta o título da coluna com este texto (fica "Nome: ordenar")
      aria: {
        orderable: ": ordenar",
        orderableReverse: ": inverter a ordenação",
        orderableRemove: ": remover a ordenação",
      },
    },
  });

  // as linhas entram em cascata só na primeira carga; buscar, filtrar ou paginar não anima nada
  tabelaItens.classList.add("tabela-itens--chegando");
  tabela.one("draw", () => {
    setTimeout(() => tabelaItens.classList.remove("tabela-itens--chegando"), 700);
  });

  tabela.on("xhr", (evento, configuracao, resposta) => {
    if (!resposta || !resumo) return;
    resumo.textContent =
      resposta.recordsTotal === 0
        ? "Nenhum item cadastrado ainda. Cadastre o primeiro prato em “Novo item”."
        : plural(resposta.recordsTotal, "item cadastrado", "itens cadastrados");
  });

  // busca enquanto digita, com uma pequena espera para não consultar a cada tecla
  let espera;
  campoBusca?.addEventListener("input", () => {
    clearTimeout(espera);
    espera = setTimeout(() => tabela.search(campoBusca.value).draw(), 300);
  });

  camposGrupo.forEach((campo) => campo.addEventListener("change", () => {
    tabela.column(1).search(grupoEscolhido()).draw();
    mostrarFaixa();
    montarCarta();
  }));

  /* ---------- Conta do dia, fichas, faixa do grupo e carta ----------
     Usam a mesma consulta da tabela (GetDataPage), pedindo todos os itens de uma vez. */

  const conta = document.querySelector("[data-conta]");
  const faixa = document.querySelector("[data-faixa-grupo]");
  const carta = document.querySelector("[data-carta]");
  const botoesVista = [...document.querySelectorAll("[data-vista]")];
  let itens = [];

  const criar = (tag, classe, texto) => {
    const elemento = document.createElement(tag);
    if (classe) elemento.className = classe;
    if (texto !== undefined) elemento.textContent = texto;
    return elemento;
  };

  const gravura = (nomeGrupo) => {
    const imagem = criar("img");
    imagem.src = window.gravuraDoGrupo?.(nomeGrupo, tabelaItens) ?? "";
    imagem.alt = "";
    return imagem;
  };

  const nomeDoGrupo = (id) => camposGrupo.find((campo) => campo.value === id)?.closest("label").querySelector(".ficha-grupo__nome").textContent.trim() ?? "";

  const buscaCombina = (item) => {
    const termo = campoBusca?.value.trim().toLowerCase() ?? "";
    return !termo || [item.nome, item.detalhes, item.nomeGrupoCardapio].some((texto) => String(texto ?? "").toLowerCase().includes(termo));
  };

  const mostrarConta = () => {
    if (!conta) return;
    const ativos = itens.filter((item) => item.ativo);
    const disponiveis = ativos.filter((item) => item.disponivel).length;
    const emFalta = ativos.length - disponiveis;
    const comPreco = ativos.filter((item) => item.preco !== null);
    const precoMedio = comPreco.reduce((soma, item) => soma + item.preco, 0) / Math.max(comPreco.length, 1);
    conta.querySelector('[data-conta="disponiveis"]').textContent = disponiveis;
    conta.querySelector('[data-conta="falta"]').textContent = emFalta;
    conta.querySelector('[data-conta="fora"]').textContent = itens.length - ativos.length;
    conta.querySelector('[data-conta="preco"]').textContent = comPreco.length ? reais.format(precoMedio) : "—";
    conta.querySelector('[data-conta-barra="disponiveis"]').style.setProperty("--parte", disponiveis);
    conta.querySelector('[data-conta-barra="falta"]').style.setProperty("--parte", emFalta);
    conta.hidden = itens.length === 0;
  };

  const mostrarQuantidades = () => {
    document.querySelectorAll("[data-quantidade-grupo]").forEach((quantidade) => {
      const id = quantidade.dataset.quantidadeGrupo;
      const total = id ? itens.filter((item) => String(item.idGrupoCardapio) === id).length : itens.length;
      quantidade.replaceChildren(String(total), criar("span", "visually-hidden", total === 1 ? " item" : " itens"));
    });
  };

  const mostrarFaixa = () => {
    if (!faixa) return;
    const id = grupoEscolhido();
    faixa.hidden = !id;
    if (!id) return;
    const pratos = itens.filter((item) => item.ativo && String(item.idGrupoCardapio) === id);
    const precos = pratos.map((item) => item.preco).filter((preco) => preco !== null);
    const emFalta = pratos.filter((item) => !item.disponivel).length;
    const nome = nomeDoGrupo(id);
    const imagem = gravura(nome);
    imagem.className = "faixa-grupo__gravura";
    const texto = criar("div");
    const dados = [plural(pratos.length, "prato", "pratos"), `${emFalta} em falta`];
    if (precos.length) dados.push(`de ${reais.format(Math.min(...precos))} a ${reais.format(Math.max(...precos))}`);
    texto.append(criar("p", "faixa-grupo__nome", nome), criar("p", "faixa-grupo__dados", dados.join(" · ")));
    faixa.replaceChildren(imagem, texto);
  };

  // a carta mostra os itens ativos como o cliente vê, grupo a grupo, respeitando a busca e o grupo escolhido
  const montarCarta = () => {
    if (!carta || carta.hidden) return;
    const id = grupoEscolhido();
    const grupos = camposGrupo.map((campo) => campo.value).filter((valor) => valor && (!id || valor === id));
    const secoes = grupos.map((grupo) => {
      const pratos = itens.filter((item) => item.ativo && String(item.idGrupoCardapio) === grupo && buscaCombina(item));
      if (pratos.length === 0) return null;
      const nome = nomeDoGrupo(grupo);
      const secao = criar("section", "carta__grupo");
      const cabeca = criar("header", "carta__cabeca");
      const imagem = gravura(nome);
      imagem.className = "carta__gravura";
      cabeca.append(imagem, criar("h2", "carta__titulo", nome));
      const lista = criar("ul", "carta__pratos u-lista-limpa");
      pratos.forEach((item) => {
        const linha = criar("li", item.disponivel ? "carta__prato" : "carta__prato carta__prato--em-falta");
        const texto = criar("span", "carta__texto");
        texto.append(criar("span", "carta__nome", item.nome));
        if (item.detalhes) texto.append(criar("span", "carta__detalhe", item.detalhes));
        if (!item.disponivel) texto.append(criar("span", "selo", "Em falta"));
        const pontos = criar("span", "u-pontos");
        pontos.setAttribute("aria-hidden", "true");
        const preco = criar("data", "carta__preco", item.preco === null ? "—" : reais.format(item.preco));
        if (item.preco !== null) preco.value = Number(item.preco).toFixed(2);
        linha.append(texto, pontos, preco);
        lista.append(linha);
      });
      secao.append(cabeca, lista);
      return secao;
    }).filter(Boolean);
    carta.replaceChildren(...(secoes.length ? secoes : [criar("p", "carta__vazia", "Nenhum item encontrado. Confira a busca ou escolha outro grupo.")]));
  };

  const carregarItens = async () => {
    const corpo = new URLSearchParams({ draw: "1", start: "0", length: "1000", "columns[0][data]": "nome", "columns[1][data]": "nomeGrupoCardapio", "order[0][column]": "0", "order[0][dir]": "asc" });
    try {
      const resposta = await fetch(url.pagina, { method: "POST", body: corpo });
      if (!resposta.ok) return;
      itens = (await resposta.json()).data;
    } catch {
      return;
    }
    mostrarConta();
    mostrarQuantidades();
    mostrarFaixa();
    montarCarta();
  };

  const mostrarVista = (vista) => {
    const emCarta = vista === "carta";
    botoesVista.forEach((botao) => botao.setAttribute("aria-pressed", String(botao.dataset.vista === vista)));
    if (carta) carta.hidden = !emCarta;
    const caixaTabela = tabelaItens.closest(".dt-container") ?? tabelaItens;
    caixaTabela.hidden = emCarta;
    montarCarta();
    try {
      sessionStorage.setItem("vista-itens", vista);
    } catch {
      // sem sessionStorage, a vista só não é lembrada
    }
  };

  botoesVista.forEach((botao) => botao.addEventListener("click", () => mostrarVista(botao.dataset.vista)));
  campoBusca?.addEventListener("input", () => montarCarta());
  tabela.one("draw", () => {
    let vista = "tabela";
    try {
      vista = sessionStorage.getItem("vista-itens") ?? "tabela";
    } catch {
      vista = "tabela";
    }
    if (vista === "carta") mostrarVista("carta");
  });

  /* ---------- Interruptor de disponibilidade: troca sem recarregar a página ----------
     Envia o mesmo formulário para AlternarDisponivel; o aviso que o servidor prepara para a página
     é aproveitado aqui. Se algo falhar, o formulário segue pelo caminho normal. */

  const mostrarAvisos = (html) => {
    const avisos = new DOMParser().parseFromString(html, "text/html").querySelectorAll(".aviso");
    if (avisos.length === 0) return;
    let area = document.querySelector(".avisos");
    if (!area) {
      area = criar("section", "avisos");
      area.setAttribute("aria-label", "Avisos");
      document.querySelector("#conteudo").before(area);
    }
    avisos.forEach((aviso) => {
      const novo = document.importNode(aviso, true);
      area.append(novo);
      window.prepararAviso?.(novo);
    });
  };

  // o interruptor é um botão comum (role="switch" não combina com type="submit"): ele envia o próprio formulário
  tabelaItens.addEventListener("click", (evento) => {
    const botao = evento.target.closest(".interruptor-item");
    if (botao && botao.getAttribute("aria-busy") !== "true") botao.form.requestSubmit();
  });

  tabelaItens.addEventListener("submit", async (evento) => {
    const formulario = evento.target.closest("[data-alternar]");
    if (!formulario) return;
    evento.preventDefault();
    const botao = formulario.querySelector(".interruptor-item");
    // o interruptor muda na hora; a tabela e os números se atualizam quando o servidor responde
    const ligado = botao.getAttribute("aria-checked") === "true";
    botao.setAttribute("aria-checked", String(!ligado));
    botao.setAttribute("aria-busy", "true");
    try {
      const resposta = await fetch(formulario.action, { method: "POST", body: new FormData(formulario) });
      if (!resposta.ok) throw new Error(String(resposta.status));
      mostrarAvisos(await resposta.text());
      tabela.ajax.reload(null, false);
      await carregarItens();
    } catch {
      formulario.submit();
    }
  });

  carregarItens();

  // atalho "/" leva para a busca, como no ItaPets
  document.addEventListener("keydown", (evento) => {
    const digitando = evento.target.closest("input, textarea, select");
    if (evento.key === "/" && !digitando && campoBusca) {
      evento.preventDefault();
      campoBusca.focus();
    }
  });
}
