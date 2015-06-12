
#region Usings

using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using AudioBoo.Controls;
using AudioBoo.Helpers;
using AudioBoo.Web.Api;
using System;
using System.Net;
using System.Windows;
using System.Windows.Navigation;

#endregion

namespace AudioBoo.Views.Account
{
    public partial class ProfilePage : PhoneApplicationPage
    {
        #region Fields

        private static AudioBooAPI _api;

        // App bar buttons
        ApplicationBarIconButton buttonSaveDetails;
        ApplicationBarIconButton buttonRemovePhoto;

        // The newly selected image bytes
        byte[] imageBytes;
        string profileImageUrl;

        #endregion


        #region Constructors

        public ProfilePage()
        {
            InitializeComponent();

            // Initialise an instance of the user api
            _api = new AudioBooAPI(false);
            _api.UpdateProfileCompleted += _api_UpdateProfileCompleted;

            // Create the buttons for app bars
            CreateAppBarButtons();

            // Create default app bar
            CreateApplicationBarMyProfile();

            // Register for the pivot selection changed event
            PivotControl.SelectionChanged += PivotControl_SelectionChanged;

            // Register for sub control events
            ProfileControl.UpdateProfileStarting += ProfileControl_UpdateProfileStarting;

            // Subscribe to the photo control events
            ProfilePhotoControl.ProgressChanged += PhotoControl_ProgressChanged;
            ProfilePhotoControl.CameraStateChanged += PhotoControl_CameraStateChanged;

            // Set page names
            ProfilePivotHeader.PageName = LanguageHelper.Profile.ProfileTitle;
            PhotoPivotHeader.PageName = LanguageHelper.Profile.PhotoTitle;

            // Initialise the photo control
            profileImageUrl = App.ViewModel.DbViewModel.CurrentUser.ProfileImage;
            imageBytes = null;

            ProfilePhotoControl.PopulateImage(profileImageUrl, imageBytes);
        }


        #endregion


        #region Page Event Handlers

        /// <summary>
        /// This also gets run when we navigate back from the clubs list and from the activate page
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Set the data context for the Profile view.
            ProfileControl.DataContext = App.ViewModel.DbViewModel;
            DataContext = App.ViewModel.DbViewModel;

            // If nothing has been bound to the control, add watermarks
            ProfileControl.ApplyDefaultWatermarks();

            // Set up the default app bar
            CreateApplicationBarMyProfile();
        }

        #endregion


        #region Event Handlers

        void ProfileControl_UpdateProfileStarting(object sender, Controls.ProfileControl.UpdateProfileEventArgs e)
        {




            _api.UpdateProfile(e.RealName, e.Location, e.WebAddress, e.Description, imageBytes);
        }


        void _api_UpdateProfileCompleted(object sender, UpdateProfileEventArgs e)
        {
            if (e.Status == HttpStatusCode.OK)
            {
                MessageBox.Show("Profile updated");
            }
            else
            {
                MessageBox.Show("Profile update failed");
            }
        }



        /// <summary>
        /// When the pivot selection changes we want to alter the app bar
        /// </summary>
        void PivotControl_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // close the keyboard if its open
            this.Focus();

