using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Hestia.Common
{
    /// <summary>
    /// Třída implementující rozhraní INotifyPropertyChanged
    /// </summary>
    public class NotifyBase : INotifyPropertyChanged 
    {
        public event PropertyChangedEventHandler PropertyChanged;
        

        public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
