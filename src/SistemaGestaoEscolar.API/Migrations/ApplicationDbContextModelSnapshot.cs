using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SistemaGestaoEscolar.API.Configuration;

#nullable disable

namespace SistemaGestaoEscolar.API.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("SistemaGestaoEscolar.Escolas.Dominio.Entidades.Escola", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("CNPJ")
                        .IsRequired()
                        .HasMaxLength(14)
                        .HasColumnType("varchar(14)");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)");

                    b.Property<Guid?>("RedeEscolarId")
                        .HasColumnType("char(36)");

                    b.Property<int>("TipoEscola")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.HasIndex("CNPJ")
                        .IsUnique();

                    b.HasIndex("RedeEscolarId");

                    b.ToTable("Escolas", (string)null);
                });

            modelBuilder.Entity("SistemaGestaoEscolar.Escolas.Dominio.Entidades.RedeEscolar", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("CNPJ")
                        .IsRequired()
                        .HasMaxLength(14)
                        .HasColumnType("varchar(14)");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.HasIndex("CNPJ")
                        .IsUnique();

                    b.ToTable("RedesEscolares", (string)null);
                });

            modelBuilder.Entity("SistemaGestaoEscolar.Escolas.Dominio.Entidades.Escola", b =>
                {
                    b.HasOne("SistemaGestaoEscolar.Escolas.Dominio.Entidades.RedeEscolar", "RedeEscolar")
                        .WithMany("Escolas")
                        .HasForeignKey("RedeEscolarId");

                    b.OwnsOne("SistemaGestaoEscolar.Escolas.Dominio.ObjetosDeValor.Endereco", "Endereco", b1 =>
                        {
                            b1.Property<Guid>("EscolaId")
                                .HasColumnType("char(36)");

                            b1.Property<string>("Bairro")
                                .IsRequired()
                                .HasMaxLength(100)
                                .HasColumnType("varchar(100)")
                                .HasColumnName("Endereco_Bairro");

                            b1.Property<string>("CEP")
                                .IsRequired()
                                .HasMaxLength(8)
                                .HasColumnType("varchar(8)")
                                .HasColumnName("Endereco_CEP");

                            b1.Property<string>("Cidade")
                                .IsRequired()
                                .HasMaxLength(100)
                                .HasColumnType("varchar(100)")
                                .HasColumnName("Endereco_Cidade");

                            b1.Property<string>("Complemento")
                                .HasMaxLength(100)
                                .HasColumnType("varchar(100)")
                                .HasColumnName("Endereco_Complemento");

                            b1.Property<string>("Estado")
                                .IsRequired()
                                .HasMaxLength(2)
                                .HasColumnType("varchar(2)")
                                .HasColumnName("Endereco_Estado");

                            b1.Property<string>("Logradouro")
                                .IsRequired()
                                .HasMaxLength(200)
                                .HasColumnType("varchar(200)")
                                .HasColumnName("Endereco_Logradouro");

                            b1.Property<string>("Numero")
                                .IsRequired()
                                .HasMaxLength(10)
                                .HasColumnType("varchar(10)")
                                .HasColumnName("Endereco_Numero");

                            b1.HasKey("EscolaId");

                            b1.ToTable("Escolas");

                            b1.WithOwner()
                                .HasForeignKey("EscolaId");
                        });

                    b.Navigation("Endereco")
                        .IsRequired();

                    b.Navigation("RedeEscolar");
                });

            modelBuilder.Entity("SistemaGestaoEscolar.Escolas.Dominio.Entidades.RedeEscolar", b =>
                {
                    b.Navigation("Escolas");
                });
#pragma warning restore 612, 618
        }
    }
}