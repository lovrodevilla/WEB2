namespace WebApplication2.Models
{
    public class CreateUser
    {
        public int? Id { get; set; }

        public string? Username { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Email { get; set; }

        public string? Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
