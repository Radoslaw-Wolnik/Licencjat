namespace Backend.Application.DTOs.Auth;

public class LoginUserInfo
{
  public Guid   Id          { get; set; }
  public string? Email       { get; set; } = default!;
  public string? UserName    { get; set; } = default!;
  // public string? PasswordHash{ get; set; } = default!;
  // any other security fields: SecurityStamp, etc.
}
