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
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;
            var entityAntigo = await _unitOfWork.ObraFornecedorRepository.GetByIdAsync(dto.Id);
            if (entityAntigo == null) return;

            var entityNovo = _mapper.Map(dto, entityAntigo);
            _unitOfWork.ObraFornecedorRepository.Update(entityNovo);
            await _auditoriaService.RegistrarAtualizacaoAsync(entityAntigo, entityNovo, usuarioLogadoId);
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
