using ConstructionManager.Api.Application.DTOs;

namespace ConstructionManager.Api.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<TokenDto> LoginAsync(LoginDto dto);
        Task<TokenDto> RegisterAsync(RegisterDto dto, string role = "User");
        Task<TokenDto> RefreshTokenAsync(RefreshTokenDto dto);
        Task LogoutAsync(string username);
    }
}