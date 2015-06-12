
#region Usings

using AudioBoo.Controls;
using AudioBoo.Models;
using AudioBoo.ViewModels;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Navigation;

#endregion

namespace AudioBoo.Views
{
    public partial class MyBoosPage : PhoneApplicationPage
    {
        #region Fields

        private readonly MyBoosViewModel viewModel;
        
        // The popup vote control
        //Popup popupControl;
        //PopupRecordBooControl recordBooControl;

        #endregion


        #region Properties

        public ObservableCollection<BooDbRecord> MyDraftBoos { get; set; }

        #endregion


        #region Constructors

        public MyBoosPage()
        {
            InitializeComponent();

            // Register for the pivot selection changed event
            MainPivot.SelectionChanged += PivotControl_SelectionChanged;

            // Create an app bar
            ApplicationBar = new ApplicationBar();

            // Create the popups
            //popupControl = new Popup();
            //recordBooControl = new PopupRecordBooControl();
            //popupControl.Child = recordBooControl;
            //popupControl.VerticalOffset = 40;

            // My boos view model
            //if (!DesignerProperties.IsInDesignTool)
            //{
                viewModel = (MyBoosViewModel)Resources["MyBoosViewModel"];
                viewModel.GetMyBoos(1);
            //}

            // My draft boos
            MyDraftBoosList.ItemsSource = App.ViewModel.DbViewModel.AllDraftBoos;
            MyDraftBoosList.ItemTemplate = (DataTemplate)Resources["DraftBooClipTemplate"];
            MyDraftBoosList.SelectionChanged += new SelectionChangedEventHandler(draftBoosListLongList_SelectionChanged);
        }

        #endregion


        #region Event handlers


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            MyDraftBoosList.SelectedItem = null;
        }

        /// <summary>
        /// When the pivot selection changes we want to alter the app bar
        /// </summary>
        void PivotControl_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // close the keyboard if its open
            this.Focus();

            // enable disable app bar depending on the pivot
            switch (((Pivot)sender).SelectedIndex)
            {
                case 0:
                    CreateApplicationBarForPublishedBoos();
                    break;

                case 1:
                    CreateApplicationBarForDraftBoos();
                    break;
            }
        }

        /// <summary>
        /// User wants to create a new boo
        /// </summary>
        void AppBarMenuItemNewBoo_Click(object sender, EventArgs e)
        {
            BooDraftDbRecord boo = App.ViewModel.DbViewModel.CreateDraftBoo();

            App.ViewModel.DbViewModel.OrderAllDraftBoos();

            MyDraftBoosList.ItemsSource = App.ViewModel.DbViewModel.AllDraftBoos;

            MyDraftBoosList.ScrollTo(App.ViewModel.DbViewModel.AllDraftBoos[0]);
        }

        /// <summary>
        /// A draft boo has been clicked
        /// </summary>
        void draftBoosListLongList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ( MyDraftBoosList.SelectedItem == null )
            {
                // Because this is raised when we explicitly set it to null to clear it above
                return;
            }

            // Get the boo
            BooDraftDbRecord boo = (BooDraftDbRecord)MyDraftBoosList.SelectedItem;

            // Load the boo pcm from isolated storage into the memorystream
            boo.LoadSound();

            // Load the associated image
            boo.LoadImage();

            // Set it as the context on the popup
            //recordBooControl.DataContext = boo;

            // What boo are we recording into?
            //recordBooControl.Boo = boo;

            NavigationService.Navigate(new Uri("/Views/EditDraftBoos/RecordBooPage.xaml?draftBooId=" + boo.BooId, UriKind.RelativeOrAbsolute));
        }

        #endregion


        #region Helpers

        void CreateApplicationBarForPublishedBoos()
        {
            // clear the app bar
            ApplicationBar.MenuItems.Clear();
            ApplicationBar.Buttons.Clear();

            ApplicationBar.Mode = ApplicationBarMode.Default;
            ApplicationBar.Opacity = 1.0;
            ApplicationBar.IsVisible = true;
            ApplicationBar.IsMenuEnabled = true;
        }


        /// <summary>
        /// Create our application bar for my account pivot
        /// </summary>
        void CreateApplicationBarForDraftBoos()
        {
            // clear the app bar
            ApplicationBar.MenuItems.Clear();
            ApplicationBar.Buttons.Clear();

            ApplicationBar.Mode = ApplicationBarMode.Default;
            ApplicationBar.Opacity = 1.0;
            ApplicationBar.IsVisible = true;
            ApplicationBar.IsMenuEnabled = true;

            // The new boo button
            ApplicationBarIconButton newBoo = new ApplicationBarIconButton();
            newBoo.IconUri = new Uri("Resources/Images/AppBar/appbar.add.rest.png", UriKind.Relative);
            newBoo.Text = "new boo";
            newBoo.Click += new EventHandler(AppBarMenuItemNewBoo_Click);
            
            ApplicationBar.Buttons.Add(newBoo);

            //CreateStandardMenuItems();
        }

        #endregion

    }
}