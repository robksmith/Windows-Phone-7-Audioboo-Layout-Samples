#region Usings

using AudioBoo.Helpers;
using AudioBoo.Models;
using ImageTools;
using ImageTools.Filtering;
using Microsoft.Phone.Tasks;
using System;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

#endregion

namespace AudioBoo.Controls
{
    public partial class ProfilePhotoControl : UserControl
    {
        public enum PhotoState
        {
            NoPictureLoaded,
            PictureLoaded,
            Processing
        }

        #region Inner Classes

        public class CameraStateEventArgs : EventArgs
        {
            public PhotoState PhotoState { get; set; }

            public byte[] Image { get; set; }
        }

        public class ProgressChangedEventArgs : EventArgs
        {
            public bool IsProgress { get; set; }
        }

        #endregion


        #region Events

        public event EventHandler<CameraStateEventArgs> CameraStateChanged;
        public event EventHandler<ProgressChangedEventArgs> ProgressChanged;

        #endregion


        #region Fields

        CameraCaptureTask cameraCapture;
        PhotoChooserTask photoChooser;
        
        Thread threadRotate;

        byte[] imageBytes;

        #endregion


        #region Properties

        #endregion


        #region Constructors

        public ProfilePhotoControl()
        {
            InitializeComponent();

            // Set up the camera capture tasks
            cameraCapture = new CameraCaptureTask();
            cameraCapture.Completed += new EventHandler<PhotoResult>(cameraCapture_Completed);

            // Set up the photo chooser task
            photoChooser = new PhotoChooserTask();
            photoChooser.Completed += new EventHandler<PhotoResult>(cameraCapture_Completed);
        }

        #endregion


        #region Initialiser

        /// <summary>
        /// Set up the control with an image
        /// </summary>
        internal void PopulateImage(string profileUrl, byte[] image)
        {
            imageBytes = image;

            // If a new image is chosen
            if (imageBytes != null) 
            {
                // Fill in the image if we have one
                imagePhoto.Source = imageBytes.ToBitmapImage();
            }
            else if (!string.IsNullOrEmpty(profileUrl))
            {
                // otherwise use the profile image
                imagePhoto.Source = new BitmapImage(new Uri(profileUrl));
            }
            else 
            {
                // otherwise use a no photo blank image
                imagePhoto.Source = new BitmapImage(new Uri("../../Resources/Images/Photo/NoPhoto.png", UriKind.Relative));
            }
        }

        #endregion


        #region Camera Capture Events

        /// <summary>
        /// A picture has been taken or selected from the library
        /// </summary>
        void cameraCapture_Completed(object sender, PhotoResult e)
        {
            // Set the boo image
            if (e.ChosenPhoto != null && e.TaskResult == TaskResult.OK)
            {
                Image i = e.ChosenPhoto.ToImage();
                ExtendedImage exImage = i.ToImage();
                ExtendedImage resizedImage = ExtendedImage.Resize(exImage, 300, new NearestNeighborResizer());

                // Put the image into the bytes array
                imageBytes = resizedImage.ToByteArray();

                // Tell the containing page, it needs to repopulate the control
                RaiseCameraStateChanged(PhotoState.PictureLoaded, imageBytes);
            }
            else
            {
                // Set the app bar state depending on whether there is an image loaded or not
                if (imageBytes != null)
                {
                    // Tell the containing page, it needs to repopulate the control
                    RaiseCameraStateChanged(PhotoState.PictureLoaded, imageBytes);
                }
                else
                {
                    // Tell the containing page, it needs to repopulate the control
                    RaiseCameraStateChanged(PhotoState.NoPictureLoaded, null);
                }
            }

            // Collapse visibility on the progress bar once writeable bitmap is visible.
            RaiseProgressIndicatorChanged(false);
        }

        #endregion


        #region Public Camera Actions

        public void Take()
        {
            //Set progress bar to visible to show time between user snapshot and decoding of image into a writeable bitmap object.
            RaiseProgressIndicatorChanged(true);

            // Disable the buttons
            RaiseCameraStateChanged(PhotoState.Processing, imageBytes);

            // Show the camera capture screen
            try
            {
                cameraCapture.Show();
            }
            catch (InvalidOperationException)
            {
                //Collapse visibility on the progress bar once writeable bitmap is visible.
                RaiseProgressIndicatorChanged(false);

                // Turn a few buttons on the app bar off
                RaiseCameraStateChanged(PhotoState.PictureLoaded, null);
            }
        }

        public void Choose()
        {
            //Set progress bar to visible to show time between user snapshot and decoding of image into a writeable bitmap object.
            RaiseProgressIndicatorChanged(true);

            // Disable the buttons
            RaiseCameraStateChanged(PhotoState.Processing, null);

            // show the chooser
            try
            {
                photoChooser.Show();
            }
            catch (InvalidOperationException)
            {
                //Collapse visibility on the progress bar once writeable bitmap is visible.
                RaiseProgressIndicatorChanged(false);

                // Turn a few buttons on the app bar off
                RaiseCameraStateChanged(PhotoState.PictureLoaded, imageBytes);
            }
        }

        public void Rotate()
        {
            //Set progress bar to visible 
            RaiseProgressIndicatorChanged(true);

            // Turn everything on the app bar off
            RaiseCameraStateChanged(PhotoState.Processing, imageBytes);

            // Start the thread to rotate photo
            threadRotate = new Thread(DoRotation);
            threadRotate.Start();
        }

        public void Clear()
        {
            RaiseCameraStateChanged(PhotoState.NoPictureLoaded, null);
        }

        #endregion


        #region Button Events

        private void imagePhoto_Tap(object sender, GestureEventArgs e)
        {
            Take();
        }

        #endregion


        #region Helpers

        /// <summary>
        /// Rotate the photo
        /// </summary>
        private void DoRotation()
        {
            this.Dispatcher.BeginInvoke(delegate()
            {
                //System.Threading.Thread.Sleep(3000);

                // Rotate the photo
                Image i = new Image();
                //image.Source = boo.Image.ToBitmapImage();
                i.Source = this.imageBytes.ToBitmapImage();
                ExtendedImage exImage = i.ToImage();
                ExtendedImage rotatedImage = ExtendedImage.Transform(exImage, RotationType.Rotate90, FlippingType.None);

                // Put the image into the boo
                imageBytes = rotatedImage.ToByteArray();

                // Assign it to our image control
                //imagePhoto.Source = rotatedImage.ToBitmap();

                //Set progress bar to hidden 
                RaiseProgressIndicatorChanged(false);

                // Enable the buttons
                RaiseCameraStateChanged(PhotoState.PictureLoaded, imageBytes);
            });
        }

        #endregion


        #region Raise Events

        private void RaiseCameraStateChanged(PhotoState photoLoaded, byte[] image)
        {
            if (CameraStateChanged != null)
            {
                CameraStateChanged(this, new CameraStateEventArgs() { PhotoState = photoLoaded, Image=image });
            }
        }

        void RaiseProgressIndicatorChanged(bool enabled)
        {
            if (ProgressChanged != null)
            {
                ProgressChanged(this, new ProgressChangedEventArgs() { IsProgress = enabled });
            }
        }

        #endregion


    }
}
