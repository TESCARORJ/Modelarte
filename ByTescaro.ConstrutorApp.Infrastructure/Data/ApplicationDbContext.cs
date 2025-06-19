using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Entities.Admin;
using Microsoft.EntityFrameworkCore;

namespace ByTescaro.ConstrutorApp.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options)
{
    // === Administração ===
    public DbSet<Usuario> Usuario => Set<Usuario>();
    public DbSet<PerfilUsuario> PerfilUsuario => Set<PerfilUsuario>();

    // === Cadastros ===
    public DbSet<Cliente> Cliente => Set<Cliente>();
    public DbSet<Funcionario> Funcionario => Set<Funcionario>();
    public DbSet<Funcao> Funcao => Set<Funcao>();
    public DbSet<Equipamento> Equipamento => Set<Equipamento>();
    public DbSet<Fornecedor> Fornecedor => Set<Fornecedor>();
    public DbSet<Insumo> Insumo => Set<Insumo>();
    public DbSet<FornecedorInsumo> FornecedorInsumo => Set<FornecedorInsumo>();

     public DbSet<Servico> Servico => Set<Servico>();
    public DbSet<FornecedorServico> FornecedorServico => Set<FornecedorServico>();


    // === Projetos e Obras ===
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



    // === Execução ===
    public DbSet<ObraEtapaPadrao> ObraEtapaPadrao => Set<ObraEtapaPadrao>();
    public DbSet<ObraItemEtapaPadrao> ObraItemEtapaPadrao => Set<ObraItemEtapaPadrao>();
    public DbSet<ObraEtapa> ObraEtapa => Set<ObraEtapa>();
    public DbSet<ObraItemEtapa> ObraItemEtapa => Set<ObraItemEtapa>();

    // === Auditoria ===
    public DbSet<LogAuditoria> LogAuditoria => Set<LogAuditoria>();


    // === ObraItemEtapaPadraoInsumo ====
    public DbSet<ObraItemEtapaPadraoInsumo> ObraItemEtapaPadraoInsumos => Set<ObraItemEtapaPadraoInsumo>();

    // === Orçamento ===
    public DbSet<Orcamento> Orcamento => Set<Orcamento>();
    public DbSet<OrcamentoItem> OrcamentoItem => Set<OrcamentoItem>();
    public DbSet<OrcamentoObra> OrcamentoObra => Set<OrcamentoObra>();



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureUsuario(modelBuilder);
        ConfigurePerfilUsuario(modelBuilder);
        ConfigureRelacionamentosObra(modelBuilder);
        ConfigureObraEtapa(modelBuilder);
        ConfigureObraEtapaPadrao(modelBuilder);
        ConfigureObraItemEtapa(modelBuilder);
        ConfigureObraItemEtapaPadrao(modelBuilder);
        ConfigureFuncionario(modelBuilder);
        ConfigureFuncao(modelBuilder);
        ConfigureObraEquipamento(modelBuilder);
        ConfigureObraFuncionario(modelBuilder);
        ConfigureObraFornecedor(modelBuilder);
        ConfigureEquipamento(modelBuilder);
        ConfigureRetrabalho(modelBuilder);
        ConfigurePendencia(modelBuilder);
        ConfigureObraDocumento(modelBuilder);
        ConfigureObraImagem(modelBuilder);
        ConfigureObraItemEtapaPadraoInsumo(modelBuilder);


