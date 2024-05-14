using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Media.SpeechRecognition;
using Windows.Media.SpeechSynthesis;
using Windows.Storage;
using Windows.UI.Core;
using Windows.Media.Audio;
using Windows.UI.Xaml.Controls;
using Hestia.Common;
using System.Reflection;

namespace Hestia.Speech
{
    /// <summary>
    /// Třída pro zpracování hlasových povelů
    /// </summary>
    public static class SpeechContext
    {
        private static CoreDispatcher mDispatcher;

        private static SpeechRecognizer mSpeechRecognizer;
        private static SpeechSynthesizer mSynthesizer;

        private static uint HResultRecognizerNotFound = 0x8004503a;

        public static event Action<string> OnRoomRecognized;
        public static event Action<string> OnDeviceRecognized;
        public static event Action<string> OnNavigateTo;
        public static event Action<string> OnRecognitionChange;
        public static event Action<string> OnActionRecognized;
        public static event Action<string, string, string> OnAddressRecognized;

        public static event Action<string> OnConfigurationRoomRecognized;
        public static event Action<string> OnConfigurationDeviceRecognized;
        public static event Action<string, string, string> OnNameRecognized;
        public static event Action<string, string> OnCategoryRecognized;

        public static event Action<string, string> OnCommandRecognized;
        public static event Action<string, string> OnValueRecognized;

        public static event Action<string, string> OnSettingsRecognized;
        public static event Action<string> OnSettingsCommandsRecognized;

        private static MediaElement mMedia;

        public static ViewType CurrentViewModel;

        private static object mRecognizerLock = new object();

        public static SpeechRecognizer SpeechRecognizer
        {
            get
            {
                lock (mRecognizerLock)
                {
                    return mSpeechRecognizer;
                }
            }
            set
            {
                lock (mRecognizerLock)
                {
                    if (mSpeechRecognizer != value)
                        mSpeechRecognizer = value;
                }
            }
        }
        public static CoreDispatcher Dispatcher
        {
            get
            {
                return mDispatcher;
            }
            set
            {
                if (mDispatcher != value)
                    mDispatcher = value;
            }
        }

        /// <summary>
        /// Inicializace rozhraní pro ohlasové ovládání, načtení gramatiky
        /// </summary>
        /// <returns></returns>
        public static async Task InitializeRecognizer()
        {
            mSynthesizer = new SpeechSynthesizer();
            mMedia = new MediaElement();

            if (SpeechRecognizer != null)
            {
                SpeechRecognizer.StateChanged -= SpeechRecognizer_StateChanged;
                SpeechRecognizer.ContinuousRecognitionSession.ResultGenerated -= ContinuousRecognitionSession_ResultGenerated;

                SpeechRecognizer.Dispose();
                SpeechRecognizer = null;
            }

            try
            {
                StorageFile grammarContentFile = await Package.Current.InstalledLocation.GetFileAsync("SRGS\\SRGS.xml");

                SpeechRecognizer = new SpeechRecognizer(SpeechRecognizer.SystemSpeechLanguage);

                SpeechRecognizer.StateChanged += SpeechRecognizer_StateChanged;

                SpeechRecognitionGrammarFileConstraint grammarConstraint = new SpeechRecognitionGrammarFileConstraint(grammarContentFile);
                SpeechRecognizer.Constraints.Add(grammarConstraint);


                SpeechRecognitionCompilationResult result = await SpeechRecognizer.CompileConstraintsAsync();
                if (result.Status != SpeechRecognitionResultStatus.Success)
                {
                    GlobalContext.InsertLog("", "Nelze zkompilovat gramatiku");
                }
                else
                {
                    SpeechRecognizer.Timeouts.EndSilenceTimeout = TimeSpan.FromSeconds(1);
                    SpeechRecognizer.ContinuousRecognitionSession.ResultGenerated += ContinuousRecognitionSession_ResultGenerated;
                }
            }
            catch (Exception ex)
            {
                if ((uint)ex.HResult == HResultRecognizerNotFound)
                {                    
                }
                else
                {
                    var messageDialog = new Windows.UI.Popups.MessageDialog(ex.Message, "Exception");
                    await messageDialog.ShowAsync();
                }
            }
        }

