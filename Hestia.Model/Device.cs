using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hestia.Common;
using System.Xml.Serialization;
using Windows.UI.Xaml;

namespace Hestia.Model
{
    [XmlRoot("Device")]
    public class Device : NotifyBase
    {
        [XmlIgnore]
        private string mName;
        [XmlIgnore]
        private int mCategory;
        [XmlIgnore]
        private int mSlatTiltValue;
        [XmlIgnore]
        private int mBlindHeightValue;

        [XmlIgnore]
        public int LightStatus { get; set; }

        [XmlIgnore]
        public int SlatTiltValue
        {
            get
            {
                return mSlatTiltValue;
            }
            set
            {
                if (mSlatTiltValue != value)
                {
                    mSlatTiltValue = value;
                    RaisePropertyChanged();
                }
            }
        }
        public int BlindHeightValue
        {
            get
            {
                return mBlindHeightValue;
            }
            set
            {
                if (mBlindHeightValue != value)
                {
                    mBlindHeightValue = value;
                    RaisePropertyChanged();
                }
            }
        }

        [XmlAttribute("id")]
        public Guid Id { get; set; }

        [XmlElement("Name")]
        public string Name
        {
            get
            {
                return mName ?? string.Empty;
            }
            set
            {
                if (mName != value)
                {
                    mName = value;
                    RaisePropertyChanged();
                }
            }
        }

        [XmlArray("AddressTypes")]
        public List<AddressType> AddressTypes { get; set; }

        [XmlElement("RoomId")]
        public Guid RoomId { get; set; }

        [XmlElement("Category")]
        public int Category
        {
            get
            {
                return mCategory;
            }
            set
            {
                if (mCategory != value)
                {
                    mCategory = value;
                    RaisePropertyChanged();
                }
            }
        }


        [XmlIgnore]
        public string LightsOnOffAddress
        {
            get
            {
                return AddressTypes?.FirstOrDefault(aR => aR.FunctionTypeId == (int)FunctionTypeCategory.OnOff)?.Address ?? String.Empty;
            }
            set
            {
                AddressType lAdressType = AddressTypes.FirstOrDefault(aR => aR.FunctionTypeId == (int)FunctionTypeCategory.OnOff);
                if (lAdressType != null)
                    AddressTypes.FirstOrDefault(aR => aR.FunctionTypeId == (int)FunctionTypeCategory.OnOff).Address = value;
                else
                    AddressTypes.Add(new AddressType() { Address = value, FunctionTypeId = (int)FunctionTypeCategory.OnOff });
                RaisePropertyChanged();
            }
        }

        [XmlIgnore]
        public string LightsDimming
        {
            get
            {
                return AddressTypes?.FirstOrDefault(aR => aR.FunctionTypeId == (int)FunctionTypeCategory.Dimming)?.Address ?? String.Empty;
            }
            set
            {
                AddressType lAdressType = AddressTypes?.FirstOrDefault(aR => aR.FunctionTypeId == (int)FunctionTypeCategory.Dimming);
                if (lAdressType != null)
                    AddressTypes.FirstOrDefault(aR => aR.FunctionTypeId == (int)FunctionTypeCategory.Dimming).Address = value;
                else
                    AddressTypes?.Add(new AddressType() { Address = value, FunctionTypeId = (int)FunctionTypeCategory.Dimming });
                RaisePropertyChanged();
            }
        }

        [XmlIgnore]
        public string LightsValue
        {
            get
            {
                return AddressTypes?.FirstOrDefault(aR => aR.FunctionTypeId == (int)FunctionTypeCategory.BrightnessValue)?.Address ?? String.Empty;
            }
            set
            {
                AddressType lAdressType = AddressTypes?.FirstOrDefault(aR => aR.FunctionTypeId == (int)FunctionTypeCategory.BrightnessValue);
                if (lAdressType != null)
                    AddressTypes.FirstOrDefault(aR => aR.FunctionTypeId == (int)FunctionTypeCategory.BrightnessValue).Address = value;
                else
                    AddressTypes?.Add(new AddressType() { Address = value, FunctionTypeId = (int)FunctionTypeCategory.BrightnessValue });
                RaisePropertyChanged();
            }
        }