        ConfigureCliente(modelBuilder);
        ConfigureFornecedor(modelBuilder);
        ConfigureInsumo(modelBuilder);
        ConfigureObraInsumo(modelBuilder);
        ConfigureObraInsumoLista(modelBuilder);
        ConfigureFornecedorInsumo(modelBuilder);    
        ConfigureServico(modelBuilder);
        ConfigureObraServico(modelBuilder);
        ConfigureObraServicoLista(modelBuilder);
        ConfigureFornecedorServico(modelBuilder);



    }

    #region Configurações de Entidades

    private static void ConfigureEquipamento(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Equipamento>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(100);
            entity.Property(e => e.CustoLocacaoDiaria).HasPrecision(18, 2);
            entity.Property(e => e.DataHoraCadastro).IsRequired();
            entity.Property(e => e.UsuarioCadastro).IsRequired().HasMaxLength(100);
        });
    }

    private static void ConfigureInsumo(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Insumo>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(100);
            entity.Property(e => e.DataHoraCadastro).IsRequired();
            entity.Property(e => e.UsuarioCadastro).IsRequired().HasMaxLength(100);
        });
    }

    private static void ConfigureServico(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Servico>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(100);
            entity.Property(e => e.DataHoraCadastro).IsRequired();
            entity.Property(e => e.UsuarioCadastro).IsRequired().HasMaxLength(100);
        });
    }


    private static void ConfigureUsuario(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.Property(e => e.SenhaHash).IsRequired();
            entity.Property(e => e.DataHoraCadastro).IsRequired();
            entity.Property(e => e.UsuarioCadastro).IsRequired().HasMaxLength(100);

            entity.HasOne(e => e.PerfilUsuario)
                  .WithMany(p => p.Usuarios)
                  .HasForeignKey(e => e.PerfilUsuarioId);
        });
    }

    private static void ConfigurePerfilUsuario(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PerfilUsuario>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(100);
        });
    }

    private static void ConfigureRelacionamentosObra(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ObraFuncionario>()
            .HasKey(x => x.Id);
        
        modelBuilder.Entity<ObraFornecedor>()
            .HasKey(x => x.Id);

        modelBuilder.Entity<ObraInsumo>()
            .HasKey(x => x.Id);
        
        modelBuilder.Entity<ObraServico>()
            .HasKey(x => x.Id);

        modelBuilder.Entity<ObraEquipamento>()
            .HasKey(x => x.Id);
    }

    private static void ConfigureObraEtapa(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ObraEtapa>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(150);
            entity.Property(e => e.Ordem).IsRequired();
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.DataInicio);
            entity.Property(e => e.DataConclusao);

            entity.HasOne(e => e.Obra)
                  .WithMany(o => o.Etapas)
                  .HasForeignKey(e => e.ObraId);
        });
    }

    private static void ConfigureObraEtapaPadrao(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ObraEtapaPadrao>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(150);
            entity.Property(e => e.Ordem).IsRequired();
            entity.Property(e => e.DataHoraCadastro).IsRequired();
            entity.Property(e => e.UsuarioCadastro).IsRequired().HasMaxLength(100);
        });
    }

    private static void ConfigureObraItemEtapa(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ObraItemEtapa>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Ordem).IsRequired();
            entity.Property(e => e.Concluido).IsRequired();
            entity.Property(e => e.IsDataPrazo);
            entity.Property(e => e.DiasPrazo);
            entity.Property(e => e.PrazoAtivo);
            entity.Property(e => e.DataConclusao);
            entity.Property(e => e.DataHoraCadastro).IsRequired();
            entity.Property(e => e.UsuarioCadastro).IsRequired().HasMaxLength(100);

            entity.HasOne(e => e.ObraEtapa)
                  .WithMany(e => e.Itens)
                  .HasForeignKey(e => e.ObraEtapaId);
        });

    }

    private static void ConfigureObraItemEtapaPadrao(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ObraItemEtapaPadrao>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Ordem).IsRequired();
            entity.Property(e => e.IsDataPrazo);
            entity.Property(e => e.DiasPrazo);
            entity.Property(e => e.DataHoraCadastro).IsRequired();
            entity.Property(e => e.UsuarioCadastro).IsRequired().HasMaxLength(100);

            entity.HasOne(e => e.ObraEtapaPadrao)
                  .WithMany(eo => eo.Itens)
                  .HasForeignKey(e => e.ObraEtapaPadraoId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureFuncionario(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Funcionario>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(100);
            entity.Property(e => e.CpfCnpj).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Salario).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.DataHoraCadastro).IsRequired();
            entity.Property(e => e.UsuarioCadastro).IsRequired().HasMaxLength(100);
        });
    }

    private static void ConfigureFuncao(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Funcao>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Ativo).IsRequired();
        });
    }

    private static void ConfigureObraInsumo(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ObraInsumo>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Quantidade).HasPrecision(18, 2);
            entity.Property(e => e.DataHoraCadastro).IsRequired();
            entity.Property(e => e.UsuarioCadastro).IsRequired().HasMaxLength(100);

            entity.HasOne(e => e.Lista)
                  .WithMany(l => l.Itens)
                  .HasForeignKey(e => e.ObraInsumoListaId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Insumo)
                  .WithMany()
                  .HasForeignKey(e => e.InsumoId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureObraInsumoLista(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ObraInsumoLista>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Data).IsRequired();

            entity.HasOne(e => e.Obra)
                  .WithMany(o => o.ListasInsumo)
                  .HasForeignKey(e => e.ObraId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Responsavel)
                  .WithMany()
                  .HasForeignKey(e => e.ResponsavelId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }

     private static void ConfigureObraServico(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ObraServico>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Quantidade).HasPrecision(18, 2);
            entity.Property(e => e.DataHoraCadastro).IsRequired();
            entity.Property(e => e.UsuarioCadastro).IsRequired().HasMaxLength(100);

            entity.HasOne(e => e.Lista)
                  .WithMany(l => l.Itens)
                  .HasForeignKey(e => e.ObraServicoListaId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Servico)
                  .WithMany()
                  .HasForeignKey(e => e.ServicoId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureObraServicoLista(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ObraServicoLista>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Data).IsRequired();

            entity.HasOne(e => e.Obra)
                  .WithMany(o => o.ListasServico)
                  .HasForeignKey(e => e.ObraId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Responsavel)
                  .WithMany()
                  .HasForeignKey(e => e.ResponsavelId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }



    private static void ConfigureObraFuncionario(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ObraFuncionario>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FuncionarioNome).HasMaxLength(100);
            entity.Property(e => e.FuncaoNoObra).HasMaxLength(100);
            entity.Property(e => e.DataInicio).IsRequired();
            entity.Property(e => e.DataHoraCadastro).IsRequired();
            entity.Property(e => e.UsuarioCadastro).IsRequired().HasMaxLength(100);

            entity.HasOne(e => e.Obra)
                  .WithMany(o => o.Funcionarios)
                  .HasForeignKey(e => e.ObraId);

         
        });
    }
    private static void ConfigureObraFornecedor(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ObraFornecedor>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FornecedorNome).HasMaxLength(100);
            entity.Property(e => e.DataInicio).IsRequired();
            entity.Property(e => e.DataHoraCadastro).IsRequired();
            entity.Property(e => e.UsuarioCadastro).IsRequired().HasMaxLength(100);

            entity.HasOne(e => e.Obra)
                  .WithMany(o => o.Fornecedores)
                  .HasForeignKey(e => e.ObraId);

         
        });
    }

    private static void ConfigureObraEquipamento(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ObraEquipamento>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DataInicioUso).IsRequired();
            entity.Property(e => e.DataHoraCadastro).IsRequired();
            entity.Property(e => e.UsuarioCadastro).IsRequired().HasMaxLength(100);

            entity.HasOne(e => e.Obra)
                  .WithMany(o => o.Equipamentos)
                  .HasForeignKey(e => e.ObraId);

         
        });
    }

    private static void ConfigureRetrabalho(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ObraRetrabalho>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Titulo).IsRequired().HasMaxLength(150);
            entity.Property(e => e.Descricao).HasMaxLength(500);
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.DataHoraCadastro).IsRequired();
            entity.Property(e => e.UsuarioCadastro).IsRequired().HasMaxLength(100);

            entity.HasOne(e => e.Obra)
                  .WithMany(o => o.Retrabalhos)
                  .HasForeignKey(e => e.ObraId);

            entity.HasOne(e => e.Responsavel)
                  .WithMany()
                  .HasForeignKey(e => e.ResponsavelId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigurePendencia(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ObraPendencia>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Titulo).IsRequired().HasMaxLength(150);
            entity.Property(e => e.Descricao).HasMaxLength(500);
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.DataHoraCadastro).IsRequired();
            entity.Property(e => e.UsuarioCadastro).IsRequired().HasMaxLength(100);

            entity.HasOne(e => e.Obra)
                  .WithMany(o => o.Pendencias)
                  .HasForeignKey(e => e.ObraId);

            entity.HasOne(e => e.Responsavel)
                  .WithMany()
                  .HasForeignKey(e => e.ResponsavelId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureObraDocumento(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ObraDocumento>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.NomeOriginal).IsRequired().HasMaxLength(255);
            entity.Property(e => e.CaminhoRelativo).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Extensao).IsRequired().HasMaxLength(10);
            entity.Property(e => e.TamanhoEmKb).HasPrecision(18, 2);
            entity.Property(e => e.DataHoraCadastro).IsRequired();
            entity.Property(e => e.UsuarioCadastro).IsRequired().HasMaxLength(100);

            entity.HasOne(e => e.Obra)
                  .WithMany(o => o.Documentos)
                  .HasForeignKey(e => e.ObraId);
        });


    }

    private static void ConfigureObraImagem(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ObraImagem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.NomeOriginal).IsRequired().HasMaxLength(255);
            entity.Property(e => e.CaminhoRelativo).IsRequired().HasMaxLength(500);
            entity.Property(e => e.TamanhoEmKb).HasPrecision(18, 2);
            entity.Property(e => e.DataHoraCadastro).IsRequired();
            entity.Property(e => e.UsuarioCadastro).IsRequired().HasMaxLength(100);

            entity.HasOne(e => e.Obra)
                  .WithMany(o => o.Imagens)
                  .HasForeignKey(e => e.ObraId);
        });
    }


    private static void ConfigureObraItemEtapaPadraoInsumo(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ObraItemEtapaPadraoInsumo>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.QuantidadeSugerida).HasPrecision(18, 2);
            entity.Property(e => e.DataHoraCadastro).IsRequired();
            entity.Property(e => e.UsuarioCadastro).IsRequired().HasMaxLength(100);

            entity.HasOne(e => e.ObraItemEtapaPadrao)
                  .WithMany(p => p.Insumos)
                  .HasForeignKey(e => e.ObraItemEtapaPadraoId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }


    private static void ConfigureCliente(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(100);
            entity.Property(e => e.CpfCnpj).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.DataHoraCadastro).IsRequired();
            entity.Property(e => e.UsuarioCadastro).IsRequired().HasMaxLength(100);
        });
    }

    private static void ConfigureFornecedor(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Fornecedor>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(100);
            entity.Property(e => e.CpfCnpj).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.DataHoraCadastro).IsRequired();
            entity.Property(e => e.UsuarioCadastro).IsRequired().HasMaxLength(100);
        });
    }

    private static void ConfigureFornecedorInsumo(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FornecedorInsumo>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PrecoUnitario).HasPrecision(18, 2);
            entity.Property(e => e.DataHoraCadastro).IsRequired();
            entity.Property(e => e.UsuarioCadastro).IsRequired().HasMaxLength(100);

            entity.HasOne(e => e.Fornecedor)
                  .WithMany()
                  .HasForeignKey(e => e.FornecedorId)
                  .OnDelete(DeleteBehavior.Restrict);


            entity.HasOne(e => e.Insumo)
                  .WithMany()
                  .HasForeignKey(e => e.InsumoId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }


    private static void ConfigureFornecedorServico(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FornecedorServico>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PrecoUnitario).HasPrecision(18, 2);
            entity.Property(e => e.DataHoraCadastro).IsRequired();
            entity.Property(e => e.UsuarioCadastro).IsRequired().HasMaxLength(100);

            entity.HasOne(e => e.Fornecedor)
                  .WithMany()
                  .HasForeignKey(e => e.FornecedorId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Servico)
                  .WithMany()
                  .HasForeignKey(e => e.ServicoId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }



    #endregion
}