        /// <summary>
        /// Syntéza hlasu na základě vstupí zprávy
        /// </summary>
        /// <param name="aMessage">text pro syntézu</param>
        public static async void SpeechSynthesisSpeak(string aMessage)
        {
            try
            {
                SpeechSynthesisStream sys = await mSynthesizer.SynthesizeTextToStreamAsync(GlobalContext.ResourceLoader.GetString(aMessage));

                mMedia.AutoPlay = true;
                mMedia.SetSource(sys, sys.ContentType);                
                mMedia.Play();
            }
            catch (Exception ex)
            {
                GlobalContext.InsertLog(ex.Message, ex.StackTrace);
            }
        }
        private static void SpeechRecognizer_StateChanged(SpeechRecognizer sender, SpeechRecognizerStateChangedEventArgs args)
        {
            if (OnRecognitionChange != null)
                OnRecognitionChange(args.State.ToString());

        }

        /// <summary>
        /// Vyvolá se v případě vyslovení příkazu z gramatiky a na základě jistoty vyřčení se dále zpracovává
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static async void ContinuousRecognitionSession_ResultGenerated(SpeechContinuousRecognitionSession sender, SpeechContinuousRecognitionResultGeneratedEventArgs args)
        {

            if (args.Result.Confidence == SpeechRecognitionConfidence.Medium ||
               args.Result.Confidence == SpeechRecognitionConfidence.High)
            {
                await mDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    HandleRecognitionResult(args.Result);
                });
            }
            else if (args.Result.Confidence == SpeechRecognitionConfidence.Rejected || args.Result.Confidence == SpeechRecognitionConfidence.Low)
            {
                if (OnRecognitionChange != null)
                    OnRecognitionChange(GlobalContext.ResourceLoader.GetString("infoRepeatCommand"));
            }
        }

        /// <summary>
        /// Prvotní zpracování sémantického výsledku
        /// </summary>
        /// <param name="recoResult"></param>
        private static void HandleRecognitionResult(SpeechRecognitionResult recoResult)
        {
            if (recoResult.Confidence == SpeechRecognitionConfidence.High ||
            recoResult.Confidence == SpeechRecognitionConfidence.Medium)
            {
                if (recoResult.SemanticInterpretation.Properties.ContainsKey(Strings.CommandType) && recoResult.SemanticInterpretation.Properties[Strings.CommandType][0].ToString() != Strings.Dots)
                {
                    try
                    {
                        typeof(SpeechContext).GetMethod(recoResult.SemanticInterpretation.Properties[Strings.CommandType][0].ToString()).Invoke(null, new object[] { recoResult });
                    }
                    catch (Exception lEx)
                    {
                        GlobalContext.InsertLog(lEx.Message, lEx.StackTrace);
                    }                   
                }
            }
        }


        /// <summary>
        /// Zpracování příkazů v nastavení
        /// </summary>
        /// <param name="recoResult"></param>
        public static void ProccessSettingsCommands(SpeechRecognitionResult recoResult)
        {
            if (CurrentViewModel == ViewType.SettingsView)
            {
                if (recoResult.SemanticInterpretation.Properties.ContainsKey(Strings.What) && recoResult.SemanticInterpretation.Properties[Strings.What][0].ToString() != Strings.Dots)
                {
                    if (OnSettingsCommandsRecognized != null)
                        OnSettingsCommandsRecognized(recoResult.SemanticInterpretation.Properties[Strings.What][0].ToString());
                }
            }
        }

        /// <summary>
        /// Zpracování změn nastavení
        /// </summary>
        /// <param name="recoResult"></param>
        public static void ProccessSettings(SpeechRecognitionResult recoResult)
        {
            if(CurrentViewModel == ViewType.SettingsView)
            {
                if (recoResult.SemanticInterpretation.Properties.ContainsKey(Strings.What) && recoResult.SemanticInterpretation.Properties[Strings.What][0].ToString() != Strings.Dots
                    && recoResult.SemanticInterpretation.Properties.ContainsKey(Strings.To) && recoResult.SemanticInterpretation.Properties[Strings.To][0].ToString() != Strings.Dots)
                {
                    if (OnSettingsRecognized != null)
                        OnSettingsRecognized(recoResult.SemanticInterpretation.Properties[Strings.What][0].ToString(), recoResult.SemanticInterpretation.Properties[Strings.To][0].ToString());
                }
            }
        }
        /// <summary>
        /// Zpracování vstupních hodnot pro ovládání žaluzií
        /// </summary>
        /// <param name="recoResult"></param>
        public static void ProccessInput(SpeechRecognitionResult recoResult)
        {
            if (CurrentViewModel == ViewType.ControlView)
            {
                if (recoResult.SemanticInterpretation.Properties.ContainsKey(Strings.Property) && recoResult.SemanticInterpretation.Properties[Strings.Property][0].ToString() != Strings.Dots
                    && recoResult.SemanticInterpretation.Properties.ContainsKey(Strings.Number) && recoResult.SemanticInterpretation.Properties[Strings.Number][0].ToString() != Strings.Dots)
                {
                    if (OnValueRecognized != null)
                        OnValueRecognized(recoResult.SemanticInterpretation.Properties[Strings.Property][0].ToString(), recoResult.SemanticInterpretation.Properties[Strings.Number][0].ToString());
                }
            }
        }

        /// <summary>
        /// Zpracování příkazů v konfiguraci
        /// </summary>
        /// <param name="recoResult"></param>
        public static void ProccessCommand(SpeechRecognitionResult recoResult)
        {
            if (CurrentViewModel == ViewType.ConfigurationView)
            {
                if (recoResult.SemanticInterpretation.Properties.ContainsKey(Strings.Command) && recoResult.SemanticInterpretation.Properties[Strings.Command][0].ToString() != Strings.Dots)
                {
                    if (recoResult.SemanticInterpretation.Properties.ContainsKey(Strings.What) && recoResult.SemanticInterpretation.Properties[Strings.What][0].ToString() != Strings.Dots
                        && recoResult.SemanticInterpretation.Properties.ContainsKey(Strings.PropertyPart) && recoResult.SemanticInterpretation.Properties[Strings.PropertyPart][0].ToString() != Strings.Dots)
                    {
                        if (OnCommandRecognized != null)
                            OnCommandRecognized(recoResult.SemanticInterpretation.Properties[Strings.Command][0].ToString() + recoResult.SemanticInterpretation.Properties[Strings.What][0].ToString(), recoResult.SemanticInterpretation.Properties[Strings.PropertyPart][0].ToString() + recoResult.SemanticInterpretation.Properties[Strings.What][0].ToString());
                    }
                    else
                    {
                        if (OnCommandRecognized != null)
                            OnCommandRecognized(recoResult.SemanticInterpretation.Properties[Strings.Command][0].ToString(), null);
                    }
                }
            }
        }

        /// <summary>
        /// Změna kategorie
        /// </summary>
        /// <param name="recoResult"></param>
        public static void ProccessCategory(SpeechRecognitionResult recoResult)
        {
            if(CurrentViewModel == ViewType.ConfigurationView)
            {
                if (recoResult.SemanticInterpretation.Properties.ContainsKey(Strings.ActionType) && recoResult.SemanticInterpretation.Properties[Strings.ActionType][0].ToString() != Strings.Dots
                    && recoResult.SemanticInterpretation.Properties.ContainsKey(Strings.Category) && recoResult.SemanticInterpretation.Properties[Strings.Category][0].ToString() != Strings.Dots)
                {
                    if (OnCategoryRecognized != null)
                        OnCategoryRecognized(recoResult.SemanticInterpretation.Properties[Strings.ActionType][0].ToString(), recoResult.SemanticInterpretation.Properties[Strings.Category][0].ToString());
                }
            }
        }
        
        /// <summary>
        /// Zadání jména zařízení nebo místnosti
        /// </summary>
        /// <param name="recoResult"></param>
        public static void ProccessName(SpeechRecognitionResult recoResult)
        {
            if (CurrentViewModel == ViewType.ConfigurationView)
            {
                if (recoResult.SemanticInterpretation.Properties.ContainsKey(Strings.What) && recoResult.SemanticInterpretation.Properties[Strings.What][0].ToString() != Strings.Dots
                    && recoResult.SemanticInterpretation.Properties.ContainsKey(Strings.ActionType) && recoResult.SemanticInterpretation.Properties[Strings.ActionType][0].ToString() != Strings.Dots 
                    && recoResult.SemanticInterpretation.Properties.ContainsKey(Strings.Name) && recoResult.SemanticInterpretation.Properties[Strings.Name][0].ToString() != Strings.Dots)
                {
                    string lNumber = null;
                    if (recoResult.SemanticInterpretation.Properties.ContainsKey(Strings.Number) && recoResult.SemanticInterpretation.Properties[Strings.Number][0].ToString() != Strings.Dots)
                        lNumber = recoResult.SemanticInterpretation.Properties[Strings.Number][0].ToString();
                    if (OnNameRecognized != null)
                        OnNameRecognized(recoResult.SemanticInterpretation.Properties[Strings.ActionType][0].ToString() + recoResult.SemanticInterpretation.Properties[Strings.What][0].ToString(), recoResult.SemanticInterpretation.Properties[Strings.Name][0].ToString(), lNumber ??string.Empty);
                }
            }
        }
        
        /// <summary>
        /// Zpracování zadané adresy a typu funkce
        /// </summary>
        /// <param name="recoResult"></param>
        public static void ProccessAddress(SpeechRecognitionResult recoResult)
        {
            if (CurrentViewModel == ViewType.ConfigurationView)
            {
                if (recoResult.SemanticInterpretation.Properties.ContainsKey(Strings.Address) && recoResult.SemanticInterpretation.Properties[Strings.Address][0].ToString() != Strings.Dots
                    && recoResult.SemanticInterpretation.Properties.ContainsKey(Strings.ActionType) && recoResult.SemanticInterpretation.Properties[Strings.ActionType][0].ToString() != Strings.Dots)
                {
                    if (OnAddressRecognized != null)
                        OnAddressRecognized(recoResult.SemanticInterpretation.Properties[Strings.AddressType][0].ToString(), recoResult.SemanticInterpretation.Properties[Strings.Address][0].ToString(), recoResult.SemanticInterpretation.Properties[Strings.ActionType][0].ToString() + "Device");                    
                }
            }
            else
            {
                if (OnRecognitionChange != null)
                    OnRecognitionChange("Neplatný příkaz.");
            }

        }

        /// <summary>
        /// Zpracování akce v ovládání
        /// </summary>
        /// <param name="recoResult"></param>
        public static void ProcessControlAction(SpeechRecognitionResult recoResult)
        {
            if (CurrentViewModel == ViewType.ControlView)
            {
                if (recoResult.SemanticInterpretation.Properties.ContainsKey(Strings.Action) && recoResult.SemanticInterpretation.Properties[Strings.Action][0].ToString() != Strings.Dots)
                {
                    OnActionRecognized(recoResult.SemanticInterpretation.Properties[Strings.Action][0].ToString());
                }

                else
                {
                    if (OnRecognitionChange != null)
                        OnRecognitionChange("Neplatný příkaz.");
                }
            }
        }

        /// <summary>
        /// Zpracování výběru zařízení nebo místnosti
        /// </summary>
        /// <param name="recoResult"></param>
        public static void ProcessSelect(SpeechRecognitionResult recoResult)
        {
            if (recoResult.SemanticInterpretation.Properties.ContainsKey(Strings.What) && recoResult.SemanticInterpretation.Properties[Strings.What][0].ToString() != Strings.Dots)
            {
                if (recoResult.SemanticInterpretation.Properties[Strings.What][0].ToString() == "room")
                {
                    if (recoResult.SemanticInterpretation.Properties.ContainsKey(Strings.What) && recoResult.SemanticInterpretation.Properties[Strings.Number][0].ToString() != Strings.Dots)
                    {
                        if (CurrentViewModel == ViewType.ControlView)
                            OnRoomRecognized(recoResult.SemanticInterpretation.Properties[Strings.Number][0].ToString());
                        if (CurrentViewModel == ViewType.ConfigurationView)
                            OnConfigurationRoomRecognized(recoResult.SemanticInterpretation.Properties[Strings.Number][0].ToString());

                    }
                }
                else if (recoResult.SemanticInterpretation.Properties[Strings.What][0].ToString() == "device")
                {
                    if (recoResult.SemanticInterpretation.Properties.ContainsKey(Strings.Number) && recoResult.SemanticInterpretation.Properties[Strings.Number][0].ToString() != Strings.Dots)
                    {
                        if (CurrentViewModel == ViewType.ControlView)
                            OnDeviceRecognized(recoResult.SemanticInterpretation.Properties[Strings.Number][0].ToString());
                        if (CurrentViewModel == ViewType.ConfigurationView)
                            OnConfigurationDeviceRecognized(recoResult.SemanticInterpretation.Properties[Strings.Number][0].ToString());

                    }
                }

            }
            else if (recoResult.SemanticInterpretation.Properties.ContainsKey(Strings.What) && recoResult.SemanticInterpretation.Properties[Strings.What][0].ToString() == Strings.Dots)
            {
                if (OnRecognitionChange != null)
                    OnRecognitionChange("Neplatný příkaz.");
            }
        }

        /// <summary>
        /// Navigace mezi obrazovkami
        /// </summary>
        /// <param name="recoResult"></param>
        public static void ProcessNavigation(SpeechRecognitionResult recoResult)
        {
            if (recoResult.SemanticInterpretation.Properties.ContainsKey(Strings.GoWhere) && recoResult.SemanticInterpretation.Properties[Strings.GoWhere][0].ToString() != Strings.Dots)
            {
                string lLocation = recoResult.SemanticInterpretation.Properties[Strings.GoWhere][0].ToString();
                if (OnNavigateTo != null)
                    OnNavigateTo(lLocation);

                if (OnRecognitionChange != null)
                    OnRecognitionChange("Příkaz rozpoznán.");
            }
            else if (recoResult.SemanticInterpretation.Properties.ContainsKey(Strings.GoWhere) && recoResult.SemanticInterpretation.Properties[Strings.GoWhere][0].ToString() == Strings.Dots)
            {
                if (OnRecognitionChange != null)
                    OnRecognitionChange("Neplatný příkaz.");
            }
        }
        /// <summary>
        /// Začátek naslouchání vzukovému vstupu
        /// </summary>
        public static async void RecognitionStart()
        {
            if (SpeechRecognizer.State == SpeechRecognizerState.Idle)
            {
                try
                {
                    await SpeechRecognizer.ContinuousRecognitionSession.StartAsync();
                }
                catch (Exception ex)
                {
                    GlobalContext.InsertLog(ex.Message, ex.StackTrace);
                }
            }
            else
            {                
                await SpeechRecognizer.ContinuousRecognitionSession.CancelAsync();
            }
        }

        /// <summary>
        /// Zastavení nasloucháni 
        /// </summary>
        public static async void CancelRecognition()
        {
            if(SpeechRecognizer != null)
            {
                await SpeechRecognizer.ContinuousRecognitionSession.CancelAsync();
            }
        }
    }
}
