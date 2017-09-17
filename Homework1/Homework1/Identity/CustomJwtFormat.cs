using System;
using System.Configuration;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler.Encoder;
using System.IdentityModel.Tokens;
using Thinktecture.IdentityModel.Tokens;

namespace Homework1.Identity
{
    public class CustomJwtFormat : ISecureDataFormat<AuthenticationTicket>
    {
        private static readonly byte[] _secret = TextEncodings.Base64Url.Decode(ConfigurationManager.AppSettings["secret"]);
        private readonly string _issuer;

        public CustomJwtFormat(string issuer)
        {
            _issuer = issuer;
        }

        public string Protect(AuthenticationTicket data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            //var signingKey = new Microsoft.IdentityModel.Tokens.SigningCredentials(_secret, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256);
            //var signingKey = new System.IdentityModel.Tokens.SigningCredentials(
            //    new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.UTF8.GetBytes(ConfigurationManager.AppSettings["secret"])), 
            //    System.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256Signature);
            var signingKey = new HmacSigningCredentials(_secret);
            var issued = data.Properties.IssuedUtc;
            var expires = data.Properties.ExpiresUtc;

            return new System.IdentityModel.Tokens.JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(_issuer, null, data.Identity.Claims, issued.Value.UtcDateTime, expires.Value.UtcDateTime, signingKey));
        }

        public AuthenticationTicket Unprotect(string protectedText)
        {
            throw new NotImplementedException();
        }
    }
}