
#region Usings

using Microsoft.Phone.BackgroundAudio;
using AudioBoo.AudioPlaybackAgentDatabase;
using AudioBoo.Helpers;
using AudioBoo.Models;
using AudioBoo.ViewModels;
using AudioBoo.Web.Responses;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

#endregion


namespace AudioBoo.Controls
{
    public partial class MediaPlayerControl : UserControl
    {
        #region Fields

        private readonly MediaPlayerViewModel _viewModel;

        //public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(Uri), typeof(MediaPlayerControl), new PropertyMetadata(null));

        //public static readonly DependencyProperty ImageUriProperty = DependencyProperty.Register("ImageUri", typeof(Uri), typeof(MediaPlayerControl), new PropertyMetadata(null));

        private bool scrubbing;

        #endregion


        #region Properties

        //public Uri Source
        //{
        //    get { return (Uri)this.GetValue(SourceProperty); }
        //    set { this.SetValue(SourceProperty, value); }
        //}

        //public Uri ImageUri
        //{
        //    get { return (Uri)this.GetValue(ImageUriProperty); }
        //    set { this.SetValue(ImageUriProperty, value); }
        //}

        public BooClip Boo { get; set; }

        #endregion


        #region Constructors

        public MediaPlayerControl()
        {
            InitializeComponent();

            // Get the viewmodel
            _viewModel = (MediaPlayerViewModel)Resources["ViewModel"];

            // Subscribe to the slider controls
            PositionIndicator.ManipulationStarted += PlayPositionIndicator_ManipulationStarted;
            PositionIndicator.ManipulationDelta += PlayPositionIndicator_ManipulationDelta;
            PositionIndicator.ManipulationCompleted += PlayPositionIndicator_ManipulationCompleted;

            // Subscribe to the state changed of the background agent
            BackgroundAudioPlayer.Instance.PlayStateChanged += Instance_PlayStateChanged;

            // Set up atimer to update the ui
            DispatcherTimer playTimer;
            playTimer = new DispatcherTimer();
            playTimer.Interval = TimeSpan.FromMilliseconds(1000); 
            playTimer.Tick += new EventHandler(playTimer_Tick);
            playTimer.Start();
        }

        #endregion



        internal void Setup(BooClip boo)
        {
            Boo = boo;

            if (boo != null)
            {
                PlayTimeMarker.Text = TimeSpan.Zero.ToString(@"hh\:mm\:ss");
                EndTimeMarker.Text = boo.Duration.ToString(@"hh\:mm\:ss");
            }

            GridLoading.Visibility = System.Windows.Visibility.Collapsed;
        }


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

            // If scrubbing, dont update any values
            if (scrubbing)
            {
                return;
            }

            //PlayButton.Visibility = System.Windows.Visibility.Visible;
            //PauseButton.Visibility = System.Windows.Visibility.Visible;

            // Get the bup instance
            BackgroundAudioPlayer baPlayer = BackgroundAudioPlayer.Instance;

            // If we are not on the page of the track that is currently being played/paused then return without updating any values
            bool currentlyProcessingThis = CurrentlyProcessingThis(baPlayer);
            if (!currentlyProcessingThis)
            {
                PlayTimeMarker.Text = TimeSpan.Zero.ToString(@"hh\:mm\:ss");
                return;
            }

