using System.ComponentModel.DataAnnotations;

namespace MovieApp.Models.Domain
{
    public class ChangePasswordModel
    {
        public int Id { get; set; }
        [Required, DataType(DataType.Password), Display(Name = "Current Password")]
        public string CurrentPassword { get; set; }
        [Required, DataType(DataType.Password), Display(Name = "New Password")]
        public string NewPassword { get; set; }
        [Required, DataType(DataType.Password), Display(Name = "Confirm new pasword")]
        [Compare("NewPassword", ErrorMessage = "Passwords doesn't match")]
        public string ConfirmNewPassword {get;set;}
    }
}
