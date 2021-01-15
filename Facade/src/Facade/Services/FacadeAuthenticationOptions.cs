using Microsoft.AspNetCore.Authentication;

namespace Facade.Services
{
    public class FacadeAuthenticationOptions : AuthenticationSchemeOptions
    {
        public string BackendUrl { get; set; }
    }
}