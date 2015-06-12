
#region Usings

using AudioBoo.Helpers;
using AudioBoo.Views.Account;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

#endregion


namespace AudioBoo.Controls
{
    public partial class RegisterControl
    {
        public class RegisterStartingEventArgs : EventArgs
        {
            public string Username { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class NavigateToEventArgs : EventArgs
        {
            public string Url { get; set; }
        }

        #region Events

        public event EventHandler<RegisterStartingEventArgs> RegisterStarting;
        public event EventHandler<NavigateToEventArgs> NavigateTo;

        #endregion


        #region Fields

        readonly string usernameWatermark = LanguageHelper.Register.UsernameWatermark;
        readonly string emailWatermark = LanguageHelper.Register.EmailWatermark;
        readonly string passwordWatermark = LanguageHelper.Register.PasswordWatermark;
        readonly string passwordConfirmWatermark = LanguageHelper.Register.ConfirmPasswordWatermark;

        #endregion


        #region Constructors

        public RegisterControl()
        {
            InitializeComponent();

            TextBoxUsername.Text = usernameWatermark;
            TextBoxUsername.Foreground = App.AppConstants.WatermarkTextColourBrush;

            TextBoxEmail.Text = emailWatermark;
            TextBoxEmail.Foreground = App.AppConstants.WatermarkTextColourBrush;

            TextBoxPasswordWatermark.Text = passwordWatermark;
            TextBoxPasswordWatermark.Foreground = App.AppConstants.WatermarkTextColourBrush;

            TextBoxPasswordConfirmWatermark.Text = passwordConfirmWatermark;
            TextBoxPasswordConfirmWatermark.Foreground = App.AppConstants.WatermarkTextColourBrush;

            // Set the titles
            ResetErrors();

            HyperlinkTermsOfService.Click += HyperlinkTermsOfService_Click;
        }

        #endregion


        #region Navigation

        void HyperlinkTermsOfService_Click(object sender, RoutedEventArgs e)
        {
            if (NavigateTo != null)
            {
                NavigateTo(this, new NavigateToEventArgs() { Url = "/Views/BrowserPage.xaml?url=http://audioboo.fm/terms" });
            }
        }

        #endregion


        #region Username Textbox

        /// <summary>
        /// The email box has got focus
        /// </summary>
        private void TextBoxUsername_GotFocus(object sender, RoutedEventArgs e)
        {
            // if our watermark is set in the control
            if (TextBoxUsername.Text == usernameWatermark)
            {
                TextBoxUsername.Text = string.Empty;
                TextBoxUsername.Foreground = App.AppConstants.NormalTextColourBrush;
            }
        }

        /// <summary>
        /// The email box has lost focus
        /// </summary>
        private void TextBoxUsername_LostFocus(object sender, RoutedEventArgs e)
        {
            // If nothing has been entered on leaving our textbox, then set our watermark again
            if (TextBoxUsername.Text == string.Empty)
            {
                TextBoxUsername.Text = usernameWatermark;
                TextBoxUsername.Foreground = App.AppConstants.WatermarkTextColourBrush;
            }
        }

        /// <summary>
        /// Also validate on a key up
        /// </summary>
        private void TextBoxUsername_KeyUp(object sender, KeyEventArgs e)
        {
            ResetErrors();

            // remove focus from the textbox when enter has been pressed
            if (e.Key == Key.Enter)
            {
                TextBoxEmail.Focus();
            }
        }

        #endregion


        #region Email Textbox

        /// <summary>
        /// The email box has got focus
        /// </summary>
        private void TextBoxEmail_GotFocus(object sender, RoutedEventArgs e)
        {
            // if our watermark is set in the control
            if (TextBoxEmail.Text == emailWatermark)
            {
                TextBoxEmail.Text = string.Empty;
                TextBoxEmail.Foreground = App.AppConstants.NormalTextColourBrush;
            }
        }

        /// <summary>
        /// The email box has lost focus
        /// </summary>
        private void TextBoxEmail_LostFocus(object sender, RoutedEventArgs e)
        {
            // If nothing has been entered on leaving our textbox, then set our watermark again
            if (TextBoxEmail.Text == string.Empty)
            {
                TextBoxEmail.Text = emailWatermark;
                TextBoxEmail.Foreground = App.AppConstants.WatermarkTextColourBrush;
            }
        }

        /// <summary>
        /// Also validate on a key up
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
            // Reset the errors if a key has been pressed
            if (e.Key != Key.Unknown)
            {
                ResetErrors();
            }

            // remove focus from the textbox when enter has been pressed
            if (e.Key == Key.Enter)
            {
                PasswordBoxUserPasswordConfirm.Focus();
            }
        }

