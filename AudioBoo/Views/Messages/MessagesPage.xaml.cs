
#region Usings

using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using AudioBoo.Controls;
using AudioBoo.Helpers;
using AudioBoo.ViewModels;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Navigation;

#endregion


namespace AudioBoo.Views
{
    public partial class MessagesPage : PhoneApplicationBasePage
    {
        #region Fields

        private readonly MessagesViewModel messagesViewModel;

        #endregion


        #region Properties

        #endregion


        #region Constructors

        public MessagesPage()
        {
            InitializeComponent();

            messagesViewModel = (MessagesViewModel)Resources["MessagesViewModel"];

            PageHeaderControl.PageTitle = "Audioboo";
            PageHeaderControl.PageName = "Messages";

            Loaded += MessagesPage_Loaded;

            MessagesOnPage.MessageSelected += MessagesOnPage_MessageSelected;
        }

        #endregion


        #region Event Handlers

        void MessagesPage_Loaded(object sender, RoutedEventArgs e)
        {
            var progressIndicator = SystemTray.ProgressIndicator;
            if (progressIndicator != null)
            {
                return;
            }

            progressIndicator = new ProgressIndicator();

            SystemTray.SetProgressIndicator(this, progressIndicator);

            var binding = new Binding("IsLoading") { Source = messagesViewModel };
            BindingOperations.SetBinding(progressIndicator, ProgressIndicator.IsVisibleProperty, binding);

            binding = new Binding("IsLoading") { Source = messagesViewModel };
            BindingOperations.SetBinding(progressIndicator, ProgressIndicator.IsIndeterminateProperty, binding);
            progressIndicator.Text = "Loading...";

            // Start loading the messages
            MessagesOnPage.GetMessages(messagesViewModel);
        }

        void MessagesOnPage_MessageSelected(object sender, LongListMessagesControl.MessageSelectedEventArgs e)
        {
            //string url = string.Format("{0}?messageId={1}&userId={2}", PageLocationHelper.MessagesPage, 12, e.Message.UserId);

            Navigate(NavigationHelper.MessageDetailPage, e.Message);

            //PhoneApplicationService.Current.State["image"] = e.Message.Image;
            //PhoneApplicationService.Current.State["audio"] = e.Message.Audio;

            //var uri = new Uri(url, UriKind.RelativeOrAbsolute);

            //NavigationService.Navigate(uri);
        }

        #endregion
    }
}