
#region Usings

using System;
using System.Windows;
using System.Windows.Controls;

#endregion

namespace FAS.Controls
{
    public partial class AskRegisterOrLoginControl : UserControl
    {
        #region Events

        public event EventHandler<EventArgs> RegisterPressed;
        public event EventHandler<EventArgs> LoginPressed;
        public event EventHandler<EventArgs> LaterPressed;

        #endregion


        #region Properties

        public AskRegisterOrLoginControl()
        {
            InitializeComponent();
        }

        #endregion


        #region Event Handlers

        private void ButtonRegister_Click(object sender, RoutedEventArgs e)
        {
            if (RegisterPressed != null)
            {
                RegisterPressed(this, EventArgs.Empty);
            }
        }

        private void ButtonLogin_Click(object sender, RoutedEventArgs e)
        {
            if (LoginPressed != null)
            {
                LoginPressed(this, EventArgs.Empty);
            }
        }

        private void HyperlinkButtonLater_Click(object sender, RoutedEventArgs e)
        {
            if (LaterPressed != null)
            {
                LaterPressed(this, EventArgs.Empty);
            }
        }

        #endregion
    }
}
