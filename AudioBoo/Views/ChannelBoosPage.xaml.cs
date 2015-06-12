using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using AudioBoo.Models;
using AudioBoo.ViewModels;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace AudioBoo.Views
{
    public partial class ChannelBoosPage
    {
        #region Properties

        private readonly ChannelBoosViewModel viewModel;
        private int pageNumber;
        private int channelId;
        private const int OffsetKnob = 5;

        #endregion

        #region Constructor

        public ChannelBoosPage()
        {
            InitializeComponent();
            viewModel = (ChannelBoosViewModel)Resources["ViewModel"];
            Loaded += OnLoaded;
            ChannelBoos.Link += ChannelBoosOnLink;
        }

        #endregion

        #region Event Handlers

        private void ChannelBoosOnLink(object sender, LinkUnlinkEventArgs e)
        {
            if ((ChannelBoos.ItemsSource as ObservableCollection<BooDbRecord>) == null) return;

            int currentItemCount = (ChannelBoos.ItemsSource as ObservableCollection<BooDbRecord>).Count;

            if (!viewModel.IsLoading && currentItemCount >= OffsetKnob && (e.ContentPresenter.Content as BooDbRecord) != null)
            {
                if ((e.ContentPresenter.Content as BooDbRecord).Equals((ChannelBoos.ItemsSource as ObservableCollection<BooDbRecord>)[currentItemCount - OffsetKnob]))
                {
                    viewModel.GetChannelBoos(channelId, ++pageNumber);
                }
            }
        }

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

        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if(NavigationContext.QueryString.ContainsKey("channelId"))
            {
                channelId = Int32.Parse(NavigationContext.QueryString["channelId"]);

                viewModel.Channel = PhoneApplicationService.Current.State["channel"] as ChannelData;

                viewModel.GetChannelBoos(channelId, ++pageNumber);
            }
        }
        private void ChannelBoos_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((LongListSelector)sender).SelectedItem == null)
                return;

            var selectedBoo = ((LongListSelector)sender).SelectedItem as BooDbRecord;
            if (selectedBoo != null)
            {
                var uri = new Uri("/Views/BooDetailPage.xaml?booId=" + selectedBoo.BooId, UriKind.RelativeOrAbsolute);
                NavigationService.Navigate(uri);
            }

            
            ((LongListSelector)sender).SelectedItem = null;
        }

        #endregion
    }
}