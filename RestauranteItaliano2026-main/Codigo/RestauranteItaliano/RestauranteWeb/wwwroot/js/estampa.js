"use strict";

/* ---------- Estampa de fundo da página inicial ----------
   As gravuras já flutuam sozinhas pelo CSS. Aqui, com mouse, a estampa inteira acompanha o ponteiro:
   o JS passa a posição (de -1 a 1) nas variáveis --estampa-x e --estampa-y e cada peça anda conforme
   a própria profundidade (--profundidade, no CSS), como camadas de papel. Uma mola deixa o movimento
   suave e interrompível, e tudo volta ao centro quando o ponteiro sai da janela.
   No toque e com "reduzir movimento", nada disto roda. */

(() => {
  const estampa = document.querySelector("[data-estampa]");
  if (
    !estampa ||
    window.matchMedia("(prefers-reduced-motion: reduce)").matches ||
    !window.matchMedia("(hover: hover) and (pointer: fine)").matches
  ) {
    return;
  }

  const RIGIDEZ = 50;
  const AMORTECIMENTO = 13;
  const estado = { x: 0, y: 0, vx: 0, vy: 0, alvoX: 0, alvoY: 0 };
  let quadro = null;
  let ultimo = 0;

  const passo = (agora) => {
    const dt = Math.min((agora - (ultimo || agora)) / 1000, 1 / 30);
    ultimo = agora;
    for (const [eixo, velocidade, alvo] of [["x", "vx", "alvoX"], ["y", "vy", "alvoY"]]) {
      const forca = (estado[alvo] - estado[eixo]) * RIGIDEZ - estado[velocidade] * AMORTECIMENTO;
      estado[velocidade] += forca * dt;
      estado[eixo] += estado[velocidade] * dt;
    }

    // as peças andam para o lado oposto ao ponteiro, como se estivessem mais fundo que a tela
    estampa.style.setProperty("--estampa-x", (-estado.x).toFixed(3));
    estampa.style.setProperty("--estampa-y", (-estado.y).toFixed(3));

    const parado =
      Math.abs(estado.alvoX - estado.x) < 0.001 && Math.abs(estado.alvoY - estado.y) < 0.001 &&
      Math.abs(estado.vx) < 0.001 && Math.abs(estado.vy) < 0.001;
    quadro = parado ? null : requestAnimationFrame(passo);
    if (parado) ultimo = 0;
  };

  const animar = () => {
    if (quadro === null) quadro = requestAnimationFrame(passo);
  };

  window.addEventListener(
    "pointermove",
    (evento) => {
      estado.alvoX = (evento.clientX / window.innerWidth - 0.5) * 2;
      estado.alvoY = (evento.clientY / window.innerHeight - 0.5) * 2;
      animar();
    },
    { passive: true }
  );

  document.documentElement.addEventListener("pointerleave", () => {
    estado.alvoX = 0;
    estado.alvoY = 0;
    animar();
  });
})();
