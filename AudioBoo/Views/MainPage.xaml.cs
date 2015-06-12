#region Usings

using Microsoft.Phone.Shell;
using AudioBoo.Helpers;
using AudioBoo.Models;
using AudioBoo.Resources.Strings;
using AudioBoo.ViewModels;
using FAS.Controls;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;

#endregion

namespace AudioBoo.Views
{
    public partial class MainPage : PhoneApplicationBasePage
    {
        #region Fields

        readonly MainPageViewModel viewModel;

        ApplicationBarIconButton recordBoo;

        // The popups
        Popup popupControl;
        AskRegisterOrLoginControl registerOrLoginControl;

        #endregion


        #region Constructors

        public MainPage()
        {
            InitializeComponent();

            viewModel = (MainPageViewModel)Resources["MainPageViewModel"];
            Loaded += OnLoaded;

            // Create the popup
            popupControl = new Popup();
            popupControl.VerticalOffset = 28;

            // Create the popup control
            registerOrLoginControl = new AskRegisterOrLoginControl();

            // Subscribe to register/login control events
            registerOrLoginControl.RegisterPressed += RegisterOrLoginControl_RegisterPressed;
            registerOrLoginControl.LoginPressed += RegisterOrLoginControl_LoginPressed;
            registerOrLoginControl.LaterPressed += RegisterOrLoginControl_LaterPressed;

            NowPlayingControl.ImageTapped += NowPlayingControl_ImageTapped;
            
            // Create an app bar
            ApplicationBar = new ApplicationBar();
            CreateApplicationBar();

            BrowseList.ManipulationStarted += AnimationHelper.Standard_ManipulationStarted_1;
            BrowseList.ManipulationCompleted += AnimationHelper.Standard_ManipulationCompleted_1;
        }

        /// <summary>
        /// The image has been tapped - send them to the boo detail page
        /// </summary>
        void NowPlayingControl_ImageTapped(object sender, Controls.NowPlayingImageTappedEventArgs e)
        {
            Navigate(NavigationHelper.BooDetailPage, e.BooId);
        }

        #endregion


        #region Page Events

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            HandleNowPlayingVisibility();
        }

        #endregion


        #region App Bar Creation

        /// <summary>
        /// Create the application bar 
        /// </summary>
        void CreateApplicationBar()
        {
            // clear the app bar
            ApplicationBar.MenuItems.Clear();
            ApplicationBar.Buttons.Clear();

            ApplicationBar.Mode = ApplicationBarMode.Default;
            ApplicationBar.Opacity = 1.0;
            ApplicationBar.IsVisible = true;
            ApplicationBar.IsMenuEnabled = true;

            // The record boo button
            recordBoo = new ApplicationBarIconButton();
            recordBoo.IconUri = new Uri("Resources/Images/AppBar/record.png", UriKind.Relative);
            recordBoo.Text = "record boo";
            recordBoo.Click += recordBoo_Click;

            ApplicationBar.Buttons.Add(recordBoo);

            CreateStandardMenuItems();
        }

        private void CreateStandardMenuItems()
        {
            ApplicationBarMenuItem settingsItem = new ApplicationBarMenuItem(AppStrings.AppBarSettings);
            settingsItem.Click += SettingsItemOnClick;
            ApplicationBar.MenuItems.Add(settingsItem);

            ApplicationBarMenuItem mockupUpItem = new ApplicationBarMenuItem("record mockup");
            mockupUpItem.Click += MockupItemOnClick;
            ApplicationBar.MenuItems.Add(mockupUpItem);
        }

        #endregion


        #region Private Methods

        private void Search()
        {
            NavigationService.Navigate(new Uri("/Views/SearchPage.xaml?searchTerm=" + SearchBox.Text, UriKind.RelativeOrAbsolute));
        }

        private void HandleNowPlayingVisibility()
        {
            bool hasCurrentlyPlaying = AudioHelper.HasCurrentlyPlaying();

            if (hasCurrentlyPlaying)
            {
                //NowPlayingItem.IsEnabled = false;
                NowPlayingItem.Visibility = System.Windows.Visibility.Visible;

                //Controls.NowPlayingPanoramaControl npc = new Controls.NowPlayingPanoramaControl();

                //AppPanorama.Items.Add(npc);
            }
            else
            {
                NowPlayingItem.Visibility = System.Windows.Visibility.Collapsed;

                //PanoramaItem toRemove = null;

                //foreach (PanoramaItem item in AppPanorama.Items)
                //{
                //    if (item.Name == "NowPlayingItem")
                //    {
                //        toRemove = item;
                //    }
                //}

                //if (toRemove != null)
                //{
                //    AppPanorama.Items.Remove(toRemove);
                //}
            }

            //NowPlayingControl.Update();
        }

