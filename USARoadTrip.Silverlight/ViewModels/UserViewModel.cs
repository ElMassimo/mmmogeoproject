using System;
using System.ComponentModel.DataAnnotations;
using USARoadTrip.Silverlight.WCFServices;

namespace USARoadTrip.Silverlight.ViewModels
{
    public class UserViewModel
    {
        [Display(Name = "Nick", Description = "Please enter your user name")]
        [Required]
        public string Nick { get; set; }

        [Display(Name = "Password", Description = "Please enter your password")]
        [DataType(DataType.Password)]
        [Required]
        public string Password { get; set; }

        [Display(Name = "Password confirmation", Description = "Please enter your password confirmation")]
        [DataType(DataType.Password)]
        [Required]
        public string PasswordConfirmation { get; set; }

        public bool AnyFieldEmpty
        {
            get
            {
                return String.IsNullOrWhiteSpace(Nick)
                    || String.IsNullOrWhiteSpace(Password)
                    || String.IsNullOrWhiteSpace(PasswordConfirmation);
            }
        }

        public bool PasswordsMatch
        {
            get { return Password == PasswordConfirmation; }
        }

        public UserViewModel() {}

        public UserViewModel(User user)
        {
            Nick = user.Nick;
            Password = user.Password;
        }

        public User ToUser()
        {
            return new User() { Nick = this.Nick, Password = this.Password };
        }
    }
}
