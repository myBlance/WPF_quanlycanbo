using Microsoft.EntityFrameworkCore;
using WPF_quanlycanbo.Model;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<CanBo> CanBos { get; set; }
       

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=BBLANCE;Database=WPF_quanlycanbo;Trusted_Connection=True;TrustServerCertificate=True;");
    }
}
