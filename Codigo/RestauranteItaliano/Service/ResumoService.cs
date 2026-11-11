using Core;
using Core.DTO;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service
{
    public class ResumoService : IResumoService
    {
        // quantidade de grupos listados no painel da página inicial
        private const int MaximoGrupos = 5;

        private readonly RestauranteContext context;

        public ResumoService(RestauranteContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Monta o resumo da página inicial: itens ativos do cardápio (disponíveis e em falta),
        /// os grupos com mais itens ativos e a quantidade de registros de cada cadastro.
        /// As contagens são feitas no banco, sem carregar os registros.
        /// </summary>
        /// <returns>O resumo do sistema</returns>
        public ResumoDTO GetResumo()
        {
            var itensAtivos = context.Itemcardapios.AsNoTracking().Where(item => item.Ativo == 1);
            var itensNoCardapio = itensAtivos.Count();
            var itensDisponiveis = itensAtivos.Count(item => item.Disponivel == "1");

            return new ResumoDTO
            {
                ItensNoCardapio = itensNoCardapio,
                ItensDisponiveis = itensDisponiveis,
                ItensEmFalta = itensNoCardapio - itensDisponiveis,
                Grupos = context.Grupocardapios
                    .AsNoTracking()
                    .Where(grupo => grupo.Itemcardapios.Any(item => item.Ativo == 1))
                    .OrderByDescending(grupo => grupo.Itemcardapios.Count(item => item.Ativo == 1))
                    .ThenBy(grupo => grupo.Nome)
                    .Take(MaximoGrupos)
                    .Select(grupo => new GrupoResumoDTO
                    {
                        Nome = grupo.Nome,
                        QuantidadeItens = grupo.Itemcardapios.Count(item => item.Ativo == 1)
                    }),
                QuantidadeGruposCardapio = context.Grupocardapios.Count(),
                QuantidadeTiposFuncionario = context.Tipofuncionarios.Count(),
                QuantidadeTiposRestaurante = context.Tiporestaurantes.Count(),
                QuantidadeFormasPagamento = context.Formapagamentos.Count()
            };
        }
    }
}
