using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hestia.Common;
using System.Xml.Serialization;
using System.Collections.ObjectModel;

namespace Hestia.Model
{
    [XmlRoot("Room")]
    public class Room : NotifyBase
    {
        [XmlIgnore]
        private string mName;
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

        [XmlIgnore]
        public List<Device> Devices
        {
            get
            {
                return DatabaseContext.Devices.Where(aR => aR.RoomId == this.Id).ToList();
            }
        }

        public Room(bool aGenerateGuid = false)
        {
            if (aGenerateGuid)
                Id = Guid.NewGuid();
        }

        public Room() { }

        public bool ValidateEmpty(bool IsFromSpeech, out string ErrorMessage)
        {
            if (this.Name == string.Empty)
            {
                ErrorMessage = IsFromSpeech ? "speechNewRoomName" : "warNewRoomName";
                return true;
            }

            ErrorMessage = string.Empty;
            return false;
        }
    }
}
