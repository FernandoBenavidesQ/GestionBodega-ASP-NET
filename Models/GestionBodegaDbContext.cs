using Microsoft.EntityFrameworkCore;
using GestionBodega.Models;

namespace GestionBodega.Models
{
    public class GestionBodegaDbContext : DbContext
    {
        public GestionBodegaDbContext(DbContextOptions<GestionBodegaDbContext> options) : base(options) { }

        public virtual DbSet<Categoria> Categorias { get; set; } = null!;
        public virtual DbSet<Material> Materials { get; set; } = null!;
        public virtual DbSet<Movimiento> Movimientos { get; set; } = null!;
        public virtual DbSet<Personal> Personals { get; set; } = null!;
        public virtual DbSet<Catalogo> Catalogos { get; set; } = null!;
        public virtual DbSet<Usuario> Usuarios { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>().ToTable("Usuario");
            modelBuilder.Entity<Categoria>().ToTable("Categoria");
            modelBuilder.Entity<Material>().ToTable("Material");
            modelBuilder.Entity<Personal>().ToTable("Personal");
            modelBuilder.Entity<Catalogo>().ToTable("Catalogo");

            modelBuilder.Entity<Movimiento>(entity => {
                entity.ToTable("Movimiento");
                entity.HasOne(d => d.Personal)
                    .WithMany(p => p.Movimiento)
                    .HasForeignKey(d => d.PersonalId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });
        }
    }
}