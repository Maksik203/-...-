using CarWashIS.Models;

namespace CarWashIS.Data;

internal static class UserSession
{
    public static UserAccount? Current { get; private set; }

    public static bool IsLoggedIn => Current is not null;

    public static bool IsAdmin => Current?.Role == UserRole.Admin;

    public static void SignIn(UserAccount account) => Current = account;

    public static void SignOut() => Current = null;
}
