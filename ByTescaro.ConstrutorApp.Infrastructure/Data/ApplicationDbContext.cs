using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Entities.Admin;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ByTescaro.ConstrutorApp.Infrastructure.Data
{

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
                builder.HasKey(p => p.Id);
                builder.Property(p => p.Nome).HasMaxLength(255);
                builder.Property(p => p.Sobrenome).HasMaxLength(255);
                builder.Property(p => p.Ativo);
                builder.Property(p => p.DataHoraCadastro);
                builder.Property(p => p.UsuarioCadastroId);
                builder.Property(p => p.CpfCnpj).HasMaxLength(20);
                builder.Property(p => p.Email).HasMaxLength(255);
                builder.Property(p => p.TelefonePrincipal).HasMaxLength(20);
                builder.Property(p => p.TelefoneWhatsApp).HasMaxLength(20);
                builder.Property(p => p.TipoPessoa).HasColumnType("int");
                builder.Property(p => p.TipoEntidade).HasColumnType("int");

                builder.HasOne(p => p.Endereco)
                    .WithMany()
                    .HasForeignKey(p => p.EnderecoId)

                    .OnDelete(DeleteBehavior.NoAction);
            }
        }
        public class EnderecoConfiguration : IEntityTypeConfiguration<Endereco>
        {
            public void Configure(EntityTypeBuilder<Endereco> builder)
            {
                builder.ToTable("Endereco");
                builder.HasKey(e => e.Id);
                builder.Property(e => e.Ativo);
                builder.Property(e => e.Logradouro).HasMaxLength(255);
                builder.Property(e => e.Numero).HasMaxLength(50);
                builder.Property(e => e.Bairro).HasMaxLength(150);
                builder.Property(e => e.Cidade).HasMaxLength(150);
                builder.Property(e => e.Estado).HasMaxLength(100);
                builder.Property(e => e.UF).HasMaxLength(2);
                builder.Property(e => e.CEP).HasMaxLength(9);
                builder.Property(e => e.Complemento).HasMaxLength(255);
            }
        }

        public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
        {
            public void Configure(EntityTypeBuilder<Cliente> builder)
            {
                builder.ToTable("Cliente");
                builder.HasMany(c => c.Projetos)
                       .WithOne()
                       .HasForeignKey("ClienteId");
            }
        }

        public class FuncionarioConfiguration : IEntityTypeConfiguration<Funcionario>
        {
            public void Configure(EntityTypeBuilder<Funcionario> builder)
            {
                builder.ToTable("Funcionario");
                builder.Property(f => f.Salario).HasColumnType("decimal(18, 2)");
                builder.Property(f => f.DataAdmissao);
                builder.Property(f => f.DataDemissao);
                builder.HasOne(f => f.Funcao)
                       .WithMany()
                       .HasForeignKey(f => f.FuncaoId);

                builder.HasMany(f => f.ProjetoFuncionarios)
                       .WithOne(of => of.Funcionario)
                       .HasForeignKey(of => of.FuncionarioId);
            }
        }

        public class FornecedorConfiguration : IEntityTypeConfiguration<Fornecedor>
        {
            public void Configure(EntityTypeBuilder<Fornecedor> builder)
            {
                builder.ToTable("Fornecedor");
                builder.Property(f => f.TipoFornecedor).HasColumnType("int");
            }
        }

        public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
        {
            public void Configure(EntityTypeBuilder<Usuario> builder)
            {
                builder.ToTable("Usuario");
                builder.Property(u => u.Email).HasMaxLength(255);
                builder.HasIndex(u => u.Email);
                builder.Property(u => u.SenhaHash).HasMaxLength(255);

                builder.HasOne(u => u.PerfilUsuario)
                       .WithMany(p => p.Usuarios)
                       .HasForeignKey(u => u.PerfilUsuarioId);
            }
        }

        public class ProjetoConfiguration : IEntityTypeConfiguration<Projeto>
        {
            public void Configure(EntityTypeBuilder<Projeto> builder)
            {
                builder.ToTable("Projeto");
                builder.HasKey(p => p.Id);
                builder.Property(p => p.Nome).HasMaxLength(255);
                builder.Property(p => p.Ativo);
                builder.Property(p => p.DataHoraCadastro);
                builder.Property(p => p.UsuarioCadastroId);
                builder.Property(p => p.Status).HasColumnType("int");
                builder.Property(p => p.DataInicio);
                builder.Property(p => p.DataFim);
                builder.Property(p => p.TelefonePrincipal).HasMaxLength(20);

                builder.HasOne(p => p.Cliente)
                       .WithMany(c => c.Projetos)
                       .HasForeignKey(p => p.ClienteId)
                       .OnDelete(DeleteBehavior.Cascade);

                builder.Navigation(o => o.Cliente).AutoInclude();


                builder.HasOne(p => p.Endereco)
                       .WithMany()
                       .HasForeignKey(p => p.EnderecoId)
                       .OnDelete(DeleteBehavior.SetNull);
            }
        }

        public class ObraConfiguration : IEntityTypeConfiguration<Obra>
        {
            public void Configure(EntityTypeBuilder<Obra> builder)
            {

                builder.ToTable("Obra");
                builder.HasKey(o => o.Id);
                builder.Property(o => o.Nome).HasMaxLength(255);
                builder.Property(o => o.Ativo);
                builder.Property(o => o.DataHoraCadastro);
                builder.Property(o => o.UsuarioCadastroId);
                builder.Property(o => o.Status).HasColumnType("int");
                builder.Property(o => o.DataInicioExecucao);
                builder.Property(o => o.ResponsavelMaterial).HasColumnType("int");

                builder.HasOne(o => o.Projeto)
                       .WithMany(p => p.Obras)
                       .HasForeignKey(o => o.ProjetoId)
                       .IsRequired()
                       .OnDelete(DeleteBehavior.Cascade);

                builder.Navigation(o => o.Projeto).AutoInclude();


                builder.HasOne(o => o.ResponsavelObra)
                       .WithMany()
                       .HasForeignKey(o => o.ResponsavelObraId)
                       .OnDelete(DeleteBehavior.SetNull);


                builder.HasMany(o => o.Etapas)
                       .WithOne(oe => oe.Obra)
                       .HasForeignKey(oe => oe.ObraId)
                       .OnDelete(DeleteBehavior.Cascade);


                builder.HasMany(o => o.Funcionarios)
                       .WithOne(of => of.Obra)
                       .HasForeignKey(of => of.ObraId)
                       .OnDelete(DeleteBehavior.Cascade);

                builder.HasMany(o => o.Fornecedores)
                       .WithOne(of => of.Obra)
                       .HasForeignKey(of => of.ObraId)
                       .OnDelete(DeleteBehavior.Cascade);

                builder.HasMany(o => o.Insumos)
                       .WithOne(oi => oi.Obra)
                       .HasForeignKey(oi => oi.ObraId)
                       .OnDelete(DeleteBehavior.Cascade);

                builder.HasMany(o => o.ListasInsumo)
                       .WithOne(oil => oil.Obra)
                       .HasForeignKey(oil => oil.ObraId)
                       .OnDelete(DeleteBehavior.Cascade);

                builder.HasMany(o => o.Equipamentos)
                       .WithOne(oe => oe.Obra)
                       .HasForeignKey(oe => oe.ObraId)
                       .OnDelete(DeleteBehavior.Cascade);

                builder.HasMany(o => o.Retrabalhos)
                       .WithOne(or => or.Obra)
                       .HasForeignKey(or => or.ObraId)
                       .OnDelete(DeleteBehavior.Cascade);

                builder.HasMany(o => o.Pendencias)
                       .WithOne(op => op.Obra)
                       .HasForeignKey(op => op.ObraId)
                       .OnDelete(DeleteBehavior.Cascade);

                builder.HasMany(o => o.Documentos)
                       .WithOne(od => od.Obra)
                       .HasForeignKey(od => od.ObraId)
                       .OnDelete(DeleteBehavior.Cascade);

                builder.HasMany(o => o.Imagens)
                       .WithOne(oi => oi.Obra)
                       .HasForeignKey(oi => oi.ObraId)
                       .OnDelete(DeleteBehavior.Cascade);

                builder.HasMany(o => o.Servicos)
                       .WithOne(os => os.Obra)
                       .HasForeignKey(os => os.ObraId)
                       .OnDelete(DeleteBehavior.Cascade);

                builder.HasMany(o => o.ListasServico)
                       .WithOne(osl => osl.Obra)
                       .HasForeignKey(osl => osl.ObraId)
                       .OnDelete(DeleteBehavior.Cascade);
            }
        }


        public class EquipamentoConfiguration : IEntityTypeConfiguration<Equipamento>
        {
            public void Configure(EntityTypeBuilder<Equipamento> builder)
            {
                builder.ToTable("Equipamento");
                builder.HasKey(e => e.Id);
                builder.Property(e => e.Nome).HasMaxLength(255);
                builder.Property(e => e.Ativo);
                builder.Property(e => e.DataHoraCadastro);
                builder.Property(e => e.UsuarioCadastroId);
                builder.Property(e => e.Descricao).HasMaxLength(500);
                builder.Property(e => e.Patrimonio).HasMaxLength(50);
                builder.HasIndex(e => e.Patrimonio);
                builder.Property(e => e.Status).HasColumnType("int");
                builder.Property(e => e.CustoLocacaoDiaria).HasColumnType("decimal(18, 2)");
                builder.HasMany(e => e.ProjetoEquipamentos) // Um Equipamento tem muitos ObraEquipamentos
                       .WithOne(oe => oe.Equipamento) // Um ObraEquipamento pertence a um Equipamento
                       .HasForeignKey(oe => oe.EquipamentoId) // A chave estrangeira está em ObraEquipamento e se chama EquipamentoId
                       ; // A associação é obrigatória
            }
        }

        public class InsumoConfiguration : IEntityTypeConfiguration<Insumo>
        {
            public void Configure(EntityTypeBuilder<Insumo> builder)
            {
                builder.ToTable("Insumo");
                builder.HasKey(e => e.Id);
                builder.Property(e => e.Nome).HasMaxLength(255);
                builder.Property(e => e.Ativo);
                builder.Property(e => e.DataHoraCadastro);
                builder.Property(e => e.UsuarioCadastroId);
                builder.Property(e => e.Descricao).HasMaxLength(500);
                builder.Property(e => e.UnidadeMedida).HasColumnType("int");
            }
        }

        public class ServicoConfiguration : IEntityTypeConfiguration<Servico>
        {
            public void Configure(EntityTypeBuilder<Servico> builder)
            {
                builder.ToTable("Servico");
                builder.HasKey(e => e.Id);
                builder.Property(e => e.Ativo);
                builder.Property(e => e.Nome).HasMaxLength(255);
                builder.Property(e => e.Descricao).HasMaxLength(500);
                builder.Property(e => e.DataHoraCadastro);
                builder.Property(e => e.UsuarioCadastroId);

            }
        }

        public class FuncaoConfiguration : IEntityTypeConfiguration<Funcao>
        {
            public void Configure(EntityTypeBuilder<Funcao> builder)
            {
                builder.ToTable("Funcao");
                builder.HasKey(e => e.Id);
                builder.Property(e => e.Nome).HasMaxLength(255);
                builder.Property(e => e.Ativo);
                builder.Property(e => e.DataHoraCadastro);
                builder.Property(e => e.UsuarioCadastroId);

            }
        }

        public class PerfilUsuarioConfiguration : IEntityTypeConfiguration<PerfilUsuario>
        {
            public void Configure(EntityTypeBuilder<PerfilUsuario> builder)
            {
                builder.ToTable("PerfilUsuario");
                builder.HasKey(e => e.Id);
                builder.Property(e => e.Nome).HasMaxLength(255);
                builder.Property(e => e.Ativo);
                builder.Property(e => e.DataHoraCadastro);

                // A propriedade já existe na entidade base ou na própria entidade
                // builder.Property(e => e.UsuarioCadastroId); // Não precisa declarar de novo

                // --- Relacionamento Muitos-para-Um: Muitos perfis podem ser cadastrados por UM usuário ---
                // Adicione esta configuração para o usuário que CADASTROU o perfil.
                // Supondo que sua entidade PerfilUsuario tenha "public Usuario UsuarioCadastro { get; set; }"
                builder.HasOne(e => e.UsuarioCadastro)
                       .WithMany() // Um usuário pode cadastrar muitos perfis
                       .HasForeignKey(e => e.UsuarioCadastroId)
                       .OnDelete(DeleteBehavior.Restrict); // Evita que um usuário seja deletado se cadastrou perfis


                // --- Relacionamento Um-para-Muitos: UM perfil tem MUITOS usuários ---
                builder.HasMany(p => p.Usuarios)
                       .WithOne(u => u.PerfilUsuario)
                       .HasForeignKey(u => u.PerfilUsuarioId)
                       .OnDelete(DeleteBehavior.Restrict);
            }
        }

        public class FornecedorInsumoConfiguration : IEntityTypeConfiguration<FornecedorInsumo>
        {
            public void Configure(EntityTypeBuilder<FornecedorInsumo> builder)
            {
                builder.ToTable("FornecedorInsumo");
                builder.HasKey(e => e.Id);
                builder.Property(e => e.Ativo);
                builder.Property(e => e.PrecoUnitario).HasColumnType("decimal(18, 2)");
                builder.Property(e => e.PrazoEntregaDias);
                builder.Property(e => e.Observacao).HasMaxLength(5000);
                builder.Property(e => e.DataHoraCadastro);
                builder.Property(e => e.UsuarioCadastroId);


                builder.HasOne(e => e.Fornecedor)
                       .WithMany()
                       .HasForeignKey(e => e.FornecedorId)
                       .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(e => e.Insumo)
                       .WithMany()
                       .HasForeignKey(e => e.InsumoId)
                       .OnDelete(DeleteBehavior.Restrict);


            }
        }

        public class FornecedorServicoConfiguration : IEntityTypeConfiguration<FornecedorServico>
        {
            public void Configure(EntityTypeBuilder<FornecedorServico> builder)
            {
                builder.ToTable("FornecedorServico");
                builder.HasKey(e => e.Id);
                builder.Property(e => e.Ativo);
                builder.Property(e => e.PrecoUnitario).HasColumnType("decimal(18, 2)");
                builder.Property(e => e.PrazoEntregaDias);
                builder.Property(e => e.Observacao).HasMaxLength(5000);
                builder.Property(e => e.DataHoraCadastro);
                builder.Property(e => e.UsuarioCadastroId);


                builder.HasOne(e => e.Fornecedor)
                       .WithMany()
                       .HasForeignKey(e => e.FornecedorId)
                       .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(e => e.Servico)
                       .WithMany()
                       .HasForeignKey(e => e.ServicoId)
                       .OnDelete(DeleteBehavior.Restrict);


            }
        }


        public class ObraFuncionarioConfiguration : IEntityTypeConfiguration<ObraFuncionario>
        {
            public void Configure(EntityTypeBuilder<ObraFuncionario> builder)
            {
                builder.ToTable("ObraFuncionario");
                builder.HasKey(e => e.Id);
                builder.Property(e => e.Ativo);
                builder.Property(e => e.FuncionarioNome).HasMaxLength(100);
                builder.Property(e => e.FuncaoNoObra).HasMaxLength(100);
                builder.Property(e => e.DataInicio);
                builder.Property(e => e.DataFim);
                builder.Property(e => e.DataHoraCadastro);
                builder.Property(e => e.UsuarioCadastroId);


                builder.HasOne(e => e.Obra)
                       .WithMany(o => o.Funcionarios)
                       .HasForeignKey(e => e.ObraId)
                       .OnDelete(DeleteBehavior.Cascade);

                //builder.Navigation(o => o.Equipamento).AutoInclude();




            }
        }

        public class ObraFornecedorConfiguration : IEntityTypeConfiguration<ObraFornecedor>
        {
            public void Configure(EntityTypeBuilder<ObraFornecedor> builder)
            {
                builder.ToTable("ObraFornecedor");
                builder.HasKey(e => e.Id);
                builder.Property(e => e.Ativo);
                builder.Property(e => e.FornecedorNome).HasMaxLength(100);
                builder.Property(e => e.DataInicio);
                builder.Property(e => e.DataFim);
                builder.Property(e => e.DataHoraCadastro);
                builder.Property(e => e.UsuarioCadastroId);


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
                builder.ToTable("ObraEquipamento");
                builder.HasKey(e => e.Id);
                builder.Property(e => e.Ativo);
                builder.Property(e => e.EquipamentoNome).HasMaxLength(100);
                builder.Property(e => e.DataInicioUso);
                builder.Property(e => e.DataFimUso);
                builder.Property(e => e.DataHoraCadastro);
                builder.Property(e => e.UsuarioCadastroId);


                builder.HasOne(e => e.Obra)
                       .WithMany(o => o.Equipamentos)
                       .HasForeignKey(e => e.ObraId)
                       .OnDelete(DeleteBehavior.Restrict);

                builder.Navigation(o => o.Equipamento).AutoInclude();



            }
        }

        public class ObraInsumoConfiguration : IEntityTypeConfiguration<ObraInsumo>
        {
            public void Configure(EntityTypeBuilder<ObraInsumo> builder)
            {
                builder.ToTable("ObraInsumo");

                builder.HasKey(e => e.Id);
                builder.Property(e => e.Ativo);
                builder.Property(e => e.Quantidade).HasColumnType("decimal(18, 2)");
                builder.Property(e => e.DataHoraCadastro);
                builder.Property(e => e.UsuarioCadastroId);


                builder.HasOne(e => e.Obra)
                    .WithMany(o => o.Insumos)
                    .HasForeignKey(e => e.ObraId)
                    .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(e => e.Lista)
                    .WithMany(l => l.Itens)
                    .HasForeignKey(e => e.ObraInsumoListaId)
                    .OnDelete(DeleteBehavior.Cascade);

                builder.HasOne(e => e.Insumo)
                    .WithMany()
                    .HasForeignKey(e => e.InsumoId)
                    .OnDelete(DeleteBehavior.Restrict);

            }
        }

        public class ObraInsumoListaConfiguration : IEntityTypeConfiguration<ObraInsumoLista>
        {
            public void Configure(EntityTypeBuilder<ObraInsumoLista> builder)
            {
                builder.ToTable("ObraInsumoLista");

                builder.HasKey(e => e.Id);
                builder.Property(e => e.Ativo);
                builder.Property(e => e.Data);
                builder.Property(e => e.DataHoraCadastro);
                builder.Property(e => e.UsuarioCadastroId);


                builder.HasOne(e => e.Obra)
                    .WithMany(o => o.ListasInsumo)
                    .HasForeignKey(e => e.ObraId)
                    .OnDelete(DeleteBehavior.Cascade);

                builder.HasOne(e => e.Responsavel)
                    .WithMany() // Um funcionário pode ser responsável por várias listas de insumo
                    .HasForeignKey(e => e.ResponsavelId)
                    .OnDelete(DeleteBehavior.Restrict);

            }
        }

        public class ObraServicoConfiguration : IEntityTypeConfiguration<ObraServico>
        {
            public void Configure(EntityTypeBuilder<ObraServico> builder)
            {
                builder.ToTable("ObraServico");

                builder.HasKey(e => e.Id);
                builder.Property(e => e.Ativo);
                builder.Property(e => e.Quantidade).HasColumnType("decimal(18, 2)");
                builder.Property(e => e.DataHoraCadastro);
                builder.Property(e => e.UsuarioCadastroId);


                builder.HasOne(e => e.Obra)
                    .WithMany(o => o.Servicos)
                    .HasForeignKey(e => e.ObraId)
                    .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(e => e.Lista)
                    .WithMany(l => l.Itens)
                    .HasForeignKey(e => e.ObraServicoListaId)
                    .OnDelete(DeleteBehavior.Cascade);

                builder.HasOne(e => e.Servico)
                    .WithMany() // Se Servico não tiver uma ICollection<ObraServico>
                    .HasForeignKey(e => e.ServicoId)
                    .OnDelete(DeleteBehavior.Restrict);
            }
        }

        public class ObraServicoListaConfiguration : IEntityTypeConfiguration<ObraServicoLista>
        {
            public void Configure(EntityTypeBuilder<ObraServicoLista> builder)
            {
                builder.ToTable("ObraServicoLista");

                builder.HasKey(e => e.Id);
                builder.Property(e => e.Ativo);
                builder.Property(e => e.Data);
                builder.Property(e => e.DataHoraCadastro);
                builder.Property(e => e.UsuarioCadastroId);


                builder.HasOne(e => e.Obra)
                    .WithMany(o => o.ListasServico)
                    .HasForeignKey(e => e.ObraId)
                    .OnDelete(DeleteBehavior.Cascade);

                builder.HasOne(e => e.Responsavel)
                    .WithMany() // Um funcionário pode ser responsável por várias listas de serviço
                    .HasForeignKey(e => e.ResponsavelId)
                    .OnDelete(DeleteBehavior.Restrict);
            }
        }

        public class ObraEtapaConfiguration : IEntityTypeConfiguration<ObraEtapa>
        {
            public void Configure(EntityTypeBuilder<ObraEtapa> builder)
            {
                builder.ToTable("ObraEtapa");

                builder.HasKey(e => e.Id);
                builder.Property(e => e.Nome).HasMaxLength(255);
                builder.Property(e => e.Ativo);
                builder.Property(e => e.Ordem);
                builder.Property(e => e.Status).HasColumnType("int");
                builder.Property(e => e.DataInicio);
                builder.Property(e => e.DataConclusao);
                builder.Property(e => e.DataHoraCadastro);
                builder.Property(e => e.UsuarioCadastroId);


                builder.HasOne(e => e.Obra)
                    .WithMany(o => o.Etapas)
                    .HasForeignKey(e => e.ObraId)
                    .OnDelete(DeleteBehavior.Cascade);

                builder.Navigation(o => o.Itens).AutoInclude();

            }
        }

        public class ObraItemEtapaConfiguration : IEntityTypeConfiguration<ObraItemEtapa>
        {
            public void Configure(EntityTypeBuilder<ObraItemEtapa> builder)
            {
                builder.ToTable("ObraItemEtapa");

                builder.HasKey(e => e.Id);
                builder.Property(e => e.Nome).HasMaxLength(255);
                builder.Property(e => e.Ativo);
                builder.Property(e => e.Ordem);
                builder.Property(e => e.Concluido);
                builder.Property(e => e.IsDataPrazo);
                builder.Property(e => e.DiasPrazo);
                builder.Property(e => e.PrazoAtivo);
                builder.Property(e => e.DataConclusao);
                builder.Property(e => e.DataHoraCadastro);
                builder.Property(e => e.UsuarioCadastroId);


                builder.HasOne(e => e.ObraEtapa)
                    .WithMany(oe => oe.Itens)
                    .HasForeignKey(e => e.ObraEtapaId)
                    .OnDelete(DeleteBehavior.Cascade);

            }
        }

        public class ObraEtapaPadraoConfiguration : IEntityTypeConfiguration<ObraEtapaPadrao>
        {
            public void Configure(EntityTypeBuilder<ObraEtapaPadrao> builder)
            {
                builder.ToTable("ObraEtapaPadrao");

                builder.Property(e => e.Nome).HasMaxLength(255);
                builder.HasKey(e => e.Id);
                builder.Property(e => e.Ativo);
                builder.Property(e => e.Ordem);
                builder.Property(e => e.Status).HasColumnType("int");
                builder.Property(e => e.DataHoraCadastro);
                builder.Property(e => e.UsuarioCadastroId);


                builder.HasMany(e => e.Itens)
                    .WithOne(oiep => oiep.ObraEtapaPadrao)
                    .HasForeignKey(oiep => oiep.ObraEtapaPadraoId)
                    .OnDelete(DeleteBehavior.Cascade);



            }
        }

        public class ObraItemEtapaPadraoConfiguration : IEntityTypeConfiguration<ObraItemEtapaPadrao>
        {
            public void Configure(EntityTypeBuilder<ObraItemEtapaPadrao> builder)
            {
                builder.ToTable("ObraItemEtapaPadrao");

                builder.HasKey(e => e.Id);
                builder.Property(e => e.Nome).HasMaxLength(255);
                builder.Property(e => e.Ativo);
                builder.Property(e => e.Ordem);
                builder.Property(e => e.IsDataPrazo);
                builder.Property(e => e.DiasPrazo);
                builder.Property(e => e.DataHoraCadastro);
                builder.Property(e => e.UsuarioCadastroId);


                builder.HasOne(e => e.ObraEtapaPadrao)
                    .WithMany(eo => eo.Itens)
                    .HasForeignKey(e => e.ObraEtapaPadraoId)
                    .OnDelete(DeleteBehavior.Cascade);



                builder.HasMany(e => e.Insumos)
                    .WithOne(oiepi => oiepi.ObraItemEtapaPadrao)
                    .HasForeignKey(oiepi => oiepi.ObraItemEtapaPadraoId)
                    .OnDelete(DeleteBehavior.Cascade);
            }
        }

        public class ObraItemEtapaPadraoInsumoConfiguration : IEntityTypeConfiguration<ObraItemEtapaPadraoInsumo>
        {
            public void Configure(EntityTypeBuilder<ObraItemEtapaPadraoInsumo> builder)
            {
                builder.ToTable("ObraItemEtapaPadraoInsumo");

                builder.HasKey(e => e.Id);
                builder.Property(e => e.Ativo);
                builder.Property(e => e.QuantidadeSugerida).HasColumnType("decimal(18, 2)");
                builder.Property(e => e.DataHoraCadastro);
                builder.Property(e => e.UsuarioCadastroId);


                builder.HasOne(e => e.Insumo)
                    .WithMany()
                    .HasForeignKey(e => e.InsumoId)
                    .OnDelete(DeleteBehavior.Restrict);

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
                builder.ToTable("ObraRetrabalho");

                builder.HasKey(e => e.Id);
                builder.Property(e => e.Ativo);
                builder.Property(e => e.Titulo).HasMaxLength(255);
                builder.Property(e => e.Descricao).HasMaxLength(500);
                builder.Property(e => e.Status).HasColumnType("int");
                builder.Property(e => e.DataInicio);
                builder.Property(e => e.DataConclusao);
                builder.Property(e => e.DataHoraCadastro);
                builder.Property(e => e.UsuarioCadastroId);


                builder.HasOne(e => e.Obra)
                    .WithMany(o => o.Retrabalhos)
                    .HasForeignKey(e => e.ObraId)
                    .OnDelete(DeleteBehavior.Cascade);

                builder.HasOne(e => e.Responsavel)
                    .WithMany() // Um funcionário pode ser responsável por vários retrabalhos
                    .HasForeignKey(e => e.ResponsavelId)
                    .OnDelete(DeleteBehavior.SetNull); // Se o funcionário for deletado, a FK vira NULL

            }
        }

        public class ObraPendenciaConfiguration : IEntityTypeConfiguration<ObraPendencia>
        {
            public void Configure(EntityTypeBuilder<ObraPendencia> builder)
            {
                builder.ToTable("ObraPendencia");

                builder.HasKey(e => e.Id);
                builder.Property(e => e.Titulo).HasMaxLength(255);
                builder.Property(e => e.Descricao).HasMaxLength(500);
                builder.Property(e => e.Ativo);
                builder.Property(e => e.Status).HasColumnType("int");
                builder.Property(e => e.DataInicio);
                builder.Property(e => e.DataConclusao);
                builder.Property(e => e.DataHoraCadastro);
                builder.Property(e => e.UsuarioCadastroId);


                builder.HasOne(e => e.Obra)
                    .WithMany(o => o.Pendencias)
                    .HasForeignKey(e => e.ObraId)
                    .OnDelete(DeleteBehavior.Cascade);

                builder.HasOne(e => e.Responsavel)
                    .WithMany()
                    .HasForeignKey(e => e.ResponsavelId)
                    .OnDelete(DeleteBehavior.SetNull);
            }
        }

        public class ObraDocumentoConfiguration : IEntityTypeConfiguration<ObraDocumento>
        {
            public void Configure(EntityTypeBuilder<ObraDocumento> builder)
            {
                builder.ToTable("ObraDocumento");

                builder.HasKey(e => e.Id);
                builder.Property(e => e.NomeOriginal).HasMaxLength(255);
                builder.Property(e => e.CaminhoRelativo);
                builder.Property(e => e.Extensao).HasMaxLength(10);
                builder.Property(e => e.TamanhoEmKb);
                builder.Property(e => e.Ativo);
                builder.Property(e => e.DataHoraCadastro);
                builder.Property(e => e.UsuarioCadastroId);


                builder.HasOne(e => e.Obra)
                    .WithMany(o => o.Documentos)
                    .HasForeignKey(e => e.ObraId)
                    .OnDelete(DeleteBehavior.Cascade);
            }
        }

        public class ObraImagemConfiguration : IEntityTypeConfiguration<ObraImagem>
        {
            public void Configure(EntityTypeBuilder<ObraImagem> builder)
            {
                builder.ToTable("ObraImagem");

                builder.HasKey(e => e.Id);
                builder.Property(e => e.NomeOriginal).HasMaxLength(255);
                builder.Property(e => e.CaminhoRelativo);
                builder.Property(e => e.TamanhoEmKb);
                builder.Property(e => e.Ativo);
                builder.Property(e => e.DataHoraCadastro);
                builder.Property(e => e.UsuarioCadastroId);


                builder.HasOne(e => e.Obra)
                    .WithMany(o => o.Imagens)
                    .HasForeignKey(e => e.ObraId)
                    .OnDelete(DeleteBehavior.Cascade); // Se a obra for deletada, suas imagens são deletadas.
            }
        }

        public class EventoConfiguration : IEntityTypeConfiguration<Evento>
        {
            public void Configure(EntityTypeBuilder<Evento> builder)
            {
                builder.ToTable("Evento");

                builder.HasKey(e => e.Id);
                builder.Property(e => e.Titulo).HasMaxLength(200);
                builder.Property(e => e.Descricao);
                builder.Property(e => e.Ativo);
                builder.Property(e => e.IsRecorrente);
                builder.Property(e => e.FrequenciaRecorrencia).HasColumnType("int");
                builder.Property(e => e.DataFimRecorrencia);
                builder.Property(e => e.Visibilidade).HasColumnType("int");
                builder.Property(e => e.DataHoraInicio);
                builder.Property(e => e.DataHoraFim);
                builder.Property(e => e.DataHoraCadastro);
                builder.Property(e => e.UsuarioCadastroId);


                builder.HasOne(e => e.UsuarioCadastro)
                    .WithMany() // Um usuário pode criar muitos eventos
                    .HasForeignKey(e => e.UsuarioCadastroId)
                    .OnDelete(DeleteBehavior.Restrict);

                builder.HasMany(e => e.Participantes)
                    .WithOne(pe => pe.Evento)
                    .HasForeignKey(pe => pe.EventoId)
                    .OnDelete(DeleteBehavior.Cascade);

                builder.HasMany(e => e.Lembretes)
         .WithOne(le => le.Evento)
         .HasForeignKey(le => le.EventoId)
         .OnDelete(DeleteBehavior.Cascade);
            }
        }

        public void Configure(EntityTypeBuilder<ParticipanteEvento> builder)
        {
            builder.ToTable("ParticipanteEvento");

            builder.HasKey(pe => pe.Id);
            builder.Property(pe => pe.Ativo);
            builder.Property(pe => pe.DataResposta);
            builder.Property(pe => pe.StatusParticipacao).HasColumnType("int");
            builder.Property(pe => pe.DataHoraCadastro);
            builder.Property(pe => pe.UsuarioCadastroId);


            builder.HasOne(pe => pe.Evento)
                .WithMany(e => e.Participantes)
                .HasForeignKey(pe => pe.EventoId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(pe => pe.Usuario)
                .WithMany() // Um usuário pode participar de muitos eventos, mas Evento não tem coleção de ParticipanteEvento
                            // Se Usuario tivesse uma ICollection<ParticipanteEvento> (e.g., ParticipacoesEmEventos),
                            // seria .WithMany(u => u.ParticipacoesEmEventos)
                .HasForeignKey(pe => pe.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict); // Não deletar o usuário se ele estiver associado a participações em eventos.
        }

        public class LembreteEventoConfiguration : IEntityTypeConfiguration<LembreteEvento>
        {
            public void Configure(EntityTypeBuilder<LembreteEvento> builder)
            {
                builder.ToTable("LembreteEvento");

                builder.HasKey(le => le.Id);
                builder.Property(le => le.Ativo);
                builder.Property(le => le.TempoAntes);
                builder.Property(le => le.UnidadeTempo).HasColumnType("int");
                builder.Property(le => le.Enviado);
                builder.Property(le => le.DataHoraCadastro);
                builder.Property(le => le.UsuarioCadastroId);

                builder.HasOne(le => le.Evento)
                    .WithMany(e => e.Lembretes)
                    .HasForeignKey(le => le.EventoId)
                    .OnDelete(DeleteBehavior.Cascade);
            }
        }
        public class ConfiguracaoLembreteDiarioConfiguration : IEntityTypeConfiguration<ConfiguracaoLembreteDiario>
        {
            public void Configure(EntityTypeBuilder<ConfiguracaoLembreteDiario> builder)
            {
                builder.ToTable("ConfiguracaoLembreteDiario");

                builder.HasKey(x => x.Id);

                builder.Property(x => x.Ativo);
                builder.Property(x => x.HoraDoDia).HasColumnType("time");
                builder.Property(x => x.Descricao).HasMaxLength(500);
                builder.Property(x => x.DataHoraCadastro);
                builder.Property(x => x.UsuarioCadastroId);

                // Relacionamento com Usuario (o usuário que possui/cadastrou esta configuração)
                // Usa UsuarioCadastroId da EntidadeBase como chave estrangeira.
                builder.HasOne(x => x.UsuarioCadastro) // Relacionamento da ConfiguraçãoLembreteDiario com Usuario
                       .WithMany() // Um usuário pode ter várias configurações de lembrete diário
                       .HasForeignKey(x => x.UsuarioCadastroId) // FK para o usuário na tabela ConfiguracaoLembreteDiario
                                                                // A configuração deve estar associada a um usuário
                       .OnDelete(DeleteBehavior.Cascade);
            }
        }
        public class OrcamentoConfiguration : IEntityTypeConfiguration<Orcamento>
        {
            public void Configure(EntityTypeBuilder<Orcamento> builder)
            {
                builder.ToTable("Orcamento");

                builder.HasKey(o => o.Id);

                builder.Property(o => o.Ativo);
                builder.Property(o => o.Responsavel).HasMaxLength(255);
                builder.Property(o => o.DataReferencia);
                builder.Property(o => o.TotalEstimado).HasColumnType("decimal(18, 2)");
                builder.Property(o => o.DataHoraCadastro);
                builder.Property(o => o.UsuarioCadastroId);
            }
        }
        public class OrcamentoItemConfiguration : IEntityTypeConfiguration<OrcamentoItem>
        {
            public void Configure(EntityTypeBuilder<OrcamentoItem> builder)
            {
                builder.ToTable("OrcamentoItem");

                builder.HasKey(oi => oi.Id);

                builder.Property(oi => oi.Ativo);
                builder.Property(oi => oi.Quantidade).HasColumnType("decimal(18, 2)");
                builder.Property(oi => oi.PrecoUnitario).HasColumnType("decimal(18, 2)");
                builder.Property(oi => oi.DataHoraCadastro);
                builder.Property(oi => oi.UsuarioCadastroId);

                // Mapeamento da propriedade Total, que é somente leitura e calculada.
                // O EF Core não mapeia propriedades somente leitura por padrão, a menos que especificado.
                // Como é uma propriedade calculada, não deve ser mapeada para uma coluna no banco de dados.
                builder.Ignore(oi => oi.Total);

                builder.HasOne(oi => oi.OrcamentoObra)
                    .WithMany(o => o.Itens)
                    .HasForeignKey(oi => oi.OrcamentoObraId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relacionamento com Insumo (opcional)
                builder.HasOne(oi => oi.Insumo)
                    .WithMany() // Um insumo pode estar em muitos itens de orçamento
                    .HasForeignKey(oi => oi.InsumoId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Relacionamento com Servico (opcional)
                builder.HasOne(oi => oi.Servico)
                    .WithMany() // Um serviço pode estar em muitos itens de orçamento
                    .HasForeignKey(oi => oi.ServicoId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Relacionamento com Fornecedor (opcional)
                builder.HasOne(oi => oi.Fornecedor)
                    .WithMany() // Um fornecedor pode estar em muitos itens de orçamento
                    .HasForeignKey(oi => oi.FornecedorId)
                    .OnDelete(DeleteBehavior.Restrict);
            }
        }
        public class OrcamentoObraConfiguration : IEntityTypeConfiguration<OrcamentoObra>
        {
            public void Configure(EntityTypeBuilder<OrcamentoObra> builder)
            {
                builder.ToTable("OrcamentoObra");

                builder.HasKey(oo => oo.Id);

                builder.Property(oo => oo.Ativo);
                builder.Property(oo => oo.DataReferencia);
                builder.Property(oo => oo.Responsavel).HasMaxLength(255);
                builder.Property(oo => oo.Observacoes);
                builder.Property(oo => oo.Status).HasColumnType("int");
                builder.Property(oo => oo.TotalEstimado).HasColumnType("decimal(18, 2)");
                builder.Property(oo => oo.DataHoraCadastro);
                builder.Property(oo => oo.UsuarioCadastroId);

                builder.HasMany(oo => oo.Itens)
                    .WithOne(oi => oi.OrcamentoObra)
                    .HasForeignKey(oi => oi.OrcamentoObraId)

                    .OnDelete(DeleteBehavior.Cascade);

                builder.HasIndex(oo => new { oo.ObraId, oo.DataReferencia }).IsUnique();
            }
        }

        public class LogAuditoriaConfiguration : IEntityTypeConfiguration<LogAuditoria>
        {
            public void Configure(EntityTypeBuilder<LogAuditoria> builder)
            {
                builder.ToTable("LogAuditoria");

                builder.HasKey(l => l.Id);
                builder.Property(l => l.UsuarioId);
                builder.Property(l => l.UsuarioNome).HasMaxLength(100);
                builder.Property(l => l.Entidade).HasMaxLength(100);
                builder.Property(l => l.TipoLogAuditoria).HasColumnType("int");
                builder.Property(l => l.Descricao).HasColumnType("nvarchar(max)");
                builder.Property(l => l.DataHora);
                builder.Property(l => l.DadosAnteriores).HasColumnType("nvarchar(max)");
                builder.Property(l => l.DadosAtuais).HasColumnType("nvarchar(max)");
                builder.Property(l => l.IdEntidade).HasMaxLength(50);

                // Índices podem ser úteis para consultas frequentes, por exemplo, por UsuarioId ou Entidade
                builder.HasIndex(l => l.DataHora);
                builder.HasIndex(l => l.UsuarioId);
                builder.HasIndex(l => l.Entidade);
                builder.HasIndex(l => l.TipoLogAuditoria);
            }
        }
    }

    #endregion

}