using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media.Imaging;

namespace AudioBoo.Controls
{
    public partial class ProfileUserDetailsControl : UserControl
    {
        public ProfileUserDetailsControl()
        {
            InitializeComponent();

            //ImageUser.Source = new BitmapImage(new Uri("/Resources/Images/ABAnonymousBoo.jpg", UriKind.Relative));

        }
    }
}