        #endregion


        #region Event Handlers

        /// <summary>
        /// Handler for the app bar record button
        /// </summary>
        void recordBoo_Click(object sender, EventArgs e)
        {
            //NavigationService.Navigate(new Uri("/Views/EditDraftBoos/RecordBooPage.xaml", UriKind.RelativeOrAbsolute));
            Navigate(NavigationHelper.RecordBooPage);
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            var progressIndicator = SystemTray.ProgressIndicator;
            if (progressIndicator != null)
            {
                return;
            }

            progressIndicator = new ProgressIndicator();

            SystemTray.SetProgressIndicator(this, progressIndicator);

            var binding = new Binding("IsLoading") {Source = viewModel};
            BindingOperations.SetBinding(progressIndicator, ProgressIndicator.IsVisibleProperty, binding);

            binding = new Binding("IsLoading") {Source = viewModel};
            BindingOperations.SetBinding(progressIndicator, ProgressIndicator.IsIndeterminateProperty, binding);
            progressIndicator.Text = "Loading...";

            viewModel.GetFeaturedBoos(1, 6);
        }

        private void FeaturedList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ListBox) sender).SelectedIndex == -1)
                return;

            var selectedBoo = ((ListBox)sender).SelectedItem as BooDbRecord;
            if (selectedBoo != null)
            {
                Navigate(NavigationHelper.BooDetailPage, selectedBoo.BooId);
            }

            ((ListBox) sender).SelectedIndex = -1;
        }

        private void BrowseList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (((ListBox)sender).SelectedIndex == -1)
                return;

            var item = ((ListBox)sender).SelectedItem as BrowseItem;
            if (item != null)
            {
                if (item.BrowseTitle == "Browse")
                {
                    Navigate(NavigationHelper.BrowsePage);
                }
                else if (item.BrowseTitle == "My Boos")
                {
                    Navigate(NavigationHelper.MyBoosPage);
                }
                else if (item.BrowseTitle == "Downloads")
                {
                    Navigate(NavigationHelper.DownloadsPage);
                }
            }

            ((ListBox)sender).SelectedIndex = -1;
        }

        private void MoreFeatured_Click(object sender, RoutedEventArgs e)
        {
            Navigate(NavigationHelper.FeaturedBooPage);
        }

        private void SettingsItemOnClick(object sender, EventArgs eventArgs)
        {
            Navigate(NavigationHelper.SettingsPage);
        }

        private void MockupItemOnClick(object sender, EventArgs eventArgs)
        {
            Navigate(NavigationHelper.MockupRecordBooPage);
        }

        private void Search_OnGotFocus(object sender, RoutedEventArgs e)
        {
            SearchBox.SelectAll();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            Search();
        }

        private void Search_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
                Search();
        }

        #endregion

        /// <summary>
        /// Go to user profile - if they aren't logged on, show the register/login popup
        /// </summary>
        private void ButtonProfile_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if ( App.ViewModel.DbViewModel.IsLoggedOn() )
            {
                //this.NavigationService.Navigate(new Uri("/Views/Account/ProfilePage.xaml", UriKind.Relative));
                Navigate(NavigationHelper.ProfilePage);
            }
            else
            {
                // open the popup
                popupControl.Child = registerOrLoginControl;

                popupControl.IsOpen = true;

                // hide the app bar
                ApplicationBar.IsVisible = false;
            }
        }


        private void ButtonMessages_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //this.NavigationService.Navigate(new Uri("/Views/Messages/MessagesPage.xaml", UriKind.Relative));
            Navigate(NavigationHelper.MessagesPage);
        }


        #region Events for the register/login popup control

        void RegisterOrLoginControl_RegisterPressed(object sender, EventArgs e)
        {
            //this.NavigationService.Navigate(new Uri("/Views/Account/RegisterPage.xaml", UriKind.Relative));
            Navigate(NavigationHelper.RegisterPage);
            popupControl.IsOpen = false;
        }

        void RegisterOrLoginControl_LoginPressed(object sender, EventArgs e)
        {
            //this.NavigationService.Navigate(new Uri("/Views/Account/LoginPage.xaml", UriKind.Relative));
            Navigate(NavigationHelper.LoginPage);
            popupControl.IsOpen = false;
        }

        void RegisterOrLoginControl_LaterPressed(object sender, EventArgs e)
        {
            popupControl.IsOpen = false;
        }

        #endregion


        #region Back Key

        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            base.OnBackKeyPress(e);

            if (popupControl.IsOpen)
            {
                // Close the popup
                popupControl.IsOpen = false;

                e.Cancel = true;
            }
        }

        #endregion
    }
}