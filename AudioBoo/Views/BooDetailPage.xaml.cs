
#region Usings

using Microsoft.Phone.Shell;
using AudioBoo.Helpers;
using AudioBoo.ViewModels;
using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Navigation;

#endregion

namespace AudioBoo.Views
{
    public partial class BooDetailPage : PhoneApplicationBasePage
    {
        #region Events

        private readonly BooDetailViewModel _viewModel;

        private int booDetail;

        #endregion


        #region Constructors

        public BooDetailPage()
        {
            InitializeComponent();

            Loaded += OnLoaded;

            // So we can send them to the user profile page
            ImageProfile.Tap += ImageProfile_Tap;

            _viewModel = (BooDetailViewModel) Resources["ViewModel"];
            
            // Subscribe to the view model loaded event so we can setup the media player when it has loaded
            _viewModel.BooDetailLoaded += _viewModel_BooDetailLoaded;
        }

        #endregion


        #region Page Events

        /// <summary>
        /// The page has loaded
        /// </summary>
        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            var progressIndicator = SystemTray.ProgressIndicator;
            if (progressIndicator != null)
            {
                return;
            }

            progressIndicator = new ProgressIndicator();

            SystemTray.SetProgressIndicator(this, progressIndicator);

            Binding binding = new Binding("IsLoading") {Source = _viewModel};
            BindingOperations.SetBinding(progressIndicator, ProgressIndicator.IsVisibleProperty, binding);

            binding = new Binding("IsLoading") {Source = _viewModel};
            BindingOperations.SetBinding(progressIndicator, ProgressIndicator.IsIndeterminateProperty, binding);
            progressIndicator.Text = "Loading...";
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Get the requested boo
            _viewModel.GetBoo(booDetail);
        }

        protected override void OnParameterReceived()
        {
            base.OnParameterReceived();

            booDetail = (int)Parameter;
        }

        #endregion


        #region Events

        /// <summary>
        /// The viewmodel has loaded successfully
        /// </summary>
        void _viewModel_BooDetailLoaded(object sender, BooDetailLoadedEventArgs e)
        {
            // Set up the media player
            MediaPlayer.Setup(e.BooDetail);
        }

        /// <summary>
        /// On image tap, send the user to the user profile page
        /// </summary>
        void ImageProfile_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Navigate(NavigationHelper.UserProfilePage, _viewModel);
        }

        private void Download_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Share_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Favourite_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void AddToBoard_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}