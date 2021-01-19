﻿using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using MessagePack;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Facade.LibraryStuff.Authentication
{
    public class AuthenticationClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AuthenticationClient> _logger;

        public AuthenticationClient(HttpClient httpClient, ILogger<AuthenticationClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<AuthenticateResult> AuthenticateAsync(HttpRequest request)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "");

            foreach (var header in request.Headers)
            {
                if (header.Key.Equals("Cookie", StringComparison.OrdinalIgnoreCase)
                    || header.Key.Equals("Authorization", StringComparison.OrdinalIgnoreCase))
                {
                    requestMessage.Headers.Add(header.Key, header.Value.AsEnumerable());
                }
            }

            try
            {
                var response = await _httpClient.SendAsync(requestMessage);

                if (response.IsSuccessStatusCode)
                {
                    var stream = await response.Content.ReadAsStreamAsync();
                    var claimsPrincipal = await DeserializePrincipal(stream);
                    var ticket = new AuthenticationTicket(claimsPrincipal, FacadeAuthenticationDefaults.Scheme);
                    return AuthenticateResult.Success(ticket);
                }

                return AuthenticateResult.NoResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return AuthenticateResult.Fail(ex);
            }
        }

        private static async Task<ClaimsPrincipal> DeserializePrincipal(Stream stream)
        {
            var messagePrincipal =
                await MessagePackSerializer.DeserializeAsync<MessagePrincipal>(stream);
            return messagePrincipal.ToClaimsPrincipal();
        }
    }
}