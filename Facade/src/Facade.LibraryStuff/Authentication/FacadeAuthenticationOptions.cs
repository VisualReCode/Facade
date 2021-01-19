using Microsoft.AspNetCore.Authentication;

namespace Facade.LibraryStuff.Authentication
{
    public class FacadeAuthenticationOptions : AuthenticationSchemeOptions
    {
        public string BackendUrl { get; set; }
    }
}