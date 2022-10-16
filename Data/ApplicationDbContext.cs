using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace online_store_api.Data{
    public class ApplicationDbContext : DbContext {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {}
        //  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //     => optionsBuilder.UseNpgsql("");

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
    }

    // table na§
    [Table("users")]
    public class Product{
        public int Id { get; set;}
        [MaxLength(50)]

        public string Name { get; set; }
    }

    [Table("categories")]
    public class Category{
        public int Id { get; set;}
        [MaxLength(50)]

        public string Name { get; set; }
    }

    

    

    
    
}