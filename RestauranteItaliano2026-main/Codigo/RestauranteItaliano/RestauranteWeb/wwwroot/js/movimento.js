"use strict";

/* ---------- Movimento: cardápio 3D da página inicial e cartões que acompanham o mouse ----------
   Só transform e opacity. Nada roda em telas de toque nem para quem pediu menos movimento
   (prefers-reduced-motion): nesses casos o CSS mostra tudo parado, já no lugar. */

const menosMovimento = window.matchMedia("(prefers-reduced-motion: reduce)").matches;
const comMouse = window.matchMedia("(hover: hover) and (pointer: fine)").matches;

/* O cardápio abre (a aba gira até a posição aberta) só na primeira visita da sessão,
   para não repetir a animação toda vez que a pessoa volta para o início. */
const cardapio = document.querySelector("[data-cardapio]");

if (cardapio && !menosMovimento && window.matchMedia("(min-width: 768px)").matches) {
  let jaAbriu = false;
  try {
    jaAbriu = sessionStorage.getItem("cardapio-aberto") === "sim";
    sessionStorage.setItem("cardapio-aberto", "sim");
  } catch {
    // sem sessionStorage (modo privado, por exemplo): abre do mesmo jeito
  }
  if (!jaAbriu) cardapio.classList.add("cardapio--abrindo");
}

/* Inclinação com mola: o elemento gira alguns graus na direção do mouse e volta ao soltar.
   A mola (rigidez e amortecimento) deixa o movimento natural e interrompível. */
if (comMouse && !menosMovimento) {
  const RIGIDEZ = 170;
  const AMORTECIMENTO = 22;

  document.querySelectorAll("[data-inclinar]").forEach((elemento) => {
    // o cardápio responde ao mouse em toda a abertura; os outros cartões, só neles mesmos
    const area = elemento.closest(".abertura") ?? elemento;
    const maximo = elemento.closest("[data-cardapio]") ? 6 : 3;
    const estado = { x: 0, y: 0, vx: 0, vy: 0, alvoX: 0, alvoY: 0 };
    let quadro = null;
    let ultimo = 0;

    const passo = (agora) => {
      const dt = Math.min((agora - (ultimo || agora)) / 1000, 1 / 30);
      ultimo = agora;
      for (const eixo of ["x", "y"]) {
        const alvo = eixo === "x" ? estado.alvoX : estado.alvoY;
        const v = eixo === "x" ? "vx" : "vy";
        const forca = (alvo - estado[eixo]) * RIGIDEZ - estado[v] * AMORTECIMENTO;
        estado[v] += forca * dt;
        estado[eixo] += estado[v] * dt;
      }
      elemento.style.setProperty("--giro-x", `${estado.x.toFixed(2)}deg`);
      elemento.style.setProperty("--giro-y", `${estado.y.toFixed(2)}deg`);

      const parado =
        Math.abs(estado.alvoX - estado.x) < 0.01 && Math.abs(estado.alvoY - estado.y) < 0.01 &&
        Math.abs(estado.vx) < 0.01 && Math.abs(estado.vy) < 0.01;
      quadro = parado ? null : requestAnimationFrame(passo);
      if (parado) ultimo = 0;
    };

    const animar = () => {
      if (quadro === null) quadro = requestAnimationFrame(passo);
    };

    area.addEventListener("pointermove", (evento) => {
      const caixa = elemento.getBoundingClientRect();
      const x = (evento.clientX - (caixa.left + caixa.width / 2)) / caixa.width;
      const y = (evento.clientY - (caixa.top + caixa.height / 2)) / caixa.height;
      estado.alvoY = Math.max(-1, Math.min(1, x)) * maximo;
      estado.alvoX = Math.max(-1, Math.min(1, -y)) * maximo;
      animar();
    });

    area.addEventListener("pointerleave", () => {
      estado.alvoX = 0;
      estado.alvoY = 0;
      animar();
    });
  });
}

/* Gravura de um grupo do cardápio pelo nome: as palavras-chave vêm do servidor (Models/Gravuras.cs)
   em data-gravuras, para a regra ficar num lugar só. Usada pela tabela de itens e pela prévia. */
window.gravuraDoGrupo = (nomeGrupo, elementoComDados) => {
  const pasta = elementoComDados?.dataset.pastaGravuras ?? "/images/gravuras/";
  let palavras = [];
  try {
    palavras = JSON.parse(elementoComDados?.dataset.gravuras ?? "[]");
  } catch {
    palavras = [];
  }
  const nome = String(nomeGrupo ?? "")
    .normalize("NFD")
    .replace(/[̀-ͯ]/g, "")
    .toLowerCase();
  const achada = palavras.find(([palavra]) => nome.includes(palavra));
  return `${pasta}${achada ? achada[1] : "prato"}.svg`;
};
