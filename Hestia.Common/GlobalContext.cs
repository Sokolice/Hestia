using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KNXLib.Universal;
using Windows.ApplicationModel.Resources;
using System.IO;
using Windows.UI.Xaml;
using System.Globalization;

namespace Hestia.Common
{
    public static class GlobalContext
    {
        public static event Action OnThemeChanged;

        private static KnxRouting mKnxRouting;
        private static ResourceLoader mLoader;
        private static ElementTheme mMainTheme = ElementTheme.Default;

        private static object mKNXlock = new object();
        private static FontSize mFontsize = FontSize.Medium;

        /// <summary>
        /// Připojení ke sběrnici pomocí KNXRoutingu
        /// </summary>
        public static KnxRouting KNX
        {
            get
            {
                lock (mKNXlock)
                {
                    if (mKnxRouting == null)
                    {
                        mKnxRouting = new KnxRouting(Common.Extensions.GetIPAddress()) { IsDebug = false, ActionMessageCode = 0x29 };
                    }
                    return mKnxRouting;
                }
            }
            set
            {
                lock (mKNXlock)
                {
                    if (mKnxRouting != value)
                        mKnxRouting = value;
                }
            }
        }

        /// <summary>
        /// Objekt ResourceLoader sloužící ori načítání jazykových variant textů
        /// </summary>
        public static ResourceLoader ResourceLoader
        {
            get
            {
                if (mLoader == null)
                    mLoader = new ResourceLoader();
                return mLoader;
            }
        }

        /// <summary>
        /// Barevné schéma aplikace
        /// </summary>
        public static ElementTheme MainTheme
        {
            get
            {
                return mMainTheme;
            }
            set
            {
                if (mMainTheme != value)
                {
                    mMainTheme = value;
                    if (OnThemeChanged != null)
                        OnThemeChanged();
                }
            }
        }
        
        /// <summary>
        /// Uložený jazyk aplikace
        /// </summary>
        public static int MainLanguage{ get;set;}
        
        public static int CurrentLanguage { get; set; }

        /// <summary>
        /// Metoda pro logování chyb do soubotu log.txt
        /// </summary>
        /// <param name="aMessage"></param>
        /// <param name="aStackTrace"></param>
        public static void InsertLog(string aMessage, string aStackTrace)
        {
            string lContent = DateTime.Now.ToString() + " " + aMessage + " " + aStackTrace + "\r\n";
            File.AppendAllText(Globals.LogFile, lContent);
        }


        /// <summary>
        /// Změna jazyka
        /// </summary>
        /// <param name="aVal"></param>
        public static void ChangeLanguage(object aVal)
        {
            var culture = new CultureInfo((int.Parse(aVal.ToString()) == 1) ? "en" : "cs");
            Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = culture.Name;
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
        }

        public static FontSize FontSize
        {
            get
            {
                return mFontsize;
            }
            set
            {
                if (mFontsize != value)
                    mFontsize = value;
            }
        }
        
        /// <summary>
        /// Ukladádání nastevaní schématu a písma do souboru settings.txt 
        /// </summary>
        public static void SaveSettings()
        {
            string lContent = MainTheme.ToString() + ";" + FontSize.ToString();
            
            File.WriteAllText(Globals.SettingsFile, lContent);
        }


        /// <summary>
        /// Načítání nastavení
        /// </summary>
        public static void LoadSettings()
        {
            if (File.Exists(Globals.SettingsFile))
            {
                try
                {
                    string lContent = File.ReadAllText(Globals.SettingsFile);

                    var lParts = lContent.Split(';');

                    MainTheme = (ElementTheme)Enum.Parse(typeof(ElementTheme), lParts[0]);
                    FontSize = (FontSize)Enum.Parse(typeof(FontSize), lParts[1]);
                }
                catch(Exception ex)
                {
                    GlobalContext.InsertLog(ex.Message, ex.StackTrace);
                }
                
            }
            else
            {
                MainTheme = ElementTheme.Default;
                FontSize = FontSize.Medium;
            }
        }
    }
}