        [XmlIgnore]
        public string BlindsUpdDown
        {
            get
            {
                return AddressTypes?.FirstOrDefault(aR => aR.FunctionTypeId == (int)FunctionTypeCategory.UpDown)?.Address ?? String.Empty;
            }
            set
            {
                AddressType lAdressType = AddressTypes?.FirstOrDefault(aR => aR.FunctionTypeId == (int)FunctionTypeCategory.UpDown);
                if (lAdressType != null)
                    AddressTypes.FirstOrDefault(aR => aR.FunctionTypeId == (int)FunctionTypeCategory.UpDown).Address = value;
                else
                    AddressTypes?.Add(new AddressType() { Address = value, FunctionTypeId = (int)FunctionTypeCategory.UpDown });
                RaisePropertyChanged();
            }
        }

        [XmlIgnore]
        public string BlindsMovement
        {
            get
            {
                return AddressTypes?.FirstOrDefault(aR => aR.FunctionTypeId == (int)FunctionTypeCategory.MovementValue)?.Address ?? String.Empty;
            }
            set
            {
                AddressType lAdressType = AddressTypes?.FirstOrDefault(aR => aR.FunctionTypeId == (int)FunctionTypeCategory.MovementValue);
                if (lAdressType != null)
                    AddressTypes.FirstOrDefault(aR => aR.FunctionTypeId == (int)FunctionTypeCategory.MovementValue).Address = value;
                else
                    AddressTypes?.Add(new AddressType() { Address = value, FunctionTypeId = (int)FunctionTypeCategory.MovementValue });
                RaisePropertyChanged();
            }
        }

        [XmlIgnore]
        public string BlindsSlat
        {
            get
            {
                return AddressTypes?.FirstOrDefault(aR => aR.FunctionTypeId == (int)FunctionTypeCategory.SlatTilt)?.Address ?? String.Empty;
            }
            set
            {
                AddressType lAdressType = AddressTypes?.FirstOrDefault(aR => aR.FunctionTypeId == (int)FunctionTypeCategory.SlatTilt);
                if (lAdressType != null)
                    AddressTypes.FirstOrDefault(aR => aR.FunctionTypeId == (int)FunctionTypeCategory.SlatTilt).Address = value;
                else
                    AddressTypes?.Add(new AddressType() { Address = value, FunctionTypeId = (int)FunctionTypeCategory.SlatTilt });
                RaisePropertyChanged();
            }
        }

        [XmlIgnore]
        public string BlindsStatus
        {
            get
            {
                return AddressTypes?.FirstOrDefault(aR => aR.FunctionTypeId == (int)FunctionTypeCategory.BlindsStatus)?.Address ?? String.Empty;
            }
            set
            {
                AddressType lAdressType = AddressTypes?.FirstOrDefault(aR => aR.FunctionTypeId == (int)FunctionTypeCategory.BlindsStatus);
                if (lAdressType != null)
                    AddressTypes.FirstOrDefault(aR => aR.FunctionTypeId == (int)FunctionTypeCategory.BlindsStatus).Address = value;
                else
                    AddressTypes?.Add(new AddressType() { Address = value, FunctionTypeId = (int)FunctionTypeCategory.BlindsStatus });
                RaisePropertyChanged();
            }
        }
        [XmlIgnore]
        public string LightsStatusAddress
        {
            get
            {
                return AddressTypes?.FirstOrDefault(aR => aR.FunctionTypeId == (int)FunctionTypeCategory.LightsStatus)?.Address ?? String.Empty;
            }
            set
            {
                AddressType lAdressType = AddressTypes?.FirstOrDefault(aR => aR.FunctionTypeId == (int)FunctionTypeCategory.LightsStatus);
                if (lAdressType != null)
                    AddressTypes.FirstOrDefault(aR => aR.FunctionTypeId == (int)FunctionTypeCategory.LightsStatus).Address = value;
                else
                    AddressTypes?.Add(new AddressType() { Address = value, FunctionTypeId = (int)FunctionTypeCategory.LightsStatus });
                RaisePropertyChanged();
            }
        }

        public Device(Guid aRoomId, bool aGenerateGuid = false)
        {
            if (aGenerateGuid)
                Id = Guid.NewGuid();
            RoomId = aRoomId;
            AddressTypes = new List<AddressType>();
        }

        public Device()
        {
            AddressTypes = new List<AddressType>();
        }


        public bool ValidateEmpty(bool isFromSpeech, out string ErrorMessage)
        {
            if(Name == string.Empty)
            {
                ErrorMessage = isFromSpeech ? "speechNewDevName" : "warNewDevName";
                return true;
            }
            if(!AddressTypes.Any(aR =>aR.Address != "") ||(AddressTypes !=null && AddressTypes.Count == 0))
            {
                ErrorMessage = isFromSpeech ? "speechNewDevAddress" : "warNewDevAddress";
                return true;
            }
            ErrorMessage = string.Empty;
            return false;
        }
    }
}
