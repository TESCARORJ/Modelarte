using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using Microsoft.AspNetCore.Http;

namespace ByTescaro.ConstrutorApp.Application.Services
{
    public class ObraFuncionarioService : IObraFuncionarioService
    {
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditoriaService _auditoriaService;
        private readonly IUsuarioLogadoService _usuarioLogadoService;

        public ObraFuncionarioService(IMapper mapper, IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;            
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<ObraFuncionarioDto>> ObterPorObraIdAsync(long obraId)
        {
            var list = await _unitOfWork.ObraFuncionarioRepository.FindAllWithIncludesAsync(x => x.ObraId == obraId);
            return _mapper.Map<List<ObraFuncionarioDto>>(list);
        }

        public async Task CriarAsync(ObraFuncionarioDto dto)
        {
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;
            var entity = _mapper.Map<ObraFuncionario>(dto);
            entity.DataHoraCadastro = DateTime.Now;
            entity.UsuarioCadastroId = usuarioLogadoId;
            _unitOfWork.ObraFuncionarioRepository.Add(entity);
            await _auditoriaService.RegistrarCriacaoAsync(entity, usuarioLogadoId);
            await _unitOfWork.CommitAsync();
        }

        public async Task AtualizarAsync(ObraFuncionarioDto dto)
        {
            // Obtém o ID do usuário logado (usando await para obter Task<T>.Result de forma segura)
            var usuarioLogado = await _usuarioLogadoService.ObterUsuarioAtualAsync();
            var usuarioLogadoId = usuarioLogado?.Id ?? 0;

            // 1. Busque a entidade antiga SOMENTE PARA FINS DE AUDITORIA, SEM RASTREAMENTO.
            // Essa instância 'obraFuncionarioAntigoParaAuditoria' NÃO será modificada pelo AutoMapper,
            // preservando o estado original para o log de auditoria.
            var obraFuncionarioAntigoParaAuditoria = await _unitOfWork.ObraFuncionarioRepository.GetByIdNoTrackingAsync(dto.Id);

            if (obraFuncionarioAntigoParaAuditoria == null)
            {
                // Se a entidade não foi encontrada, não há o que atualizar ou auditar.
                throw new KeyNotFoundException($"Obra Funcionário com ID {dto.Id} não encontrado para auditoria.");
            }

            // 2. Busque a entidade que REALMENTE SERÁ ATUALIZADA, COM RASTREAMENTO.
            // Essa instância 'obraFuncionarioParaAtualizar' é a que o EF Core está monitorando
            // e que terá suas propriedades alteradas e salvas no banco de dados.
            var obraFuncionarioParaAtualizar = await _unitOfWork.ObraFuncionarioRepository.GetByIdAsync(dto.Id);

            if (obraFuncionarioParaAtualizar == null)
            {
                // Isso deve ser raro se 'obraFuncionarioAntigoParaAuditoria' foi encontrado,
                // mas é uma boa verificação de segurança para o fluxo de atualização.
                throw new KeyNotFoundException($"Obra Funcionário com ID {dto.Id} não encontrado para atualização.");
            }

            // 3. Mapeie as propriedades do DTO para a entidade 'obraFuncionarioParaAtualizar' (a rastreada).
            // O AutoMapper irá aplicar as mudanças DIRETAMENTE nesta instância.
            _mapper.Map(dto, obraFuncionarioParaAtualizar);

            // Se houver campos de auditoria de criação (UsuarioCadastroId, DataHoraCadastro)
            // que não devem ser alterados pelo DTO, você pode reatribuí-los aqui,
            // usando os valores de 'obraFuncionarioAntigoParaAuditoria'.
            // Exemplo:
            // obraFuncionarioParaAtualizar.UsuarioCadastroId = obraFuncionarioAntigoParaAuditoria.UsuarioCadastroId;
            // obraFuncionarioParaAtualizar.DataHoraCadastro = obraFuncionarioAntigoParaAuditoria.DataHoraCadastro;

            // A chamada a .Update() no repositório é geralmente redundante se a entidade já está
            // rastreada e suas propriedades foram alteradas diretamente. O EF Core detecta essas mudanças automaticamente.
            // _unitOfWork.ObraFuncionarioRepository.Update(obraFuncionarioParaAtualizar);

            // 4. Registre a auditoria, passando a cópia original e a entidade atualizada.
            // 'obraFuncionarioAntigoParaAuditoria' tem os dados ANTES da mudança.
            // 'obraFuncionarioParaAtualizar' tem os dados DEPOIS da mudança.
            await _auditoriaService.RegistrarAtualizacaoAsync(obraFuncionarioAntigoParaAuditoria, obraFuncionarioParaAtualizar, usuarioLogadoId);

            // 5. Salve TODAS as alterações no banco de dados em uma única transação.
            await _unitOfWork.CommitAsync();
        }
        public async Task RemoverAsync(long id)
        {
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;

            var entity = await _unitOfWork.ObraFuncionarioRepository.GetByIdAsync(id);
            if (entity != null)
                _unitOfWork.ObraFuncionarioRepository.Remove(entity);

            await _auditoriaService.RegistrarExclusaoAsync(entity, usuarioLogadoId);


            await _unitOfWork.CommitAsync();
        }

        public async Task<List<FuncionarioDto>> ObterFuncionariosDisponiveisAsync(long obraId)
        {

            var todosFuncionarios = await _unitOfWork.FuncionarioRepository.GetAllAsync();
            var funcionariosAlocados = await _unitOfWork.ObraFuncionarioRepository.GetAllAsync(); // Todos alocados em qualquer obra

            var idsAlocadosEmOutrasObras = funcionariosAlocados
                .Where(f => f.ObraId != obraId)
                .Select(f => f.FuncionarioId)
                .ToHashSet();

            var disponiveis = todosFuncionarios
                .Where(f => !idsAlocadosEmOutrasObras.Contains(f.Id))
                .ToList();

            return _mapper.Map<List<FuncionarioDto>>(disponiveis);
        }


        public async Task<List<FuncionarioDto>> ObterFuncionariosTotalDisponiveisAsync()
        {
            var todosFuncionarios = await _unitOfWork.FuncionarioRepository
                .GetAllIncludingAsync(f => f.Funcao); 

            var disponiveis = todosFuncionarios.ToList();

            return _mapper.Map<List<FuncionarioDto>>(disponiveis);
        }


        public async Task<List<FuncionarioDto>> ObterFuncionariosTotalAlocadosAsync()
        {
            // Busca todos os funcionarios alocados em alguma obra
            var funcionariosAlocados = await _unitOfWork.ObraFuncionarioRepository.GetAllAsync(); // ObraFuncionario
            var obras = await _unitOfWork.ObraRepository.GetAllAsync();
            var proejtos = await _unitOfWork.ProjetoRepository.GetAllAsync();
            var clientes = await _unitOfWork.ClienteRepository.GetAllAsync();
            var funcionarios = await _unitOfWork.FuncionarioRepository.GetAllAsync();
            var funcoes = await _unitOfWork.FuncaoRepository.GetAllAsync();

            var resultado = funcionariosAlocados
                .Select(eqAlocado =>
                {
                    var funcionario = funcionarios.FirstOrDefault(e => e.Id == eqAlocado.FuncionarioId);
                    var obra = obras.FirstOrDefault(o => o.Id == eqAlocado.ObraId);
                    var projeto = proejtos.FirstOrDefault(p => p.Id == obra.ProjetoId);
                    var cliente = clientes.FirstOrDefault(c => c.Id == projeto.ClienteId);
                    var funcao = funcoes.FirstOrDefault(f => f.Id == funcionario.FuncaoId);

                    return new FuncionarioDto
                    {
                        Id = funcionario?.Id ?? 0,
                        Nome = funcionario?.Nome ?? string.Empty,
                        ObraNome = obra?.Nome ?? string.Empty,
                        ProjetoNome = projeto?.Nome ?? string.Empty,
                        ClienteNome = cliente.Nome ?? string.Empty,
                        FuncaoNome = funcao.Nome
                    };
                }).ToList();

            return resultado;


        }


    }


}
