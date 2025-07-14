using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Entities.Admin;
using ByTescaro.ConstrutorApp.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ByTescaro.ConstrutorApp.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options)
{
    #region === DbSets ===

    // --- Entidades Principais com Herança ---
    public DbSet<Pessoa> Pessoas => Set<Pessoa>();
    public DbSet<Endereco> Enderecos => Set<Endereco>();

    // --- Tipos específicos (Opcionais, mas úteis para queries diretas) ---
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Funcionario> Funcionarios => Set<Funcionario>();
    public DbSet<Fornecedor> Fornecedores => Set<Fornecedor>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();

    // --- Administração ---
    public DbSet<PerfilUsuario> PerfilUsuario => Set<PerfilUsuario>();
    public DbSet<Funcao> Funcao => Set<Funcao>();

    // --- Cadastros ---
    public DbSet<Equipamento> Equipamento => Set<Equipamento>();
    public DbSet<Insumo> Insumo => Set<Insumo>();
    public DbSet<FornecedorInsumo> FornecedorInsumo => Set<FornecedorInsumo>();
    public DbSet<Servico> Servico => Set<Servico>();
    public DbSet<FornecedorServico> FornecedorServico => Set<FornecedorServico>();

    // --- Projetos e Obras ---
    public DbSet<Projeto> Projeto => Set<Projeto>();
    public DbSet<Obra> Obra => Set<Obra>();
    public DbSet<ObraFuncionario> ObraFuncionario => Set<ObraFuncionario>();
    public DbSet<ObraFornecedor> ObraFornecedor => Set<ObraFornecedor>();
    public DbSet<ObraInsumo> ObraInsumo => Set<ObraInsumo>();
    public DbSet<ObraInsumoLista> ObraInsumoLista => Set<ObraInsumoLista>();
    public DbSet<ObraServico> ObraServico => Set<ObraServico>();
    public DbSet<ObraServicoLista> ObraServicoLista => Set<ObraServicoLista>();
    public DbSet<ObraEquipamento> ObraEquipamento => Set<ObraEquipamento>();
    public DbSet<ObraRetrabalho> ObraRetrabalho => Set<ObraRetrabalho>();
    public DbSet<ObraPendencia> ObraPendencia => Set<ObraPendencia>();
    public DbSet<ObraDocumento> ObraDocumento => Set<ObraDocumento>();
    public DbSet<ObraImagem> ObraImagem => Set<ObraImagem>();

    // --- Execução ---
    public DbSet<ObraEtapaPadrao> ObraEtapaPadrao => Set<ObraEtapaPadrao>();
    public DbSet<ObraItemEtapaPadrao> ObraItemEtapaPadrao => Set<ObraItemEtapaPadrao>();
    public DbSet<ObraEtapa> ObraEtapa => Set<ObraEtapa>();
    public DbSet<ObraItemEtapa> ObraItemEtapa => Set<ObraItemEtapa>();

    // --- Auditoria ---
    public DbSet<LogAuditoria> LogAuditoria => Set<LogAuditoria>();

    // --- Relacionamentos N-N ---
    public DbSet<ObraItemEtapaPadraoInsumo> ObraItemEtapaPadraoInsumos => Set<ObraItemEtapaPadraoInsumo>();

    // --- Orçamento ---
    public DbSet<Orcamento> Orcamento => Set<Orcamento>();
    public DbSet<OrcamentoItem> OrcamentoItem => Set<OrcamentoItem>();
    public DbSet<OrcamentoObra> OrcamentoObra => Set<OrcamentoObra>();

    #endregion

    #region COMPROMISSO
    public DbSet<Evento> Eventos { get; set; }
    public DbSet<ParticipanteEvento> ParticipantesEvento { get; set; }
    public DbSet<LembreteEvento> LembretesEvento { get; set; }
    public DbSet<ConfiguracaoLembreteDiario> ConfiguracoesLembreteDiario { get; set; } 

    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // O comportamento de exclusão será definida em cada classe.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

    }

    #region Configurações de Entidades

    public class PessoaConfiguration : IEntityTypeConfiguration<Pessoa>
    {
        public void Configure(EntityTypeBuilder<Pessoa> builder)
        {
            builder.ToTable("Pessoa");

            // Configura a estratégia de herança Table-per-Hierarchy (TPH)
            // A coluna "TipoEntidade" será o Discriminator para saber qual tipo de pessoa cada registro é.
            //builder.HasDiscriminator(p => p.TipoEntidade)
            //       .HasValue<Cliente>(TipoEntidadePessoa.Cliente)
            //       .HasValue<Funcionario>(TipoEntidadePessoa.Funcionario)
            //       .HasValue<Fornecedor>(TipoEntidadePessoa.Fornecedor)
            //       .HasValue<Usuario>(TipoEntidadePessoa.Usuario);

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Nome).HasMaxLength(100);
            builder.Property(e => e.CpfCnpj).HasMaxLength(20);
            builder.Property(e => e.Email).HasMaxLength(100);
            builder.Property(e => e.UsuarioCadastro).IsRequired().HasMaxLength(100);

            // Relacionamento com Endereco (One-to-One opcional)
            builder.HasOne(p => p.Endereco)
              .WithMany() // ou WithOne() se for 1:1, mas exige HasForeignKey(e => e.PessoaId) em Endereco
              .HasForeignKey(p => p.EnderecoId) // EnderecoId é a FK em Pessoa
              .IsRequired(false) // Permite que EnderecoId seja NULL
              .OnDelete(DeleteBehavior.SetNull); // Se o Endereco for deletado, a FK em Pessoa vira NULL
        }
    }

    public class EnderecoConfiguration : IEntityTypeConfiguration<Endereco>
    {
        public void Configure(EntityTypeBuilder<Endereco> builder)
        {
            builder.ToTable("Endereco");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Logradouro).HasMaxLength(255);
            builder.Property(e => e.Numero).HasMaxLength(50);
            builder.Property(e => e.Bairro).HasMaxLength(150);
            builder.Property(e => e.Cidade).HasMaxLength(150);
            builder.Property(e => e.Estado).HasMaxLength(100);
            builder.Property(e => e.UF).HasMaxLength(2);
            builder.Property(e => e.CEP).HasMaxLength(9);
        }
    }

    public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
    {
        public void Configure(EntityTypeBuilder<Cliente> builder)
        {
            builder.ToTable("Cliente");

        }
    }

    public class FuncionarioConfiguration : IEntityTypeConfiguration<Funcionario>
    {
        public void Configure(EntityTypeBuilder<Funcionario> builder)
        {
            builder.ToTable("Funcionario");

            // Configura apenas as propriedades específicas de Funcionario
            builder.Property(e => e.Salario).HasColumnType("decimal(18, 2)");

            builder.HasOne(e => e.Funcao)
                   .WithMany() // Supondo que uma Função pode pertencer a vários funcionários
                   .HasForeignKey(e => e.FuncaoId);
        }
    }

    public class FornecedorConfiguration : IEntityTypeConfiguration<Fornecedor>
    {
        public void Configure(EntityTypeBuilder<Fornecedor> builder)
        {
            builder.ToTable("Fornecedor");

            builder.Property(e => e.Tipo).IsRequired();
        }
    }

    public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.ToTable("Usuario");


            builder.Property(e => e.SenhaHash).IsRequired();
            builder.Property(e => e.Sobrenome).HasMaxLength(100);

            builder.HasOne(e => e.PerfilUsuario)
                   .WithMany(p => p.Usuarios)
                   .HasForeignKey(e => e.PerfilUsuarioId);
        }
    }

    public class ProjetoConfiguration : IEntityTypeConfiguration<Projeto>
    {
        public void Configure(EntityTypeBuilder<Projeto> builder)
        {
            builder.ToTable("Projeto");
            builder.HasKey(e => e.Id);

            builder.HasOne(p => p.Cliente)
                   .WithMany(c => c.Projetos)
                   .HasForeignKey(p => p.ClienteId)
                   .OnDelete(DeleteBehavior.Cascade); // Se deletar um cliente, seus projetos são deletados.

            builder.HasOne(p => p.Endereco)
                   .WithMany()
                   .HasForeignKey(p => p.EnderecoId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class ObraConfiguration : IEntityTypeConfiguration<Obra>
    {
        public void Configure(EntityTypeBuilder<Obra> builder)
        {
            builder.HasOne(d => d.ResponsavelObra)
                  .WithMany() // Um funcionário pode ser responsável por várias obras
                  .HasForeignKey(d => d.ResponsavelObraId)
                  .OnDelete(DeleteBehavior.NoAction); // Não deletar o funcionário se a obra for removida

            builder.HasOne(o => o.Projeto)
                   .WithMany(p => p.Obras)
                   .HasForeignKey(o => o.ProjetoId)
                   .OnDelete(DeleteBehavior.Cascade); // Se deletar o projeto, suas obras são deletadas.
        }
    }


    public class EquipamentoConfiguration : IEntityTypeConfiguration<Equipamento>
    {
        public void Configure(EntityTypeBuilder<Equipamento> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Nome).IsRequired().HasMaxLength(100);
            builder.Property(e => e.CustoLocacaoDiaria).HasPrecision(18, 2);
            builder.Property(e => e.DataHoraCadastro).IsRequired();
            builder.Property(e => e.UsuarioCadastro).IsRequired().HasMaxLength(100);
        }
    }

    public class InsumoConfiguration : IEntityTypeConfiguration<Insumo>
    {
        public void Configure(EntityTypeBuilder<Insumo> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Nome).IsRequired().HasMaxLength(100);
            builder.Property(e => e.DataHoraCadastro).IsRequired();
            builder.Property(e => e.UsuarioCadastro).IsRequired().HasMaxLength(100);
        }
    }

    public class ServicoConfiguration : IEntityTypeConfiguration<Servico>
    {
        public void Configure(EntityTypeBuilder<Servico> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Nome).IsRequired().HasMaxLength(100);
            builder.Property(e => e.DataHoraCadastro).IsRequired();
            builder.Property(e => e.UsuarioCadastro).IsRequired().HasMaxLength(100);
        }
    }

    public class FuncaoConfiguration : IEntityTypeConfiguration<Funcao>
    {
        public void Configure(EntityTypeBuilder<Funcao> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Nome).IsRequired().HasMaxLength(100);
            builder.Property(e => e.Ativo).IsRequired();
        }
    }

    public class PerfilUsuarioConfiguration : IEntityTypeConfiguration<PerfilUsuario>
    {
        public void Configure(EntityTypeBuilder<PerfilUsuario> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Nome).IsRequired().HasMaxLength(100);
        }
    }

    public class FornecedorInsumoConfiguration : IEntityTypeConfiguration<FornecedorInsumo>
    {
        public void Configure(EntityTypeBuilder<FornecedorInsumo> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.PrecoUnitario).HasPrecision(18, 2);
            builder.Property(e => e.DataHoraCadastro).IsRequired();
            builder.Property(e => e.UsuarioCadastro).IsRequired().HasMaxLength(100);

            builder.HasOne(e => e.Fornecedor).WithMany().HasForeignKey(e => e.FornecedorId);
            builder.HasOne(e => e.Insumo).WithMany().HasForeignKey(e => e.InsumoId);
        }
    }

    public class FornecedorServicoConfiguration : IEntityTypeConfiguration<FornecedorServico>
    {
        public void Configure(EntityTypeBuilder<FornecedorServico> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.PrecoUnitario).HasPrecision(18, 2);
            builder.Property(e => e.DataHoraCadastro).IsRequired();
            builder.Property(e => e.UsuarioCadastro).IsRequired().HasMaxLength(100);

            builder.HasOne(e => e.Fornecedor).WithMany().HasForeignKey(e => e.FornecedorId);
            builder.HasOne(e => e.Servico).WithMany().HasForeignKey(e => e.ServicoId);
        }
    }


    public class ObraFuncionarioConfiguration : IEntityTypeConfiguration<ObraFuncionario>
    {
        public void Configure(EntityTypeBuilder<ObraFuncionario> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.FuncionarioNome).HasMaxLength(100);
            builder.Property(e => e.FuncaoNoObra).HasMaxLength(100);
            builder.Property(e => e.DataInicio).IsRequired();
            builder.Property(e => e.DataHoraCadastro).IsRequired();
            builder.Property(e => e.UsuarioCadastro).IsRequired().HasMaxLength(100);

            builder.HasOne(e => e.Obra)
                .WithMany(o => o.Funcionarios)
                .HasForeignKey(e => e.ObraId)
                .OnDelete(DeleteBehavior.Restrict); 

            builder.HasOne(e => e.Funcionario)
                .WithMany()
                .HasForeignKey(e => e.FuncionarioId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class ObraFornecedorConfiguration : IEntityTypeConfiguration<ObraFornecedor>
    {
        public void Configure(EntityTypeBuilder<ObraFornecedor> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.FornecedorNome).HasMaxLength(100);
            builder.Property(e => e.DataInicio).IsRequired();
            builder.Property(e => e.DataHoraCadastro).IsRequired();
            builder.Property(e => e.UsuarioCadastro).IsRequired().HasMaxLength(100);

            builder.HasOne(e => e.Obra)
                .WithMany(o => o.Fornecedores)
                .HasForeignKey(e => e.ObraId)
                .OnDelete(DeleteBehavior.Restrict); 

            builder.HasOne(e => e.Fornecedor)
                .WithMany()
                .HasForeignKey(e => e.FornecedorId);
        }
    }
    public class ObraEquipamentoConfiguration : IEntityTypeConfiguration<ObraEquipamento>
    {
        public void Configure(EntityTypeBuilder<ObraEquipamento> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.DataInicioUso).IsRequired();
            builder.Property(e => e.DataHoraCadastro).IsRequired();
            builder.Property(e => e.UsuarioCadastro).IsRequired().HasMaxLength(100);

            builder.HasOne(e => e.Obra).WithMany(o => o.Equipamentos).HasForeignKey(e => e.ObraId);
        }
    }

    public class ObraInsumoConfiguration : IEntityTypeConfiguration<ObraInsumo>
    {
        public void Configure(EntityTypeBuilder<ObraInsumo> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Quantidade).HasPrecision(18, 2);
            builder.Property(e => e.DataHoraCadastro).IsRequired();
            builder.Property(e => e.UsuarioCadastro).IsRequired().HasMaxLength(100);

            builder.HasOne(e => e.Lista).WithMany(l => l.Itens).HasForeignKey(e => e.ObraInsumoListaId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(e => e.Insumo).WithMany().HasForeignKey(e => e.InsumoId);
        }
    }

    public class ObraInsumoListaConfiguration : IEntityTypeConfiguration<ObraInsumoLista>
    {
        public void Configure(EntityTypeBuilder<ObraInsumoLista> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Data).IsRequired();

            builder.HasOne(e => e.Obra).WithMany(o => o.ListasInsumo).HasForeignKey(e => e.ObraId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(e => e.Responsavel).WithMany().HasForeignKey(e => e.ResponsavelId);
        }
    }

    public class ObraServicoConfiguration : IEntityTypeConfiguration<ObraServico>
    {
        public void Configure(EntityTypeBuilder<ObraServico> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Quantidade).HasPrecision(18, 2);
            builder.Property(e => e.DataHoraCadastro).IsRequired();
            builder.Property(e => e.UsuarioCadastro).IsRequired().HasMaxLength(100);

            builder.HasOne(e => e.Lista).WithMany(l => l.Itens).HasForeignKey(e => e.ObraServicoListaId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(e => e.Servico).WithMany().HasForeignKey(e => e.ServicoId);
        }
    }

    public class ObraServicoListaConfiguration : IEntityTypeConfiguration<ObraServicoLista>
    {
        public void Configure(EntityTypeBuilder<ObraServicoLista> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Data).IsRequired();

            builder.HasOne(e => e.Obra).WithMany(o => o.ListasServico).HasForeignKey(e => e.ObraId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(e => e.Responsavel).WithMany().HasForeignKey(e => e.ResponsavelId);
        }
    }

    public class ObraEtapaConfiguration : IEntityTypeConfiguration<ObraEtapa>
    {
        public void Configure(EntityTypeBuilder<ObraEtapa> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Nome).IsRequired().HasMaxLength(150);
            builder.Property(e => e.Ordem).IsRequired();
            builder.Property(e => e.Status).IsRequired();

            builder.HasOne(e => e.Obra).WithMany(o => o.Etapas).HasForeignKey(e => e.ObraId);
        }
    }

    public class ObraItemEtapaConfiguration : IEntityTypeConfiguration<ObraItemEtapa>
    {
        public void Configure(EntityTypeBuilder<ObraItemEtapa> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Nome).IsRequired().HasMaxLength(200);
            builder.Property(e => e.Ordem).IsRequired();
            builder.Property(e => e.Concluido).IsRequired();
            builder.Property(e => e.DataHoraCadastro).IsRequired();
            builder.Property(e => e.UsuarioCadastro).IsRequired().HasMaxLength(100);

            builder.HasOne(e => e.ObraEtapa).WithMany(e => e.Itens).HasForeignKey(e => e.ObraEtapaId);
        }
    }

    public class ObraEtapaPadraoConfiguration : IEntityTypeConfiguration<ObraEtapaPadrao>
    {
        public void Configure(EntityTypeBuilder<ObraEtapaPadrao> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Nome).IsRequired().HasMaxLength(150);
            builder.Property(e => e.Ordem).IsRequired();
            builder.Property(e => e.DataHoraCadastro).IsRequired();
            builder.Property(e => e.UsuarioCadastro).IsRequired().HasMaxLength(100);
        }
    }

    public class ObraItemEtapaPadraoConfiguration : IEntityTypeConfiguration<ObraItemEtapaPadrao>
    {
        public void Configure(EntityTypeBuilder<ObraItemEtapaPadrao> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Nome).IsRequired().HasMaxLength(200);
            builder.Property(e => e.Ordem).IsRequired();
            builder.Property(e => e.DataHoraCadastro).IsRequired();
            builder.Property(e => e.UsuarioCadastro).IsRequired().HasMaxLength(100);

            builder.HasOne(e => e.ObraEtapaPadrao)
                   .WithMany(eo => eo.Itens)
                   .HasForeignKey(e => e.ObraEtapaPadraoId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class ObraItemEtapaPadraoInsumoConfiguration : IEntityTypeConfiguration<ObraItemEtapaPadraoInsumo>
    {
        public void Configure(EntityTypeBuilder<ObraItemEtapaPadraoInsumo> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.QuantidadeSugerida).HasPrecision(18, 2);
            builder.Property(e => e.DataHoraCadastro).IsRequired();
            builder.Property(e => e.UsuarioCadastro).IsRequired().HasMaxLength(100);

            builder.HasOne(e => e.ObraItemEtapaPadrao)
                   .WithMany(p => p.Insumos)
                   .HasForeignKey(e => e.ObraItemEtapaPadraoId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class ObraRetrabalhoConfiguration : IEntityTypeConfiguration<ObraRetrabalho>
    {
        public void Configure(EntityTypeBuilder<ObraRetrabalho> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Titulo).IsRequired().HasMaxLength(150);
            builder.Property(e => e.Descricao).HasMaxLength(500);
            builder.Property(e => e.Status).IsRequired();
            builder.Property(e => e.DataHoraCadastro).IsRequired();
            builder.Property(e => e.UsuarioCadastro).IsRequired().HasMaxLength(100);

            builder.HasOne(e => e.Obra).WithMany(o => o.Retrabalhos).HasForeignKey(e => e.ObraId);
            builder.HasOne(e => e.Responsavel).WithMany().HasForeignKey(e => e.ResponsavelId);
        }
    }

    public class ObraPendenciaConfiguration : IEntityTypeConfiguration<ObraPendencia>
    {
        public void Configure(EntityTypeBuilder<ObraPendencia> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Titulo).IsRequired().HasMaxLength(150);
            builder.Property(e => e.Descricao).HasMaxLength(500);
            builder.Property(e => e.Status).IsRequired();
            builder.Property(e => e.DataHoraCadastro).IsRequired();
            builder.Property(e => e.UsuarioCadastro).IsRequired().HasMaxLength(100);

            builder.HasOne(e => e.Obra).WithMany(o => o.Pendencias).HasForeignKey(e => e.ObraId);
            builder.HasOne(e => e.Responsavel).WithMany().HasForeignKey(e => e.ResponsavelId);
        }
    }

    public class ObraDocumentoConfiguration : IEntityTypeConfiguration<ObraDocumento>
    {
        public void Configure(EntityTypeBuilder<ObraDocumento> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.NomeOriginal).IsRequired().HasMaxLength(255);
            builder.Property(e => e.CaminhoRelativo).IsRequired().HasMaxLength(500);
            builder.Property(e => e.Extensao).IsRequired().HasMaxLength(10);
            builder.Property(e => e.TamanhoEmKb).HasPrecision(18, 2);
            builder.Property(e => e.DataHoraCadastro).IsRequired();
            builder.Property(e => e.UsuarioCadastro).IsRequired().HasMaxLength(100);

            builder.HasOne(e => e.Obra).WithMany(o => o.Documentos).HasForeignKey(e => e.ObraId);
        }
    }

    public class ObraImagemConfiguration : IEntityTypeConfiguration<ObraImagem>
    {
        public void Configure(EntityTypeBuilder<ObraImagem> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.NomeOriginal).IsRequired().HasMaxLength(255);
            builder.Property(e => e.CaminhoRelativo).IsRequired().HasMaxLength(500);
            builder.Property(e => e.TamanhoEmKb).HasPrecision(18, 2);
            builder.Property(e => e.DataHoraCadastro).IsRequired();
            builder.Property(e => e.UsuarioCadastro).IsRequired().HasMaxLength(100);

            builder.HasOne(e => e.Obra).WithMany(o => o.Imagens).HasForeignKey(e => e.ObraId);
        }
    }

    public class EventoConfiguration : IEntityTypeConfiguration<Evento>
    {
        public void Configure(EntityTypeBuilder<Evento> builder)
        {
            builder.HasKey(e => e.Id);

            builder.HasOne(e => e.Criador)
                .WithMany()
                .HasForeignKey(e => e.CriadorId);

            builder.Property(e => e.Titulo).HasMaxLength(200);
            builder.Property(e => e.Descricao).HasMaxLength(2000);

        }
    }

    public class ParticipanteEventoConfiguration : IEntityTypeConfiguration<ParticipanteEvento>
    {
        public void Configure(EntityTypeBuilder<ParticipanteEvento> builder)
        {
            builder.HasKey(pe => pe.Id);

            builder.HasOne(pe => pe.Evento)
                .WithMany(e => e.Participantes)
                .HasForeignKey(pe => pe.EventoId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(pe => pe.Usuario)
                .WithMany()
                .HasForeignKey(pe => pe.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict); 

            builder.Property(pe => pe.StatusParticipacao).IsRequired(false);
            builder.Property(pe => pe.DataHoraCadastro).IsRequired();
            builder.Property(pe => pe.UsuarioCadastro).HasMaxLength(100);
        }
    }

    public class LembreteEventoConfiguration : IEntityTypeConfiguration<LembreteEvento>
    {
        public void Configure(EntityTypeBuilder<LembreteEvento> builder)
        {
            builder.HasKey(le => le.Id);

            builder.HasOne(le => le.Evento)
                .WithMany(e => e.Lembretes)
                .HasForeignKey(le => le.EventoId);


        }
    }
    public class ConfiguracaoLembreteDiarioConfiguration : IEntityTypeConfiguration<ConfiguracaoLembreteDiario>
    {
        public void Configure(EntityTypeBuilder<ConfiguracaoLembreteDiario> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.UsuarioCadastro)
                .WithMany()
                .HasForeignKey(x => x.UsuarioCadastroId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }

    #endregion

}