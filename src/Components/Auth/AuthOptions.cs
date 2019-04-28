using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Auth
{
    public class AuthOptions
    {
        public const string ISSUER = "MyAuthServer";
        public const string AUDIENCE = "http://localhost:3001/";
        const string KEY = "@e38hr@!r%$^%@^$7324f2&%$%$&&*@sd";
        public const int LIFETIME = 1;
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}