
#region Usings

using AudioBoo.Helpers;
using AudioBoo.Web.Api;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

#endregion

namespace AudioBoo.Controls
{
    public class LoginStartingEventArgs : EventArgs
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public partial class LoginControl : UserControl
    {
        #region Events

        public event EventHandler<LoginStartingEventArgs> LoginStarting;
        public event EventHandler<EventArgs> RemoveKeyboard;

        #endregion


        #region Fields

        string usernameWatermarkText = "enter username";
        string passwordWatermarkText = "enter password";

        #endregion


        #region Constructors

        public LoginControl()
        {
            InitializeComponent();

            // Fill in the password watermark
            TextBoxPasswordWatermark.Text = passwordWatermarkText;
            TextBoxPasswordWatermark.Foreground = App.AppConstants.WatermarkTextColourBrush;

            // Fill in the email watermark
            TextBoxUsername.Text = usernameWatermarkText;
            TextBoxUsername.Foreground = App.AppConstants.WatermarkTextColourBrush;

            // Set the titles
            TitleUsername.ClearError();
            TitlePassword.ClearError();
        }

        #endregion


        #region Email Textbox

        /// <summary>
        /// The email box has got focus
        /// </summary>
        private void TextBoxEmail_GotFocus(object sender, RoutedEventArgs e)
        {
            //App.AppConstants.SetTextBoxFocusColours(sender as TextBox);

            // if our watermark is set in the control
            if (TextBoxUsername.Text == usernameWatermarkText)
            {
                TextBoxUsername.Text = string.Empty;
                TextBoxUsername.Foreground = App.AppConstants.NormalTextColourBrush;
            }
        }

        /// <summary>
        /// The email box has lost focus
        /// </summary>
        private void TextBoxEmail_LostFocus(object sender, RoutedEventArgs e)
        {
            // If nothing has been entered on leaving our textbox, then set our watermark again
            if (TextBoxUsername.Text == string.Empty)
            {
                TextBoxUsername.Text = usernameWatermarkText;
                TextBoxUsername.Foreground = App.AppConstants.WatermarkTextColourBrush;
            }
        }

        /// <summary>
        /// The email textbox has received a key up
        /// </summary>
        private void TextBoxEmail_KeyUp(object sender, KeyEventArgs e)
        {
            ResetErrors();

            // remove focus from the textbox when enter has been pressed
            if (e.Key == Key.Enter)
            {
                PasswordBoxUserPassword.Focus();
            }
        }

        #endregion


        #region Password Focus etc

        private void PasswordBoxUserPassword_GotFocus(object sender, RoutedEventArgs e)
        {
            //App.AppConstants.SetTextBoxFocusColours(sender as PasswordBox);

            // If the password box gets focus, turn off the watermark no matter what is in the password
            TextBoxPasswordWatermark.Opacity = 0;
            TextBoxPasswordWatermark.Background.Opacity = 0;

            // And turn on the password box
            PasswordBoxUserPassword.Opacity = 100;
        }

        private void PasswordBoxUserPassword_LostFocus(object sender, RoutedEventArgs e)
        {
            var passwordEmpty = string.IsNullOrEmpty(PasswordBoxUserPassword.Password);

            // If password empty, turn the password box off and display the watermark- if it has something in it we want to see the *'s in it
            PasswordBoxUserPassword.Opacity = passwordEmpty ? 0 : 100;

            // If password empty, turn the watermark on
            TextBoxPasswordWatermark.Opacity = passwordEmpty ? 100 : 0;
            TextBoxPasswordWatermark.Background.Opacity = TextBoxPasswordWatermark.Opacity;
        }


        /// <summary>
        /// The email textbox has received a key up
        /// </summary>
        private void PasswordBoxUserPassword_KeyUp(object sender, KeyEventArgs e)
        {
            ResetErrors();

            // remove focus from the textbox when enter has been pressed
            if (e.Key == Key.Enter)
            {
                if (RemoveKeyboard != null)
                {
                    RemoveKeyboard(this, EventArgs.Empty);
                }
            }
        }

        #endregion


        #region Login Has Been Pressed

        /// <summary>
        /// The containing page will call login when the user is ready to login
        /// </summary>
        internal void Login()
        {
            bool pageValid = false;

            ResetErrors();

            if (Validation.ValidateExists(TitleUsername, TextBoxUsername, usernameWatermarkText, "Please enter your username"))
            {
                if (Validation.ValidateExists(TitlePassword, PasswordBoxUserPassword, passwordWatermarkText, "Please enter your password"))
                {
                    pageValid = true;
                }
            }

            // If page is valid then do the login
            if (pageValid)
            {
                // Raise message to containing page telling them we are starting a login
                if (LoginStarting != null)
                {
                    LoginStarting(this, new LoginStartingEventArgs() { Username = TextBoxUsername.Text, Password = PasswordBoxUserPassword.Password });
                }
            }
        }

        #endregion



        private void ResetErrors()
        {
            TitleUsername.ClearError();
            TitlePassword.ClearError();
        }
    }
}
