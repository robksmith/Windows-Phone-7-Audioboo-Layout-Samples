
#region Usings

using AudioBoo.Helpers;
using AudioBoo.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using AudioBoo.Controls;
using AudioBoo.Helpers;
using System;
using System.ComponentModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Threading;
using System.Windows;
using System.Windows.Resources;
using System.Windows.Threading;

#endregion

namespace AudioBoo.Controls
{
    public partial class RecordBooLargeControl : INotifyPropertyChanged
    {
        #region Properties

        // What boo record are we dealing with?
        public BooDraftDbRecord Boo { get; set; }

        private int bufferReadyCount;
        private bool wasPlaying;
        private TimeSpan scannedTo;

        #endregion


        #region Constructor

        public RecordBooLargeControl()
        {
            InitializeComponent();

            App.SoundRecorderHelper.Initialise();

            // Subscribe to events on the recorder
            App.SoundRecorderHelper.RecordingStart += SoundRecorder_RecordingStart;
            App.SoundRecorderHelper.RecordingStopped += SoundRecorder_RecordStopped;
            App.SoundRecorderHelper.RecordingTick += SoundRecorder_RecordingTick;
            App.SoundRecorderHelper.RecordingMicrophoneBufferReady += SoundRecorder_RecordingMicrophoneBufferReady;
            App.SoundRecorderHelper.RecordingBufferProcessed += SoundRecorder_BufferProcessed;

            // subscribe to media recorder events
            MediaRecorder.RecordPressed += MediaRecorder_RecordPressed;
            MediaRecorder.PausePressed += MediaRecorder_PausePressed;



            App.SoundPlayerHelper.Initialise();

            // Subscribe to events on the player
            App.SoundPlayerHelper.PlayingStart += SoundPlayer_PlayingStart;
            App.SoundPlayerHelper.PlayingPaused += SoundPlayer_PlayingPaused;
            App.SoundPlayerHelper.PlayingFinished += SoundPlayer_PlayingFinished;
            App.SoundPlayerHelper.PlayingTick += SoundPlayer_PlayingTick;

            // Subscribe to media player events
            MediaPlayer.PlayPressed += MediaPlayer_PlayPressed;
            MediaPlayer.PausePressed += MediaPlayer_PausePressed;

            MediaPlayer.ScanningStarted += MediaPlayer_ScanningStarted;
            MediaPlayer.ScanningCompleted += MediaPlayer_ScanningCompleted;

           // PlayPositionIndicator.Style = (Style)(Application.Current.Resources["ProgressBarStyle1"]);
        }
        
        #endregion


        #region Helpers

        internal void Initialise(BooDraftDbRecord boo)
        {
            Boo = boo;
            MediaPlayer.SetProgressPosition(TimeSpan.Zero, Boo.TotalDuration);
        }


        internal void OnBack()
        {
            if (Boo.IsPlaying)
            {
                App.SoundPlayerHelper.PausePlaying();
            }
        }

        #endregion


        #region Media Recorder Button Events

        void MediaRecorder_RecordPressed(object sender, EventArgs e)
        {
            App.SoundRecorderHelper.StartRecording(Boo.Stream);
        }

        void MediaRecorder_PausePressed(object sender, EventArgs e)
        {
            App.SoundRecorderHelper.StopRecording();
        }

        #endregion


        #region Sound Recorder Events

        /// <summary>
        /// Recording has started
        /// </summary>
        void SoundRecorder_RecordingStart(object sender, EventArgs e)
        {
            bufferReadyCount = 0;

            //App.PopupHelper.PopupMessages.Enqueue(new PopupMessage(new PopupMessageControl() { Message = "3" }, TimeSpan.FromSeconds(1)));
            //App.PopupHelper.PopupMessages.Enqueue(new PopupMessage(new PopupMessageControl() { Message = "2" }, TimeSpan.FromSeconds(1)));
            //App.PopupHelper.PopupMessages.Enqueue(new PopupMessage(new PopupMessageControl() { Message = "1" }, TimeSpan.FromSeconds(1)));

            //if (e.Success)
            //{
                Boo.IsRecording = true;
            //}
        }

        /// <summary>
        /// Event when the sound recorder ticks
        /// </summary>
        void SoundRecorder_RecordingTick(object sender, RecordingTickEventArgs e)
        {
            // Length of just this section
            Boo.SectionDuration = e.Duration;

            // What is the TOTAL length of the boo for calculation purposes
            TimeSpan totalBooLength = Boo.TotalDuration + Boo.SectionDuration;

            // What is this users recording limit
            TimeSpan recordingLimit = App.ViewModel.DbViewModel.CurrentUser != null ? TimeSpan.FromSeconds(App.ViewModel.DbViewModel.CurrentUser.Duration) : TimeSpan.FromSeconds(180);

            // How much time do they have left
            TimeSpan recordingTimeAvailable = recordingLimit - totalBooLength;

            // Update time left
            MediaRecorder.Update(recordingTimeAvailable);

            // Ran out of time
            //if (recordingTimeAvailable <= TimeSpan.FromSeconds(0))
            //{
            //    // Stop recording....
            //}
        }

