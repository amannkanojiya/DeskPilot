using Identity.Api.Models;

namespace Identity.Api.Services;

public interface ITokenService
{
    string GenerateAccessToken(User user);
}