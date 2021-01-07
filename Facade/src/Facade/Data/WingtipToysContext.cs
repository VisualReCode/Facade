using Microsoft.EntityFrameworkCore;

namespace Facade.Data
{
    public class WingtipToysContext : DbContext
    {
        public WingtipToysContext(DbContextOptions options) : base(options)
        {
            
        }
        public DbSet<Category> Categories { get; set; }
        
        public DbSet<CartItem> CartItems { get; set; }
    }
}