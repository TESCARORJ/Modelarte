using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using Microsoft.AspNetCore.Http;

namespace ByTescaro.ConstrutorApp.Application.Services
{
    public class ObraFornecedorService : IObraFornecedorService
    {

        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditoriaService _auditoriaService;
        private readonly IUsuarioLogadoService _usuarioLogadoService;

        public ObraFornecedorService(IMapper mapper, IUnitOfWork unitOfWork, IAuditoriaService auditoriaService, IUsuarioLogadoService usuarioLogadoService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _auditoriaService = auditoriaService;
            _usuarioLogadoService = usuarioLogadoService;
        }

        public async Task<List<ObraFornecedorDto>> ObterPorObraIdAsync(long obraId)
        {
            var list = await _unitOfWork.ObraFornecedorRepository.GetByObraIdAsync(obraId);
            return _mapper.Map<List<ObraFornecedorDto>>(list);
        }

        public async Task CriarAsync(ObraFornecedorDto dto)
        {
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;
            var entity = _mapper.Map<ObraFornecedor>(dto);
            entity.DataHoraCadastro = DateTime.Now;
            entity.UsuarioCadastroId = usuarioLogadoId;
            _unitOfWork.ObraFornecedorRepository.Add(entity);
            await _auditoriaService.RegistrarCriacaoAsync(entity, usuarioLogadoId);
            await _unitOfWork.CommitAsync();

        }

        public async Task AtualizarAsync(ObraFornecedorDto dto)
        {
            // Obtém o ID do usuário logado (usando await para obter Task<T>.Result de forma segura)
            var usuarioLogado = await _usuarioLogadoService.ObterUsuarioAtualAsync();
            var usuarioLogadoId = usuarioLogado?.Id ?? 0;

            // 1. Busque a entidade antiga SOMENTE PARA FINS DE AUDITORIA, SEM RASTREAMENTO.
            // Essa instância 'obraFornecedorAntigoParaAuditoria' NÃO será modificada pelo AutoMapper,
            // preservando o estado original para o log de auditoria.
            var obraFornecedorAntigoParaAuditoria = await _unitOfWork.ObraFornecedorRepository.GetByIdNoTrackingAsync(dto.Id);

            if (obraFornecedorAntigoParaAuditoria == null)
            {
                // Se a entidade não foi encontrada, não há o que atualizar ou auditar.
                throw new KeyNotFoundException($"Obra Fornecedor com ID {dto.Id} não encontrado para auditoria.");
            }

            // 2. Busque a entidade que REALMENTE SERÁ ATUALIZADA, COM RASTREAMENTO.
            // Essa instância 'obraFornecedorParaAtualizar' é a que o EF Core está monitorando
            // e que terá suas propriedades alteradas e salvas no banco de dados.
            var obraFornecedorParaAtualizar = await _unitOfWork.ObraFornecedorRepository.GetByIdAsync(dto.Id);

            if (obraFornecedorParaAtualizar == null)
            {
                // Isso deve ser raro se 'obraFornecedorAntigoParaAuditoria' foi encontrado,
                // mas é uma boa verificação de segurança para o fluxo de atualização.
                throw new KeyNotFoundException($"Obra Fornecedor com ID {dto.Id} não encontrado para atualização.");
            }

            // 3. Mapeie as propriedades do DTO para a entidade 'obraFornecedorParaAtualizar' (a rastreada).
            // O AutoMapper irá aplicar as mudanças DIRETAMENTE nesta instância.
            _mapper.Map(dto, obraFornecedorParaAtualizar);

            // Se houver campos de auditoria de criação (UsuarioCadastroId, DataHoraCadastro)
            // que não devem ser alterados pelo DTO, você pode reatribuí-los aqui,
            // usando os valores de 'obraFornecedorAntigoParaAuditoria'.
            // Exemplo:
            // obraFornecedorParaAtualizar.UsuarioCadastroId = obraFornecedorAntigoParaAuditoria.UsuarioCadastroId;
            // obraFornecedorParaAtualizar.DataHoraCadastro = obraFornecedorAntigoParaAuditoria.DataHoraCadastro;

            // A chamada a .Update() no repositório é geralmente redundante se a entidade já está
            // rastreada e suas propriedades foram alteradas diretamente. O EF Core detecta essas mudanças automaticamente.
            // _unitOfWork.ObraFornecedorRepository.Update(obraFornecedorParaAtualizar);

            // 4. Registre a auditoria, passando a cópia original e a entidade atualizada.
            // 'obraFornecedorAntigoParaAuditoria' tem os dados ANTES da mudança.
            // 'obraFornecedorParaAtualizar' tem os dados DEPOIS da mudança.
            await _auditoriaService.RegistrarAtualizacaoAsync(obraFornecedorAntigoParaAuditoria, obraFornecedorParaAtualizar, usuarioLogadoId);

            // 5. Salve TODAS as alterações no banco de dados em uma única transação.
            await _unitOfWork.CommitAsync();
        }

