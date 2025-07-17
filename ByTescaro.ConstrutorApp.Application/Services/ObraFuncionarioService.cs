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
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;

            var entityAntigo = await _unitOfWork.ObraFuncionarioRepository.GetByIdAsync(dto.Id);
            if (entityAntigo == null) return;

            var entityNovo = _mapper.Map(dto, entityAntigo);
            _unitOfWork.ObraFuncionarioRepository.Update(entityNovo);
            await _auditoriaService.RegistrarAtualizacaoAsync(entityAntigo, entityNovo, usuarioLogadoId);

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
