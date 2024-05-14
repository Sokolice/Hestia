using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Core;
using Windows.UI;
using Windows.Media.SpeechRecognition;
using Hestia.Common;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Hestia.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainView : Page
    {

        public static MainView Current = null;
        public Frame AppFrame { get { return this.frame; } }

        public MainView()
        {
            this.InitializeComponent();
            this.Loaded += (sender, args) =>
            {
                Current = this;
            };

            (DataContext as ViewModel.MainViewModel).OnNavigate += MainView_OnNavigate;
            (DataContext as ViewModel.MainViewModel).OnLanguageChanged += MainView_OnLanguageChanged;
         }

        private void MainView_OnLanguageChanged()
        {

            HomeBtn.Label = GlobalContext.ResourceLoader.GetString("btnControl/Label");

            this.AppFrame.Navigate(typeof(SettingsView));

        }

        private async void MainView_OnNavigate(string obj)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                switch (obj)
                {
                    case "control":
                        NavigateToControl();
                        break;
                    case "help":
                        NavigateToHelp();
                        break;
                    case "settings":
                        NavigateToSettings();
                        break;
                }

            });
        }
        public Action<object, NavigationFailedEventArgs> NavigationFailed { get; internal set; }

        internal void Navigate(Type type, string arguments)
        {
            throw new NotImplementedException();
        }
        private void OnNavigatedToPage(object sender, NavigationEventArgs e)
        {
            // After a successful navigation set keyboard focus to the loaded page
            if (e.Content is Page && e.Content != null)
            {
                var control = (Page)e.Content;
                control.Loaded += Page_Loaded;
            }
        }
        private void OnNavigatingToPage(object sender, NavigatingCancelEventArgs e)
        {

        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ((Page)sender).Focus(FocusState.Programmatic);
            ((Page)sender).Loaded -= Page_Loaded;
        }

        private void HomeBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigateToControl();
        }

        private void SettingsBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigateToSettings();
        }
        
        private void HelpBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigateToHelp();
        }
        
        private void NavigateToControl()
        {
            if (this.AppFrame.CurrentSourcePageType != typeof(ControlView))
            {
                this.AppFrame.Navigate(typeof(ControlView));
            }
        }
        private void NavigateToSettings()
        {
            if (this.AppFrame.CurrentSourcePageType != typeof(SettingsView))
            {
                this.AppFrame.Navigate(typeof(SettingsView));
            }
        }

        private void NavigateToHelp()
        {
            if (this.AppFrame.CurrentSourcePageType != typeof(HelpView))
            {
                this.AppFrame.Navigate(typeof(HelpView));
            }
        }

    }
}
