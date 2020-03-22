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
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;

namespace JWTWebSite.Pages
{
    public class VerifyModel : PageModel
    {
        private readonly ILogger<VerifyModel> _logger;
        [BindProperty]
        public Message Message { get; set; }

        public VerifyModel(ILogger<VerifyModel> logger)
        {
            _logger = logger;
        }
        public void OnGet()
        {

        }

        public async void OnPost()
        {
            //REPLACE BY OTHER PROJECT
            //JUST FOR REFERENCE NOW
            Message.Body = "started validating";

            SecurityToken parsedToken = null;

            var certid = "https://mahoekstkeyvault.vault.azure.net/certificates/mahoekstsigningcert/433f9ac74bef4213817105287d954ba1";
            var provider = new AzureServiceTokenProvider();
            var kv = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(provider.KeyVaultTokenCallback));
            var cert = await kv.GetCertificateAsync(certid); //we schould cache this cert for x hours 


            string signedtokentoverify = Message.SignedBody;


            X509Certificate2 tokenSigningCert = new X509Certificate2(cert.Cer);

            var validationParameters = new TokenValidationParameters
            {
                ValidAudience = "my custom audience",
                ValidIssuer = "Matthijs",
                IssuerSigningKey = new X509SecurityKey(tokenSigningCert),
                ValidateLifetime = true,
                RequireExpirationTime = true

            };

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var claimsPrincipal = tokenHandler.ValidateToken(signedtokentoverify, validationParameters, out parsedToken);
                Message.Body = parsedToken.ToString();
            }
            catch (Exception ex)
            {
                Message.Body = "ERROR VALIDATING:" + ex.Message;

            }

            Message.Body = "Matthijs was here!";



        }
    }
}