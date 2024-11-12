using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;

namespace Application.CryptoFacilBrasil.BasicAuthentication
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            System.Text.Encodings.Web.UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock) { }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            Console.WriteLine("Request Headers: ");
            foreach (var header in Request.Headers)
            {
                Console.WriteLine($"{header.Key}: {string.Join(", ", header.Value)}");
            }
            // Verificar se o cabeçalho Authorization existe
            if (!Request.Headers.ContainsKey("Authorization"))
                return Task.FromResult(AuthenticateResult.Fail("Authorization header missing"));

            try
            {
                var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                var credentialsBytes = Convert.FromBase64String(authHeader.Parameter ?? string.Empty);
                var credentials = Encoding.UTF8.GetString(credentialsBytes).Split(':', 2);
                var username = credentials[0];
                var password = credentials[1];

                // Validar credenciais (usuarios fixos)
                if (!IsValidUser(username, password))
                    return Task.FromResult(AuthenticateResult.Fail("Invalid username or password"));

                var claims = new[] { new Claim(ClaimTypes.Name, username) };
                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);

                return Task.FromResult(AuthenticateResult.Success(ticket));
            }
            catch
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization header"));
            }
        }

        private bool IsValidUser(string username, string password)
        {
            // Defina os dois usuários e senhas fixos
            var users = new Dictionary<string, string>
            {
                { "thiago.monteiro", "Xr7$bkQ1z!" },
                { "victor.oliveira", "qW4%Lm8@jK" },
                { "botApplication", "(Pdzk`94tS:y#YqG"}
            };

            return users.TryGetValue(username, out var validPassword) && validPassword == password;
        }
    }
}
