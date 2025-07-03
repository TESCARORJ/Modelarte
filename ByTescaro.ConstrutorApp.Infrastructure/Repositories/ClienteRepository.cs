using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ByTescaro.ConstrutorApp.Infrastructure.Repositories
{
    public class ClienteRepository : Repository<Cliente>, IClienteRepository
    {
        public ClienteRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Cliente?> ObterPorCpfCnpjAsync(string cpfCnpj)
        {
            return await _dbSet.FirstOrDefaultAsync(c => c.CpfCnpj == cpfCnpj);
        }

        public async Task<Cliente?> GetByIdWithEnderecoAsync(long id)
        {
            return await _dbSet
                .Include(c => c.Endereco)
                .FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}