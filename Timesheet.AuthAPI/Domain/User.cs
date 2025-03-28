using System.Security.Cryptography;
using System.Text;

namespace TimeSheet.AuthAPI.Domain
{
    public class User
    {
        public User()
        {
            UserSessions = new List<UserSession>();
        }

        public User(string email, string password)
        {
            Email = email;
            Password = HashPassword(password);
            UserSessions = new List<UserSession>();
        }

        public bool PasswordMatchesUserPassword(string password) => HashPassword(password) == Password;


        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public virtual ICollection<UserSession> UserSessions { get; set; }

        private string HashPassword(string password)
        {
            SHA512 myHash = SHA512.Create();
            byte[] hash = myHash.ComputeHash(Encoding.ASCII.GetBytes(password)); //compute hash
            return Convert.ToBase64String(hash);
        }
    }
}
