using System.Collections.Generic;
using Facade.Data;

namespace Facade.Models.Admin
{
    public class AdminPageViewModel
    {
        public AdminPageViewModel(List<Product> products)
        {
            Products = products;
        }

        public List<Product> Products { get; }
    }
}