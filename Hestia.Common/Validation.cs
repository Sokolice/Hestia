using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Hestia.Common
{
    public static class Validation
    {
        /// <summary>
        /// Validace skupinové adresy pomocí regulárního výrazu
        /// </summary>
        /// <param name="aAddress">skupinová adresa</param>
        /// <returns></returns>
        public static bool ValidateAddress(string aAddress)
        {
            Regex rx = new Regex("^([01]?[0-9]?[0-9]|2[0-4][0-9]|25[0-5])/([01]?[0-9]?[0-9]|2[0-4][0-9]|25[0-5])/([01]?[0-9]?[0-9]|2[0-4][0-9]|25[0-5])$");

            return rx.IsMatch(aAddress);
        }
    }
}
