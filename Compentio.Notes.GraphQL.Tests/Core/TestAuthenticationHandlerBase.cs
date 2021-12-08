namespace Compentio.Notes.GraphQL.Tests.Core
{
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using System.Security.Claims;
    using System.Text.Encodings.Web;
    using System.Threading.Tasks;


    public abstract class TestAuthenticationHandlerBase : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        protected TestAuthenticationHandlerBase(IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected abstract Claim[] CreateClaims();

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = CreateClaims();  

            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "Test");

            var result = AuthenticateResult.Success(ticket);
            return Task.FromResult(result);
        }
    }
}
