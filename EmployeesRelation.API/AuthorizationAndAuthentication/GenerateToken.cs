using EmployeesRelation.API.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EmployeesRelation.API.AuthorizationAndAuthentication
{
    public class GenerateToken
    {
        private readonly TokenConfiguration _tokenConfiguration;

        public GenerateToken(TokenConfiguration tokenConfiguration)
        {
            _tokenConfiguration = tokenConfiguration;
        }

        public string GenerateJwt(Users user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_tokenConfiguration.Secret));
            var tokenHandler = new JwtSecurityTokenHandler();

            var nameClaim = new Claim(ClaimTypes.Name, user.UserName);
            var roleClaim = new Claim(ClaimTypes.Role, user.Role);
            var subjectClaim = new Claim(ClaimValueTypes.String, _tokenConfiguration.Subject);
            var moduleClaim = new Claim(ClaimValueTypes.String, _tokenConfiguration.Module);
            List<Claim> claims = new List<Claim>();
            claims.Add(nameClaim);
            claims.Add(roleClaim);
            claims.Add(subjectClaim);
            claims.Add(moduleClaim);

            var jwtToken = new JwtSecurityToken(
                issuer: _tokenConfiguration.Issuer,
                audience: _tokenConfiguration.Audience,
                claims: claims,
                expires: DateTime.Now.AddHours(_tokenConfiguration.ExpirationTimeInHours),
                signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature)
                );
            return tokenHandler.WriteToken(jwtToken);
        }
    }
}
