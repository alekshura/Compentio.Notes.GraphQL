using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Compentio.Notes.GraphQL.Api.Authentication
{
    public class AdminAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public AdminAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, 
            ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, "Notes Admin"),
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                new Claim("role", "Admin")
            };

            var identity = new ClaimsIdentity(claims, "Notes");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "Notes");

            var result = AuthenticateResult.Success(ticket);
            return Task.FromResult(result);
        }
    }
}
