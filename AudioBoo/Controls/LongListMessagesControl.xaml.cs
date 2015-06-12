
#region usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using AudioBoo.Web.Responses;
using AudioBoo.ViewModels;
using System.Windows.Data;

#endregion


namespace AudioBoo.Controls
{
    public partial class LongListMessagesControl : UserControl
    {
        public class MessageSelectedEventArgs : EventArgs
        {
            public MessagesViewModel.MessageDataViewModel Message { get; set; }
        }

        #region Fields

        #endregion


        public event EventHandler<MessageSelectedEventArgs> MessageSelected;


        public LongListMessagesControl()
        {
            InitializeComponent();
        }


        internal void GetMessages(MessagesViewModel messagesViewModel)
        {
            MessagesList.DataContext = messagesViewModel;
            messagesViewModel.GetMessages();
        }

        private void MessagesList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MessagesViewModel.MessageDataViewModel md = (MessagesViewModel.MessageDataViewModel)MessagesList.SelectedItem;

            if (md != null)
            {
                if (MessageSelected != null)
                {
                    MessageSelected(this, new MessageSelectedEventArgs() { Message = md });
                }

                MessagesList.SelectedItem = null;
            }
        }

    }
}
