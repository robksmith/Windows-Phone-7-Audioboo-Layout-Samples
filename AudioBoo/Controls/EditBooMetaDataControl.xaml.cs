using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace AudioBoo.Controls
{
    public partial class EditBooMetaDataControl : UserControl
    {
        public event EventHandler<EventArgs> ButtonClicked;

        public EditBooMetaDataControl()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (ButtonClicked != null)
            {
                ButtonClicked(this, new EventArgs());
            }
        }
    }
}
