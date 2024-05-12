using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MobileAppTp
{
    public class AppDbContext : IdentityDbContext<AppUser,IdentityRole<string>, string>
    {
        public AppDbContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public DbSet<Product> Products { get; set; }
        public IConfiguration Configuration { get; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //if (Configuration["UseSqlite"] == "True")
            //{
            //    optionsBuilder.UseSqlite("Data Source=mydb.db");
            //    this.Database.EnsureCreated();
            //    this.Database.Migrate();
            //}
            //else
            //{
                optionsBuilder.UseSqlServer("Server=sql.bsite.net\\MSSQL2016;Database=bouzid1096_;User Id=bouzid1096_;password=code123456789;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
            //}
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AppUser>()
                .HasMany(a => a.Favorites)
                .WithMany();
        }
    }
}
