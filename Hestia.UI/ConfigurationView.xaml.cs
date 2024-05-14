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
using Hestia.Common;
using Windows.UI;
using Windows.UI.Popups;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Hestia.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ConfigurationView : Page
    {
        public ConfigurationView()
        {
            this.InitializeComponent();
        }
        
        private void cmbCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbCategory.SelectedIndex == (int)DeviceCategory.Blinds)
            {
                gridBlindsAddresses.Visibility = Visibility.Visible;
                gridLightsAddresses.Visibility = Visibility.Collapsed;
            }

            if (cmbCategory.SelectedIndex == (int)DeviceCategory.Lights)
            {
                gridBlindsAddresses.Visibility = Visibility.Collapsed;
                gridLightsAddresses.Visibility = Visibility.Visible;
            }
        }

        private void Root_Unloaded(object sender, RoutedEventArgs e)
        {
            if ((this.DataContext as Hestia.ViewModel.ConfigurationViewModel) != null)
            {
                (this.DataContext as Hestia.ViewModel.ConfigurationViewModel).Dispose();
                this.DataContext = null;
            }
        }
        
        private void txt_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox lTxtx = sender as TextBox;
            lTxtx.BorderBrush = ((Common.Validation.ValidateAddress(lTxtx.Text) || lTxtx.Text == string.Empty) ? new SolidColorBrush(Colors.White) : new SolidColorBrush(Colors.Red));            
        }        
    }
}
