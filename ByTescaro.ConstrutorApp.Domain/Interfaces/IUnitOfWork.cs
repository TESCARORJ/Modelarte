using ByTescaro.ConstrutorApp.Domain.Interfaces.ByTescaro.ConstrutorApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByTescaro.ConstrutorApp.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable, IAsyncDisposable
    {
        Task<int> CommitAsync(CancellationToken cancellationToken = default);
        IClienteRepository ClienteRepository { get; }
        IEquipamentoRepository EquipamentoRepository { get; }
        IFornecedorInsumoRepository FornecedorInsumoRepository { get; }
        IFornecedorRepository FornecedorRepository { get; }
        IFornecedorServicoRepository FornecedorServicoRepository { get; }
        IFuncaoRepository FuncaoRepository { get; }
        IFuncionarioRepository FuncionarioRepository { get; }
        IInsumoRepository InsumoRepository { get; }
        IObraDocumentoRepository ObraDocumentoRepository { get; }
        IObraEquipamentoRepository ObraEquipamentoRepository { get; }
        IObraEtapaPadraoRepository ObraEtapaPadraoRepository { get; }
        IObraEtapaRepository ObraEtapaRepository { get; }
        IObraFornecedorRepository ObraFornecedorRepository { get; }
        IObraFuncionarioRepository ObraFuncionarioRepository { get; }
        IObraImagemRepository ObraImagemRepository { get; }
        IObraInsumoListaRepository ObraInsumoListaRepository { get; }
        IObraInsumoRepository ObraInsumoRepository { get; }
        IObraItemEtapaPadraoInsumoRepository ObraItemEtapaPadraoInsumoRepository { get; }
        IObraItemEtapaPadraoRepository ObraItemEtapaPadraoRepository { get; }
        IObraItemEtapaRepository ObraItemEtapaRepository { get; }
        IObraPendenciaRepository ObraPendenciaRepository { get; }
        IObraRepository ObraRepository { get; }
        IObraRetrabalhoRepository ObraRetrabalhoRepository { get; }
        IObraServicoListaRepository ObraServicoListaRepository { get; }
        IObraServicoRepository ObraServicoRepository { get; }
        IOrcamentoItemRepository OrcamentoItemRepository { get; }
        IOrcamentoObraRepository OrcamentoObraRepository { get; }
        IOrcamentoRepository OrcamentoRepository { get; }
        IPerfilUsuarioRepository PerfilUsuarioRepository { get; }
        IProjetoRepository ProjetoRepository { get; }
        IServicoRepository ServicoRepository { get; }
        IUsuarioRepository UsuarioRepository { get; }
    }
}
