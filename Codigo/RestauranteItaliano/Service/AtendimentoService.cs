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

        public IEnumerable<MesaAtendimentoDTO> GetMapaMesas()
        {
            return context.Mesas
                .AsNoTracking()
                .OrderBy(m => m.Identificacao)
                .Select(m => new MesaAtendimentoDTO
                {
                    IdMesa = m.Id,
                    Identificacao = m.Identificacao,
                    Ocupada = m.Atendimentos.Any(a => a.Status == "I"),
                    InicioAtendimento = m.Atendimentos
                        .Where(a => a.Status == "I")
                        .OrderByDescending(a => a.DataHoraInicio)
                        .Select(a => (DateTime?)a.DataHoraInicio)
                        .FirstOrDefault()
                });
        }

        public uint Iniciar(int idMesa)
        {
            var mesa = context.Mesas.Find(idMesa);

            if (mesa == null)
            {
                throw new ArgumentException("Mesa não encontrada.");
            }

            var jaExisteAtendimentoAtivo = context.Atendimentos.Any(a => a.IdMesa == idMesa && a.Status == "I");

            if (jaExisteAtendimentoAtivo)
            {
                throw new InvalidOperationException("Esta mesa já possui um atendimento iniciado.");
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
