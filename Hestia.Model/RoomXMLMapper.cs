using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Hestia.Common;

namespace Hestia.Model
{

    public static class RoomXmlMapper
    {
        /// <summary>
        /// Načtení seznamů místností z XML souboru
        /// </summary>
        /// <returns></returns>
        public static List<Room> SelectAll()
        {
            var xRooms = DatabaseContext.xDoc.Descendants("Room").ToList();

            List<Room> Rooms = new List<Room>();

            if (xRooms != null)
            {
                foreach (var xRoom in xRooms)
                {
                    Room lRoom = new Room()
                    {
                        Name = xRoom.Element("Name").Value,
                        Id = Guid.Parse(xRoom.Attribute("id").Value)
                    };

                    Rooms.Add(lRoom);
                }
            }
            return Rooms;
        }

        /// <summary>
        /// Uložení objektu místnosti do XML souboru
        /// </summary>
        /// <param name="aRoom"></param>
        /// <returns></returns>
        public static XElement Save(this IEnumerable<Room> aRoom)
        {
            if (aRoom == null) return null;
            else return new XElement("Rooms", aRoom.Select(aR => aR.ToXElement<Room>()));
        }
    }
}
