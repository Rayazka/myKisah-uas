// DOMAIN: Core
// PURPOSE: Scoped service — identitas user login per Blazor circuit (Inject di Program.cs)

using myKisah.Models;

namespace myKisah.Services;

public class UserSession
{
    public User? CurrentUser { get; private set; }
    public bool IsLoggedIn => CurrentUser != null;

    public void Login(User user) => CurrentUser = user;
    public void Logout() => CurrentUser = null;
}
