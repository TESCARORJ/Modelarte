using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByTescaro.ConstrutorApp.Infrastructure.Repositories
{
    public class PersonalizacaoRepository : Repository<Personalizacao>, IPersonalizacaoRepository
    {
        public PersonalizacaoRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Personalizacao> ObterUnicaAsync()
        {
            var personalizacao = await _dbSet.FirstOrDefaultAsync();
            if (personalizacao == null)
            {
                personalizacao = new Personalizacao();
                _dbSet.Add(personalizacao);
                await _context.SaveChangesAsync();
            }
            return personalizacao;
        }
    }
}
