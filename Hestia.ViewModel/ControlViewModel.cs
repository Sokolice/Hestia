using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hestia.Model;
using System.Collections.ObjectModel;
using Hestia.Common;
using System.Collections;
using Hestia.Speech;
using System.Reflection;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;

namespace Hestia.ViewModel
{
    public class ControlViewModel : NotifyBase, IDisposable
    {
        private Room mControlRoom;
        private ObservableCollection<Device> mControlDevices;
        private Device mControlDevice;
        public CommandBase LightOnCommand { get; set; }
        public CommandBase LightOffCommand { get; private set; }
        public CommandBase DimmingPlus { get; private set; }
        public CommandBase DimmingMinus { get; private set; }
        public CommandBase UpCommand { get; private set; }
        public CommandBase DownCommand { get; private set; }
        public CommandBase SlatTiltCommand { get; private set; }
        public CommandBase BlindHeightCommand { get; private set; }
        public ElementTheme Theme
        {
            get
            {
                return GlobalContext.MainTheme;
            }            
        }
        public ObservableCollection<Device> ControlDevices
        {
            get
            {
                return mControlDevices;
            }
            set
            {
                if (mControlDevices != value)
                    mControlDevices = value;
                RaisePropertyChanged();
            }
        }

        public Room ControlRoom
        {
            get
            {
                return mControlRoom;
            }
            set
            {
                if (mControlRoom != value)
                {
                    mControlRoom = value;
                    if (mControlRoom != null)
                        ControlDevices = new ObservableCollection<Device>(ControlRoom.Devices);
                    RaisePropertyChanged();
                }
            }
        }

        public Device ControlDevice
        {
            get
            {
                return mControlDevice;
            }
            set
            {
                if (mControlDevice != value)
                {
                    mControlDevice = value;
                    RaisePropertyChanged();
                }
            }
        }
        public ObservableCollection<Room> Rooms
        {
            get
            {
                return DatabaseContext.Rooms;
            }
            set
            {
                DatabaseContext.Rooms = value;
                RaisePropertyChanged();
            }
        }

        public ControlViewModel()
        {
            ControlRoom = new Room();
            ControlDevice = new Device();

            LightOnCommand = new CommandBase(LightOn);
            LightOffCommand = new CommandBase(LightOff);
            DimmingPlus = new CommandBase(Plus);
            DimmingMinus = new CommandBase(Minus);
            UpCommand = new CommandBase(Up);
            DownCommand = new CommandBase(Down);
            SlatTiltCommand = new CommandBase(Tilt);
            BlindHeightCommand = new CommandBase(BlindHeight);

            SpeechContext.OnRoomRecognized += SpeechContext_OnRoomRecognized;
            SpeechContext.OnDeviceRecognized += SpeechContext_OnDeviceRecognized;
            SpeechContext.OnActionRecognized += SpeechContext_OnActionRecognized;
            SpeechContext.OnValueRecognized += SpeechContext_OnValueRecognized;
            
            SpeechContext.CurrentViewModel = ViewType.ControlView;
        }
        
        /// <summary>
        /// Nastavení hodnot posvníků pro náklon a posuv žaluzií
        /// </summary>
        /// <param name="aProperty"></param>
        /// <param name="aValue"></param>
        private void SpeechContext_OnValueRecognized(string aProperty, string aValue)
        {
            try
            {
                Globals.SetProperty(this, "ControlDevice." + aProperty, int.Parse(aValue));
            }
            catch (Exception lEx)
            {
                GlobalContext.InsertLog(lEx.Message, lEx.StackTrace);
            }

        }

        /// <summary>
        /// Vyvolání na základě hlasového povelu pomocí reflexe např. zhasnutí světel 
        /// </summary>
        /// <param name="obj"></param>
        private void SpeechContext_OnActionRecognized(string obj)
        {
            try
            {
                if (ControlDevice.Id == Guid.Empty)
                    SpeechContext.SpeechSynthesisSpeak("errSelDev");
                else
                    GetType().GetMethod(obj).Invoke(this, new object[] { ControlDevice.Id });
            }
            catch (Exception lEx)
            {
                GlobalContext.InsertLog(lEx.Message, lEx.StackTrace);
            }
        }

        /// <summary>
        /// Výběr zařízení podle vysloveného příkazu
        /// </summary>
        /// <param name="obj"></param>
        private void SpeechContext_OnDeviceRecognized(string obj)
        {
            try
            {
                if (ControlRoom != null)
                {
                    int lNumber = int.Parse(obj);

                    if (lNumber > 0 && lNumber <= ControlDevices.Count)
                        ControlDevice = ControlDevices[lNumber - 1];
                    else
                        SpeechContext.SpeechSynthesisSpeak("errExDevice");
                }
                else
                {
                    SpeechContext.SpeechSynthesisSpeak("errSelRoom");
                }
            }
            catch (Exception lEx)
            {
                GlobalContext.InsertLog(lEx.Message, lEx.StackTrace);
            }
        }

        /// <summary>
        /// Výběr místnosti podle vysvloveného příkazu
        /// </summary>
        /// <param name="obj"></param>
        private void SpeechContext_OnRoomRecognized(string obj)
        {
            try
            {
                int lNumber = int.Parse(obj);

                if (lNumber > 0 && lNumber <= Rooms.Count)
                    ControlRoom = Rooms[lNumber - 1];
                else
                    SpeechContext.SpeechSynthesisSpeak("errExRoom");
            }
            catch (Exception lEx)
            {
                GlobalContext.InsertLog(lEx.Message, lEx.StackTrace);
            }
        }