        #endregion


        #region Password ConfirmFocus etc

        private void PasswordBoxUserPasswordConfirm_GotFocus(object sender, RoutedEventArgs e)
        {
            // If the password box gets focus, turn off the watermark no matter what is in the password
            TextBoxPasswordConfirmWatermark.Opacity = 0;
            TextBoxPasswordConfirmWatermark.Background.Opacity = 0;

            // And turn on the password box
            PasswordBoxUserPasswordConfirm.Opacity = 100;
        }

        private void PasswordBoxUserPasswordConfirm_LostFocus(object sender, RoutedEventArgs e)
        {
            var passwordEmpty = string.IsNullOrEmpty(PasswordBoxUserPasswordConfirm.Password);

            // If password empty, turn the password box off and display the watermark- if it has something in it we want to see the *'s in it
            PasswordBoxUserPasswordConfirm.Opacity = passwordEmpty ? 0 : 100;

            // If password empty, turn the watermark on
            TextBoxPasswordConfirmWatermark.Opacity = passwordEmpty ? 100 : 0;
            TextBoxPasswordConfirmWatermark.Background.Opacity = TextBoxPasswordConfirmWatermark.Opacity;
        }

        /// <summary>
        /// The email textbox has received a key up
        /// </summary>
        private void PasswordBoxUserPasswordConfirm_KeyUp(object sender, KeyEventArgs e)
        {
            ResetErrors();

            // remove focus from the textbox when enter has been pressed
            if (e.Key == Key.Enter)
            {
                //this.Focus();
            }
        }

        public void ResetErrors()
        {
            TitleUsername.ClearError();
            TitleEmail.ClearError();
            TitlePassword.ClearError();
            TitlePasswordConfirm.ClearError();
            TitleAcceptTC.ClearError();
        }

        #endregion


        #region Regsitration Events

        public void Register()
        {
            bool pageValid = false;

            ResetErrors();

            if (Validation.ValidateExists(TitleUsername, TextBoxUsername, usernameWatermark, "Please choose a username"))
            {
                if (Validation.ValidateExists(TitleEmail, TextBoxEmail, emailWatermark, "Please enter your email address"))
                {
                    if (Validation.EmailValid(TitleEmail, TextBoxEmail, emailWatermark, "Please enter a valid email address"))
                    {
                        if (Validation.ValidateExists(TitlePassword, PasswordBoxUserPassword, passwordWatermark, "Please enter your password"))
                        {
                            if (Validation.MinLength(TitlePassword, PasswordBoxUserPassword, passwordWatermark, "At least 6 characters"))
                            {
                                if (Validation.SamePassword(TitlePasswordConfirm, PasswordBoxUserPasswordConfirm, PasswordBoxUserPassword.Password, passwordConfirmWatermark, "Passwords must match"))
                                {
                                    if (CheckBoxTC.IsChecked == true)
                                    {
                                        pageValid = true;
                                    }
                                    else
                                    {
                                        MessageBox.Show("Please accept the terms and conditions", "Terms and conditions", MessageBoxButton.OK);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if ( pageValid )
            {
                string terms = CheckBoxTC.IsChecked == true ? "on" : "off";

                // Raise message to containing page telling them we are starting a login
                if (RegisterStarting != null)
                {
                    RegisterStarting(this, new RegisterStartingEventArgs() { Username=TextBoxUsername.Text, Email=TextBoxEmail.Text, Password=PasswordBoxUserPassword.Password });
                }
            }
        }

        #endregion
    }
}
