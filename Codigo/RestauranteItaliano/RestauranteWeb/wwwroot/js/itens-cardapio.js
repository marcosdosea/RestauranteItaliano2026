"use strict";

/* ---------- Itens do cardápio: tabela com DataTables ----------
   A busca, o filtro por grupo, a ordenação e a paginação são feitos no servidor
   (ItemCardapioController.GetDataPage), como no projeto Biblioteca do professor. */

const tabelaItens = document.querySelector("#tabela-itens");

if (tabelaItens && window.jQuery) {
  const campoBusca = document.querySelector("[data-busca-itens]");
  const campoGrupo = document.querySelector("[data-grupo-itens]");
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

  const colunaNome = (dados, tipo, item) => {
    const detalhes = item.detalhes
      ? `<span class="prato__detalhe">${escapar(item.detalhes)}</span>`
      : "";
    return `<a class="tabela-itens__link" href="${url.detalhes}/${item.id}">
      <span class="prato__nome">${escapar(item.nome)}</span>${detalhes}</a>`;
  };

  // grupo com a gravura dele; a gravura repete o nome, por isso é decorativa (alt vazio)
  const colunaGrupo = (grupo) =>
    `<span class="tabela-itens__grupo"><img class="tabela-itens__gravura" src="${window.gravuraDoGrupo?.(grupo, tabelaItens) ?? ""}" alt="">${escapar(grupo ?? "—")}</span>`;

  // <data>: o preço formatado para quem lê e o número puro no value
  const colunaPreco = (preco) =>
    preco === null || preco === undefined ? "—" : `<data value="${Number(preco).toFixed(2)}">${reais.format(preco)}</data>`;

  const colunaSituacao = (dados, tipo, item) => {
    if (!item.ativo) return '<span class="prato__selo prato__selo--inativo">Fora do cardápio</span>';
    if (!item.disponivel) return '<span class="prato__selo">Em falta</span>';
    return '<span class="tabela-itens__disponivel">Disponível</span>';
  };

  const colunaAcoes = (dados, tipo, item) => {
    const nome = escapar(item.nome);
    const alternar = item.ativo
      ? `<li>
          <form class="prato__formulario" action="${url.disponibilidade}/${item.id}" method="post">
            <input type="hidden" name="__RequestVerificationToken" value="${escapar(token)}">
            <input type="hidden" name="disponivel" value="${item.disponivel ? "false" : "true"}">
            <button class="prato__acao" type="submit" aria-label="${item.disponivel ? "Marcar em falta" : "Marcar disponível"}: ${nome}">${item.disponivel ? "Marcar em falta" : "Marcar disponível"}</button>
          </form>
        </li>`
      : "";
    // <menu>: a lista de comandos que se pode fazer com o item
    return `<menu class="tabela-itens__acoes u-lista-limpa">
        <li><a class="prato__acao" href="${url.editar}/${item.id}" aria-label="Editar ${nome}">Editar</a></li>
        ${alternar}
        <li><a class="prato__acao prato__acao--perigo" href="${url.excluir}/${item.id}" aria-label="Excluir ${nome}">Excluir</a></li>
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
    },
  });

  // as linhas entram em cascata só na primeira carga; buscar, filtrar ou paginar não anima nada
  tabelaItens.classList.add("tabela-itens--chegando");
  tabela.one("draw", () => {
    setTimeout(() => tabelaItens.classList.remove("tabela-itens--chegando"), 600);
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

  campoGrupo?.addEventListener("change", () => {
    tabela.column(1).search(campoGrupo.value).draw();
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
