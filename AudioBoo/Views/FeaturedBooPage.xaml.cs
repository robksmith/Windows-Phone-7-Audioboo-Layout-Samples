
#region Usings

using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using AudioBoo.Helpers;
using AudioBoo.Models;
using AudioBoo.ViewModels;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

#endregion

namespace AudioBoo.Views
{
    public partial class FeaturedBooPage : PhoneApplicationBasePage
    {
        #region Properties

        private readonly FeaturedBoosViewModel viewModel;
        private int pageNumber;
        private const int OffsetKnob = 5;

        #endregion


        #region Constructors

        public FeaturedBooPage()
        {
            InitializeComponent();
            viewModel = (FeaturedBoosViewModel)Resources["ViewModel"];
            Loaded += OnLoaded;
            FeaturedBoosLongList.Link += FeaturedBoos_FeaturedBoosOnLink;
        }

        #endregion


        #region Page Event Handlers

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

            viewModel.GetFeaturedBoos(++pageNumber);
        }

        #endregion


        #region Long List Selector Event Handlers

        /// <summary>
        /// Infinite loading
        /// </summary>
        private void FeaturedBoos_FeaturedBoosOnLink(object sender, LinkUnlinkEventArgs e)
        {
            ObservableCollection<BooDbRecord> records = (FeaturedBoosLongList.ItemsSource as ObservableCollection<BooDbRecord>);

            if (records == null) return;

            int currentItemCount = records.Count;

            if (!viewModel.IsLoading && currentItemCount >= OffsetKnob && (e.ContentPresenter.Content as BooDbRecord) != null)
            {
                if ((e.ContentPresenter.Content as BooDbRecord).Equals(records[currentItemCount - OffsetKnob]))
                {
                    viewModel.GetFeaturedBoos(++pageNumber);
                }
            }
        }

        /// <summary>
        /// Go to view details page
        /// </summary>
        private void FeaturedBoos_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((LongListSelector)sender).SelectedItem == null)
                return;

            var selectedBoo = ((LongListSelector)sender).SelectedItem as BooDbRecord;
            if (selectedBoo != null)
            {
                Navigate(NavigationHelper.BooDetailPage, selectedBoo.BooId);
            }
            
            ((LongListSelector)sender).SelectedItem = null;
        }

        #endregion
    }
}