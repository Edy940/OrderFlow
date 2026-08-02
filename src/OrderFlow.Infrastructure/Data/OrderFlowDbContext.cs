using Microsoft.EntityFrameworkCore;
using OrderFlow.Domain.Entities;

namespace OrderFlow.Infrastructure.Data
{
    public class OrderFlowDbContext : DbContext
    {
        public OrderFlowDbContext(DbContextOptions<OrderFlowDbContext> options)
            : base(options)
        {
        }

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.Email).HasMaxLength(254);
                entity.Property(u => u.Nome).HasMaxLength(150);
                entity.Property(u => u.Papel).HasMaxLength(50);
                entity.HasMany(u => u.RefreshTokens)
                    .WithOne(t => t.Usuario)
                    .HasForeignKey(t => t.UsuarioId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasIndex(t => t.TokenHash).IsUnique();
                entity.Property(t => t.TokenHash).HasMaxLength(64);
                entity.Property(t => t.SubstituidoPorTokenHash).HasMaxLength(64);
            });
        }
    }
}
