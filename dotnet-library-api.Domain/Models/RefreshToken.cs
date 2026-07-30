
namespace dotnet_library_api.Domain.Models;
public class RefreshToken
{
    public int Id { get; set; }
    public string Token { get; set; } = string.Empty;
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public DateTime ExpirationDate { get; set; }
    public bool IsRevoked { get; set; }
}
