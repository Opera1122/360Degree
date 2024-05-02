/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System.Globalization;

namespace InfinityCode.uPano
{
    public static class CultureHelper
    {
        public static CultureInfo cultureInfo
        {
            get { return CultureInfo.InvariantCulture; }
        }

        public static NumberFormatInfo numberFormat
        {
            get { return cultureInfo.NumberFormat; }
        }
    }
}