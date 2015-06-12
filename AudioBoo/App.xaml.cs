
#region Usings

using AudioBoo.Helpers;
using AudioBoo.ViewModels;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using AudioBoo.Helpers;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.BackgroundAudio;
using System.IO.IsolatedStorage;
using System.Windows.Resources;

#endregion

namespace AudioBoo
{
    public partial class App : Application
    {
        #region Fields

        private static MainViewModel viewModel = null;

        private static SoundRecorderHelper soundRecorderHelper = null;

        private static SoundPlayerHelper soundPlayerHelper = null;

        private static PopupHelper popupHelper = null;

        private static AppConstants appConstants = null;

        #endregion


        #region Properties

        /// <summary>
        /// Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        public PhoneApplicationFrame RootFrame { get; private set; }

        public static MediaElement GlobalMediaElement
        {
            get { return Current.Resources["GlobalMedia"] as MediaElement; }
        }

        #endregion


        /// <summary>
        /// Constructor for the Application object.
        /// </summary>
        public App()
        {
            CopyToIsolatedStorage();

            // record the app start time
            appStartTime = DateTime.Now;

            // Global handler for uncaught exceptions. 
            UnhandledException += Application_UnhandledException;

            // Standard Silverlight initialization
            InitializeComponent();

            // Phone-specific initialization
            InitializePhoneApplication();

            // Initialise a new audioboo database and agent database if they dont already exist
            DatabaseHelper.CreateTheDatabases();

            // Show graphics profiling information while debugging.
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Close the background audio player in case it  was running from a previous debugging session.
                BackgroundAudioPlayer.Instance.Close();

                // Display the current frame rate counters
                //Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // Show the areas of the app that are being redrawn in each frame.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode, 
                // which shows areas of a page that are handed off to GPU with a colored overlay.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;

                // Disable the application idle detection by setting the UserIdleDetectionMode property of the
                // application's PhoneApplicationService object to Disabled.
                // Caution:- Use this under debug mode only. Application that disables user idle detection will continue to run
                // and consume battery power when the user is not using the phone.
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }

            // If the currently streaming sound isn't ours then kill it
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                AudioHelper.KillAlienSounds();
            }

            RootFrame.Navigating += RootFrame_Navigating;
        }

        /// <summary>
        /// We want to cature a navigating event - if its to the dummy landing page, then check whether we are logged in and send to the relevant page
        /// </summary>
        void RootFrame_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            // Only care about our landing page 
            if (e.Uri.ToString().Contains("/LandingPage.xaml"))
            {
                bool loggedIn = App.ViewModel.DbViewModel.IsLoggedOn();

                // Work out whether we need to hold the splash screen - otherwise on fast devices it just flashes on and off the screen
                TimeSpan timeTakenToGetHere = DateTime.Now - appStartTime;
                TimeSpan timeToSleepFor = TimeSpan.FromSeconds(App.AppConstants.SecondsToHoldSplashScreen) - timeTakenToGetHere;

                if (timeToSleepFor > TimeSpan.FromSeconds(0))
                {
                    System.Threading.Thread.Sleep(timeToSleepFor);
                }

                // Cancel current navigation and schedule the real navigation for the next tick 
                // (we can't navigate immediately as that will fail; no overlapping navigations are allowed) 
                e.Cancel = true;
                RootFrame.Dispatcher.BeginInvoke(delegate
                {
                    if (loggedIn)
                    {
                        RootFrame.Navigate(new Uri("/Views/MainPage.xaml", UriKind.Relative));
                    }
                    else
                    {
                        //RootFrame.Navigate(new Uri("/Views/WelcomePage.xaml", UriKind.Relative));
                        RootFrame.Navigate(new Uri("/Views/MainPage.xaml", UriKind.Relative));
                    }
                });
            }
        }

        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
        }

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
        }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
            // Ensure that required application state is persisted here.
        }

        // Code to execute if a navigation fails
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        // Code to execute on Unhandled Exceptions
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized = false;
        private DateTime appStartTime;

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            RootFrame = new PhoneApplicationFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;
        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        #endregion



        /// <summary>
        /// Copies the files from the application data to isolated storage.
        /// </summary>
        private void CopyToIsolatedStorage()
        {
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                // Copy audio files to isolated storage
                string[] files = new string[] { "Ring01.wma", "Ring02.wma", "Ring03.wma" };

                foreach (var _fileName in files)
                {
                    if (!storage.FileExists(_fileName))
                    {
                        string _filePath = "Resources/Audio/" + _fileName;
                        StreamResourceInfo resource = Application.GetResourceStream(new Uri(_filePath, UriKind.Relative));

                        using (IsolatedStorageFileStream file = storage.CreateFile(_fileName))
                        {
                            int chunkSize = 4096;
                            byte[] bytes = new byte[chunkSize];
                            int byteCount;

                            while ((byteCount = resource.Stream.Read(bytes, 0, chunkSize)) > 0)
                            {
                                file.Write(bytes, 0, byteCount);
                            }
                        }
                    }
                }

                // Copy Tile icons for each track to isolated storage
                string[] icons = new string[] { "Ring01.jpg", "Ring02.jpg", "Ring03.jpg", "Episode29.jpg" };

                foreach (var _fileName in icons)
                {
                    if (!storage.FileExists("Shared/Media/" + _fileName))
                    {
                        string _filePath = "Resources/Images/TrackIcons/" + _fileName;
                        StreamResourceInfo iconResource = Application.GetResourceStream(new Uri(_filePath, UriKind.Relative));

                        // The Tile images MUST be located in the Shared/Media directory in order 
                        // to get picked up by the Now Playing Tile in the Music + Videos Hub
                        using (IsolatedStorageFileStream file = storage.CreateFile("Shared/Media/" + _fileName))
                        {
                            int chunkSize = 4096;
                            byte[] bytes = new byte[chunkSize];
                            int byteCount;

                            while ((byteCount = iconResource.Stream.Read(bytes, 0, chunkSize)) > 0)
                            {
                                file.Write(bytes, 0, byteCount);
                            }
                        }
                    }
                }

            }
        }
    }
}