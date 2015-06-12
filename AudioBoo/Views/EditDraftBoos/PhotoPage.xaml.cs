
#region Usings

using AudioBoo.Controls;
using AudioBoo.Models;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.Windows.Navigation;

#endregion

namespace AudioBoo.Views.EditDraftBoos
{
    public partial class PhotoPage : PhoneApplicationPage
    {
       #region Fields

        // What boo are we dealing with
        BooDraftDbRecord boo;

        //ApplicationBarIconButton takeButton;
        //ApplicationBarIconButton chooseButton;
        //ApplicationBarIconButton rotateButton;
        ApplicationBarIconButton deleteButton;
        
        #endregion


        #region Properties

        #endregion


        #region Constructors

        public PhotoPage()
        {
            InitializeComponent();

            // Assign app bar buttons for easy access
            //takeButton = (ApplicationBarIconButton)ApplicationBar.Buttons[0];
            //chooseButton = (ApplicationBarIconButton)ApplicationBar.Buttons[1];
            //rotateButton = (ApplicationBarIconButton)ApplicationBar.Buttons[2];
            //deleteButton = (ApplicationBarIconButton)ApplicationBar.Buttons[3];

            deleteButton = (ApplicationBarIconButton)ApplicationBar.Buttons[0];

            // Subscribe to the photo control events
            PhotoControl.ProgressChanged += PhotoControl_ProgressChanged;
            PhotoControl.CameraStateChanged += PhotoControl_CameraStateChanged;
        }

        #endregion


        #region Page Events

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Get the draft boo id if one is passed in
            IDictionary<string, string> parameters = this.NavigationContext.QueryString;
            if (parameters.ContainsKey("draftBooId"))
            {
                string booId = parameters["draftBooId"];
                int id = Convert.ToInt32(booId);

                boo = App.ViewModel.DbViewModel.GetDraftBoo(id);
            }

            // Set the datacontext
            DataContext = boo;

            // Initialise the control - what boo are we setting the photo for?
            PhotoControl.Initialise(boo);
        }

        #endregion


        #region App Bar Events

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

        private void ButtonClearPhoto_Click(object sender, System.EventArgs e)
        {
            PhotoControl.Clear();
        }

        private void HyperlinkSlectFromFolder_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            PhotoControl.Choose();
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
                deleteButton.IsEnabled = false;
            }

            if (e.PhotoState == PhotoControl.PhotoState.PictureLoaded)
            {
                TextboxInstructionsTap.Visibility = System.Windows.Visibility.Visible;
                PhotoControl.Visibility = System.Windows.Visibility.Visible;

                //takeButton.IsEnabled = true;
                //chooseButton.IsEnabled = true;
                //rotateButton.IsEnabled = true;
                deleteButton.IsEnabled = true;
            }
            else if (e.PhotoState == PhotoControl.PhotoState.NoPictureLoaded)
            {
                TextboxInstructionsTap.Visibility = System.Windows.Visibility.Visible;
                PhotoControl.Visibility = System.Windows.Visibility.Visible;

                //takeButton.IsEnabled = true;
                //chooseButton.IsEnabled = true;
                //rotateButton.IsEnabled = false;
                deleteButton.IsEnabled = false;
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