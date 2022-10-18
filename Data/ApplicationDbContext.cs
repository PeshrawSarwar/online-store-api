using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace online_store_api.Data{
    public class ApplicationDbContext : DbContext {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {}
        //  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //     => optionsBuilder.UseNpgsql("");

        public DbSet<Product> Products { get; set; }

        // users
        public DbSet<User> Users { get; set; }
  
    }

    // table na§
    [Table("products")]
    public class Product{
        public int Id { get; set;}
        [MaxLength(50)]

        public string Name { get; set; }
    }

    // users table
    [Table("users")]
    public class User{

        [StringLength(50)]
        public string Id { get; set;}

        [StringLength(50)]
        public string FullName { get; set; }

        [StringLength(50)]
        // email
        public string Email { get; set; }

        [StringLength(50)]
        public string PasswordHash { get; set; }

        [StringLength(50)]
        public string PasswordSalt { get; set; }

        // Role
        [StringLength(50)]
        public string Role { get; set; }

        public bool IsActive { get; set; } = true;
    }

    

    

    

    
    
}