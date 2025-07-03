// Arquivo: Infrastructure/Repositories/ProjetoRepository.cs
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ByTescaro.ConstrutorApp.Infrastructure.Repositories
{
    public class ProjetoRepository : Repository<Projeto>, IProjetoRepository
    {
        public ProjetoRepository(ApplicationDbContext context) : base(context)
        {
        }

        //public void AnexarEntidade(Projeto entidade)

        //{

        //    _context.Set<Projeto>().Attach(entidade);

        //}



        //public void RemoverEntidade(Projeto entidade)

        //{

        //    _context.Set<Projeto>().Remove(entidade);

        //}

        // Sobrescrevemos o método base para fornecer uma implementação com includes.
        public override async Task<Projeto?> GetByIdAsync(long id)
        {
            return await GetQueryWithAllIncludes() // Reutiliza a lógica de includes
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        // Sobrescrevemos o método base para fornecer uma implementação com includes.
        public override async Task<List<Projeto>> GetAllAsync()
        {
            return await GetQueryWithPartialIncludes() // Reutiliza a lógica de includes parciais
                .AsNoTracking()
                .OrderBy(p => p.Nome)
                .ToListAsync();
        }

        public async Task<List<T>> GetAllProjectedAsync<T>(Expression<Func<Projeto, T>> projection)
        {
            return await _dbSet
                .AsNoTracking()
                .OrderBy(p => p.Nome)
                .Select(projection)
                .ToListAsync();
        }

        public IQueryable<Projeto> GetQueryable()
        {
            return _dbSet.AsNoTracking();
        }

        public async Task<List<Projeto>> GetAllListAsync()
        {
            return await _dbSet
                .Include(p => p.Obras)
                .AsNoTracking()
                .OrderBy(p => p.Nome)
                .ToListAsync();
        }

        public async Task<List<Projeto>> FindAllAsync(Expression<Func<Projeto, bool>> filtro)
        {
            return await _dbSet.Where(filtro).AsNoTracking().ToListAsync();
        }

        #region Métodos Privados para Reutilização

        /// <summary>
        /// Centraliza a lógica de Eager Loading para a consulta completa de um Projeto.
        /// </summary>
        private IQueryable<Projeto> GetQueryWithAllIncludes()
        {
            return _dbSet
                .Include(p => p.Cliente)
                .Include(p => p.Obras).ThenInclude(o => o.Etapas).ThenInclude(e => e.Itens)
                .Include(p => p.Obras).ThenInclude(o => o.Documentos)
                .Include(p => p.Obras).ThenInclude(o => o.Imagens)
                .Include(p => p.Obras).ThenInclude(o => o.Funcionarios)
                .Include(p => p.Obras).ThenInclude(o => o.Equipamentos)
                .Include(p => p.Obras).ThenInclude(o => o.Pendencias)
                .Include(p => p.Obras).ThenInclude(o => o.Retrabalhos)
                // Inclua outras relações profundas aqui, se necessário...
                .AsSplitQuery(); // Otimização crítica para múltiplas coleções
        }

        /// <summary>
        /// Centraliza a lógica de Eager Loading para a listagem geral de Projetos.
        /// </summary>
        private IQueryable<Projeto> GetQueryWithPartialIncludes()
        {
            return _dbSet
                .Include(p => p.Cliente)
                .Include(p => p.Obras).ThenInclude(o => o.Funcionarios);
            // Adicione outros includes rasos que façam sentido para uma lista
        }

        #endregion

       
    }
}



//// Arquivo: Infrastructure/Repositories/ProjetoRepository.cs
//using ByTescaro.ConstrutorApp.Domain.Entities;
//using ByTescaro.ConstrutorApp.Domain.Interfaces;
//using ByTescaro.ConstrutorApp.Infrastructure.Data;
//using Microsoft.EntityFrameworkCore;
//using System.Linq.Expressions;

//namespace ByTescaro.ConstrutorApp.Infrastructure.Repositories
//{
//    public class ProjetoRepository : Repository<Projeto>, IProjetoRepository
//    {
//        public ProjetoRepository(ApplicationDbContext context) : base(context)
//        {
//        }

//        public void AnexarEntidade(Projeto entidade)

//        {

//            _context.Set<Projeto>().Attach(entidade);

//        }



//        public void RemoverEntidade(Projeto entidade)

//        {

//            _context.Set<Projeto>().Remove(entidade);

//        }



//        public async Task<Projeto?> GetByIdWithAllIncludesAsync(long id)
//        {
//            // Reutiliza o método privado que contém todos os Includes e a otimização
//            return await GetFullGraphQuery()
//                .FirstOrDefaultAsync(p => p.Id == id);
//        }

//        public async Task<List<Projeto>> GetAllWithAllIncludesAsync()
//        {
//            // Reutiliza o método privado
//            return await GetFullGraphQuery()
//                .AsNoTracking()
//                .OrderBy(p => p.Nome)
//                .ToListAsync();
//        }

//        public async Task<List<T>> GetAllProjectedAsync<T>(Expression<Func<Projeto, T>> projection)
//        {
//            return await _dbSet
//                .AsNoTracking()
//                .OrderBy(p => p.Nome)
//                .Select(projection)
//                .ToListAsync();
//        }

//        public IQueryable<Projeto> GetQueryable()
//        {
//            return _dbSet.AsNoTracking();
//        }

//        private IQueryable<Projeto> GetFullGraphQuery()
//        {
//            return _dbSet
//                .Include(p => p.Cliente)
//                .Include(p => p.Obras).ThenInclude(o => o.Etapas).ThenInclude(e => e.Itens)
//                .Include(p => p.Obras).ThenInclude(o => o.Documentos)
//                .Include(p => p.Obras).ThenInclude(o => o.Imagens)
//                .Include(p => p.Obras).ThenInclude(o => o.Funcionarios)
//                .Include(p => p.Obras).ThenInclude(o => o.Equipamentos)
//                .Include(p => p.Obras).ThenInclude(o => o.Pendencias)
//                .Include(p => p.Obras).ThenInclude(o => o.Retrabalhos)
//                .Include(p => p.Obras).ThenInclude(o => o.Insumos)
//                .Include(p => p.Obras).ThenInclude(o => o.Servicos)
//                .Include(p => p.Obras).ThenInclude(o => o.Fornecedores)
//                .AsSplitQuery();
//        }


//    }
//}