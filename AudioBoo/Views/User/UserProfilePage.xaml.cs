
#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using AudioBoo.Helpers;
using AudioBoo.ViewModels;
using System.Windows.Data;

#endregion

namespace AudioBoo.Views
{
    public partial class UserProfilePage : PhoneApplicationBasePage
    {
        #region Fields

        // Passed from the previous page
        BooDetailViewModel booDetailViewModel;

        UserProfileViewModel viewModel;

        //private int pageNumber;
        //private const int OffsetKnob = 5;

        #endregion


        #region Constructors

        public UserProfilePage()
        {
            InitializeComponent();

            // Create our view model
            viewModel = new UserProfileViewModel();
            viewModel.UserProfileLoaded += viewModel_UserProfileLoaded;

            Loaded += OnLoaded;

            // Set our view model as the data context
            DataContext = viewModel;
        }

        #endregion


        #region Page Events

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var progressIndicator = SystemTray.ProgressIndicator;
            if (progressIndicator != null)
            {
                return;
            }

            progressIndicator = new ProgressIndicator();

            SystemTray.SetProgressIndicator(this, progressIndicator);

            var binding = new Binding("IsLoading") { Source = viewModel };
            BindingOperations.SetBinding(progressIndicator, ProgressIndicator.IsVisibleProperty, binding);

            binding = new Binding("IsLoading") { Source = viewModel };
            BindingOperations.SetBinding(progressIndicator, ProgressIndicator.IsIndeterminateProperty, binding);
            progressIndicator.Text = "Loading...";


            //// Get the users profile
            //viewModel.GetProfile(booDetailViewModel.BooData.UserDetails.UserId);

            //// Get the first page of users boos
            //AllBoosListControl.Initialise(viewModel, booDetailViewModel.BooData.UserDetails.UserId, 1);

            //// Get the first page of users favourite boos
            //FavBoosListControl.Initialise(viewModel, booDetailViewModel.BooData.UserDetails.UserId, 1);
        }

        /// <summary>
        /// Receive the passed in parameter
        /// </summary>
        protected override void OnParameterReceived()
        {
            base.OnParameterReceived();

            booDetailViewModel = Parameter as BooDetailViewModel;
        }

        #endregion


        #region Button Events

        private void ButtonFollow_Click(object sender, RoutedEventArgs e)
        {
            viewModel.Follow(booDetailViewModel.BooData.UserDetails.UserId);

        }

        private void UserCountBoos_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Navigate(NavigationHelper.UserListOfBoosPage, null);
        }

        #endregion



        void viewModel_UserProfileLoaded(object sender, UserProfileLoadedEventArgs e)
        {
            //if (e.Userdata != null)
            //{
            //    if (e.Userdata.UserCounts.AudioClips <= 0)
            //    {
            //        PivotControl.Items.Remove(UsersBoosPivot);
            //    }

            //    if (e.Userdata.UserCounts.Favourites <= 0)
            //    {
            //        PivotControl.Items.Remove(FavouriteBoosPivot);
            //    }
            //}
        }

        private void PivotControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (( PivotControl.SelectedItem as PivotItem).Name )
            {
                case "ProfilePivot":

                    // Get the users profile
                    viewModel.GetProfile(booDetailViewModel.BooData.UserDetails.UserId);

                    break;

                case "UsersBoosPivot":

                    // Get the first page of users boos
                    AllBoosListControl.Initialise(viewModel, booDetailViewModel.BooData.UserDetails.UserId, 1);

                    break;

                case "FavouriteBoosPivot":

                    // Get the first page of users favourite boos
                    FavBoosListControl.Initialise(viewModel, booDetailViewModel.BooData.UserDetails.UserId, 1);

                    break;
            }


        }
    }
}