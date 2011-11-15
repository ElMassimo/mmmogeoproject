using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using USARoadTrip.Silverlight.Services;
using USARoadTrip.Silverlight.Utility;
using USARoadTrip.Silverlight.ViewModels;
using USARoadTrip.Silverlight.WCFServices;

namespace USARoadTrip.Silverlight.UserControls
{
    public partial class RegistrationWindow : ChildWindow
    {
        public RegistrationWindow()
        {
            InitializeComponent();
            RegistrationForm.DataContext = new List<UserViewModel>() { new UserViewModel() };
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            UserViewModel registrationInfo = ((List<UserViewModel>)RegistrationForm.DataContext)[0];

            RegistrationValidationSummary.Errors.Clear();

            if (registrationInfo.AnyFieldEmpty)
                RegistrationValidationSummary.Errors.Add(new ValidationSummaryItem("Please fill all the fields in the form"));
            else if (!registrationInfo.PasswordsMatch)
                RegistrationValidationSummary.Errors.Add(new ValidationSummaryItem("Passwords do not match"));
            else
            {
                BusyIndicator.IsBusy = true;
                User user = registrationInfo.ToUser();
                RoadTripServices.GetRegistrationService(RegistrationService_Completed).RegisterAsync(user);
            }
        }

        private void RegistrationService_Completed(object sender, RegisterCompletedEventArgs e)
        {
            BusyIndicator.IsBusy = false;
            if (e.Error != null)
            {
                GuiUtils.ShowConnectionErrorMessage();
                this.DialogResult = false;
            }
            else if (e.Result)
                this.DialogResult = true;
            else
                MessageBox.Show("The username selected is not available", "Registration", MessageBoxButton.OK);
        }
    }
}
