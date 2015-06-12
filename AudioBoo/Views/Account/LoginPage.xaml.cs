
#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using AudioBoo.ViewModels;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using AudioBoo.Web.Api;
using AudioBoo.Web.Responses;

#endregion

namespace AudioBoo.Views
{
    public partial class LoginPage : PhoneApplicationPage
    {
        #region Fields

        private static AudioBooAPI _api;

        #endregion


        #region Properties

        #endregion


        #region Constructor

        public LoginPage()
        {
            InitializeComponent();

            // Subscribe to the login events
            LoginControl.LoginStarting += LoginControl_LoginStarting;
            LoginControl.RemoveKeyboard += LoginControl_RemoveKeyboard;

            // Create the app bar
            BuildApplicationBar();
            
            // Initialise an instance of the user api
            _api = new AudioBooAPI();
            _api.LinkCompleted += _api_LinkCompleted;
            _api.GetStatusCompleted += APIOnGetStatusCompleted;
        }

        #endregion


        #region App Bar

        private void BuildApplicationBar()
        {
            ApplicationBar = new ApplicationBar();

            var button = new ApplicationBarIconButton(new Uri("/Resources/Images/AppBar/check.png", UriKind.RelativeOrAbsolute)) { Text = "Login" };
            button.Click += AppBarMenuLogin_Click;
            ApplicationBar.Buttons.Add(button);
        }

        #endregion


        #region App Bar Events

        private void AppBarMenuLogin_Click(object sender, EventArgs e)
        {
            LoginControl.Login();
        }

        #endregion


        #region Login Control Events

        /// <summary>
        /// They have pressed the login tick button
        /// </summary>
        void LoginControl_LoginStarting(object sender, Controls.LoginStartingEventArgs e)
        {
            ApplicationBarIconButton b = (ApplicationBarIconButton)ApplicationBar.Buttons[0];
            b.IsEnabled = false;

            // Get rid of the keyboard
            this.Focus();

            // Start the login
            _api.Link(e.Username, e.Password);
        }

        #endregion


        #region Api Events

        void _api_LinkCompleted(object sender, LinkEventArgs e)
        {
            if (e.StatusCode == APIConnectionResult.Good)
            {
                if (e.ServerResponse.LinkedUserDetail.Linked)
                {
                    _api.GetStatus();
                }
            }
            else
            {
                //    // Log them off just to be sure
                //    //App.ViewModel.DbViewModel.Logout();

                // Show a message
                MessageBox.Show(e.Message, "Login Failed", MessageBoxButton.OK);

                // re-enable the app bar
                ApplicationBarIconButton b = (ApplicationBarIconButton)ApplicationBar.Buttons[0];
                b.IsEnabled = true;
            }
        }

        private void APIOnGetStatusCompleted(object sender, StatusEventArgs e)
        {
            if (e.StatusCode == APIConnectionResult.Good)
            {
                // Log the user on
                App.ViewModel.DbViewModel.Login(e.ServerResponse);

                //LoginCompleted(this, new LoginCompletedEventArgs { Sucess = true, Message = "Login Successful" });
                NavigationService.GoBack();
            }
            else
            {
                // Show a message
                MessageBox.Show(e.Message, "Login Failed", MessageBoxButton.OK);

                // re-enable the app bar
                ApplicationBarIconButton b = (ApplicationBarIconButton)ApplicationBar.Buttons[0];
                b.IsEnabled = true;
            }
        }
        
        #endregion


        void LoginControl_RemoveKeyboard(object sender, EventArgs e)
        {
            this.Focus();
        }
    }
}