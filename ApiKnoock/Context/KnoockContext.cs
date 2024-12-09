using System;
using System.Collections.Generic;
using ApiKnoock.Domains;
using Microsoft.EntityFrameworkCore;

namespace ApiKnoock.Context;

public partial class KnoockContext : DbContext
{
    public KnoockContext()
    {
    }

    public KnoockContext(DbContextOptions<KnoockContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Afiliado> Afiliados { get; set; }

    public virtual DbSet<Condomino> Condominos { get; set; }

    public virtual DbSet<Entrega> Entregas { get; set; }

    public virtual DbSet<Notificacao> Notificacaos { get; set; }

    public virtual DbSet<NotificacaoEntrega> NotificacaoEntregas { get; set; }

    public virtual DbSet<Tipo> Tipos { get; set; }

    public virtual DbSet<TipoUsuario> TipoUsuarios { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<Veiculo> Veiculos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            //var connectionString = "Data Source=DUDU\\SQLEXPRESS; Initial Catalog=KNOOCK; Integrated Security=True; TrustServerCertificate=true";
            //optionsBuilder.UseSqlServer(connectionString);

            //var connectionString = "Data Source=NOTE18-S21; Initial Catalog=KNOOCK; User Id=sa; Password=Senai@134; TrustServerCertificate=true";
            //optionsBuilder.UseSqlServer(connectionString);

            var connectionString = "Server=tcp:knoockserver.database.windows.net,1433;Initial Catalog=KnoockDatabase;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30; User Id = knoockserver; Pwd = Senai@134";
            optionsBuilder.UseSqlServer(connectionString);

        }
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Afiliado>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Afiliado__3214EC27E68EE383");

            entity.ToTable("Afiliado");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.FgOnline).HasColumnName("Fg_Online");
            entity.Property(e => e.FgResgatado).HasColumnName("Fg_Resgatado");
            entity.Property(e => e.FgTransito).HasColumnName("Fg_Transito");
            entity.Property(e => e.TipoUsuarioId).HasColumnName("TipoUsuario_ID");

