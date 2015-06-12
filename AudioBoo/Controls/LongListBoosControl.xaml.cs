
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
using System.Collections.ObjectModel;
using AudioBoo.Web.Responses;
using AudioBoo.ViewModels;

#endregion


namespace AudioBoo.Controls
{
    public partial class LongListBoosControl : UserControl
    {
        #region Fields

        private int pageNumber;
        private const int OffsetKnob = 5;
        UserProfileViewModel viewModel;
        int userId;

        #endregion


        #region Constructors

        public LongListBoosControl()
        {
            InitializeComponent();

            BooLongList.Link += Boos_FeaturedBoosOnLink;
        }

        #endregion



        internal void Initialise(UserProfileViewModel viewModel, int userId, int page)
        {
            this.viewModel = viewModel;
            this.userId = userId;
            this.pageNumber = page;


            // Set our view model as the data context
            DataContext = viewModel;

            viewModel.GetBoos(userId, pageNumber);
        }


        internal void GetBoos(int userId, int page)
        {
            viewModel.GetBoos(userId, page);
        }


        #region Long List Selector Event Handlers

        /// <summary>
        /// Infinite loading
        /// </summary>
        private void Boos_FeaturedBoosOnLink(object sender, LinkUnlinkEventArgs e)
        {
            ObservableCollection<BooClip> records = (BooLongList.ItemsSource as ObservableCollection<BooClip>);

            if (records == null) return;

            int currentItemCount = records.Count;

            if (!viewModel.IsLoading && currentItemCount >= OffsetKnob && (e.ContentPresenter.Content as BooClip) != null)
            {
                if ((e.ContentPresenter.Content as BooClip).Equals(records[currentItemCount - OffsetKnob]))
                {
                    viewModel.GetBoos(userId, ++pageNumber);
                }
            }
        }

        /// <summary>
        /// Go to view details page
        /// </summary>
        private void FeaturedBoos_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((LongListSelector)sender).SelectedItem == null)
                return;

            var selectedBoo = ((LongListSelector)sender).SelectedItem as BooClip;
            if (selectedBoo != null)
            {
                //Navigate(NavigationHelper.BooDetailPage, selectedBoo.BooId);
            }

            ((LongListSelector)sender).SelectedItem = null;
        }

        #endregion


        #region Animation

        private void Animation_ManipulationStarted_1(object sender, System.Windows.Input.ManipulationStartedEventArgs e)
        {
            ((UIElement)sender).RenderTransform = new System.Windows.Media.TranslateTransform() { X = 2, Y = 2 };
        }

        private void Animation_ManipulationCompleted_1(object sender, System.Windows.Input.ManipulationCompletedEventArgs e)
        {
            ((UIElement)sender).RenderTransform = null;
        }

        #endregion

    }
}
