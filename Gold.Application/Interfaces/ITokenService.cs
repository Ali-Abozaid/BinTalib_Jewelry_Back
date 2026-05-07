using Gold.Core.Entities;

namespace Gold.Application.Interfaces;

public interface ITokenService
{
    (string token, DateTime expiresAt) CreateToken(ApplicationUser user, IEnumerable<string> roles);
}
