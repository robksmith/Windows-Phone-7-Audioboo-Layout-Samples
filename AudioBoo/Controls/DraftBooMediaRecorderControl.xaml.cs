
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
using Microsoft.Xna.Framework;
using System.Windows.Media;

#endregion


namespace AudioBoo.Controls
{
    public partial class DraftBooMediaRecorderControl : UserControl
    {
        #region Events

        public event EventHandler<EventArgs> RecordPressed;
        public event EventHandler<EventArgs> PausePressed;

        #endregion


        #region Constructors

        public DraftBooMediaRecorderControl()
        {
            InitializeComponent();
        }

        #endregion


        #region Helpers

        /// <summary>
        /// Update the recording time available
        /// </summary>
        internal void Update(TimeSpan recordingTimeAvailable)
        {
            if (recordingTimeAvailable <= TimeSpan.FromSeconds(10))
            {
                // change colour to red to indicate they are running out of time
                TextBlockTimeRemaining.Foreground = new SolidColorBrush(Colors.Red);
            }

            if (recordingTimeAvailable <= TimeSpan.FromSeconds(10))
            {
                if (PausePressed != null)
                {
                    PausePressed(this, new EventArgs());
                }
            }


            TimeSpan displayTime = recordingTimeAvailable + TimeSpan.FromSeconds(1);

            TextBlockTimeRemaining.Text = string.Format("{0:D2}:{1:D2}:{2:D2}", displayTime.Hours, displayTime.Minutes, displayTime.Seconds);
        }

        #endregion


        #region Record Button Events

        /// <summary>
        /// Start recording button has been pressed
        /// </summary>
        private void RecordButton_Click(object sender, RoutedEventArgs e)
        {
            if (RecordPressed != null)
            {
                RecordPressed(this, new EventArgs());
            }
        }

        /// <summary>
        /// Pause recording
        /// </summary>
        private void PauseRecordingButton_Click(object sender, RoutedEventArgs e)
        {
            if (PausePressed != null)
            {
                PausePressed(this, new EventArgs());
            }
        }

        #endregion

    }
}
