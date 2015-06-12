
#region Usings

using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using AudioBoo.Controls;
using AudioBoo.Web.Api;
using AudioBoo.Web.Responses;
using System;
using System.Windows;

#endregion


namespace AudioBoo.Views.Account
{
    public partial class RegisterPage : PhoneApplicationPage
    {
        #region Fields

        private static AudioBooAPI _api;

        #endregion


        #region Constructors

        public RegisterPage()
        {
            InitializeComponent();

            // Subscribe to the register events
            RegisterControl.RegisterStarting += RegisterControl_RegisterStarting;
            RegisterControl.NavigateTo += RegisterControl_NavigateTo;

            // Create the app bar
            BuildApplicationBar();

            // Initialise an instance of the user api
            _api = new AudioBooAPI();
            _api.RegisterUserCompleted += _api_RegisterUserCompleted;
        }


        #endregion


        #region Event Handlers


        void RegisterControl_NavigateTo(object sender, RegisterControl.NavigateToEventArgs e)
        {
            this.NavigationService.Navigate(new Uri(e.Url, UriKind.Relative));
        }

        void RegisterControl_RegisterStarting(object sender, RegisterControl.RegisterStartingEventArgs e)
        {
            ApplicationBarIconButton b = (ApplicationBarIconButton)ApplicationBar.Buttons[0];
            b.IsEnabled = false;

            // Get rid of the keyboard
            this.Focus();

            // Start the registration
            _api.RegisterUser(e.Username, e.Password, e.Email);
        }

        void _api_RegisterUserCompleted(object sender, RegisterUserEventArgs e)
        {
            // show any general errors
            if (e.ServerResponse.body.HasErrors())
            {
                MessageBox.Show(e.ServerResponse.body.errors[0], "Registration Failed", MessageBoxButton.OK);
            }
            
            // If a user object is attached
            if (e.ServerResponse.body.HasUser())
            {
                if (e.ServerResponse.body.user.HasErrors())
                {
                    // Show a user error message
                    MessageBox.Show(e.ServerResponse.body.user.username + " " + e.ServerResponse.body.user.errors.username[0], "User Registration Failed", MessageBoxButton.OK);
                }
                else
                {
                    // Show a success message
                    MessageBox.Show(e.Message, "Registration Success", MessageBoxButton.OK);
                }
            }

            // re-enable the app bar
            ApplicationBarIconButton b = (ApplicationBarIconButton)ApplicationBar.Buttons[0];
            b.IsEnabled = true;
        }

        #endregion


        #region App Bar

        private void BuildApplicationBar()
        {
            ApplicationBar = new ApplicationBar();

            var button = new ApplicationBarIconButton(new Uri("/Resources/Images/AppBar/check.png", UriKind.RelativeOrAbsolute)) { Text = "Login" };
            button.Click += AppBarMenuTick_Click;
            ApplicationBar.Buttons.Add(button);
        }

        #endregion


        #region App Bar Events

        private void AppBarMenuTick_Click(object sender, EventArgs e)
        {
            RegisterControl.Register();
        }

        #endregion
    }
}