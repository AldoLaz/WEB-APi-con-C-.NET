﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WebApiAutores;

#nullable disable

namespace WebApiAutores.Migrations
{
    [DbContext(typeof(AplicationDbContext))]
    [Migration("20240610201410_AutoresLibros")]
    partial class AutoresLibros
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("WebApiAutores.Entidades.Autor", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("id"));

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasMaxLength(80)
                        .HasColumnType("nvarchar(80)");

                    b.HasKey("id");

                    b.ToTable("Autores");
                });

            modelBuilder.Entity("WebApiAutores.Entidades.AutorLibro", b =>
                {
                    b.Property<int>("libroID")
                        .HasColumnType("int");

                    b.Property<int>("autorID")
                        .HasColumnType("int");

                    b.Property<int>("orden")
                        .HasColumnType("int");

                    b.HasKey("libroID", "autorID");

                    b.HasIndex("autorID");

                    b.ToTable("AutorLibros");
                });

            modelBuilder.Entity("WebApiAutores.Entidades.Comentarios", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Contenido")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("LibroId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("LibroId");

                    b.ToTable("Comentarios");
                });

            modelBuilder.Entity("WebApiAutores.Entidades.Libro", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("id"));

                    b.Property<string>("Titulo")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.HasKey("id");

                    b.ToTable("Libros");
                });

            modelBuilder.Entity("WebApiAutores.Entidades.AutorLibro", b =>
                {
                    b.HasOne("WebApiAutores.Entidades.Autor", "Autor")
                        .WithMany("AutorLibro")
                        .HasForeignKey("autorID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WebApiAutores.Entidades.Libro", "Libro")
                        .WithMany("AutorLibro")
                        .HasForeignKey("libroID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Autor");

                    b.Navigation("Libro");
                });

            modelBuilder.Entity("WebApiAutores.Entidades.Comentarios", b =>
                {
                    b.HasOne("WebApiAutores.Entidades.Libro", "Libro")
                        .WithMany("Comentario")
                        .HasForeignKey("LibroId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Libro");
                });

            modelBuilder.Entity("WebApiAutores.Entidades.Autor", b =>
                {
                    b.Navigation("AutorLibro");
                });

            modelBuilder.Entity("WebApiAutores.Entidades.Libro", b =>
                {
                    b.Navigation("AutorLibro");

                    b.Navigation("Comentario");
                });
#pragma warning restore 612, 618
        }
    }
}
