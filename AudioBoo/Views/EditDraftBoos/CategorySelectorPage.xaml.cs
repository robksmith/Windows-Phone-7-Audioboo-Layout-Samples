
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
using AudioBoo.ViewModels;
using AudioBoo.Models;
using AudioBoo.Helpers;

#endregion

namespace AudioBoo.Views
{
    public partial class CategorySelectorPage : PhoneApplicationPage
    {
        #region Fields

        //readonly CategoriesViewModel viewModel;
        
        BooDraftDbRecord boo;

        #endregion


        #region Constructors

        public CategorySelectorPage()
        {
            InitializeComponent();

            //viewModel = (CategoriesViewModel)Resources["ViewModel"];
            //viewModel.GetCategories();

            categoriesLongList.DataContext = App.ViewModel.CategoriesViewModel;
        }

        #endregion


        #region Event Handlers

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Get the draft boo id if one is passwed in
            IDictionary<string, string> parameters = this.NavigationContext.QueryString;
            if (parameters.ContainsKey("draftBooId"))
            {
                string booId = parameters["draftBooId"];
                int id = Convert.ToInt32(booId);

                boo = App.ViewModel.DbViewModel.GetDraftBoo(id);
            }
        }

        /// <summary>
        /// The user has selected a category
        /// </summary>
        private void categoriesLongList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Record the country selected
            //boo.CategoryId = (categoriesLongList.SelectedItem as CategoryData).CategoryId;
            App.AppConstants.MyCategory = (categoriesLongList.SelectedItem as CategoryData);

            // Submit the changes
            //DatabaseHelper.AudioBooDataContext.SubmitChanges();

            // Go back to the register page
            NavigationService.GoBack();
        }

        #endregion
    }
}