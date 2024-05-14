using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Hestia.ViewModel;
using Hestia.Common;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Hestia.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsView : Page
    {
        public SettingsView()
        {
            this.InitializeComponent();
            (this.DataContext as SettingsViewModel).OnSettigsChanged += SettingsView_OnSettigsChanged;
        }

        private void SettingsView_OnSettigsChanged(ViewType aViewType)
        {
            Frame.Navigate(aViewType == ViewType.SettingsView? typeof(SettingsView) : typeof(ConfigurationView));
        }
        
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            (this.DataContext as SettingsViewModel).OnSettigsChanged -= SettingsView_OnSettigsChanged;

            if ((this.DataContext as Hestia.ViewModel.SettingsViewModel) != null)
            {
                (this.DataContext as Hestia.ViewModel.SettingsViewModel).Dispose();
                this.DataContext = null;
            }
        }
    }
}
