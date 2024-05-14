using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Hestia.Common;

namespace Hestia.Model
{
    public static class FunctionTypeXmlMapper
    {
        /// <summary>
        /// Načtení všech funkčních typů pro zařízení z XML souboru
        /// </summary>
        /// <returns></returns>
        public static List<FunctionType> SelectAll()
        {
            List<XElement> xFunctionTypes = DatabaseContext.xDoc.Root.Descendants("FunctionTypes").Descendants("FunctionType").ToList();

            List<FunctionType> lFunctionTypes = new List<FunctionType>();

            if (xFunctionTypes != null)
            {

                foreach (XElement xFunctionType in xFunctionTypes)
                {
                    FunctionType lFunctionType = new FunctionType()
                    {
                        Id = int.Parse(xFunctionType.Attribute("Id").Value),
                        DPT = xFunctionType.Element("DPT").Value,
                        Name = xFunctionType.Element("Name").Value,
                        Category = Int32.Parse(xFunctionType.Element("Category").Value)
                    };
                    lFunctionTypes.Add(lFunctionType);
                }
            }
            return lFunctionTypes;
        }


        /// <summary>
        /// Uložení Funkčního typu do XML souboru
        /// </summary>
        /// <param name="aFunctionTypes"></param>
        /// <returns></returns>
        public static XElement Save(this IEnumerable<FunctionType> aFunctionTypes)
        {
            if (aFunctionTypes == null) return null;
            else return new XElement("FunctionTypes", aFunctionTypes.Select(aR => aR.ToXElement<FunctionType>()));
        }        
    }
}
