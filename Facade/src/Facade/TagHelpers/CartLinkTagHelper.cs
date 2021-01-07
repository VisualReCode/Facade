using System;
using System.Threading.Tasks;
using Facade.Services;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Facade.TagHelpers
{
    public class CartLinkTagHelper : TagHelper
    {
        private readonly ShoppingCart _shoppingCart;

        public CartLinkTagHelper(ShoppingCart shoppingCart)
        {
            _shoppingCart = shoppingCart;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "a";
            int count;
            try
            {
                count = await _shoppingCart.GetCountAsync();
            }
            catch
            {
                count = 0;
            }
            output.Content.SetContent($"Cart ({count})");
        }
    }
}