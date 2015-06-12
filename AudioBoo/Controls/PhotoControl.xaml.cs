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
    public partial class PhotoControl : UserControl
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

        //BooDraftDbRecord boo;
        byte[] image;

        #endregion


        #region Properties

        #endregion


        #region Constructors

        public PhotoControl()
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
        /// Set up the control with a boo
        /// </summary>
        //internal void Initialise(BooDraftDbRecord boo)
        internal void Initialise(byte []image)
        {
            //this.boo = boo;
            this.image = image;

            //if (boo.Image != null)
            if (image != null)
            {
                // Fill in the image if we have one
                imagePhoto.Source = image.ToBitmapImage();
                RaiseCameraStateChanged(PhotoState.PictureLoaded, image);
            }
            else
            {
                // If no image attached, set a default
                imagePhoto.Source = new BitmapImage(new Uri("../../Resources/Images/Photo/NoPhoto.png", UriKind.Relative));
                RaiseCameraStateChanged(PhotoState.NoPictureLoaded, null);
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

                // Put the image into the boo
                //boo.Image = resizedImage.ToByteArray();
                image = resizedImage.ToByteArray();

                // Save the image
                //*boo.SaveImage();

                // Populate image control 
                imagePhoto.Source = resizedImage.ToBitmap();

                // Turn a few buttons on the app bar off
                RaiseCameraStateChanged(PhotoState.PictureLoaded, image);
            }
            else
            {
                // Set the app bar state depending on whether there is an image loaded or not
                //if (boo.Image != null)
                if (image != null)
                {
                    RaiseCameraStateChanged(PhotoState.PictureLoaded, image);
                }
                else
                {
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
            RaiseCameraStateChanged(PhotoState.Processing, image);

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
            RaiseCameraStateChanged(PhotoState.Processing, image);

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
                RaiseCameraStateChanged(PhotoState.PictureLoaded, image);
            }
        }

        public void Rotate()
        {
            //Set progress bar to visible 
            RaiseProgressIndicatorChanged(true);

            // Turn everything on the app bar off
            RaiseCameraStateChanged(PhotoState.Processing, image);

            // Start the thread to rotate photo
            threadRotate = new Thread(DoRotation);
            threadRotate.Start();
        }

        public void Clear()
        {
            //boo.Image = null;
            image = null;

            //Initialise(boo);
            Initialise(image);
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
                i.Source = this.image.ToBitmapImage();
                ExtendedImage exImage = i.ToImage();
                ExtendedImage rotatedImage = ExtendedImage.Transform(exImage, RotationType.Rotate90, FlippingType.None);

                // Put the image into the boo
                image = rotatedImage.ToByteArray();

                // Assign it to our image control
                imagePhoto.Source = rotatedImage.ToBitmap();

                //Set progress bar to hidden 
                RaiseProgressIndicatorChanged(false);

                // Enable the buttons
                RaiseCameraStateChanged(PhotoState.PictureLoaded, image);
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
