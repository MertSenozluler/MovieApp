using System.ComponentModel.DataAnnotations;


namespace MovieApp.Models.Domain
{
    public class ForgotPasswordModel
    {
        public int Id { get; set; }
        [Required, EmailAddress, Display(Name = "Registered email address")]
        public string Email { get; set; }
        public bool EmailSent { get; set; }

    }
}
