
#region Usings

using Microsoft.Phone.Controls;
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
    public partial class MessageDetailPage : PhoneApplicationBasePage
    {
        private readonly MessageDetailViewModel _viewModel;

        private MessagesViewModel.MessageDataViewModel messageData;

        public MessageDetailPage()
        {
            InitializeComponent();
            _viewModel = (MessageDetailViewModel) Resources["ViewModel"];
            Loaded += OnLoaded;
        }

        #region Page Events

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

            _viewModel.ImageUrl = messageData.Image;
            _viewModel.AudioUrl = messageData.Audio;
            _viewModel.Username = messageData.User;
            _viewModel.Title = messageData.Title;
            _viewModel.LocationDescription = messageData.Location;
            _viewModel.Description = messageData.Description;

            // Set up the media player
            //MediaPlayer.Setup(e.BooDetail);

            //int messageid = Int32.Parse(NavigationContext.QueryString["messageId"]);
            //_viewModel.GetBoo(id);

            //int userid = Int32.Parse(NavigationContext.QueryString["userId"]);
            //_viewModel.GetUser(userid);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
        }

        #endregion


        protected override void OnParameterReceived()
        {
            base.OnParameterReceived();

            messageData = Parameter as MessagesViewModel.MessageDataViewModel;
        }
    }
}