namespace Hubly.api.Domain.Entities{

    public class AuthenticatedUser
    {

        public int Id { get; set; }

        public string Token { get; set; }
        public string Username { get; set; }
        
    }
}