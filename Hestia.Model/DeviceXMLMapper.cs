using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Hestia.Common;

namespace Hestia.Model
{
    public static class DeviceXmlMapper
    {
        /// <summary>
        /// Načtení všech zařízení z XML souboru
        /// </summary>
        /// <returns></returns>
        public static List<Device> SelectAll()
        {
            var xDevices = DatabaseContext.xDoc.Descendants("Device").ToList();

            List<Device> Devices = new List<Device>();

            if (xDevices != null)
            {
                foreach (var xDevice in xDevices)
                {
                    List<XElement> lAddresses = xDevice.Elements("AddressTypes").Elements("AddressType").ToList();
                    List<AddressType> lAddressTypes = new List<AddressType>();
                    foreach (var nAddress in  lAddresses)
                    {
                        lAddressTypes.Add(new AddressType()
                        {
                            Address = nAddress.Element("Address").Value,
                            FunctionTypeId = int.Parse(nAddress.Element("FunctionTypeId").Value)
                        });
                    }

                    Device lDevice = new Device()
                    {
                        Name = xDevice.Element("Name").Value,
                        Category = int.Parse(xDevice.Element("Category").Value),
                        AddressTypes = lAddressTypes,
                        RoomId = Guid.Parse(xDevice.Element("RoomId").Value),
                        Id = Guid.Parse(xDevice.Attribute("id").Value)
                    };

                    Devices.Add(lDevice);
                }
            }
            return Devices;
        }

        /// <summary>
        /// Serializace zařízení do XML souboru
        /// </summary>
        /// <param name="aDevice"></param>
        /// <returns></returns>
        public static XElement Save(this IEnumerable<Device> aDevice)
        {
            var x = aDevice.Select(aR => aR.ToXElement<Device>());

            if (aDevice == null) return null;
            else return new XElement("Devices", aDevice.Select(aR => aR.ToXElement<Device>()));
        }

    }
}