        /// <summary>
        /// Recording has stopped (paused)
        /// </summary>
        void SoundRecorder_RecordStopped(object sender, RecordingStoppedEventArgs e)
        {
            //MessageBox.Show(e.Duration.ToString(), "Duration", MessageBoxButton.OK);

            // What is the total length of this boo
            Boo.TotalDuration += e.Duration;

            // We aren't recording anymore
            Boo.IsRecording = false;

            // Sync changes
            DatabaseHelper.AudioBooDataContext.SubmitChanges();

            // Set the play position marker
            MediaPlayer.SetProgressPosition(TimeSpan.Zero, Boo.TotalDuration);

            // Save the boo stream to isolated storage
            Boo.SaveSound();
        }


        /// <summary>
        /// The sound buffer is ready
        /// </summary>
        void SoundRecorder_RecordingMicrophoneBufferReady(object sender, EventArgs e)
        {
            bufferReadyCount += 1;
            TextBufferReady.Text = bufferReadyCount.ToString();
        }

        /// <summary>
        /// The buffer has been processed - update the wave
        /// </summary>
        void SoundRecorder_BufferProcessed(object sender, RecordingBufferProcessedEventArgs e)
        {
            // Update the wave control
            WaveControl.Update(e.Volume);// Math.Sin(e.Duration.TotalMilliseconds/100));
        }

        #endregion


        #region Media Player Button Events

        /// <summary>
        /// The play button has been pressed
        /// </summary>
        void MediaPlayer_PlayPressed(object sender, EventArgs e)
        {
            // Reset the play slider
            //MediaPlayer.Reset();

            // Start or resume
            App.SoundPlayerHelper.StartPlaying(Boo, scannedTo);
        }

        /// <summary>
        /// The pause button has been pressed
        /// </summary>
        void MediaPlayer_PausePressed(object sender, EventArgs e)
        {
            // Pause
            App.SoundPlayerHelper.PausePlaying();
        }

        /// <summary>
        /// Scanning of the position indicator has started
        /// </summary>
        void MediaPlayer_ScanningStarted(object sender, DraftBooMediaPlayerControl.ScanningStartedEventArgs e)
        {
            // Record whether we are playing so the scanning can continue playing when finished
            wasPlaying = App.SoundPlayerHelper.IsPlaying();

            // Stop because they are scanning using the slider
            App.SoundPlayerHelper.StopPlaying();
        }

        /// <summary>
        /// Scanning of the position indicator has finished
        /// </summary>
        void MediaPlayer_ScanningCompleted(object sender, DraftBooMediaPlayerControl.ScanningCompletedEventArgs e)
        {
            // keep the scanned to position in case we need to re-start from this scanned position
            scannedTo = e.Offset;

            if (wasPlaying)
            {
                // Re-start playing from the new scanned-to position
                MediaPlayer.Reset(e.Offset);

                // Start playing from wherever they stopped scanning to
                App.SoundPlayerHelper.StartPlaying(Boo, e.Offset);
            }
        }

        #endregion


        #region Sound Player Events

        /// <summary>
        /// The sound recorder has started playing
        /// </summary>
        void SoundPlayer_PlayingStart(object sender, PlayingStartEventArgs e)
        {
            // Turn off the recorder control
            MediaRecorder.Visibility = System.Windows.Visibility.Collapsed;

            Boo.IsPlaying = true;
        }

        void SoundPlayer_PlayingPaused(object sender, PlayingStoppedEventArgs e)
        {
            // Turn off the recorder control
            MediaRecorder.Visibility = System.Windows.Visibility.Visible;

            //MediaPlayer.Reset();

            //// What is the total length of this boo
            //Boo.TotalDuration += e.Duration;

            Boo.IsPlaying = false;
        }

        /// <summary>
        /// Playing has naturally come to an end
        /// </summary>
        void SoundPlayer_PlayingFinished(object sender, PlayingStoppedEventArgs e)
        {
            // Turn back on the recorder control
            MediaRecorder.Visibility = System.Windows.Visibility.Visible;

            scannedTo = TimeSpan.Zero;
            wasPlaying = false;

            Boo.IsPlaying = false;

            MediaPlayer.Reset();
        }

        /// <summary>
        /// When the player ticks
        /// </summary>
        void SoundPlayer_PlayingTick(object sender, PlayingTickEventArgs e)
        {
            //// Length of just this section
            //Boo.SectionDuration = e.Duration;

            //// What is the TOTAL length of the boo for calculation purposes
            //TimeSpan totalBooLength = Boo.TotalDuration + Boo.SectionDuration;

            // Fill in the media player values
            MediaPlayer.SetProgressPosition(e.Duration, Boo.TotalDuration);
        }

        #endregion


        #region Property Changed

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

    }
}
