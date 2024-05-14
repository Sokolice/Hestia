using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Hestia.Common;

namespace Hestia.Model
{
    [XmlRoot("AddressType")]
    public class AddressType: NotifyBase
    {
        [XmlIgnore]
        private string mAddress;

        [XmlElement("Address")]
        public string Address
        {
            get
            {
                return mAddress ?? string.Empty;
            }
            set
            {
                if(mAddress != value && Common.Validation.ValidateAddress(value))
                {
                    mAddress = value;
                }
            } }

        [XmlElement("FunctionTypeId")]
        public int FunctionTypeId { get; set;}

        [XmlIgnore]
        public FunctionType FunctionType
        {
            get
            {
                return DatabaseContext.FunctionTypes.FirstOrDefault(aR => aR.Id == FunctionTypeId);
            }
        }
        
        public AddressType() { }
    }
}
