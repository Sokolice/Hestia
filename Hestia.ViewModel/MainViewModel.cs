using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hestia.Speech;
using Windows.UI.Core;
using Windows.Media.SpeechRecognition;
using Hestia.Common;
using Windows.UI.Xaml.Media;
using Windows.UI;
using Windows.UI.Xaml;

namespace Hestia.ViewModel
{
    public class MainViewModel : NotifyBase, IDisposable
    {
        public CommandBase MicrophoneCommand { get; private set; }

        public ElementTheme Theme
        {
            get
            {
                return GlobalContext.MainTheme;
            }
        }

        public event Action<string> OnNavigate;
        public event Action OnLanguageChanged;
        private string mInfoText;
        private SolidColorBrush mBrush;
        public SolidColorBrush Brush
        {
            get
            {
                if (mBrush == null)
                    mBrush = new SolidColorBrush(Colors.Red);
                return mBrush;
            }
            set
            {
                if (mBrush != value)
                {
                    mBrush = value;
                    RaisePropertyChanged();
                }
            }
        }
        public string InfoText
        {
            get
            {
                return mInfoText ?? string.Empty;
            }
            set
            {
                if (mInfoText != value)
                {
                    mInfoText = value;
                    RaisePropertyChanged();
                }
            }
        }

        public MainViewModel()
        {
            SpeechContext.Dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;
            OnLoadMainVieModel();

            SpeechContext.OnNavigateTo += SpeechContext_OnNavigateTo;
            SpeechContext.OnRecognitionChange += SpeechContext_OnRecognitionChange;
            InfoText = GlobalContext.ResourceLoader.GetString("infoSpeechNotAvailable");


            GlobalContext.LoadSettings();
            GlobalContext.OnThemeChanged += GlobalContext_OnThemeChanged;
            GlobalContext.MainLanguage = ((Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride == "en") ? 1 : 0);

            try
            {
                GlobalContext.KNX.Connect();
            }
            catch(Exception ex)
            {
                GlobalContext.InsertLog(ex.Message, ex.StackTrace);
            }
            MicrophoneCommand = new CommandBase(SpeechInitialization);
        }

        private void SpeechInitialization(object obj)
        {
                SpeechContext.RecognitionStart();              
        }

        /// <summary>
        /// Vyvolání změny barevného schéma v hlavním okně aplikace
        /// </summary>
        private void GlobalContext_OnThemeChanged()
        {
            RaisePropertyChanged("Theme");
        }

        /// <summary>
        /// Změny textu v informaní liště pro hlasové rozhraní
        /// </summary>
        /// <param name="obj"></param>
        private async void SpeechContext_OnRecognitionChange(string obj)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
              () =>
              {
                  if (obj == SpeechRecognizerState.Capturing.ToString() || obj == SpeechRecognizerState.SoundEnded.ToString())
                  {
                      InfoText = GlobalContext.ResourceLoader.GetString("infoReady");
                      Brush.Color = Colors.Green;
                  }
                  else if(obj == SpeechRecognizerState.Idle.ToString())
                  {
                      InfoText = GlobalContext.ResourceLoader.GetString("infoOff");
                      Brush.Color = Colors.Red;
                  }
                  else if (obj == SpeechRecognizerState.SoundStarted.ToString() || obj == SpeechRecognizerState.SpeechDetected.ToString())
                      InfoText = GlobalContext.ResourceLoader.GetString("infoListening");
                  else
                      InfoText = obj;
              });
        }

        private void SpeechContext_OnNavigateTo(string obj)
        {
            if (OnNavigate != null)
                OnNavigate(obj);
        }

        private async void OnLoadMainVieModel()
        {
            bool permissionGained = await AudioCapturePermissions.RequestMicrophonePermission();
            if (permissionGained)
            {
                await SpeechContext.InitializeRecognizer();
                SpeechContext.RecognitionStart();
            }
        }

        public void Dispose()
        {
            try
            {
                GlobalContext.KNX.Disconnect();
            }
            catch(Exception ex)
            {
                GlobalContext.InsertLog(ex.Message, ex.StackTrace);
            }
            SpeechContext.OnNavigateTo -= SpeechContext_OnNavigateTo;
            SpeechContext.OnRecognitionChange -= SpeechContext_OnRecognitionChange;

            GlobalContext.MainLanguage = GlobalContext.CurrentLanguage;
            GlobalContext.ChangeLanguage(GlobalContext.MainLanguage);

        }
    }
}
