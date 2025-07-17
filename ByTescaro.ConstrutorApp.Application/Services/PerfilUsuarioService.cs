using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using ByTescaro.ConstrutorApp.Domain.Entities.Admin;

namespace ByTescaro.ConstrutorApp.Application.Services;

public class PerfilUsuarioService : IPerfilUsuarioService
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditoriaService _auditoriaService;
    private readonly IUsuarioLogadoService _usuarioLogadoService;

    public PerfilUsuarioService(IMapper mapper, IUnitOfWork unitOfWork, IAuditoriaService auditoriaService, IUsuarioLogadoService usuarioLogadoService)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _auditoriaService = auditoriaService;
        _usuarioLogadoService = usuarioLogadoService;
    }

    public async Task<IEnumerable<PerfilUsuarioDto>> ObterTodosAsync()
    {
        var perfilUsuarios = await _unitOfWork.PerfilUsuarioRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<PerfilUsuarioDto>>(perfilUsuarios);
    }

    public async Task<PerfilUsuarioDto?> ObterPorIdAsync(long id)
    {
        var perfilUsuario = await _unitOfWork.PerfilUsuarioRepository.GetByIdAsync(id);
        return perfilUsuario == null ? null : _mapper.Map<PerfilUsuarioDto>(perfilUsuario);
    }

    public async Task CriarAsync(PerfilUsuarioDto dto)
    {
        var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
        var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;

        var entity = _mapper.Map<PerfilUsuario>(dto);
        entity.DataHoraCadastro = DateTime.Now;
        entity.UsuarioCadastroId = usuarioLogadoId;

        _unitOfWork.PerfilUsuarioRepository.Add(entity);

        await _auditoriaService.RegistrarCriacaoAsync(entity, usuarioLogadoId);

        await _unitOfWork.CommitAsync();
    }

    public async Task AtualizarAsync(PerfilUsuarioDto dto)
    {
        var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
        var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;

        var entityAntigo = await _unitOfWork.PerfilUsuarioRepository.GetByIdAsync(dto.Id);
        if (entityAntigo == null) return;

        var entityNovo = _mapper.Map<PerfilUsuario>(dto);
        entityNovo.UsuarioCadastroId = entityAntigo.Id;
        entityNovo.DataHoraCadastro = entityAntigo.DataHoraCadastro;

        _unitOfWork.PerfilUsuarioRepository.Update(entityNovo);

        await _auditoriaService.RegistrarAtualizacaoAsync(entityAntigo, entityNovo, usuarioLogadoId);
        await _unitOfWork.CommitAsync();
    }

    public async Task RemoverAsync(long id)
    {
        var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
        var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;

        var entity = await _unitOfWork.PerfilUsuarioRepository.GetByIdAsync(id);
        if (entity == null) return;

        _unitOfWork.PerfilUsuarioRepository.Remove(entity);
        await _auditoriaService.RegistrarExclusaoAsync(entity, usuarioLogadoId);
        await _unitOfWork.CommitAsync();
    }

   
}
