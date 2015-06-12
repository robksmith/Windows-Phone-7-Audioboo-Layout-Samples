
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
using AudioBoo.Models;
using AudioBoo.Helpers;
using AudioBoo.Controls;

#endregion


namespace AudioBoo.Views
{
    public partial class EditBooMetaDataPage : PhoneApplicationPage
    {
        #region Fields

        BooDraftDbRecord boo;

        ApplicationBarIconButton buttonSaveDetails;
        ApplicationBarIconButton buttonRemovePhoto;

        #endregion


        #region Constructors

        public EditBooMetaDataPage()
        {
            InitializeComponent();

            // Register for the pivot selection changed event
            MainPivot.SelectionChanged += PivotControl_SelectionChanged;

            // Subscribe to the category selector event
            CategorySelector.CategoryPressed += CategorySelector_CategoryPressed;

            // Create the buttons for app bars
            CreateAppBarButtons();

            // Create an app bar
            BuildBooDetailsAppBar();

            // Subscribe to the photo control events
            PhotoControl.ProgressChanged += PhotoControl_ProgressChanged;
            PhotoControl.CameraStateChanged += PhotoControl_CameraStateChanged;
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

            // Populate the category with the value passed back - if the temp category is not null that means we have been to the select category page
            if (App.AppConstants.MyCategory != null)
            {
                CategorySelector.Refresh(App.AppConstants.MyCategory);
            }
            else
            {
                var cats = App.ViewModel.CategoriesViewModel.CategoryList;
                CategoryData category = (from cat in cats
                                         where cat.CategoryId == boo.CategoryId
                                         select cat).FirstOrDefault();

                CategorySelector.Refresh(category);
            }

            // Set the datacontext
            DataContext = boo;

            // Initialise the control - what boo are we setting the photo for?
            PhotoControl.Initialise(boo.Image);
        }

        void CategorySelector_CategoryPressed(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/EditDraftBoos/CategorySelectorPage.xaml?draftBooId=" + boo.BooId, UriKind.RelativeOrAbsolute));
        }

        /// <summary>
        /// When the pivot selection changes we want to alter the app bar
        /// </summary>
        void PivotControl_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // close the keyboard if its open
            this.Focus();

            // enable disable app bar depending on the pivot
            switch (((Pivot)sender).SelectedIndex)
            {
                case 0:
                    BuildBooDetailsAppBar();
                    break;

                case 1:
                    BuildBooPhotoAppBar();
                    break;
            }
        }

        #endregion


        #region App Bar Creation

        private void CreateAppBarButtons()
        {
            // The delete boo button
            buttonSaveDetails = new ApplicationBarIconButton();
            buttonSaveDetails.IconUri = new Uri("Resources/Images/AppBar/check.png", UriKind.Relative);
            buttonSaveDetails.Text = "save changes";
            buttonSaveDetails.Click += ButtonSaveDetails_Click;

            // The delete boo button
            buttonRemovePhoto = new ApplicationBarIconButton();
            buttonRemovePhoto.IconUri = new Uri("Resources/Images/AppBar/delete.png", UriKind.Relative);
            buttonRemovePhoto.Text = "remove";
            buttonRemovePhoto.Click += ButtonClearPhoto_Click;
        }

        /// <summary>
        /// Create the application bar 
        /// </summary>
        void BuildBooDetailsAppBar()
        {
            ApplicationBar = new ApplicationBar();

            // clear the app bar
            ApplicationBar.MenuItems.Clear();
            ApplicationBar.Buttons.Clear();

            ApplicationBar.Mode = ApplicationBarMode.Default;
            ApplicationBar.Opacity = 1.0;
            ApplicationBar.IsVisible = true;
            ApplicationBar.IsMenuEnabled = true;

            ApplicationBar.Buttons.Add(buttonSaveDetails);

            CreateStandardMenuItems();
        }

