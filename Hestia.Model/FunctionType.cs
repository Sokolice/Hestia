using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hestia.Common;
using System.Xml.Serialization;

namespace Hestia.Model
{
    [XmlRoot("FunctionType")]
    public class FunctionType: NotifyBase
    {
        [XmlAttribute("Id")]
        public int Id;

        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("DPT")]
        public string DPT { get; set; }

        [XmlElement("Category")]
        public int Category { get; set; }
        public FunctionType() { }
                
    }
}
