using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using MessagePack;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Facade.Services
{
    public class FacadeAuthenticationHandler : AuthenticationHandler<FacadeAuthenticationOptions>
    {
        private const string SchemeName = "Facade";
        
        public FacadeAuthenticationHandler(IOptionsMonitor<FacadeAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:24019/facadeauth")
            };
            var request = new HttpRequestMessage(HttpMethod.Get, "");

            foreach (var header in Request.Headers)
            {
                if (header.Key.Equals("Cookie", StringComparison.OrdinalIgnoreCase)
                    || header.Key.Equals("Authorization", StringComparison.OrdinalIgnoreCase))
                {
                    request.Headers.Add(header.Key, header.Value.AsEnumerable());
                }
            }

            try
            {
                var response = await httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var stream = await response.Content.ReadAsStreamAsync();
                    var claimsPrincipal = await DeserializePrincipal(stream);
                    var ticket = new AuthenticationTicket(claimsPrincipal, SchemeName);
                    return AuthenticateResult.Success(ticket);
                }
                else
                {
                    return AuthenticateResult.NoResult();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return AuthenticateResult.Fail(ex);
            }
        }

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            var relativeUri = Request.GetEncodedPathAndQuery();
            var escaped = Uri.EscapeDataString(relativeUri);
            Response.StatusCode = 302;
            Response.Headers["Location"] = $"/Account/Login?ReturnUrl={escaped}";
            return Task.CompletedTask;
        }

        private static async Task<ClaimsPrincipal> DeserializePrincipal(Stream stream)
        {
            var messagePrincipal =
                await MessagePackSerializer.DeserializeAsync<MessagePrincipal>(stream);
            return messagePrincipal.ToClaimsPrincipal();
        }
    }
}