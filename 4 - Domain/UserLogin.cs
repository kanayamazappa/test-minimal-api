namespace Domain
{
    public class UserLogin
    {
        public UserLogin(string username, string password)
        {
            this.Username = username;
            this.Password = password;
        }
        
        public string Username { get; set; }
        public string Password { get; set; }
    }
}