        /// <summary>
        /// Metoda pro naklonění lamel
        /// </summary>
        /// <param name="obj"></param>
        public void Tilt(object obj)
        {
            try
            {
                Guid lGuid = new Guid(obj.ToString());

                Device lDevice = ControlDevices.Where(aR => aR.Id == lGuid).FirstOrDefault();

                GlobalContext.KNX.Action(lDevice.BlindsSlat, (lDevice.SlatTiltValue * 255) / 100);
            }
            catch (Exception lEx)
            {
                GlobalContext.InsertLog(lEx.Message, lEx.StackTrace);
            }
        }

        /// <summary>
        /// Pohyb žaluzií dolů 
        /// </summary>
        /// <param name="obj"></param>
        public void Down(object obj)
        {
            try
            {
                Guid lGuid = new Guid(obj.ToString());

                Device lDevice = ControlDevices.Where(aR => aR.Id == lGuid).FirstOrDefault();
                
                GlobalContext.KNX.Action(lDevice.BlindsUpdDown, true);
            }
            catch (Exception lEx)
            {
                GlobalContext.InsertLog(lEx.Message, lEx.StackTrace);
            }
        }

        /// <summary>
        /// Pohyb žaluzií nahoru
        /// </summary>
        /// <param name="obj"></param>
        public void Up(object obj)
        {
            try
            {
                Guid lGuid = new Guid(obj.ToString());

                Device lDevice = ControlDevices.Where(aR => aR.Id == lGuid).FirstOrDefault();

                GlobalContext.KNX.Action(lDevice.BlindsUpdDown, false);
            }
            catch (Exception lEx)
            {
                GlobalContext.InsertLog(lEx.Message, lEx.StackTrace);
            }
        }

        /// <summary>
        /// Zvýšení jasu
        /// </summary>
        /// <param name="obj"></param>
        public void Plus(object obj)
        {
            try
            {
                Guid lGuid = new Guid(obj.ToString());

                Device lDevice = ControlDevices.Where(aR => aR.Id == lGuid).FirstOrDefault();

                GlobalContext.KNX.Action(lDevice.LightsDimming, GlobalContext.KNX.ToDataPoint(DatabaseContext.FunctionTypes.FirstOrDefault(aR => aR.Id == (int)FunctionTypeCategory.Dimming).DPT, 4));
            }
            catch (Exception lEx)
            {
                GlobalContext.InsertLog(lEx.Message, lEx.StackTrace);
            }
        }

        /// <summary>
        /// Snížení jasu
        /// </summary>
        /// <param name="obj"></param>
        public void Minus(object obj)
        {
            try
            {
                Guid lGuid = new Guid(obj.ToString());

                Device lDevice = ControlDevices.Where(aR => aR.Id == lGuid).FirstOrDefault();

                GlobalContext.KNX.Action(lDevice.LightsDimming, GlobalContext.KNX.ToDataPoint(DatabaseContext.FunctionTypes.FirstOrDefault(aR => aR.Id == (int)FunctionTypeCategory.Dimming).DPT, -4));
            }
            catch (Exception lEx)
            {
                GlobalContext.InsertLog(lEx.Message, lEx.StackTrace);
            }
        }

        /// <summary>
        /// Nastavení konkrétní výšky žaluzie
        /// </summary>
        /// <param name="obj"></param>
        public void BlindHeight(object obj)
        {
            try
            {
                Guid lGuid = new Guid(obj.ToString());
                

                Device lDevice = ControlDevices.Where(aR => aR.Id == lGuid).FirstOrDefault();
                
                GlobalContext.KNX.Action(lDevice.BlindsMovement, (lDevice.BlindHeightValue * 255) / 100);
            }
            catch (Exception lEx)
            {
                GlobalContext.InsertLog(lEx.Message, lEx.StackTrace);
            }
        }

        /// <summary>
        /// Rozsvícení světla
        /// </summary>
        /// <param name="obj"></param>
        public void LightOn(object obj)
        {
            try
            {
                Guid lGuid = new Guid(obj.ToString());

                Device lDevice = ControlDevices.Where(aR => aR.Id == lGuid).FirstOrDefault();
                GlobalContext.KNX.Action(lDevice.LightsOnOffAddress, true);
            }
            catch (Exception lEx)
            {
                GlobalContext.InsertLog(lEx.Message, lEx.StackTrace);
            }
        }

        /// <summary>
        /// Zhasnutí světla
        /// </summary>
        /// <param name="obj"></param>
        public void LightOff(object obj)
        {
            try
            {
                Guid lGuid = new Guid(obj.ToString());

                Device lDevice = ControlDevices.Where(aR => aR.Id == lGuid).FirstOrDefault();
                GlobalContext.KNX.Action(lDevice.LightsOnOffAddress, false);
            }
            catch (Exception lEx)
            {
                GlobalContext.InsertLog(lEx.Message, lEx.StackTrace);
            }
        }
               

        public void Dispose()
        {
            SpeechContext.OnRoomRecognized -= SpeechContext_OnRoomRecognized;
            SpeechContext.OnDeviceRecognized -= SpeechContext_OnDeviceRecognized;
            SpeechContext.OnActionRecognized -= SpeechContext_OnActionRecognized;
            SpeechContext.OnValueRecognized -= SpeechContext_OnValueRecognized;            
        }
    }

}
