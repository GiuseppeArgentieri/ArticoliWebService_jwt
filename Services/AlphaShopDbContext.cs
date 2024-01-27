using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArticoliWebService.Models;
using Microsoft.EntityFrameworkCore;

namespace ArticoliWebService.Services
{
    public class AlphaShopDbContext : DbContext
    {
        public AlphaShopDbContext(DbContextOptions<AlphaShopDbContext> options) : base(options)
        {
            
        }

        public virtual DbSet<Articoli> Articoli {get; set;}
        public virtual DbSet<Ean> Barcode {get; set;}
        public virtual DbSet<FamAssort> FamAssort {get; set;}
        public virtual DbSet<Ingredienti> Ingredienti {get; set;}
        public virtual DbSet<Iva> Iva {get; set;}
        public virtual DbSet<Utenti> Utenti { get; set; }
        public virtual DbSet<Profili> Profili { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Articoli>()
                .HasKey(a => new{a.CodArt});

            //relazione 1 a * tra art e barcode
            modelBuilder.Entity<Ean>()
                .HasOne<Articoli>(s => s.articolo)
                .WithMany(g => g.Barcode)
                .HasForeignKey(s => s.CodArt);

            //relazione 1 a 1 tra articoli e ingredienti
            modelBuilder.Entity<Articoli>()
                .HasOne<Ingredienti>(s => s.ingredienti)
                .WithOne(g => g.articolo)
                .HasForeignKey<Ingredienti>(s => s.CodArt);

            //relazione 1 a molti tra iva e articoli
            modelBuilder.Entity<Articoli>()
                .HasOne<Iva>(s => s.iva)
                .WithMany(g => g.articoli)
                .HasForeignKey(s => s.IdIva);

            //relazione 1 a molti tra famassort e articoli
            modelBuilder.Entity<Articoli>()
                .HasOne<FamAssort>(s => s.famAssort)
                .WithMany(g => g.articoli)
                .HasForeignKey(s => s.IdFamAss);

            modelBuilder.Entity<Utenti>()
                .HasKey(a => new {a.CodFidelity});

            modelBuilder.Entity<Profili>()
                .Property(a => a.Id)
                .ValueGeneratedOnAdd();
            
            modelBuilder.Entity<Profili>()
                .HasOne<Utenti>(s => s.Utente)
                .WithMany(g => g.Profili)
                .HasForeignKey(g => g.CodFidelity);
        }

    }
}