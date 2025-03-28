namespace TimeSheet.AuthAPI.Domain
{
    public class UserSession
    {
        public int UserSessionId { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public bool UserLoggedOut { get; set; }
        public string Token { get; set; }
        public virtual User User { get; set; }

        public UserSession() { }

        public void Logout()
        {
            UserLoggedOut = true;
        }

        public UserSession(string token, User user)
        {
            CreatedDateTime = DateTime.Now;
            UserLoggedOut = false;
            Token = token;
            User = user;
        }
    }
}
