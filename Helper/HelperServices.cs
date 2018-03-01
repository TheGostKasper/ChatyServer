using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using signalr_server.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace signalr_server.Helper
{
    public class HelperServices
    {
        public IConfiguration Configuration { get; }

        public HelperServices(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public static string MD5Hash(string text)
        {
            MD5 md5 = new MD5CryptoServiceProvider();

            //compute hash from the bytes of text
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));

            //get hash result after compute it
            byte[] result = md5.Hash;

            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                //change it into 2 hexadecimal digits
                //for each byte
                strBuilder.Append(result[i].ToString("x2"));
            }

            return strBuilder.ToString();
        }

        public string GetToken(User user)
        {
            var handler = new JwtSecurityTokenHandler();
            ClaimsIdentity identity = new ClaimsIdentity(
                new GenericIdentity(user.Email, "TokenAuth"),
                new[] {
                    new Claim("ID", user.UserId.ToString())
                }
            );

            var securityToken = handler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = Configuration["Issuer"],
                Audience = Configuration["Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SigningKey"])),
                           SecurityAlgorithms.HmacSha256),
                Subject = identity,
                Expires= DateTime.UtcNow.AddDays(60),
                NotBefore= DateTime.UtcNow,
            });

            //var token = new JwtSecurityToken
            //   (
            //       expires: DateTime.UtcNow.AddDays(60),
            //       notBefore: DateTime.UtcNow,
            //       signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SigningKey"])),
            //               SecurityAlgorithms.HmacSha256)
            //   );

            return handler.WriteToken(securityToken);
        }
    }
}

