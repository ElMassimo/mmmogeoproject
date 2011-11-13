using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using USARoadTrip.Silverlight.WCFServices;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

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
