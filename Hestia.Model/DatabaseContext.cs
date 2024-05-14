using System.Collections.Generic;
using System.Xml.Linq;
using System.IO;
using System.Linq;
using System;
using Hestia.Common;
using System.Xml.Serialization;
using System.Text;
using System.Collections.ObjectModel;

namespace Hestia.Model
{
    public static class DatabaseContext
    {
        private static ObservableCollection<Room> mRooms;
        private static ObservableCollection<Device> mDevices;

        private static ObservableCollection<FunctionType> mFunctionTypes;
        private static XDocument mDoc;
        /// <summary>
        /// Kolekce možných funkcní pro zařízení
        /// </summary>
        public static ObservableCollection<FunctionType> FunctionTypes
        {
            get
            {
                if (mFunctionTypes == null)
                {
                    mFunctionTypes = new ObservableCollection<FunctionType>(FunctionTypeXmlMapper.SelectAll());
                }
                return mFunctionTypes;
            }
            set
            {
                if (mFunctionTypes != value)
                    mFunctionTypes = value;
            }
        }

        /// <summary>
        /// Kolekce místností  
        /// </summary>
        public static ObservableCollection<Room> Rooms
        {
            get
            {
                if (mRooms == null)
                    mRooms = new ObservableCollection<Room>(RoomXmlMapper.SelectAll());
                return mRooms;
            }
            set
            {
                if(mRooms != value)
                mRooms = value;
            }
        }

        /// <summary>
        /// Kolekce zařízení
        /// </summary>
        public static ObservableCollection<Device> Devices
        {
            get
            {
                if (mDevices == null)
                {
                    mDevices = new ObservableCollection<Device>(DeviceXmlMapper.SelectAll());
                }
                return mDevices;
            }
            set
            {
                if (mDevices != value)
                    mDevices = value;
            }
        }

        /// <summary>
        /// Objekt Xdocument pro ukládání a načítání XML souboru s konfigurací
        /// </summary>
        public static XDocument xDoc
        {
            get
            {
                if (!File.Exists(Globals.ConfigFile))
                {
                    GenerateFunctionTypes();
                }

                if (mDoc == null)
                    mDoc = XDocument.Load(Globals.ConfigFile);

                return mDoc;

            }
            set
            {
                if (mDoc != value)
                    mDoc = value;
            }
        }
        
        private static void GenerateFunctionTypes()
        {
            new XDocument(new XElement("Root",
                    new XElement("FunctionTypes",
                    new XElement("FunctionType", new XAttribute("Id", (int)FunctionTypeCategory.OnOff),
                            new XElement("Name", "Vypnout/zapnout"),
                            new XElement("DPT", "bit"),
                            new XElement("Category", (int)DeviceCategory.Lights)),
                    new XElement("FunctionType", new XAttribute("Id", (int)FunctionTypeCategory.Dimming),
                            new XElement("Name", "Stmívání"),
                            new XElement("DPT", "3.008"),
                            new XElement("Category", (int)DeviceCategory.Lights)),
                    new XElement("FunctionType", new XAttribute("Id", (int)FunctionTypeCategory.BrightnessValue),
                            new XElement("Name", "Hodnota jasu"),
                            new XElement("DPT", "5.001"),
                            new XElement("Category", (int)DeviceCategory.Lights),
                    new XElement("FunctionType", new XAttribute("Id", (int)FunctionTypeCategory.UpDown),
                            new XElement("Name", "Pohyb nahoru/dolů"),
                            new XElement("DPT", "bit"),
                            new XElement("Category", (int)DeviceCategory.Blinds)),
                    new XElement("FunctionType", new XAttribute("Id", (int)FunctionTypeCategory.MovementValue),
                            new XElement("Name", "Hodnota posuvu"),
                            new XElement("DPT", "5.001"),
                            new XElement("Category", (int)DeviceCategory.Blinds)),
                    new XElement("FunctionType", new XAttribute("Id", (int)FunctionTypeCategory.SlatTilt),
                            new XElement("Name", "Naklonění lamel"),
                            new XElement("DPT", "5.003"),
                            new XElement("Category", (int)DeviceCategory.Blinds)),
                    new XElement("FunctionType", new XAttribute("Id", (int)FunctionTypeCategory.LightsStatus),
                            new XElement("Name", "Stav světla"),
                            new XElement("DPT", ""),
                            new XElement("Category", (int)DeviceCategory.Lights)),
                    new XElement("FunctionType", new XAttribute("Id", (int)FunctionTypeCategory.BlindsStatus),
                            new XElement("Name", "Stav žaluzie"),
                            new XElement("DPT", ""),
                            new XElement("Category", (int)DeviceCategory.Blinds)))))).Save(Globals.ConfigFile);
        }

        /// <summary>
        /// Slouží k serializaci DataContextu do XML souboru
        /// </summary>
        public static void SaveContext()
        {
            foreach(Device nDevice in Devices)
            {
                nDevice.AddressTypes.RemoveAll(aR => aR.FunctionType.Category != nDevice.Category);
            }

            xDoc = new XDocument(new XDeclaration("1.0", "UTF-8", null),
                new XElement("Root",
                    FunctionTypes.Save(),
                    Rooms.Save(),
                    Devices.Save()));
            xDoc.Save(Globals.ConfigFile);
        }        
    }
}
