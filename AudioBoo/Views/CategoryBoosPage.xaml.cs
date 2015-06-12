using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Navigation;
using AudioBoo.Models;
using AudioBoo.ViewModels;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace AudioBoo.Views
{
    public partial class CategoryBoosPage : PhoneApplicationPage
    {
        private readonly CategoryBoosViewModel viewModel;
        private int pageNumber;
        private int categoryId;
        private const int OffsetKnob = 5;

        public CategoryBoosPage()
        {
            InitializeComponent();
            viewModel = (CategoryBoosViewModel)Resources["ViewModel"];
            Loaded += OnLoaded;
            CategoryBoos.Link += CategoryBoosLink;
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

            var binding = new Binding("IsLoading") { Source = viewModel };
            BindingOperations.SetBinding(progressIndicator, ProgressIndicator.IsVisibleProperty, binding);

            binding = new Binding("IsLoading") { Source = viewModel };
            BindingOperations.SetBinding(progressIndicator, ProgressIndicator.IsIndeterminateProperty, binding);
            progressIndicator.Text = "Loading...";
        }

        private void CategoryBoosLink(object sender, LinkUnlinkEventArgs e)
        {
            if ((CategoryBoos.ItemsSource as ObservableCollection<BooDbRecord>) == null) return;

            int currentItemCount = (CategoryBoos.ItemsSource as ObservableCollection<BooDbRecord>).Count;

            if (!viewModel.IsLoading && currentItemCount >= OffsetKnob && (e.ContentPresenter.Content as BooDbRecord) != null)
            {
                if ((e.ContentPresenter.Content as BooDbRecord).Equals((CategoryBoos.ItemsSource as ObservableCollection<BooDbRecord>)[currentItemCount - OffsetKnob]))
                {
                    viewModel.GetCategoryBoos(categoryId, ++pageNumber);
                }
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            try
            {
                categoryId = Int32.Parse(NavigationContext.QueryString["categoryId"]);
                viewModel.GetCategoryBoos(categoryId, ++pageNumber);

            }
            catch (Exception)
            {

            }
        }

        private void CategoryBoos_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
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
    }
}