namespace Compentio.Notes.GraphQL.Tests.Core
{
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using System.Security.Claims;
    using System.Text.Encodings.Web;


    public class TestAuthenticationHandlerWithoutClaims : TestAuthenticationHandlerBase
    {
        public TestAuthenticationHandlerWithoutClaims(IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override Claim[] CreateClaims() => new[]
        {
            new Claim(ClaimTypes.Name, "Test user")
        };
    }
}
