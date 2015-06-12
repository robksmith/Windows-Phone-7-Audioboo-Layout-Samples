
#region usings

using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using AudioBoo.ViewModels;
using AudioBoo.Helpers;
using AudioBoo.Controls;
using AudioBoo.Helpers;

#endregion

namespace AudioBoo.Controls
{
    public partial class DraftBooMediaPlayerControl : UserControl
    {
        public class ScanningStartedEventArgs : EventArgs
        {
        }

        public class ScanningCompletedEventArgs : EventArgs
        {
            public TimeSpan Offset { get; set; }
        }

        #region Fields

        private readonly MediaPlayerViewModel _viewModel;

        #endregion


        #region Events

        public event EventHandler<EventArgs> PlayPressed;
        public event EventHandler<EventArgs> PausePressed;

        public event EventHandler<ScanningStartedEventArgs> ScanningStarted;
        public event EventHandler<ScanningCompletedEventArgs> ScanningCompleted;


        #endregion


        #region Constructors

        public DraftBooMediaPlayerControl()
        {
            InitializeComponent();

            _viewModel = (MediaPlayerViewModel)Resources["ViewModel"];

            PlayPositionIndicator.ManipulationStarted += PlayPositionIndicator_ManipulationStarted;
            PlayPositionIndicator.ManipulationDelta += PlayPositionIndicator_ManipulationDelta;
            PlayPositionIndicator.ManipulationCompleted += PlayPositionIndicator_ManipulationCompleted;
        }

        #endregion


        /// <summary>
        /// Reset the slider
        /// </summary>
        internal void Reset()
        {
            PlayPositionIndicator.Value = 0;
        }


        internal void Reset(TimeSpan timeSpan)
        {
            PlayPositionIndicator.Value = timeSpan.TotalSeconds;
        }

        /// <summary>
        /// Given a current timespan and a sound length, set the progress bar
        /// </summary>
        internal void SetProgressPosition(TimeSpan current, TimeSpan max)
        {
            //if (current.TotalMilliseconds < 100 || max.TotalMilliseconds < 100)
            //{
            //    PlayPositionIndicator.Value = 0;
            //    PlayPositionIndicator.Maximum = 100;
            //}
            //else
            //{
                PlayPositionIndicator.Value = current.TotalSeconds;
                PlayPositionIndicator.Maximum = max.TotalSeconds;
            //}

            // Set the play position marker
            string currentPosition = string.Format("{0:D2}:{1:D2}", current.Minutes, current.Seconds);
            string length = string.Format("{0:D2}:{1:D2}", max.Minutes, max.Seconds);

            PlayTimeMarker.Text = currentPosition + " / " + length;
        }


        #region Play Button Events

        /// <summary>
        /// Play has been clicked
        /// </summary>
        private void PlayButtonClick(object sender, RoutedEventArgs e)
        {
            if (PlayPressed != null)
            {
                PlayPressed(this, new EventArgs());
            }
        }

        /// <summary>
        /// Pause has been clicked
        /// </summary>
        private void PauseButtonClick(object sender, RoutedEventArgs e)
        {
            if (PausePressed != null)
            {
                PausePressed(this, new EventArgs());
            }
        }

        #endregion

        //private void PlayPositionIndicator_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        //{
        //}

        private void PlayPositionIndicator_ManipulationStarted(object sender, System.Windows.Input.ManipulationStartedEventArgs e)
        {
            if (ScanningStarted != null)
            {
                ScanningStarted(this, new ScanningStartedEventArgs());
            }
            //((UIElement)sender).RenderTransform = new System.Windows.Media.TranslateTransform() { X = 0, Y = -8 };
        }

        private void PlayPositionIndicator_ManipulationDelta(object sender, System.Windows.Input.ManipulationDeltaEventArgs e)
        {
            //((UIElement)sender).RenderTransform = new System.Windows.Media.TranslateTransform() { X = 8, Y = 0 };
        }

        private void PlayPositionIndicator_ManipulationCompleted(object sender, System.Windows.Input.ManipulationCompletedEventArgs e)
        {
            if (ScanningCompleted != null)
            {
                ScanningCompleted(this, new ScanningCompletedEventArgs() { Offset = TimeSpan.FromSeconds(PlayPositionIndicator.Value) });
            }

            //((UIElement)sender).RenderTransform = null;
        }

    }
}
