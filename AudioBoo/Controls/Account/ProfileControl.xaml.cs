
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
    public partial class ProfileControl
    {
        public class UpdateProfileEventArgs : EventArgs
        {
            public string RealName { get; set; }
            public string Location { get; set; }
            public string WebAddress { get; set; }
            public string Description { get; set; }
        }

        public class NavigateToEventArgs : EventArgs
        {
            public string Url { get; set; }
        }

        #region Events

        public event EventHandler<UpdateProfileEventArgs> UpdateProfileStarting;
        public event EventHandler<NavigateToEventArgs> NavigateTo;

        #endregion


        #region Fields

        readonly string realnameWatermark = LanguageHelper.Profile.RealNameWatermark;
        readonly string locationWatermark = LanguageHelper.Profile.LocationWatermark;
        readonly string webAddressWatermark = LanguageHelper.Profile.WebAddressWatermark;
        readonly string descriptionWatermark = LanguageHelper.Profile.DescriptionWatermark;

        #endregion


        #region Constructors

        public ProfileControl()
        {
            InitializeComponent();

            // Set the titles
            ResetErrors();
        }

        #endregion


        internal void ApplyDefaultWatermarks()
        {
            if (TextBoxRealName.Text == string.Empty)
            {
                TextBoxRealName.Text = realnameWatermark;
                TextBoxRealName.Foreground = App.AppConstants.WatermarkTextColourBrush;
            }

            if (TextBoxLocation.Text == string.Empty)
            {
                TextBoxLocation.Text = locationWatermark;
                TextBoxLocation.Foreground = App.AppConstants.WatermarkTextColourBrush;
            }

            if (TextBoxWebAddress.Text == string.Empty)
            {
                TextBoxWebAddress.Text = webAddressWatermark;
                TextBoxWebAddress.Foreground = App.AppConstants.WatermarkTextColourBrush;
            }

            if (TextBoxDescription.Text == string.Empty)
            {
                TextBoxDescription.Text = descriptionWatermark;
                TextBoxDescription.Foreground = App.AppConstants.WatermarkTextColourBrush;
            }
        }

        #region Real name Textbox

        /// <summary>
        /// The real name box has got focus
        /// </summary>
        private void TextBoxRealName_GotFocus(object sender, RoutedEventArgs e)
        {
            // if our watermark is set in the control
            if (TextBoxRealName.Text == realnameWatermark)
            {
                TextBoxRealName.Text = string.Empty;
                TextBoxRealName.Foreground = App.AppConstants.NormalTextColourBrush;
            }
        }

        /// <summary>
        /// The real name box has lost focus
        /// </summary>
        private void TextBoxRealName_LostFocus(object sender, RoutedEventArgs e)
        {
            // If nothing has been entered on leaving our textbox, then set our watermark again
            if (TextBoxRealName.Text == string.Empty)
            {
                TextBoxRealName.Text = realnameWatermark;
                TextBoxRealName.Foreground = App.AppConstants.WatermarkTextColourBrush;
            }
        }

        /// <summary>
        /// Also validate on a key up
        /// </summary>
        private void TextBoxRealName_KeyUp(object sender, KeyEventArgs e)
        {
            ResetErrors();

            // remove focus from the textbox when enter has been pressed
            if (e.Key == Key.Enter)
            {
                TextBoxLocation.Focus();
            }
        }

        #endregion


        #region TextBoxLocation Textbox

        /// <summary>
        /// The location has got focus
        /// </summary>
        private void TextBoxLocation_GotFocus(object sender, RoutedEventArgs e)
        {
            // if our watermark is set in the control
            if (TextBoxLocation.Text == locationWatermark)
            {
                TextBoxLocation.Text = string.Empty;
                TextBoxLocation.Foreground = App.AppConstants.NormalTextColourBrush;
            }
        }

        /// <summary>
        /// The location has lost focus
        /// </summary>
        private void TextBoxLocation_LostFocus(object sender, RoutedEventArgs e)
        {
            // If nothing has been entered on leaving our textbox, then set our watermark again
            if (TextBoxLocation.Text == string.Empty)
            {
                TextBoxLocation.Text = locationWatermark;
                TextBoxLocation.Foreground = App.AppConstants.WatermarkTextColourBrush;
            }
        }

        /// <summary>
        /// Also validate on a key up
        /// </summary>
        private void TextBoxLocation_KeyUp(object sender, KeyEventArgs e)
        {
            ResetErrors();

            // remove focus from the textbox when enter has been pressed
            if (e.Key == Key.Enter)
            {
                TextBoxWebAddress.Focus();
            }
        }

        #endregion


        #region Web address Focus etc

        /// <summary>
        /// The Web address has got focus
        /// </summary>
        private void TextBoxWebAddress_GotFocus(object sender, RoutedEventArgs e)
        {
            // if our watermark is set in the control
            if (TextBoxWebAddress.Text == webAddressWatermark)
            {
                TextBoxWebAddress.Text = string.Empty;
                TextBoxWebAddress.Foreground = App.AppConstants.NormalTextColourBrush;
            }
        }

        /// <summary>
        /// The Web address has lost focus
        /// </summary>
        private void TextBoxWebAddress_LostFocus(object sender, RoutedEventArgs e)
        {
            // If nothing has been entered on leaving our textbox, then set our watermark again
            if (TextBoxWebAddress.Text == string.Empty)
            {
                TextBoxWebAddress.Text = webAddressWatermark;
                TextBoxWebAddress.Foreground = App.AppConstants.WatermarkTextColourBrush;
            }
        }

        /// <summary>
        /// Also validate on a key up
        /// </summary>
        private void TextBoxWebAddress_KeyUp(object sender, KeyEventArgs e)
        {
            ResetErrors();

            // remove focus from the textbox when enter has been pressed
            if (e.Key == Key.Enter)
            {
                //TextBoxWebAddress.Focus();
            }
        }

        #endregion


        #region Description Keyboard Events

        private void textBoxDescription_GotFocus(object sender, RoutedEventArgs e)
        {
            // if our watermark is set in the control
            if (TextBoxDescription.Text == descriptionWatermark)
            {
                TextBoxDescription.Text = string.Empty;
                TextBoxDescription.Foreground = App.AppConstants.NormalTextColourBrush;
            }
        }

        private void textBoxDescription_LostFocus(object sender, RoutedEventArgs e)
        {
            // If nothing has been entered on leaving our textbox, then set our watermark again
            if (TextBoxDescription.Text == string.Empty)
            {
                TextBoxDescription.Text = descriptionWatermark;
                TextBoxDescription.Foreground = App.AppConstants.WatermarkTextColourBrush;
            }
        }

        private void textBoxDescription_KeyDown(object sender, KeyEventArgs e)
        {
        }


        private void textBoxDescription_KeyUp(object sender, KeyEventArgs e)
        {
        }

        #endregion


        #region Reset

        public void ResetErrors()
        {
            TitleRealName.ClearError();
            TitleLocation.ClearError();
            TitleWebAddress.ClearError();
            TitleDescription.ClearError();
        }

        #endregion


        #region Update Events

        public void Update()
        {
            bool pageValid = true;

            ResetErrors();

            // Do some validation
            //if (Validation.ValidateExists(TitleRealName, TextBoxRealName, realnameWatermark, LanguageHelper.Profile.RealnameError))
            //{
            //    if (Validation.ValidateExists(TitleLastName, TextBoxLastName, lastnameWatermark, AppResources.SurnameWatermark))
            //    {
            //        if (Validation.ValidateExists(TitleMobile, TextBoxMobile, mobileWatermark, AppResources.MobileWatermark))
            //        {
            //            pageValid = true;
            //        }
            //    }
            //}

            // If page is valid then call the api
            if (pageValid)
            {
                string realname = TextBoxRealName.Text == realnameWatermark ? string.Empty : TextBoxRealName.Text;
                string location = TextBoxLocation.Text == locationWatermark ? string.Empty : TextBoxLocation.Text;
                string website = TextBoxWebAddress.Text == webAddressWatermark ? string.Empty : TextBoxWebAddress.Text;
                string description = TextBoxDescription.Text == descriptionWatermark ? string.Empty : TextBoxDescription.Text;

                // Raise message to containing page telling them we are starting a login so they can display progress bar etc
                if (UpdateProfileStarting != null)
                {
                    UpdateProfileStarting(this, new UpdateProfileEventArgs() { RealName=realname, Location=location, WebAddress=website, Description=description });
                }
            }
        }

        #endregion

    }
}
