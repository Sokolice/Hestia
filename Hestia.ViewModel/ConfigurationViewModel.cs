using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hestia.Model;
using Hestia.Common;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using System.Reflection;
using System.Text.RegularExpressions;
using Windows.ApplicationModel.Resources;
namespace Hestia.ViewModel
{
    public class ConfigurationViewModel : NotifyBase, IDisposable
    {
        private Room mSelectedroom;
        private Device mSelectedDevice;
        private ObservableCollection<Device> mDevices;
        private Device mNewDevice;
        private Room mNewRoom;

        public CommandBase AddRoomCommand { get; set; }
        public CommandBase DeleteRoomCommand { get; set; }
        public CommandBase AddDeviceCommand { get; set; }
        public CommandBase DeleteDeviceCommand { get; set; }
        public CommandBase SaveCommand { get; set; }

        /// <summary>
        /// Barevné schéma pro zobrazení ve View
        /// </summary>
        public ElementTheme Theme
        {
            get
            {
                return GlobalContext.MainTheme;
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
        public ObservableCollection<Device> Devices
        {
            get
            {
                return mDevices;
            }
            set
            {
                if (mDevices != value)
                    mDevices = value;
                RaisePropertyChanged();
            }
        }
        public Room SelectedRoom
        {
            get
            {
                return mSelectedroom;
            }
            set
            {
                if (mSelectedroom != value)
                {
                    mSelectedroom = value;
                    if (SelectedRoom != null)
                        RefreshDevices();
                    RaisePropertyChanged();
                }
            }
        }

        public Room NewRoom
        {
            get
            {
                return mNewRoom;
            }
            set
            {
                if (mNewRoom != value)
                {
                    mNewRoom = value;
                    RaisePropertyChanged();
                }
            }
        }


        public Device SelectedDevice
        {
            get
            {
                return mSelectedDevice;
            }
            set
            {
                if (mSelectedDevice != value)
                {
                    mSelectedDevice = value;
                    RaisePropertyChanged();
                }
            }
        }
        public Device NewDevice
        {
            get
            {
                return mNewDevice;
            }
            set
            {
                if (mNewDevice != value)
                {
                    mNewDevice = value;
                    RaisePropertyChanged();
                }
            }

        }
        public ConfigurationViewModel()
        {
            SelectedDevice = new Device(new Guid());
            SelectedRoom = new Room();
            NewRoom = new Room();
            NewDevice = new Device();

            AddRoomCommand = new CommandBase(AddRoom);
            DeleteRoomCommand = new CommandBase(DeleteRoom);
            AddDeviceCommand = new CommandBase(AddDevice);
            DeleteDeviceCommand = new CommandBase(DeleteDevice);
            SaveCommand = new CommandBase(Save);

            Speech.SpeechContext.OnAddressRecognized += SpeechContext_OnAddressRecognized;
            Speech.SpeechContext.OnConfigurationDeviceRecognized += SpeechContext_OnConfigurationDeviceRecognized;
            Speech.SpeechContext.OnConfigurationRoomRecognized += SpeechContext_OnConfigurationRoomRecognized;
            Speech.SpeechContext.OnNameRecognized += SpeechContext_OnNameRecognized;
            Speech.SpeechContext.OnCategoryRecognized += SpeechContext_OnCategoryRecognized;
            Speech.SpeechContext.OnCommandRecognized += SpeechContext_OnCommandRecognized;
            Speech.SpeechContext.CurrentViewModel = ViewType.ConfigurationView;

        }

        /// <summary>
        /// Na základě sématické hodnoty z rozpoznaného hlasu pomocí reflexe zavolá příslušnou metodu
        /// </summary>
        /// <param name="aMethod"></param>
        /// <param name="aPropertyToCheck"></param>
        private void SpeechContext_OnCommandRecognized(string aMethod, string aPropertyToCheck)
        {
            if (CheckNotNullOrEmptyObject(aPropertyToCheck))
            {
                GetType().GetMethod(aMethod).Invoke(this, new object[] { null });
            }
        }

        /// <summary>
        /// Změna kategorie pomocí hlasu
        /// </summary>
        /// <param name="aActionType"></param>
        /// <param name="aCategory"></param>
        private void SpeechContext_OnCategoryRecognized(string aActionType, string aCategory)
        {
            if (CheckNotNullOrEmptyObject(aActionType == "New" ? null : aActionType + "Device")) ;
            Globals.SetProperty(this, aActionType + "Device.Category", int.Parse(aCategory));
        }

        /// <summary>
        /// Nastavení názvu místnosti nebo zařízení na základě hlasového povelu
        /// </summary>
        /// <param name="aProperty"></param>
        /// <param name="aName"></param>
        /// <param name="aNumber"></param>
        private void SpeechContext_OnNameRecognized(string aProperty, string aName, string aNumber)
        {
            if (CheckNotNullOrEmptyObject((aProperty == "NewDevice" || aProperty == "NewRoom") ? null : aProperty))
                Globals.SetProperty(this, aProperty + ".Name", aName + ((aNumber != string.Empty) ? " " + aNumber : ""));
        }

        /// <summary>
        /// Výběr místnosti na základě vysloveného příkazu
        /// </summary>
        /// <param name="aNumber"></param>
        private void SpeechContext_OnConfigurationRoomRecognized(string aNumber)
        {
            try
            {
                int lNumber = int.Parse(aNumber);

                if (lNumber > 0 && lNumber <= Rooms.Count)
                    SelectedRoom = Rooms[lNumber - 1];
                else
                    Speech.SpeechContext.SpeechSynthesisSpeak("errExRoom");
            }
            catch (Exception lEx)
            {
                GlobalContext.InsertLog(lEx.Message, lEx.StackTrace);
            }
        }

        /// <summary>
        /// Výběr zařízení na základě vysloveného příkazu
        /// </summary>
        /// <param name="aNumber"></param>
        private void SpeechContext_OnConfigurationDeviceRecognized(string aNumber)
        {
            try
            {
                if (CheckNotNullOrEmptyObject("SelectedRoom"))
                {
                    int lNumber = int.Parse(aNumber);

                    if (lNumber > 0 && lNumber <= Devices.Count)
                        SelectedDevice = Devices[lNumber - 1];
                    else
                        Speech.SpeechContext.SpeechSynthesisSpeak("errExDevice");
                }

            }
            catch(Exception lEx)
            {
                GlobalContext.InsertLog(lEx.Message, lEx.StackTrace);
            }

        }

        /// <summary>
        /// Nastavení adresy zařízení pomocí reflexe ze sématického výstupu rozpoznaného hlasového příkazu
        /// </summary>
        /// <param name="aAddresType"></param>
        /// <param name="aAddress"></param>
        /// <param name="aProperty"></param>
        private void SpeechContext_OnAddressRecognized(string aAddresType, string aAddress, string aProperty)
        {
            try
            {

                if (Common.Validation.ValidateAddress(aAddress))
                {
                    if (CheckNotNullOrEmptyObject((aProperty == "SelectedDevice") ? aProperty : null))
                        Globals.SetProperty(this, aProperty + "." + aAddresType, aAddress);
                }
                else
                    Speech.SpeechContext.SpeechSynthesisSpeak("errAddress");
            }
            catch (Exception lEx)
            {
                GlobalContext.InsertLog(lEx.Message, lEx.StackTrace);
            }
        }

        /// <summary>
        /// Uložení konfigurace
        /// </summary>
        /// <param name="obj"></param>
        public void Save(object obj)
        {
            try
            {
                DatabaseContext.SaveContext();
            }
            catch (Exception ex)
            {
                GlobalContext.InsertLog(ex.Message, ex.StackTrace);
                Globals.ShowError("errSave");
            }
        }

        /// <summary>
        /// Smazání zařízení
        /// </summary>
        /// <param name="obj"></param>
        public void DeleteDevice(object obj)
        {
            if (SelectedDevice.Id == Guid.Empty)
            {
                Globals.ShowError("errChooseDeviceDel");
            }
            else
            {
                DatabaseContext.Devices.Remove(SelectedDevice);
                RefreshDevices();
            }
        }

        /// <summary>
        /// Přidání nového zařízení
        /// </summary>
        /// <param name="obj"></param>
        public void AddDevice(object obj)
        {
            if (SelectedRoom.Id == Guid.Empty)
            {
                Globals.ShowError("errChooseRomeForInsert");
            }
            else {
                string ErrorMsg;
                if (NewDevice.ValidateEmpty(false, out ErrorMsg))
                {
                    Globals.ShowError(ErrorMsg);
                }
                else {
                    DatabaseContext.Devices.Add(new Device(SelectedRoom.Id, true)
                    {
                        Category = NewDevice.Category,
                        Name = NewDevice.Name,
                        LightsOnOffAddress = NewDevice.LightsOnOffAddress,
                        LightsDimming = NewDevice.LightsDimming,
                        LightsValue = NewDevice.LightsValue,
                        BlindsUpdDown = NewDevice.BlindsUpdDown,
                        BlindsMovement = NewDevice.BlindsMovement,
                        BlindsSlat = NewDevice.BlindsSlat,
                        BlindsStatus = NewDevice.BlindsStatus,
                        LightsStatusAddress = NewDevice.LightsStatusAddress
                    });
                    Devices = new ObservableCollection<Device>(SelectedRoom.Devices);
                    NewDevice = new Device(SelectedRoom.Id);
                }
            }
        }

        /// <summary>
        /// Přidání nové místnosti
        /// </summary>
        /// <param name="parametr"></param>
        public void AddRoom(object parametr)
        {
            string lMessage;
            if (NewRoom.ValidateEmpty(false, out lMessage))
            {
                Globals.ShowError(lMessage);
            }
            else
            {
                DatabaseContext.Rooms.Add(new Room(true) { Name = NewRoom.Name });
                NewRoom = new Room();
            }
        }

        /// <summary>
        /// Smazání místnosti
        /// </summary>
        /// <param name="parametr"></param>
        public void DeleteRoom(object parametr)
        {
            if (SelectedRoom.Id == Guid.Empty)
            {
                Globals.ShowError("errChooseRoomDel");
            }
            else {
                DatabaseContext.Rooms.Remove(SelectedRoom);
                SelectedRoom = new Room();
                RefreshDevices();
            }
        }

        public void Dispose()
        {
            Speech.SpeechContext.OnAddressRecognized -= SpeechContext_OnAddressRecognized;
            Speech.SpeechContext.OnConfigurationDeviceRecognized -= SpeechContext_OnConfigurationDeviceRecognized;
            Speech.SpeechContext.OnConfigurationRoomRecognized -= SpeechContext_OnConfigurationRoomRecognized;
            Speech.SpeechContext.OnNameRecognized -= SpeechContext_OnNameRecognized;
            Speech.SpeechContext.OnCategoryRecognized -= SpeechContext_OnCategoryRecognized;
            Speech.SpeechContext.OnCommandRecognized -= SpeechContext_OnCommandRecognized;
        }
        private void RefreshDevices()
        {
            Devices = new ObservableCollection<Device>(SelectedRoom.Devices);
        }
        /// <summary>
        /// Metoda pro zjištění zda bylo vybráno nějaké zařízení nebo místnost
        /// </summary>
        /// <param name="aProperty"></param>
        /// <returns></returns>
        public bool CheckNotNullOrEmptyObject(string aProperty)
        {
            if (aProperty == null)
                return true;

            if (aProperty == "SelectedRoom" && SelectedRoom.Id == Guid.Empty)
            {
                Speech.SpeechContext.SpeechSynthesisSpeak("errSelRoom");
                return false;
            }
            else if (aProperty == "SelectedDevice" && SelectedDevice.Id == Guid.Empty)
            {
                Speech.SpeechContext.SpeechSynthesisSpeak("errSelDev");
                return false;
            }
            else if (aProperty == "NewDevice")
            {
                if (SelectedRoom.Id == Guid.Empty)
                {
                    Speech.SpeechContext.SpeechSynthesisSpeak("errSelRoom");
                    return false;
                }
                string lMessage;
                if (NewDevice.ValidateEmpty(true, out lMessage))
                {
                    Speech.SpeechContext.SpeechSynthesisSpeak(lMessage);
                    return false;
                }
                return true;
            }
            else if (aProperty == "NewRoom")
            {
                string lMessage;
                if (NewRoom.ValidateEmpty(true, out lMessage))
                {
                    Speech.SpeechContext.SpeechSynthesisSpeak(lMessage);
                    return false;
                }
                return true;
            }
            return true;
        }

    }
}
