using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using AudioBoo.Models;
using AudioBoo.ViewModels;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace AudioBoo.Views
{
    public partial class BrowsePage : PhoneApplicationPage
    {
        #region Properties

        private readonly CategoriesViewModel categoriesViewModel;
        private readonly ChannelCategoryViewModel channelViewModel;

        #endregion

        #region Constructor

        public BrowsePage()
        {
            InitializeComponent();
            categoriesViewModel = (CategoriesViewModel) Resources["CategoriesViewModel"];
            channelViewModel = (ChannelCategoryViewModel) Resources["ChannelCategoryViewModel"];

            Loaded += OnLoaded;
        }

        #endregion

        #region Private Methods

        private void Search()
        {
            NavigationService.Navigate(new Uri("/Views/SearchPage.xaml?searchTerm=" + SearchBox.Text, UriKind.RelativeOrAbsolute));
        }

        #endregion

        #region Event Handlers

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var progressIndicator = SystemTray.ProgressIndicator;
            if (progressIndicator != null)
            {
                return;
            }

            progressIndicator = new ProgressIndicator();

            SystemTray.SetProgressIndicator(this, progressIndicator);

            var binding = new Binding("IsLoading") { Source = categoriesViewModel };
            BindingOperations.SetBinding(progressIndicator, ProgressIndicator.IsVisibleProperty, binding);

            binding = new Binding("IsLoading") { Source = categoriesViewModel };
            BindingOperations.SetBinding(progressIndicator, ProgressIndicator.IsIndeterminateProperty, binding);
            progressIndicator.Text = "Loading...";

            categoriesViewModel.GetCategories();

            binding = new Binding("IsLoading") { Source = channelViewModel };
            BindingOperations.SetBinding(progressIndicator, ProgressIndicator.IsVisibleProperty, binding);

            binding = new Binding("IsLoading") { Source = channelViewModel };
            BindingOperations.SetBinding(progressIndicator, ProgressIndicator.IsIndeterminateProperty, binding);
            progressIndicator.Text = "Loading...";

            channelViewModel.GetChannels();
        }

        private void CategoriesList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ListBox) sender).SelectedIndex == -1)
                return;

            var selectedCategory = ((ListBox)sender).SelectedItem as CategoryData;
            if (selectedCategory != null)
            {
                var uri = new Uri("/Views/CategoryBoosPage.xaml?categoryId=" + selectedCategory.CategoryId, UriKind.RelativeOrAbsolute);
                NavigationService.Navigate(uri);  
            }
            ((ListBox)sender).SelectedIndex = -1;
        }

        private void ChannelList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ListBox)sender).SelectedIndex == -1)
                return;

            var selectedCategory = ((ListBox)sender).SelectedItem as ChannelCategoryData;
            if (selectedCategory != null)
            {
                var uri = new Uri("/Views/ChannelsPage.xaml?channelCategoryId=" + selectedCategory.ChannelId, UriKind.RelativeOrAbsolute);
                NavigationService.Navigate(uri);

            }

            ((ListBox)sender).SelectedIndex = -1;
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
            if (e.Key == Key.Enter)
                Search();
        }

        #endregion
    }
}