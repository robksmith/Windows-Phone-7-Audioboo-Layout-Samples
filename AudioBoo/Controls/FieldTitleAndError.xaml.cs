
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
using System.Windows.Media;

#endregion

namespace AudioBoo.Controls
{
    public partial class FieldTitleAndError : UserControl
    {
        public string Title { get; set; }

        public FieldTitleAndError()
        {
            InitializeComponent();
        }

        internal void SetError(string errorMessage)
        {
            TextBlockMessage.Text = errorMessage;
            TextBlockMessage.Foreground = new SolidColorBrush(Colors.Red);
        }

        internal void ClearError()
        {
            TextBlockMessage.Text = Title;
            TextBlockMessage.Foreground = (SolidColorBrush)Application.Current.Resources["StandardTextColour"];
        }
    }
}