            entity.HasOne(d => d.TipoUsuario).WithMany(p => p.Afiliados)
                .HasForeignKey(d => d.TipoUsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Afiliado__TipoUs__6B24EA82");
        });

        modelBuilder.Entity<Condomino>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Condomin__3214EC27AA7A59C9");

            entity.ToTable("Condomino");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.Apartamento).HasMaxLength(10);
            entity.Property(e => e.Bloco).HasMaxLength(5);
            entity.Property(e => e.DeliveryPin).HasMaxLength(10);
            entity.Property(e => e.Pin)
                .HasMaxLength(6)
                .HasColumnName("PIN");
            entity.Property(e => e.TipoUsuarioId).HasColumnName("TipoUsuario_ID");

            entity.HasOne(d => d.TipoUsuario).WithMany(p => p.Condominos)
                .HasForeignKey(d => d.TipoUsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Condomino__TipoU__6EF57B66");
        });

        modelBuilder.Entity<Entrega>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Entrega__3214EC27AC35C1DD");

            entity.ToTable("Entrega");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.DataNotificacao)
                .HasColumnType("datetime")
                .HasColumnName("Data_Notificacao");
            entity.Property(e => e.DataRegistro)
                .HasColumnType("datetime")
                .HasColumnName("Data_Registro");
            entity.Property(e => e.DataRetirada)
                .HasColumnType("datetime")
                .HasColumnName("Data_Retirada");
            entity.Property(e => e.FotoProduto)
                .HasMaxLength(255)
                .HasColumnName("Foto_Produto");
            entity.Property(e => e.NotificacaoMorador)
                .HasDefaultValue(false)
                .HasColumnName("Notificacao_Morador");
            entity.Property(e => e.Observacao).HasColumnType("text");
            entity.Property(e => e.Origem)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PinRetirada)
                .HasMaxLength(10)
                .HasColumnName("PIN_Retirada");
            entity.Property(e => e.Status).HasMaxLength(10);
            entity.Property(e => e.TipoUsuarioId).HasColumnName("TipoUsuario_ID");

            entity.HasOne(d => d.TipoUsuario).WithMany(p => p.Entregas)
                .HasForeignKey(d => d.TipoUsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Entrega__TipoUsu__73BA3083");
        });

        modelBuilder.Entity<Notificacao>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Notifica__3214EC27DA37C977");

            entity.ToTable("Notificacao");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.DataNotificacao)
                .HasColumnType("datetime")
                .HasColumnName("Data_Notificacao");
            entity.Property(e => e.ImagemAviso)
                .HasColumnType("text")
                .HasColumnName("Imagem_Aviso");
            entity.Property(e => e.Mensagem).HasMaxLength(255);
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.Property(e => e.Tipo).HasMaxLength(20);
            entity.Property(e => e.TipoUsuarioId).HasColumnName("TipoUsuario_ID");

            entity.HasOne(d => d.TipoUsuario).WithMany(p => p.Notificacaos)
                .HasForeignKey(d => d.TipoUsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Notificac__TipoU__7C4F7684");
        });

        modelBuilder.Entity<NotificacaoEntrega>(entity =>
        {
            entity.HasKey(e => e.NotificacaoId).HasName("PK__Notifica__73F653CA139C9B6F");

            entity.ToTable("Notificacao_Entrega");

            entity.Property(e => e.NotificacaoId)
                .ValueGeneratedNever()
                .HasColumnName("Notificacao_ID");
            entity.Property(e => e.EntregaId).HasColumnName("Entrega_ID");

            entity.HasOne(d => d.Entrega).WithMany(p => p.NotificacaoEntregas)
                .HasForeignKey(d => d.EntregaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Notificac__Entre__00200768");

            entity.HasOne(d => d.Notificacao).WithOne(p => p.NotificacaoEntrega)
                .HasForeignKey<NotificacaoEntrega>(d => d.NotificacaoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Notificac__Notif__7F2BE32F");
        });

        modelBuilder.Entity<Tipo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tipo__3214EC271772B5B5");

            entity.ToTable("Tipo");

            entity.HasIndex(e => e.Tipo1, "UQ__Tipo__E7F95649E08E68AF").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.Tipo1)
                .HasMaxLength(20)
                .HasColumnName("tipo");
        });

        modelBuilder.Entity<TipoUsuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tipo_Usu__3214EC27A3570499");

            entity.ToTable("Tipo_Usuario");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.IdTipo).HasColumnName("Id_Tipo");
            entity.Property(e => e.IdUsuario).HasColumnName("Id_usuario");

            entity.HasOne(d => d.IdTipoNavigation).WithMany(p => p.TipoUsuarios)
                .HasForeignKey(d => d.IdTipo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Tipo_Usua__Id_Ti__66603565");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.TipoUsuarios)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Tipo_Usua__Id_us__656C112C");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Usuario__3214EC27E1CE909D");

            entity.ToTable("Usuario");

            entity.HasIndex(e => e.Email, "UQ__Usuario__A9D10534B17500CC").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.CodigoRecuperacao).HasColumnName("Codigo_Recuperacao");
            entity.Property(e => e.DataNascimento).HasColumnName("Data_Nascimento");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Endereco).HasMaxLength(255);
            entity.Property(e => e.FotoUsuario)
                .HasMaxLength(255)
                .HasColumnName("Foto_Usuario");
            entity.Property(e => e.Nome).HasMaxLength(100);
            entity.Property(e => e.Senha).HasMaxLength(255);
            entity.Property(e => e.Telefone).HasMaxLength(15);
        });

        modelBuilder.Entity<Veiculo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Veiculo__3214EC27A6D82869");

            entity.ToTable("Veiculo");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.EntregaId).HasColumnName("Entrega_ID");
            entity.Property(e => e.Marca).HasMaxLength(50);
            entity.Property(e => e.Modelo).HasMaxLength(50);
            entity.Property(e => e.Placa).HasMaxLength(10);
            entity.Property(e => e.TipoUsuarioId).HasColumnName("TipoUsuario_ID");

            entity.HasOne(d => d.Entrega).WithMany(p => p.Veiculos)
                .HasForeignKey(d => d.EntregaId)
                .HasConstraintName("FK__Veiculo__Entrega__787EE5A0");

            entity.HasOne(d => d.TipoUsuario).WithMany(p => p.Veiculos)
                .HasForeignKey(d => d.TipoUsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Veiculo__TipoUsu__778AC167");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
