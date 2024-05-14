using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hestia.Common;
using Windows.UI.Xaml;
using System.Reflection;

namespace Hestia.ViewModel
{
    public class SettingsViewModel : NotifyBase, IDisposable
    {
        public CommandBase SaveCommand { get; set; }
        public CommandBase OpenConfigurationCommand { get; private set; }
        public event Action<ViewType> OnSettigsChanged;
        private int mFontSize { get; set; }
        private int mSelectedLangugae { get; set; }

        public ElementTheme Theme
        {
            get
            {
                return GlobalContext.MainTheme;
            }
        }

        public int SelectedTheme
        {
            get
            {
                return (GlobalContext.MainTheme == ElementTheme.Light) ? 0 : 1;
            }
            set
            {
                if ((int)GlobalContext.MainTheme != (value + 1))
                {
                    GlobalContext.MainTheme = (ElementTheme)(value + 1);
                    RaisePropertyChanged("Theme");
                    RaisePropertyChanged();
                }
            }
        }

        public int SelectedLanguage
        {
            get
            {
                return GlobalContext.MainLanguage;
            }
            set
            {
                if (GlobalContext.MainLanguage != value)
                {
                    GlobalContext.MainLanguage = value;
                    RaisePropertyChanged();
                }
            }
        }

        public int SelectedFontSize
        {
            get
            {
                return (int)GlobalContext.FontSize;
            }

            set
            {
                if (SelectedFontSize != value)
                {
                    GlobalContext.FontSize = (FontSize)value;
                    if (OnSettigsChanged != null)
                        OnSettigsChanged(ViewType.SettingsView);
                    RaisePropertyChanged();
                }
            }
        }


        public SettingsViewModel()
        {
            SaveCommand = new CommandBase(Save);
            OpenConfigurationCommand = new CommandBase(OpenConfiguration);

            Speech.SpeechContext.OnSettingsRecognized += SpeechContext_OnSettingsRecognized;
            Speech.SpeechContext.OnSettingsCommandsRecognized += SpeechContext_OnSettingsCommandsRecognized; ;
            Speech.SpeechContext.CurrentViewModel = ViewType.SettingsView;
        }

        private void SpeechContext_OnSettingsCommandsRecognized(string obj)
        {
            try
            {
                GetType().GetMethod(obj).Invoke(this, new object[] { null });
            }
            catch (Exception ex)
            {
                GlobalContext.InsertLog(ex.Message, ex.StackTrace);
            }
        }

        public void OpenConfiguration(object obj)
        {
            if (OnSettigsChanged != null)
                OnSettigsChanged(ViewType.ConfigurationView);
        }

        /// <summary>
        /// Zmeny nastavení podle hlasového povelu
        /// </summary>
        /// <param name="aProperty"></param>
        /// <param name="aValue"></param>
        public void SpeechContext_OnSettingsRecognized(string aProperty, string aValue)
        {
            try
            {
                Globals.SetProperty(this, aProperty, int.Parse(aValue));
            }
            catch (Exception ex)
            {
                GlobalContext.InsertLog(ex.Message, ex.StackTrace);
            }
        }
        /// <summary>
        /// Uložení nastavení 
        /// </summary>
        /// <param name="obj"></param>
        public void Save(object obj)
        {
            GlobalContext.FontSize = (FontSize)SelectedFontSize;
            GlobalContext.CurrentLanguage = SelectedLanguage;
            GlobalContext.SaveSettings();
        }

        public void Dispose()
        {
            Speech.SpeechContext.OnSettingsRecognized -= SpeechContext_OnSettingsRecognized;
            Speech.SpeechContext.OnSettingsCommandsRecognized -= SpeechContext_OnSettingsCommandsRecognized; ;

        }
    }
}
