
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
using System.IO.IsolatedStorage;

#endregion

namespace AudioBoo.Views
{
    public partial class RecordBooPage : PhoneApplicationPage
    {
        #region Fields

        BooDraftDbRecord boo;

        ApplicationBarIconButton editBoo;
        ApplicationBarIconButton uploadBoo;
        ApplicationBarIconButton deleteBoo;

        #endregion


        #region Constructors

        public RecordBooPage()
        {
            InitializeComponent();

            // Create an app bar
            CreateApplicationBar();
        }

        #endregion


        #region Page Events

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Get the draft boo id if one is passwed in
            IDictionary<string, string> parameters = this.NavigationContext.QueryString;
            if (parameters.ContainsKey("draftBooId"))
            {
                string booId = parameters["draftBooId"];
                int id = Convert.ToInt32(booId);

                boo = App.ViewModel.DbViewModel.GetDraftBoo(id);
            }
            else
            {
                boo = App.ViewModel.DbViewModel.CreateDraftBoo();
            }

            DataContext = boo;

            // Initialise the control - what boo are we recording into?
            RecordBooControl.Initialise(boo);
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            base.OnBackKeyPress(e);

            if (boo.IsPlaying)
            {
                // Stop the sound ( as opposed to pause )
                App.SoundPlayerHelper.StopPlaying();
                //boo.Reset();
            }

            if (boo.IsRecording)
            {
                // Stop the sound ( as opposed to pause )
                App.SoundRecorderHelper.StopRecording();
                //boo.Reset();
            }
        }

        #endregion


        #region App Bar Creation

        /// <summary>
        /// Create the application bar 
        /// </summary>
        void CreateApplicationBar()
        {
            ApplicationBar = new ApplicationBar();

            // clear the app bar
            ApplicationBar.MenuItems.Clear();
            ApplicationBar.Buttons.Clear();

            ApplicationBar.Mode = ApplicationBarMode.Default;
            ApplicationBar.Opacity = 1.0;
            ApplicationBar.IsVisible = true;
            ApplicationBar.IsMenuEnabled = true;

            // The photo details button
            //photoBoo = new ApplicationBarIconButton();
            //photoBoo.IconUri = new Uri("Resources/Images/AppBar/camera.png", UriKind.Relative);
            //photoBoo.Text = "attach photo";
            //photoBoo.Click += photoBoo_Click;

            // The edit boo details button
            editBoo = new ApplicationBarIconButton();
            editBoo.IconUri = new Uri("Resources/Images/AppBar/edit.png", UriKind.Relative);
            editBoo.Text = "edit details";
            editBoo.Click += editBoo_Click;

            // The upload boo details button
            uploadBoo = new ApplicationBarIconButton();
            uploadBoo.IconUri = new Uri("Resources/Images/AppBar/upload.png", UriKind.Relative);
            uploadBoo.Text = "upload boo";
            uploadBoo.Click += uploadBoo_Click;

            // The delete boo button
            deleteBoo = new ApplicationBarIconButton();
            deleteBoo.IconUri = new Uri("Resources/Images/AppBar/delete.png", UriKind.Relative);
            deleteBoo.Text = "delete boo";
            deleteBoo.Click += deleteBoo_Click;

            // Add buttons
            //ApplicationBar.Buttons.Add(photoBoo);
            ApplicationBar.Buttons.Add(editBoo);
            ApplicationBar.Buttons.Add(uploadBoo);
            ApplicationBar.Buttons.Add(deleteBoo);

            CreateStandardMenuItems();
        }

        private void CreateStandardMenuItems()
        {
#if DEBUG
            ApplicationBarMenuItem infoItem = new ApplicationBarMenuItem("Show Boo Info");
            infoItem.Click += infoItem_Click;
            ApplicationBar.MenuItems.Add(infoItem);
#endif
        }

        void infoItem_Click(object sender, EventArgs e)
        {
            Int64 availableSpace;
            Int64 quotaSpace;

            using (var store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                availableSpace = store.AvailableFreeSpace;
                quotaSpace = store.Quota;
            }

            string info = string.Format("Created: {0}\nDuration: {1}\nStream length: {2}\nImage length: {3}\nAvailable space: {4}",
                boo.DateCreated.ToString(),
                boo.TotalDuration.ToString(), 
                boo.Stream != null ? boo.Stream.Length : 0, 
                boo.Image != null ? boo.Image.Length : 0,
                availableSpace
                );
            MessageBox.Show(info, boo.Title, MessageBoxButton.OK);
        }

        #endregion


        #region App Bar Event Handlers

        /// <summary>
        /// Attach photo
        /// </summary>
        void photoBoo_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/EditDraftBoos/PhotoPage.xaml?draftBooId=" + boo.BooId, UriKind.RelativeOrAbsolute));
        }

        /// <summary>
        /// Handler for the app bar edit boo button
        /// </summary>
        void editBoo_Click(object sender, EventArgs e)
        {
            App.AppConstants.MyCategory = null;

            NavigationService.Navigate(new Uri("/Views/EditDraftBoos/EditBooMetaDataPage.xaml?draftBooId=" + boo.BooId, UriKind.RelativeOrAbsolute));
        }

        /// <summary>
        /// Add to upload queue
        /// </summary>
        void uploadBoo_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Add this boo to the upload queue?", "TODO: Upload boo", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                // Add to queue
            }
        }

        /// <summary>
        /// Handler for the app bar delete button
        /// </summary>
        void deleteBoo_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete this boo?", "Are you sure", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                // Delete the boo
                App.ViewModel.DbViewModel.DeleteDraftBoo(boo);

                // go back
                NavigationService.GoBack();
            }
        }

        #endregion
    }
}