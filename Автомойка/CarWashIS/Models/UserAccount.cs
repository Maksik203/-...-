namespace CarWashIS.Models;

internal sealed class UserAccount
{
    public int Id { get; init; }
    public required string Login { get; init; }
    public UserRole Role { get; init; }
}
