using System.Linq;
using System.Threading.Tasks;
using Facade.Data;
using Facade.LibraryStuff.Session;
using Microsoft.EntityFrameworkCore;

namespace Facade.Services
{
    public class ShoppingCart
    {
        private FacadeSession _session;
        private WingtipToysContext _db;
        
        public ShoppingCart(FacadeSession session, WingtipToysContext db)
        {
            _session = session;
            _db = db;
        }

        public async Task<int> GetCountAsync()
        {
            var cartId = await _session.GetAsync<string>("CartId");
            var count = await _db.CartItems
                .Where(item => item.CartId == cartId)
                .Select(item => item.Quantity)
                .SumAsync();
            return count;
        }
    }
}