using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JWTWebSite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

using Microsoft.IdentityModel.KeyVaultExtensions;
using Microsoft.Azure.Services.AppAuthentication;

namespace JWTWebSite.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        [BindProperty]
        public Message Message { get; set; }
   
        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

        }

        public void OnPost()
        {
            var texttosign = Message.Body;
            var validinseconds = Message.validinseconds;
            if (validinseconds == 0) validinseconds = 60;

            string issuer = "Matthijs";
            string audience = "my custom audience";

            DateTime signDate = DateTime.UtcNow;
            DateTime expiryDate = DateTime.UtcNow.AddDays(3);

            var claims = new ClaimsIdentity();
            claims.AddClaims(new List<Claim>
            {
                new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub, "Subject what does this need to be?"),
                new Claim("correlation_id", "123"),
                new Claim("skus", "SKU1"),
                new Claim("skus", "SKU2"),
                new Claim("skus", "SKU3")
            });

            var azureServiceTokenProvider = new AzureServiceTokenProvider();

            var securitycredentials = new SigningCredentials(
                                         new KeyVaultSecurityKey("https://mahoekstkeyvault.vault.azure.net/keys/mahoekstsigningcert/433f9ac74bef4213817105287d954ba1",
                                         new KeyVaultSecurityKey.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback)), "RS256")
                                        {
                                            CryptoProviderFactory = new CryptoProviderFactory() { CustomCryptoProvider = new KeyVaultCryptoProvider() }
                                        };


            var handler = new JwtSecurityTokenHandler();
            var token = handler.CreateJwtSecurityToken(issuer: issuer,
                   audience: audience,
                   subject: claims,
                   notBefore: DateTime.UtcNow,
                   expires: DateTime.UtcNow.AddSeconds(validinseconds),
                   signingCredentials: securitycredentials
               );

            //Console.WriteLine(token.RawData);
            Message.SignedBody = token.RawData;
        }
    }
}
