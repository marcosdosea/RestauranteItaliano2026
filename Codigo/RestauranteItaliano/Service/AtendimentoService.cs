using Core;
using Core.Service;
using Core.DTO;
using Microsoft.EntityFrameworkCore;

namespace Service
{
    public class AtendimentoService : IAtendimentoService
    {
        private readonly RestauranteContext context;

        public AtendimentoService(RestauranteContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Busca o mapa de mesas de um restaurante, indicando se estão
        /// ocupadas e informando os dados do atendimento ativo, se houver.
        /// </summary>
        public IEnumerable<MesaAtendimentoDTO> GetMapaMesas(uint idRestaurante)
        {
            return context.Mesas
                .AsNoTracking()
                .Where(m => m.IdRestaurante == idRestaurante)
                .OrderBy(m => m.Identificacao)
                .Select(m => new MesaAtendimentoDTO
                {
                    IdMesa = m.Id,
                    Identificacao = m.Identificacao,

                    Ocupada = m.Atendimentos
                        .Any(a => a.Status == "I"),

                    IdAtendimento = m.Atendimentos
                        .Where(a => a.Status == "I")
                        .OrderByDescending(a => a.DataHoraInicio)
                        .Select(a => (uint?)a.Id)
                        .FirstOrDefault(),

                    InicioAtendimento = m.Atendimentos
                        .Where(a => a.Status == "I")
                        .OrderByDescending(a => a.DataHoraInicio)
                        .Select(a => (DateTime?)a.DataHoraInicio)
                        .FirstOrDefault()
                });
        }

        /// <summary>
        /// Inicia um atendimento em uma mesa livre e retorna o ID criado.
        /// </summary>
        public uint Iniciar(int idMesa, uint idRestaurante)
        {
            var mesa = context.Mesas.FirstOrDefault(m =>
                m.Id == idMesa &&
                m.IdRestaurante == idRestaurante);

            if (mesa == null)
            {
                throw new ServiceException(
                    "Mesa não encontrada neste restaurante.");
            }

            var jaExisteAtendimentoAtivo = context.Atendimentos.Any(a =>
                a.IdMesa == idMesa &&
                a.Status == "I");

            if (jaExisteAtendimentoAtivo)
            {
                throw new ServiceException(
                    "Esta mesa já possui um atendimento iniciado.");
            }

            var atendimento = new Atendimento
            {
                IdMesa = idMesa,
                DataHoraInicio = DateTime.Now,
                DataHoraFim = null,
                Status = "I",
                TotalConta = 0m,
                TotalServico = 0m,
                TotalDesconto = 0m,
                Total = 0m
            };

            context.Atendimentos.Add(atendimento);
            context.SaveChanges();

            return atendimento.Id;
        }
    }
}