            // If we are on the page of the track being played/paused then update the relevant values
            AudioTrack currentTrack = baPlayer.Track;
            PlayState bapState = baPlayer.PlayerState;
            if ((bapState != PlayState.Unknown) && (currentTrack != null))
            {
                TimeSpan duration = currentTrack.Duration;
                TimeSpan position = baPlayer.Position;

                PlayTimeMarker.Text = position.ToString(@"hh\:mm\:ss");
                EndTimeMarker.Text = duration.ToString(@"hh\:mm\:ss");

                if ((bapState == PlayState.Playing) && (currentTrack.Tag == "Buffering"))
                {
                    //PlayButton.Visibility = System.Windows.Visibility.Collapsed;

                    // Turn on progress bar
                    ProgressBar.IsIndeterminate = true;
                }
                else if (bapState == PlayState.Playing)
                {
                    //PlayButton.Visibility = System.Windows.Visibility.Collapsed;

                    // Turn off progress bar
                    ProgressBar.IsIndeterminate = false;
                }
                else if (bapState == PlayState.Paused)
                {
                    //PauseButton.Visibility = System.Windows.Visibility.Collapsed;

                    // Turn off progress bar
                    ProgressBar.IsIndeterminate = false;
                }

                // Playing
                PositionIndicator.Maximum = BackgroundAudioPlayer.Instance.Track.Duration.TotalSeconds;
                if (!scrubbing)
                {
                    PositionIndicator.Value = BackgroundAudioPlayer.Instance.Position.TotalSeconds;
                }
            }
            else
            {
                //PauseButton.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        #endregion


        #region Play / Pause Button Events

        /// <summary>
        /// The play/pause button has been clicked
        /// </summary>
        private void PlayButtonClick(object sender, RoutedEventArgs e)
        {
            // Get the bap player
            BackgroundAudioPlayer baPlayer = BackgroundAudioPlayer.Instance;

            // get the bap state
            PlayState bapState = baPlayer.PlayerState;

            // Are we on the page of the track currently being played/paused
            bool currentlyProcessingThis = CurrentlyProcessingThis(baPlayer);

            if (currentlyProcessingThis)
            {
                if (bapState == PlayState.Playing)
                {
                    baPlayer.Pause();
                }
                else if (bapState == PlayState.Paused)
                {
                    baPlayer.Play();

                    if (baPlayer.Track != null)
                    {
                        PositionIndicator.Maximum = baPlayer.Track.Duration.TotalSeconds;
                    }
                }
                else
                {
                    // Set up the new audio track by adding it to the database
                    AudioRecord newAudioRecord = App.ViewModel.DbAudioViewModel.CreateAudioRecord
                                                (
                                                    DatabaseHelper.PlaybackAgentDataContext,
                                                    Boo.BooId,
                                                    new Uri( Boo.BooUrls.AudioUri),
                                                    new Uri(Boo.BooUrls.ImageUri),
                                                    Boo.Title,
                                                    Boo.UserDetails.Username,
                                                    Boo.UserDetails.Username
                                                );

                    baPlayer.Play();

                    if (baPlayer.Track != null)
                    {
                        PositionIndicator.Maximum = baPlayer.Track.Duration.TotalSeconds;
                    }
                }
            }
            else
            {
                // This means we are pressing the play/pause button on a new track
                baPlayer.Stop();

                // Delete all records
                App.ViewModel.DbAudioViewModel.DeleteAudioQueue(DatabaseHelper.PlaybackAgentDataContext);

                // Set up the new audio track by adding it to the database
                AudioRecord newAudioRecord = App.ViewModel.DbAudioViewModel.CreateAudioRecord
                                                (
                                                    DatabaseHelper.PlaybackAgentDataContext,
                                                    Boo.BooId,
                                                    new Uri(Boo.BooUrls.AudioUri),
                                                    new Uri(Boo.BooUrls.ImageUri),
                                                    Boo.Title,
                                                    Boo.UserDetails.Username,
                                                    Boo.UserDetails.Username
                                                );

                baPlayer.Play();

                if (baPlayer.Track != null)
                {
                    PositionIndicator.Maximum = baPlayer.Track.Duration.TotalSeconds;
                }
            }
        }

        #endregion


        #region Slider Events

        private void PlayPositionIndicator_ManipulationStarted(object sender, System.Windows.Input.ManipulationStartedEventArgs e)
        {
            scrubbing = true;

            //if (ScanningStarted != null)
            //{
            //    ScanningStarted(this, new ScanningStartedEventArgs());
            //}
        }

        private void PlayPositionIndicator_ManipulationDelta(object sender, System.Windows.Input.ManipulationDeltaEventArgs e)
        {
        }

        private void PlayPositionIndicator_ManipulationCompleted(object sender, System.Windows.Input.ManipulationCompletedEventArgs e)
        {
            scrubbing = false;

            //if (ScanningCompleted != null)
            //{
            //    ScanningCompleted(this, new ScanningCompletedEventArgs() { Offset = TimeSpan.FromSeconds(PositionIndicator.Value) });
            //}

            if (BackgroundAudioPlayer.Instance.PlayerState != PlayState.Unknown)
            {
                // Work out the timespan depending where the user has moved the slider
                TimeSpan ts = new TimeSpan(0, 0, (int)PositionIndicator.Value);

                // Set the new bap track position
                BackgroundAudioPlayer.Instance.Position = ts;
            }
        }

        #endregion


        #region Helpers

        /// <summary>
        /// Are we on the page of the track currently being played/paused
        /// </summary>
        /// <param name="baPlayer"></param>
        /// <returns></returns>
        private bool CurrentlyProcessingThis(BackgroundAudioPlayer baPlayer)
        {
            // If the boo hasn't been set up that normally means it hasn't loaded properley
            if (Boo == null)
            {
                return false;
            }

            bool currentlyProcessingThis = false;

            // Does a track exist
            if (baPlayer.Track != null)
            {
                // Is it what this media player is set to?
                if (new Uri(Boo.BooUrls.AudioUri) == baPlayer.Track.Source)
                {
                    currentlyProcessingThis = true;
                }
            }
            return currentlyProcessingThis;
        }

        #endregion


        //#region Button Click Event Handlers

        ///// <summary>
        ///// Tells the background audio agent to skip to the previous track.
        ///// </summary>
        ///// <param name="sender">The button</param>
        ///// <param name="e">Click event args</param>
        //private void prevButton_Click(object sender, RoutedEventArgs e)
        //{
        //    BackgroundAudioPlayer.Instance.SkipPrevious();

        //    // Prevent the user from repeatedly pressing the button and causing 
        //    // a backlong of button presses to be handled. This button is re-eneabled 
        //    // in the TrackReady Playstate handler.
        //    //prevButton.IsEnabled = false;
        //}


        ///// <summary>
        ///// Tells the background audio agent to play the current 
        ///// track or to pause if we're already playing.
        ///// </summary>
        ///// <param name="sender">The button</param>
        ///// <param name="e">Click event args</param>
        //private void playButton_Click(object sender, RoutedEventArgs e)
        //{
        //    if (PlayState.Playing == BackgroundAudioPlayer.Instance.PlayerState)
        //    {
        //        BackgroundAudioPlayer.Instance.Pause();
        //    }
        //    else
        //    {
        //        BackgroundAudioPlayer.Instance.Play();
        //    }

        //}

        ///// <summary>
        ///// Tells the background audio agent to skip to the next track.
        ///// </summary>
        ///// <param name="sender">The button</param>
        ///// <param name="e">Click event args</param>
        //private void nextButton_Click(object sender, RoutedEventArgs e)
        //{
        //    BackgroundAudioPlayer.Instance.SkipNext();

        //    // Prevent the user from repeatedly pressing the button and causing 
        //    // a backlong of button presses to be handled. This button is re-eneabled 
        //    // in the TrackReady Playstate handler.
        //    //nextButton.IsEnabled = false;
        //}

        //#endregion Button Click Event Handlers

    }
}
