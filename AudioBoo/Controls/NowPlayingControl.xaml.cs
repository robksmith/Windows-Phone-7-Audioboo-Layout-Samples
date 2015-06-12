
#region Usings

using Microsoft.Phone.BackgroundAudio;
using AudioBoo.AudioPlaybackAgentDatabase;
using AudioBoo.Helpers;
using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;


#endregion

namespace AudioBoo.Controls
{
    public class NowPlayingImageTappedEventArgs : EventArgs
    {
        public int BooId { get; set; }
    }

    public partial class NowPlayingControl : UserControl
    {
        #region Events

        public event EventHandler<NowPlayingImageTappedEventArgs> ImageTapped;

        #endregion


        #region Fields

        #endregion


        #region Constructors

        public NowPlayingControl()
        {
            InitializeComponent();

            ImageBooNowPlaying.ManipulationStarted += AnimationHelper.Standard_ManipulationStarted_1;
            ImageBooNowPlaying.ManipulationCompleted += AnimationHelper.Standard_ManipulationCompleted_1;


            // Subscribe to the state changed of the background agent
            BackgroundAudioPlayer.Instance.PlayStateChanged += Instance_PlayStateChanged;

            // Set up a timer to update the ui
            DispatcherTimer playTimer;
            playTimer = new DispatcherTimer();
            playTimer.Interval = TimeSpan.FromMilliseconds(1000);
            playTimer.Tick += new EventHandler(playTimer_Tick);
            playTimer.Start();
        }

        #endregion


        #region Page Events

        /// <summary>
        /// When the state of the background audio agent has changed
        /// </summary>
        public void Instance_PlayStateChanged(object sender, EventArgs e)
        {
            BackgroundAudioPlayer baPlayer = BackgroundAudioPlayer.Instance;
            AudioTrack currentTrack = baPlayer.Track;
            PlayState bapState = baPlayer.PlayerState;

            switch (bapState)
            {
                case PlayState.Playing:

                    TimeSpan duration = currentTrack.Duration;
                    TimeSpan position = baPlayer.Position;

                    TitleMarker.Text = currentTrack.Title;
                    ArtistMarker.Text = currentTrack.Artist;

                    PlayTimeMarker.Text = position.ToString(@"hh\:mm\:ss");
                    EndTimeMarker.Text = duration.ToString(@"hh\:mm\:ss");

                    break;
            }
        }

        /// <summary>
        /// Called every second to update the ui
        /// </summary>
        public void playTimer_Tick(object sender, EventArgs e)
        {
            // check for errors 
            //string errorString = BackgroundErrorNotifier.GetError();
            //if (errorString != null)
            //{
            //    MessageBox.Show(errorString, "Audio error", MessageBoxButton.OK);
            //    progressBar.IsIndeterminate = false;
            //}

            // Get the bap instance
            BackgroundAudioPlayer baPlayer = BackgroundAudioPlayer.Instance;
            AudioTrack currentTrack = baPlayer.Track;
            PlayState bapState = baPlayer.PlayerState;

            if ((bapState != PlayState.Unknown) && (currentTrack != null))
            {
                ImageBooNowPlaying.Source = new BitmapImage(currentTrack.AlbumArt);

                TimeSpan duration = currentTrack.Duration;
                TimeSpan position = baPlayer.Position;

                TitleMarker.Text = currentTrack.Title;
                ArtistMarker.Text = currentTrack.Artist;

                PlayTimeMarker.Text = position.ToString(@"hh\:mm\:ss");
                EndTimeMarker.Text = duration.ToString(@"hh\:mm\:ss");
            }
        }

        #endregion


        #region Tap Events

        private void ImageBooNowPlaying_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            // Get the bap instance
            BackgroundAudioPlayer baPlayer = BackgroundAudioPlayer.Instance;
            AudioTrack currentTrack = baPlayer.Track;
            PlayState bapState = baPlayer.PlayerState;

            if ((bapState != PlayState.Unknown) && (currentTrack != null))
            {
                AudioRecord booPlaying = App.ViewModel.DbAudioViewModel.FindSource(currentTrack.Source);
                if (booPlaying != null)
                {
                    // If the tapped event has been subscribed to then raise it
                    if (ImageTapped != null)
                    {
                        ImageTapped(this, new NowPlayingImageTappedEventArgs() { BooId = booPlaying.BooId });
                    }
                }
            }




        }

        #endregion

    }
}
