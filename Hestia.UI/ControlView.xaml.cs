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
using Hestia.ViewModel;
using Hestia.Model;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Hestia.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ControlView : Page
    {
        public ControlView()
        {
            this.InitializeComponent();
        }

        private void RoomListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (States.CurrentState == Reduced)
            {
                var lItemId = (e.ClickedItem as Room).Id;
                Frame.Navigate(typeof(ControlDetailView), lItemId);
            }
        }

        private void MainPage_Unloaded(object sender, RoutedEventArgs e)
        {
            if ((this.DataContext as Hestia.ViewModel.ControlViewModel) != null)
            {
                (this.DataContext as Hestia.ViewModel.ControlViewModel).Dispose();
                this.DataContext = null;
            }
        }

        private void States_CurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            ChangeToDetail(e.NewState, e.OldState);
        }

        private void ChangeToDetail(VisualState aNewState, VisualState aOldState)
        {
            var lRoom = (this.DataContext as ControlViewModel).ControlRoom;

            if (lRoom != null)
            {
                if (aNewState == Reduced && aOldState == Default && lRoom.Id != Guid.Empty)
                {

                    var lItemId = (this.DataContext as ControlViewModel).ControlRoom.Id;
                    Frame.Navigate(typeof(ControlDetailView), lItemId);
                }
            }
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                if ((string)e.Parameter != string.Empty)
                    (this.DataContext as ControlViewModel).ControlRoom = DatabaseContext.Rooms.FirstOrDefault(aR => aR.Id == Guid.Parse(e.Parameter.ToString()));
            }
        }
    }
}
