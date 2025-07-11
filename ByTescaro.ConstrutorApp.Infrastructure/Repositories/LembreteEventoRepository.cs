using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Enums;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ByTescaro.ConstrutorApp.Infrastructure.Repositories
{
    public class LembreteEventoRepository : Repository<LembreteEvento>, ILembreteEventoRepository
    {
        public LembreteEventoRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<LembreteEvento>> GetLembretesPendentesParaEnvioAsync(DateTime agora)
        {
            return await _dbSet
                .Include(le => le.Evento)
                .Where(le => !le.Enviado &&
                             (le.UnidadeTempo == UnidadeTempo.Minutos && le.Evento.DataHoraInicio.AddMinutes(-le.TempoAntes) <= agora ||
                              le.UnidadeTempo == UnidadeTempo.Horas && le.Evento.DataHoraInicio.AddHours(-le.TempoAntes) <= agora ||
                              le.UnidadeTempo == UnidadeTempo.Dias && le.Evento.DataHoraInicio.AddDays(-le.TempoAntes) <= agora))
                .ToListAsync();
        }

        public async Task<IEnumerable<LembreteEvento>> GetLembretesByEventoIdAsync(long eventoId)
        {
            return await _dbSet.Where(l => l.EventoId == eventoId).ToListAsync();
        }
    }
}