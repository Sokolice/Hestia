
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Popups;

namespace Hestia.Common
{
    public static class Globals
    {
        public static string ConfigFile = ApplicationData.Current.LocalFolder.Path + "/config.xml";
        public static string LogFile = ApplicationData.Current.LocalFolder.Path + "/log.xml";
        public static string SettingsFile = ApplicationData.Current.LocalFolder.Path + "/settings.xml";

        /// <summary>
        /// Metoda pro nastavení zadané property pomocí reflexe
        /// </summary>
        /// <param name="aSource">zdrojová třída</param>
        /// <param name="aProperty">název property</param>
        /// <param name="aTarget">cílová hodnota</param>
        public static void SetProperty(object aSource, string aProperty, object aTarget)
        {
            string[] bits = aProperty.Split('.');
            for (int i = 0; i < bits.Length - 1; i++)
            {
                PropertyInfo prop = aSource.GetType().GetProperty(bits[i]);
                aSource = prop.GetValue(aSource, null);
            }
            PropertyInfo propertyToSet = aSource.GetType()
                                               .GetProperty(bits[bits.Length - 1]);
            propertyToSet.SetValue(aSource, aTarget, null);
        }

        /// <summary>
        /// Zobrazení chybového hlášení
        /// </summary>
        /// <param name="aMessage"></param>
        public static async void ShowError(string aMessage)
        {
            var lMessageDialog = new MessageDialog(GlobalContext.ResourceLoader.GetString(aMessage), GlobalContext.ResourceLoader.GetString("errWarningHeader"));
            lMessageDialog.Commands.Add(new Windows.UI.Popups.UICommand("OK") { Id = 0 });
            lMessageDialog.DefaultCommandIndex = 0;
            await lMessageDialog.ShowAsync();
        }
    }

    /// <summary>
    /// Enumarace pro typy zařízení
    /// </summary>
    public enum DeviceCategory
    {
        Lights = 0,
        Blinds = 1

    }

    /// <summary>
    /// Enumerace pro možné funkce zařízení
    /// </summary>
    public enum FunctionTypeCategory
    {
        OnOff = 0,
        Dimming = 1,
        BrightnessValue = 2,
        UpDown = 3,
        MovementValue = 4,
        SlatTilt = 5,
        LightsStatus = 6,
        BlindsStatus =7
    }

    /// <summary>
    /// Enumerace pro velikosti písma
    /// </summary>
    public enum FontSize
    {
        Small = 0, 
        Medium = 1, 
        Large = 2
    }

    /// <summary>
    /// Enumerace pro typ View
    /// </summary>
    public enum ViewType
    {
        ControlView, 
        ConfigurationView, 
        SettingsView, 
        HelpView
    }
}
