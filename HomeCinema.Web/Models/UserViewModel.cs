namespace HomeCinema.Web.Models
{
    // TODO: Should be called 'Dto'
    public class UserViewModel
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public bool IsLocked { get; set; }
    }
}