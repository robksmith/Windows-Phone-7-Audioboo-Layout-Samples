using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace AudioBoo.Controls
{
    public partial class RecordBooControl : INotifyPropertyChanged
    {
        #region Properties

        private readonly Microphone microphone = Microphone.Default;
        private byte[] buffer;
        private readonly MemoryStream stream = new MemoryStream();
        private SoundEffectInstance soundEffectInstance;
        private readonly DispatcherTimer dt;

        private int volume;
        private double duration;
       
        private bool isPlaying;
        public bool IsPlaying
        {
            get { return isPlaying; }
            set
            {
                if (isPlaying != value)
                {
                    isPlaying = value;
                    OnPropertyChanged("IsPlaying");
                }
            }
        }

        private bool isRecording;
        public bool IsRecording
        {
            get { return isRecording; }
            set
            {
                if (isRecording != value)
                {
                    isRecording = value;
                    OnPropertyChanged("IsRecording");
                }
            }
        }

        private string timeString;
        public string TimeString
        {
            get { return timeString; }
            set
            {
                if (timeString != value)
                {
                    timeString = value;
                    OnPropertyChanged("TimeString");
                }
            }
        }

        
        #endregion

        #region Constructor

        public RecordBooControl()
        {
            InitializeComponent();

            DataContext = this;
            dt = new DispatcherTimer {Interval = TimeSpan.FromMilliseconds(33)};
            dt.Tick += DtOnTick;
            dt.Start();

            microphone.BufferReady += MicrophoneOnBufferReady;
            PositionIndicator.Minimum = 0;
            TimeString = "00:00 / 00:00";
        }

        #endregion

        #region Events

        private void MicrophoneOnBufferReady(object sender, EventArgs eventArgs)
        {
            microphone.GetData(buffer);

            ushort byte1 = 0;
            ushort byte2 = 0;

            double rms = (short) (byte1 | (byte2 << 8));

            for (int i = 0; i < buffer.Length; i+=2)
            {
                byte1 = buffer[i];
                byte2 = buffer[i + 1];

                short val = (short) (byte1 | (byte2 << 8));
                rms += Math.Pow(val, 2);
            }

            rms /= buffer.Length/2.0f;
           volume = (int) Math.Floor(Math.Sqrt(rms));
            
            //if (volume <= 500)
            //{
            //    WaveControl.WaveGain = volume / 1000.0f;
            //}

            System.Diagnostics.Debug.WriteLine("Volume: {0}\n\n\n", volume);
            stream.Write(buffer, 0, buffer.Length);
        }

        private void DtOnTick(object sender, EventArgs eventArgs)
        {
            try
            {
                FrameworkDispatcher.Update();
            }
            catch (Exception)
            {

            }
            finally
            {

                if (IsRecording)
                {
                    duration += dt.Interval.TotalSeconds*2;
                    TimeString = FormatTimeRemaining();
                    //WaveControl.Update();

                }

                if (IsPlaying)
                {
                    PositionIndicator.Maximum = duration;
                    PositionIndicator.Value += dt.Interval.TotalSeconds*2;

                    TimeString = FormatTime();
                    
                    if (soundEffectInstance.State != SoundState.Playing)
                    {
                        IsPlaying = false;
                        PositionIndicator.Value = 0;

                    }
                }
            }
        }

        private string FormatTimeRemaining()
        {

            int limit = App.ViewModel.DbViewModel.CurrentUser != null ? App.ViewModel.DbViewModel.CurrentUser.Duration : 180;

            TimeSpan t = TimeSpan.FromSeconds(limit-duration);
            return string.Format("{0:D2}:{1:D2}:{2:D2}",
                        t.Hours,
                        t.Minutes,
                        t.Seconds);    
        }

        private string FormatTime()
        {
            TimeSpan t = TimeSpan.FromSeconds(PositionIndicator.Value);

            string currentPosition = string.Format("{0:D2}:{1:D2}",
                            t.Minutes,
                            t.Seconds);

            t = TimeSpan.FromSeconds(duration);

            string length = string.Format("{0:D2}:{1:D2}", t.Minutes, t.Seconds);

            return currentPosition + " / " + length;
        }

        private void PauseButtonClick(object sender, RoutedEventArgs e)
        {
            if (IsPlaying)
            {
                
                PositionIndicator.Value = 0;
                soundEffectInstance.Stop();
                IsPlaying = false;
            }
        }

        private void PlayButtonClick(object sender, RoutedEventArgs e)
        {
            if (!IsPlaying)
            {
                if (stream.Length > 0)
                {
                    
                    // Play the audio in a new thread so the UI can update.

                    var soundThread = new Thread(PlaySound);
                    soundThread.Start();
                    IsPlaying = true;
                }
            }
        }

        private void RecordButton_Click(object sender, RoutedEventArgs e)
        {
            if (microphone.State == MicrophoneState.Stopped)
            {
                microphone.BufferDuration = TimeSpan.FromMilliseconds(100);

                buffer = new byte[microphone.GetSampleSizeInBytes(microphone.BufferDuration)];

                microphone.Start();

                IsRecording = true;
            }
        }

        private void PauseRecordingButton_Click(object sender, RoutedEventArgs e)
        {
            if (microphone.State == MicrophoneState.Started)
            {
                microphone.Stop();

                //WaveControl.WaveGain = 0;
                //WaveControl.Update();
                IsRecording = false;
                TimeString = FormatTime();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Private Methods

        private void PlaySound()
        {
            if(soundEffectInstance != null) soundEffectInstance.Stop(true);
            // Play audio using SoundEffectInstance so we can monitor it's State 
            // and update the UI in the dt_Tick handler when it is done playing.
            SoundEffect sound = new SoundEffect(stream.ToArray(), microphone.SampleRate, AudioChannels.Mono);
            soundEffectInstance = sound.CreateInstance();
            soundEffectInstance.Play();
            }

        #endregion


    }
}
