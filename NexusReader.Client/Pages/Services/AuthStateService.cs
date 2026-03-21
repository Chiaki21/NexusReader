namespace NexusReader.Services
{
    // Rename the class to match your filename
    public class AuthStateService 
    {
        public bool IsAdmin { get; private set; } = false;

        public event Action? OnAuthStateChanged;

        public void SetAdmin(bool isAdmin)
        {
            IsAdmin = isAdmin;
            OnAuthStateChanged?.Invoke();
        }

        public void Logout()
        {
            IsAdmin = false;
            OnAuthStateChanged?.Invoke();
        }
    }
}