        /// <summary>
        /// Create the application bar 
        /// </summary>
        void BuildBooPhotoAppBar()
        {
            ApplicationBar = new ApplicationBar();

            // clear the app bar
            ApplicationBar.MenuItems.Clear();
            ApplicationBar.Buttons.Clear();

            ApplicationBar.Mode = ApplicationBarMode.Default;
            ApplicationBar.Opacity = 1.0;
            ApplicationBar.IsVisible = true;
            ApplicationBar.IsMenuEnabled = true;

            ApplicationBar.Buttons.Add(buttonRemovePhoto);

            CreateStandardMenuItems();
        }


        private void CreateStandardMenuItems()
        {
            //ApplicationBarMenuItem settingsItem = new ApplicationBarMenuItem(AppStrings.AppBarSettings);
            //settingsItem.Click += SettingsItemOnClick;
            //ApplicationBar.MenuItems.Add(settingsItem);
        }

        #endregion


        #region Photo Events

        private void ButtonTakePhoto_Click(object sender, System.EventArgs e)
        {
            PhotoControl.Take();
        }

        private void ButtonChoosePhoto_Click(object sender, System.EventArgs e)
        {
            PhotoControl.Choose();
        }

        private void ButtonRotatePhoto_Click(object sender, System.EventArgs e)
        {
            PhotoControl.Rotate();
        }

        private void HyperlinkSelectFromFolder_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            PhotoControl.Choose();
        }

        #endregion


        #region App Bar Events

        private void ButtonClearPhoto_Click(object sender, System.EventArgs e)
        {
            PhotoControl.Clear();
        }

        /// <summary>
        /// Only if they click the save button do we explicitly save the values
        /// </summary>
        void ButtonSaveDetails_Click(object sender, EventArgs e)
        {
            // Make the changes
            if (App.AppConstants.MyCategory != null)
            {
                boo.CategoryId = App.AppConstants.MyCategory.CategoryId;

                // Clear the temp category
                App.AppConstants.MyCategory = null;
            }
            boo.Title = TextBoxTitle.Text;
            boo.Description = TextBoxDescription.Text;

            // Submit the changes
            DatabaseHelper.AudioBooDataContext.SubmitChanges();

            // Go back
            //NavigationService.GoBack();
        }

        #endregion


        #region Events from Camera Control

        private void PhotoControl_CameraStateChanged(object sender, PhotoControl.CameraStateEventArgs e)
        {
            if (e.PhotoState == PhotoControl.PhotoState.Processing)
            {
                TextboxInstructionsTap.Visibility = System.Windows.Visibility.Collapsed;
                PhotoControl.Visibility = System.Windows.Visibility.Collapsed;

                //takeButton.IsEnabled = false;
                //chooseButton.IsEnabled = false;
                //rotateButton.IsEnabled = false;
                if ( buttonRemovePhoto != null )
                    buttonRemovePhoto.IsEnabled = false;
            }

            if (e.PhotoState == PhotoControl.PhotoState.PictureLoaded)
            {
                boo.Image = e.Image;

                // Save the image
                boo.SaveImage();

                TextboxInstructionsTap.Visibility = System.Windows.Visibility.Visible;
                PhotoControl.Visibility = System.Windows.Visibility.Visible;

                //takeButton.IsEnabled = true;
                //chooseButton.IsEnabled = true;
                //rotateButton.IsEnabled = true;
                if (buttonRemovePhoto != null)
                    buttonRemovePhoto.IsEnabled = true;
            }
            else if (e.PhotoState == PhotoControl.PhotoState.NoPictureLoaded)
            {
                TextboxInstructionsTap.Visibility = System.Windows.Visibility.Visible;
                PhotoControl.Visibility = System.Windows.Visibility.Visible;

                //takeButton.IsEnabled = true;
                //chooseButton.IsEnabled = true;
                //rotateButton.IsEnabled = false;
                if (buttonRemovePhoto != null)
                    buttonRemovePhoto.IsEnabled = false;
            }
        }

        void PhotoControl_ProgressChanged(object sender, PhotoControl.ProgressChangedEventArgs e)
        {
            ProgressIndicator progress = new ProgressIndicator
            {
                IsVisible = e.IsProgress,
                IsIndeterminate = true,
                Text = "Processing"
            };

            SystemTray.SetProgressIndicator(this, progress);
        }

        #endregion

    }
}