        public async Task RemoverAsync(long id)
        {
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;

            var entity = await _unitOfWork.ObraFornecedorRepository.GetByIdAsync(id);
            if (entity != null)
                _unitOfWork.ObraFornecedorRepository.Remove(entity);

            await _auditoriaService.RegistrarExclusaoAsync(entity, usuarioLogadoId);
            await _unitOfWork.CommitAsync();
        }

        public async Task<List<FornecedorDto>> ObterFornecedoresDisponiveisAsync(long obraId)
        {
            var todosFornecedores = await _unitOfWork.FornecedorRepository.GetAllAsync();
            var fornecedoresAlocados = await _unitOfWork.ObraFornecedorRepository.GetAllAsync(); // Todos alocados em qualquer obra

            var idsAlocadosEmOutrasObras = fornecedoresAlocados
                .Where(f => f.ObraId != obraId)
                .Select(f => f.FornecedorId)
                .ToHashSet();

            var disponiveis = todosFornecedores
                .Where(f => !idsAlocadosEmOutrasObras.Contains(f.Id))
                .ToList();

            return _mapper.Map<List<FornecedorDto>>(disponiveis);
        }

        //public async Task<List<FornecedorDto>> ObterFornecedoresTotalDisponiveisAsync()
        //{
        //    var todosFornecedores = await _fornecedorRepository.GetAllAsync();
        //    var fornecedoresAlocados = await _unitOfWork.ObraFornecedorRepository.GetAllAsync(); // Todos alocados em qualquer obra

        //    var idsAlocadosEmOutrasObras = fornecedoresAlocados
        //        .Where(f => f.ObraId > 0)
        //        .Select(f => f.FornecedorId)

        //        .ToHashSet();

        //    var disponiveis = todosFornecedores
        //        .Where(f => !idsAlocadosEmOutrasObras.Contains(f.Id))
        //        .ToList();

        //    return _mapper.Map<List<FornecedorDto>>(disponiveis);
        //}

        public async Task<List<FornecedorDto>> ObterFornecedoresTotalDisponiveisAsync()
        {
            var todosFornecedores = await _unitOfWork.FornecedorRepository.GetAllAsync();


            return _mapper.Map<List<FornecedorDto>>(todosFornecedores);
        }


        public async Task<List<FornecedorDto>> ObterFornecedoresTotalAlocadosAsync()
        {
            // Busca todos os fornecedores alocados em alguma obra
            var fornecedoresAlocados = await _unitOfWork.ObraFornecedorRepository.GetAllAsync(); // ObraFornecedor
            var obras = await _unitOfWork.ObraRepository.GetAllAsync();
            var proejtos = await _unitOfWork.ProjetoRepository.GetAllAsync();
            var clientes = await _unitOfWork.ClienteRepository.GetAllAsync();
            var fornecedores = await _unitOfWork.FornecedorRepository.GetAllAsync();
            var funcoes = await _unitOfWork.FuncaoRepository.GetAllAsync();

            var resultado = fornecedoresAlocados
                .Select(eqAlocado =>
                {
                    var fornecedor = fornecedores.FirstOrDefault(e => e.Id == eqAlocado.FornecedorId);
                    var obra = obras.FirstOrDefault(o => o.Id == eqAlocado.ObraId);
                    var projeto = proejtos.FirstOrDefault(p => p.Id == obra.ProjetoId);
                    var cliente = clientes.FirstOrDefault(c => c.Id == projeto.ClienteId);

                    return new FornecedorDto
                    {
                        Id = fornecedor?.Id ?? 0,
                        Nome = fornecedor?.Nome ?? string.Empty,

                    };
                }).ToList();

            return resultado;


        }


    }


}
