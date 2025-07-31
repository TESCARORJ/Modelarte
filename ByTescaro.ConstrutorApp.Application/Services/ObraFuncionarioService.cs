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
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditoriaService _auditoriaService;
        private readonly IUsuarioLogadoService _usuarioLogadoService;

        public ObraFuncionarioService(IMapper mapper, IUnitOfWork unitOfWork, IAuditoriaService auditoriaService, IUsuarioLogadoService usuarioLogadoService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _auditoriaService = auditoriaService;
            _usuarioLogadoService = usuarioLogadoService;
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
            await _unitOfWork.CommitAsync();
            await _auditoriaService.RegistrarCriacaoAsync(entity, usuarioLogadoId);
        }

        public async Task AtualizarAsync(ObraFuncionarioDto dto)
        {
            var usuarioLogado = await _usuarioLogadoService.ObterUsuarioAtualAsync();
            var usuarioLogadoId = usuarioLogado?.Id ?? 0;

            var obraFuncionarioAntigoParaAuditoria = await _unitOfWork.ObraFuncionarioRepository.GetByIdNoTrackingAsync(dto.Id);

            if (obraFuncionarioAntigoParaAuditoria == null)
            {
                throw new KeyNotFoundException($"Obra Funcionário com ID {dto.Id} não encontrado para auditoria.");
            }

        
            var obraFuncionarioParaAtualizar = await _unitOfWork.ObraFuncionarioRepository.GetByIdAsync(dto.Id);

            if (obraFuncionarioParaAtualizar == null)
            {

                throw new KeyNotFoundException($"Obra Funcionário com ID {dto.Id} não encontrado para atualização.");
            }


            _mapper.Map(dto, obraFuncionarioParaAtualizar);


            await _unitOfWork.CommitAsync();

            await _auditoriaService.RegistrarAtualizacaoAsync(obraFuncionarioAntigoParaAuditoria, obraFuncionarioParaAtualizar, usuarioLogadoId);

        }
        public async Task RemoverAsync(long id)
        {
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;

            var entity = await _unitOfWork.ObraFuncionarioRepository.GetByIdAsync(id);
            if (entity != null)
                _unitOfWork.ObraFuncionarioRepository.Remove(entity);

            await _unitOfWork.CommitAsync();
            await _auditoriaService.RegistrarExclusaoAsync(entity, usuarioLogadoId);


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
                .GetAllIncludingAsync(x => x.Ativo == true, f => f.Funcao); 

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
