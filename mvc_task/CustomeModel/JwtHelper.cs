using System;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

public class JwtHelper
{
    private static readonly string SecretKey = "UHJTFRTYUY787FVGHMJYAERvlkuytnbf";
    private static readonly string Issuer = "HelpHubAdminPanel";
    private static readonly string Audience = "SecureApiUser";

    public static string GenerateToken(int EmployeeId, string username, int expireMinutes = 120)
    {
        var claims = new[]
        {
            //new Claim(JwtHeaderParameterNames.Jku, username),
            new Claim(JwtHeaderParameterNames.Kid, Convert.ToString(EmployeeId)),
            new Claim(ClaimTypes.NameIdentifier, username)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: Issuer,
            audience: Audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(expireMinutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public static string ValidateToken(string token)
    {
        if (token == null)
        {
            return null;
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(SecretKey);

        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            //var jku = jwtToken.Claims.First(claim => claim.Type == "jku").Value;
            var userName = jwtToken.Claims.First(claim => claim.Type == "kid").Value;

            return userName;
        }
        catch
        {
            return null;
        }
    }
}
