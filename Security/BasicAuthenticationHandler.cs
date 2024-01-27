using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using ArticoliWebService.Models;
using ArticoliWebService.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace ArticoliWebService.Security
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IUserService userService;

        public BasicAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, 
        ILoggerFactory logger, 
        UrlEncoder encoder,
         ISystemClock clock,
         IUserService userService) : base(options, logger, encoder, clock)
        {
            this.userService = userService;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if(!Request.Headers.ContainsKey("Authorization"))
                return AuthenticateResult.Fail("Authorization Header Mancante");

            Utenti utente = null;
            bool isOk = false;

            try
            {
                var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
                var credentials = Encoding.UTF8.GetString(credentialBytes).Split(":");

                var username = credentials[0];
                var password = credentials[1];

                isOk = await userService.Authenticate(username, password);

                if(isOk)
                {
                    utente = await userService.GetUser(username);
                }
            }
            catch(Exception ex)
            {
                return AuthenticateResult.Fail("Authorization Header Non Valido!");
            }

            if(!isOk)
            {
                return AuthenticateResult.Fail("Nome utente o password errati");
            }

            ICollection<Profili> userProfiles = utente.Profili;

            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, utente.UserId));

            foreach(var profile in userProfiles)
            {
                claims.Add(new Claim(ClaimTypes.Role,profile.Tipo));
            }

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);
            
            return AuthenticateResult.Success(ticket);
        }
    }
}