            // enable disable app bar depending on the pivot
            switch (((Pivot)sender).SelectedIndex)
            {
                case 0:
                    CreateApplicationBarMyProfile();
                    break;

                case 1:
                    CreateApplicationBarForPhoto();
                    break;
            }
        }

        #endregion


        #region Photo Events

        private void ButtonTakePhoto_Click(object sender, System.EventArgs e)
        {
            ProfilePhotoControl.Take();
        }

        private void ButtonChoosePhoto_Click(object sender, System.EventArgs e)
        {
            ProfilePhotoControl.Choose();
        }

        private void ButtonRotatePhoto_Click(object sender, System.EventArgs e)
        {
            ProfilePhotoControl.Rotate();
        }

        private void HyperlinkSelectFromFolder_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ProfilePhotoControl.Choose();
        }

        #endregion


        #region Events from Camera Control

        private void PhotoControl_CameraStateChanged(object sender, ProfilePhotoControl.CameraStateEventArgs e)
        {
            if (e.PhotoState == ProfilePhotoControl.PhotoState.Processing)
            {
                TextboxInstructionsTap.Visibility = System.Windows.Visibility.Collapsed;
                ProfilePhotoControl.Visibility = System.Windows.Visibility.Collapsed;

                if (buttonRemovePhoto != null)
                    buttonRemovePhoto.IsEnabled = false;
            }

            if (e.PhotoState == ProfilePhotoControl.PhotoState.PictureLoaded)
            {
                imageBytes = e.Image;
                ProfilePhotoControl.PopulateImage(profileImageUrl, imageBytes);

                TextboxInstructionsTap.Visibility = System.Windows.Visibility.Visible;
                ProfilePhotoControl.Visibility = System.Windows.Visibility.Visible;

                if (buttonRemovePhoto != null)
                    buttonRemovePhoto.IsEnabled = true;
            }
            else if (e.PhotoState == ProfilePhotoControl.PhotoState.NoPictureLoaded)
            {
                imageBytes = null;
                profileImageUrl = string.Empty;

                ProfilePhotoControl.PopulateImage(profileImageUrl, imageBytes);

                TextboxInstructionsTap.Visibility = System.Windows.Visibility.Visible;
                ProfilePhotoControl.Visibility = System.Windows.Visibility.Visible;

                if (buttonRemovePhoto != null)
                    buttonRemovePhoto.IsEnabled = false;
            }
        }

        void PhotoControl_ProgressChanged(object sender, ProfilePhotoControl.ProgressChangedEventArgs e)
        {
            ProgressIndicator progress = new ProgressIndicator
            {
                IsVisible = e.IsProgress,
                IsIndeterminate = true,
                Text = "Processing"
            };

            SystemTray.SetProgressIndicator(this, progress);
        }

        #endregion






        #region App Bar Creation

        private void CreateAppBarButtons()
        {
            // The update button
            buttonSaveDetails = new ApplicationBarIconButton();
            buttonSaveDetails.IconUri = new Uri("Resources/Images/AppBar/check.png", UriKind.Relative);
            buttonSaveDetails.Text = "save changes";
            buttonSaveDetails.Click += AppBarMenuItemUpdate_Click;

            // The delete boo button
            buttonRemovePhoto = new ApplicationBarIconButton();
            buttonRemovePhoto.IconUri = new Uri("Resources/Images/AppBar/delete.png", UriKind.Relative);
            buttonRemovePhoto.Text = "remove";
            buttonRemovePhoto.Click += ButtonClearPhoto_Click;
        }

        /// <summary>
        /// Create our application bar for my account pivot
        /// </summary>
        void CreateApplicationBarMyProfile()
        {
            ApplicationBar = new ApplicationBar();

            // clear the app bar
            ApplicationBar.MenuItems.Clear();
            ApplicationBar.Buttons.Clear();

            ApplicationBar.Mode = ApplicationBarMode.Default;
            ApplicationBar.Opacity = 1.0;
            ApplicationBar.IsVisible = true;
            ApplicationBar.IsMenuEnabled = true;

            ApplicationBar.Buttons.Add(buttonSaveDetails);

            CreateStandardMenuItems();
        }

        /// <summary>
        /// Create app bar for my votes and my purchases
        /// </summary>
        void CreateApplicationBarForPhoto()
        {
            ApplicationBar = new ApplicationBar();

            // clear the app bar
            ApplicationBar.MenuItems.Clear();
            ApplicationBar.Buttons.Clear();

            ApplicationBar.Mode = ApplicationBarMode.Default;
            ApplicationBar.Opacity = 1.0;
            ApplicationBar.IsVisible = true;
            ApplicationBar.IsMenuEnabled = true;

            ApplicationBar.Buttons.Add(buttonRemovePhoto);

            CreateStandardMenuItems();
        }

        private void CreateStandardMenuItems()
        {
            ApplicationBarMenuItem changePassword = new ApplicationBarMenuItem();
            changePassword.Text = LanguageHelper.Profile.AppBarChangePassword;
            ApplicationBar.MenuItems.Add(changePassword);
            changePassword.Click += new EventHandler(AppBarMenuItemChangePassword_Click);

            ApplicationBarMenuItem logout = new ApplicationBarMenuItem();
            logout.Text = "logout";
            ApplicationBar.MenuItems.Add(logout);
            logout.Click += AppBarMenuItemLogout_Click;
        }

        #endregion


        #region App Bar Events

        private void ButtonClearPhoto_Click(object sender, System.EventArgs e)
        {
            ProfilePhotoControl.Clear();
        }


        /// <summary>
        /// Save changes
        /// </summary>
        void AppBarMenuItemUpdate_Click(object sender, EventArgs e)
        {
            // Save is called from the account control 
            ProfileControl.Update();
        }

        /// <summary>
        /// logout
        /// </summary>
        void AppBarMenuItemLogout_Click(object sender, EventArgs e)
        {
            // Log them out
            App.ViewModel.DbViewModel.Logout();

            // show a success message
            //App.PopupHelper.PopupMessages.Enqueue(new PopupMessage(new PopupMessageControl() { Message = AppResources.LoggedOutMessage}, new TimeSpan(0, 0, 3)));

            // Now on logout we send them to the login page with instructions to remove the settings page from the back stack
            //this.NavigationService.Navigate(new Uri("/Views/LoginPage.xaml?removeBackStack=yes", UriKind.Relative));
        }

        /// <summary>
        /// Change password
        /// </summary>
        void AppBarMenuItemChangePassword_Click(object sender, EventArgs e)
        {
            //this.NavigationService.Navigate(new Uri("/Views/PasswordUpdatePage.xaml", UriKind.Relative));
        }

        #endregion
    }
}