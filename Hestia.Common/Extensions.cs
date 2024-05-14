using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;
using System.Xml.Serialization;
using System.Net;

namespace Hestia.Common
{
    public static class Extensions
    {
        /// <summary>
        /// Ukládání do XML souboru
        /// </summary>
        /// <param name="aInputDocument">XDocument pro uložení</param>
        /// <param name="aFilePath">cesta k souboru</param>
        public static void Save(this XDocument aInputDocument, string aFilePath)
        {
            using (FileStream lFileStream = new FileStream(aFilePath, FileMode.Create))
            {
                aInputDocument.Save(lFileStream);
            }
        }

        /// <summary>
        /// Serializace do XML souboru
        /// </summary>
        /// <typeparam name="T">Typ objektu pro serializaci</typeparam>
        /// <param name="aObj"></param>
        /// <returns></returns>
        public static XElement ToXElement<T>(this object aObj)
        {
            XmlSerializerNamespaces lNameSpace = new XmlSerializerNamespaces();
            lNameSpace.Add("", "");
            using (var lMemoryStream = new MemoryStream())
            {
                using (TextWriter lStreamWriter = new StreamWriter(lMemoryStream))
                {
                    var lXmlSerializer = new XmlSerializer(typeof(T));
                    lXmlSerializer.Serialize(lStreamWriter, aObj, lNameSpace);
                    return XElement.Parse(Encoding.UTF8.GetString(lMemoryStream.ToArray()));
                }
            }
        }

        /// <summary>
        /// Deserializace z XML souboru
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xElement"></param>
        /// <returns></returns>
        public static T FromXElement<T>(this XElement xElement)
        {
            var xmlSerializer = new XmlSerializer(typeof(T));
            return (T)xmlSerializer.Deserialize(xElement.CreateReader());
        }

        /// <summary>
        /// Zjištění přidělené IP adresy v síti
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<IPAddress> GetIPAddress()
        {
            return new List<IPAddress>()
            {
                IPAddress.Parse(
                    Windows.Networking.Connectivity.NetworkInformation
                    .GetHostNames().Last().DisplayName)
            };            
        }
    }
}
