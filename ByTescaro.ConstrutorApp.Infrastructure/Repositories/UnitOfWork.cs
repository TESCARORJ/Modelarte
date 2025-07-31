
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Interfaces.ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ByTescaro.ConstrutorApp.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        private readonly ApplicationDbContext _context;

        // Recebe a fábrica
        public UnitOfWork(IDbContextFactory<ApplicationDbContext> contextFactory, ILogger<UnitOfWork> logger)
        {
            _contextFactory = contextFactory;
            // Cria uma instância do DbContext para esta unidade de trabalho
            _context = _contextFactory.CreateDbContext();
            _logger = logger;
        }


        #region Private Repository Fields
        private IEnderecoRepository? _enderecoRepository;
        private IClienteRepository? _clienteRepository;
        private IFuncionarioRepository? _funcionarioRepository;
        private IFornecedorRepository? _fornecedorRepository;
        private IUsuarioRepository? _usuarioRepository;
        private IEquipamentoRepository? _equipamentoRepository;
        private IInsumoRepository? _insumoRepository;
        private IServicoRepository? _servicoRepository;
        private IFuncaoRepository? _funcaoRepository;
        private IPerfilUsuarioRepository? _perfilUsuarioRepository;
        private IProjetoRepository? _projetoRepository;
        private IObraRepository? _obraRepository;
        private IObraFuncionarioRepository? _obraFuncionarioRepository;
        private IObraFornecedorRepository? _obraFornecedorRepository;
        private IObraEquipamentoRepository? _obraEquipamentoRepository;
        private IObraDocumentoRepository? _obraDocumentoRepository;
        private IObraImagemRepository? _obraImagemRepository;
        private IObraPendenciaRepository? _obraPendenciaRepository;
        private IObraRetrabalhoRepository? _obraRetrabalhoRepository;
        private IObraInsumoRepository? _obraInsumoRepository;
        private IObraInsumoListaRepository? _obraInsumoListaRepository;
        private IObraServicoRepository? _obraServicoRepository;
        private IObraServicoListaRepository? _obraServicoListaRepository;
        private IObraEtapaRepository? _obraEtapaRepository;
        private IObraItemEtapaRepository? _obraItemEtapaRepository;
        private IObraEtapaPadraoRepository? _obraEtapaPadraoRepository;
        private IObraItemEtapaPadraoRepository? _obraItemEtapaPadraoRepository;
        private IObraItemEtapaPadraoInsumoRepository? _obraItemEtapaPadraoInsumoRepository;
        private IOrcamentoRepository? _orcamentoRepository;
        private IOrcamentoObraRepository? _orcamentoObraRepository;
        private IOrcamentoItemRepository? _orcamentoItemRepository;
        private IFornecedorInsumoRepository? _fornecedorInsumoRepository;
        private IFornecedorServicoRepository? _fornecedorServicoRepository;
        private IEventoRepository? _eventoRepository;
        private IParticipanteEventoRepository? _participanteEventoRepository;
        private ILembreteEventoRepository? _lembreteEventoRepository;
        private IConfiguracaoLembreteDiarioRepository? _configuracaoLembreteDiarioRepository;
        private IPersonalizacaoRepository? _personalizacaoRepository;

        private readonly ILogger<UnitOfWork> _logger;

        #endregion



        // Implemente a propriedade pública para cada repositório
        #region Public Repository Properties
        public IEnderecoRepository EnderecoRepository => _enderecoRepository ??= new EnderecoRepository(_context);
        public IClienteRepository ClienteRepository => _clienteRepository ??= new ClienteRepository(_context);
        public IFuncionarioRepository FuncionarioRepository => _funcionarioRepository ??= new FuncionarioRepository(_context);
        public IFornecedorRepository FornecedorRepository => _fornecedorRepository ??= new FornecedorRepository(_context);
        public IUsuarioRepository UsuarioRepository => _usuarioRepository ??= new UsuarioRepository(_context);
        public IEquipamentoRepository EquipamentoRepository => _equipamentoRepository ??= new EquipamentoRepository(_context);
        public IInsumoRepository InsumoRepository => _insumoRepository ??= new InsumoRepository(_context);
        public IServicoRepository ServicoRepository => _servicoRepository ??= new ServicoRepository(_context);
        public IFuncaoRepository FuncaoRepository => _funcaoRepository ??= new FuncaoRepository(_context);
        public IPerfilUsuarioRepository PerfilUsuarioRepository => _perfilUsuarioRepository ??= new PerfilUsuarioRepository(_context);
        public IProjetoRepository ProjetoRepository => _projetoRepository ??= new ProjetoRepository(_context);
        public IObraRepository ObraRepository => _obraRepository ??= new ObraRepository(_context);
        public IObraFuncionarioRepository ObraFuncionarioRepository => _obraFuncionarioRepository ??= new ObraFuncionarioRepository(_context);
        public IObraFornecedorRepository ObraFornecedorRepository => _obraFornecedorRepository ??= new ObraFornecedorRepository(_context);
        public IObraEquipamentoRepository ObraEquipamentoRepository => _obraEquipamentoRepository ??= new ObraEquipamentoRepository(_context);
        public IObraDocumentoRepository ObraDocumentoRepository => _obraDocumentoRepository ??= new ObraDocumentoRepository(_context);
        public IObraImagemRepository ObraImagemRepository => _obraImagemRepository ??= new ObraImagemRepository(_context);
        public IObraPendenciaRepository ObraPendenciaRepository => _obraPendenciaRepository ??= new ObraPendenciaRepository(_context);
        public IObraRetrabalhoRepository ObraRetrabalhoRepository => _obraRetrabalhoRepository ??= new ObraRetrabalhoRepository(_context);
        public IObraInsumoRepository ObraInsumoRepository => _obraInsumoRepository ??= new ObraInsumoRepository(_context);
        public IObraInsumoListaRepository ObraInsumoListaRepository => _obraInsumoListaRepository ??= new ObraInsumoListaRepository(_context);
        public IObraServicoRepository ObraServicoRepository => _obraServicoRepository ??= new ObraServicoRepository(_context);
        public IObraServicoListaRepository ObraServicoListaRepository => _obraServicoListaRepository ??= new ObraServicoListaRepository(_context);
        public IObraEtapaRepository ObraEtapaRepository => _obraEtapaRepository ??= new ObraEtapaRepository(_context);
        public IObraItemEtapaRepository ObraItemEtapaRepository => _obraItemEtapaRepository ??= new ObraItemEtapaRepository(_context);
        public IObraEtapaPadraoRepository ObraEtapaPadraoRepository => _obraEtapaPadraoRepository ??= new ObraEtapaPadraoRepository(_context);
        public IObraItemEtapaPadraoRepository ObraItemEtapaPadraoRepository => _obraItemEtapaPadraoRepository ??= new ObraItemEtapaPadraoRepository(_context);
        public IObraItemEtapaPadraoInsumoRepository ObraItemEtapaPadraoInsumoRepository => _obraItemEtapaPadraoInsumoRepository ??= new ObraItemEtapaPadraoInsumoRepository(_context);
        public IOrcamentoRepository OrcamentoRepository => _orcamentoRepository ??= new OrcamentoRepository(_context);
        public IOrcamentoObraRepository OrcamentoObraRepository => _orcamentoObraRepository ??= new OrcamentoObraRepository(_context);
        public IOrcamentoItemRepository OrcamentoItemRepository => _orcamentoItemRepository ??= new OrcamentoItemRepository(_context);
        public IFornecedorInsumoRepository FornecedorInsumoRepository => _fornecedorInsumoRepository ??= new FornecedorInsumoRepository(_context);
        public IFornecedorServicoRepository FornecedorServicoRepository => _fornecedorServicoRepository ??= new FornecedorServicoRepository(_context);
        public IEventoRepository EventoRepository => _eventoRepository ??= new EventoRepository(_context);
        public IParticipanteEventoRepository ParticipanteEventoRepository => _participanteEventoRepository ??= new ParticipanteEventoRepository(_context);
        public ILembreteEventoRepository LembreteEventoRepository => _lembreteEventoRepository ??= new LembreteEventoRepository(_context);
        public IConfiguracaoLembreteDiarioRepository ConfiguracaoLembreteDiarioRepository => _configuracaoLembreteDiarioRepository ??= new ConfiguracaoLembreteDiarioRepository(_context);
        public IPersonalizacaoRepository PersonalizacaoRepository => _personalizacaoRepository ??= new PersonalizacaoRepository(_context);

        #endregion

        public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
        {
            // NOVO: Log de depuração para o Change Tracker
            _logger.LogInformation("--- Início do CommitAsync ---");
            foreach (var entry in _context.ChangeTracker.Entries())
            {
                _logger.LogInformation($"Entidade: {entry.Entity.GetType().Name}, Estado: {entry.State}");
                if (entry.State == EntityState.Modified)
                {
                    foreach (var property in entry.Properties)
                    {
                        if (property.IsModified)
                        {
                            _logger.LogInformation($"  Propriedade '{property.Metadata.Name}' modificada. Valor Antigo: '{property.OriginalValue}', Valor Novo: '{property.CurrentValue}'");
                        }
                    }
                }
            }
            _logger.LogInformation("--- Fim do Log do ChangeTracker ---");

            try
            {
                var affectedRows = await _context.SaveChangesAsync(cancellationToken);
                _logger.LogInformation($"SaveChanges executado. Linhas afetadas: {affectedRows}");
                return affectedRows;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Erro de concorrência ao salvar mudanças no banco de dados.");
                throw; // Ou trate de forma específica
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Erro de atualização do banco de dados ao salvar mudanças.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao salvar mudanças no banco de dados.");
                throw;
            }
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }

        public async ValueTask DisposeAsync()
        {
            // Chama o método de descarte assíncrono do DbContext.
            await _context.DisposeAsync();
            GC.SuppressFinalize(this);
        }
    }
}