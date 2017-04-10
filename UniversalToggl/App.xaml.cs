using System;
using TogglAPI;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Security.Credentials;
using Windows.Storage;
using Windows.UI.ViewManagement;
using UniversalToggl.View;
using System.IO;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections;
using Windows.UI.Core;

namespace UniversalToggl
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        public static string AppName = "UniversalToggl";

        public static ApplicationDataContainer localStorage = ApplicationData.Current.LocalSettings;
        public static PasswordVault vault = new PasswordVault();

        public static User user;
        public static Workspace currentWorkspace;

        public static DataContainer Data = new DataContainer();

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            // Each time a navigation event occurs, update the Back button's visibility
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                ((Frame)sender).CanGoBack ?
                AppViewBackButtonVisibility.Visible :
                AppViewBackButtonVisibility.Collapsed;
        }

        private void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            var rootFrame = Window.Current.Content as RootControl;

            if (rootFrame.RootFrame.CanGoBack)
            {
                e.Handled = true;
                rootFrame.RootFrame.GoBack();
            }
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            var rootFrame = Window.Current.Content as RootControl;

            // Load state from previously suspended application
            await ReadAppData();

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new RootControl();

                rootFrame.RootFrame.NavigationFailed += OnNavigationFailed;
                rootFrame.RootFrame.Navigated += OnNavigated;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                }
                // Register a handler for BackRequested events and set the
                // visibility of the Back button
                SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    rootFrame.RootFrame.CanGoBack ?
                    AppViewBackButtonVisibility.Visible :
                    AppViewBackButtonVisibility.Collapsed;

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.RootFrame.Content == null)
                {
                    rootFrame.RootFrame.Navigate(typeof(MainPage));
                }
            }

            ApplicationView.GetForCurrentView().SetDesiredBoundsMode(ApplicationViewBoundsMode.UseVisible);
            // Ensure the current window is active
            Window.Current.Activate();
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            SaveAppData();
            deferral.Complete();
        }

        /// <summary>
        /// Delete all locally storaged data
        /// </summary>
        public static async void ClearSaveData()
        {
            try
            {
                var file = await ApplicationData.Current.LocalFolder.TryGetItemAsync("TimeEntries");
                if (file != null) await file.DeleteAsync();
                file = await ApplicationData.Current.LocalFolder.TryGetItemAsync("Workspaces");
                if (file != null) await file.DeleteAsync();
                file = await ApplicationData.Current.LocalFolder.TryGetItemAsync("Projects");
                if (file != null) await file.DeleteAsync();
                file = await ApplicationData.Current.LocalFolder.TryGetItemAsync("Tags");
                if (file != null) await file.DeleteAsync();
            }
            catch (IOException)
            { }
        }

        /// <summary>
        /// Save the app data to the local storage
        /// </summary>
        /// <returns></returns>
        public static void SaveAppData()
        {
            SaveData("TimeEntries", typeof(ObservableCollection<TimeEntry>), Data.TimeEntries);
            SaveData("Workspaces", typeof(ObservableCollection<Workspace>), Data.Workspaces);
            SaveData("Projects", typeof(ObservableCollection<Project>), Data.Projects);
            SaveData("Tags", typeof(ObservableCollection<Tag>), Data.Tags);
        }

        /// <summary>
        /// Save data to the local storage
        /// </summary>
        /// <param name="name">Name of the file to storage the data in</param>
        /// <param name="type">The type of the data to store</param>
        /// <param name="data">The data to store</param>
        private static async void SaveData(string name, Type type, ICollection data)
        {
            StorageFile file = null;
            try
            {
                file = await ApplicationData.Current.LocalFolder.CreateFileAsync(name, CreationCollisionOption.ReplaceExisting);
                using (Stream writer = await file.OpenStreamForWriteAsync())
                {
                    DataContractSerializer serializer = new DataContractSerializer(type);
                    serializer.WriteObject(writer, data);
                    await writer.FlushAsync();
                    writer.Dispose();
                }
            }
            catch (SerializationException)
            {
                if (file != null)
                    await file.DeleteAsync();
            }
        }

        /// <summary>
        /// Read the data from local storage, if any
        /// </summary>
        /// <returns></returns>
        public static async Task ReadAppData()
        {
            try
            {
                var file = await ApplicationData.Current.LocalFolder.TryGetItemAsync("TimeEntries");
                if (file != null)
                {
                    using (Stream reader = await (file as StorageFile).OpenStreamForReadAsync())
                    {
                        DataContractSerializer serializer = new DataContractSerializer(typeof(ObservableCollection<TimeEntry>));
                        Data.TimeEntries = (ObservableCollection<TimeEntry>)serializer.ReadObject(reader);
                        reader.Dispose();
                    }
                }
                file = await ApplicationData.Current.LocalFolder.TryGetItemAsync("Workspaces");
                if (file != null)
                {
                    using (Stream reader = await (file as StorageFile).OpenStreamForReadAsync())
                    {
                        DataContractSerializer serializer = new DataContractSerializer(typeof(ObservableCollection<Workspace>));
                        Data.Workspaces = (ObservableCollection<Workspace>) serializer.ReadObject(reader);
                        reader.Dispose();
                    }
                }
                file = await ApplicationData.Current.LocalFolder.TryGetItemAsync("Projects");
                if (file != null)
                {
                    using (Stream reader = await (file as StorageFile).OpenStreamForReadAsync())
                    {
                        DataContractSerializer serializer = new DataContractSerializer(typeof(ObservableCollection<Project>));
                        Data.Projects = (ObservableCollection<Project>)serializer.ReadObject(reader);
                        reader.Dispose();
                    }
                }
                file = await ApplicationData.Current.LocalFolder.TryGetItemAsync("Tags");
                if (file != null)
                {
                    using (Stream reader = await (file as StorageFile).OpenStreamForReadAsync())
                    {
                        DataContractSerializer serializer = new DataContractSerializer(typeof(ObservableCollection<Tag>));
                        Data.Tags = (ObservableCollection<Tag>)serializer.ReadObject(reader);
                        reader.Dispose();
                    }
                }
            }
            catch (Exception)
            { }
        }

    }
}
