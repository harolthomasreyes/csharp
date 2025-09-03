using System.Security.Claims;

public interface IJwtService
{
    string GenerateToken(IEnumerable<Claim> claims);
}