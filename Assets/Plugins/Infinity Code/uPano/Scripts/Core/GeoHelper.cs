/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System;

namespace InfinityCode.uPano
{
    public static class GeoHelper
    {
        /// <summary>
        /// Earth radius.
        /// </summary>
        public const double R = 6371;

        public static double Clip(double n, double minValue, double maxValue)
        {
            if (n < minValue) return minValue;
            if (n > maxValue) return maxValue;
            return n;
        }

        public static void CoordinatesToTile(double lng, double lat, int zoom, out double tx, out double ty)
        {
            lat = Clip(lat, -85, 85);
            lng = Repeat(lng, -180, 180);

            double rLon = lng * MathHelper.Deg2Rad;
            double rLat = lat * MathHelper.Deg2Rad;

            const double a = 6378137;
            const double d = 53.5865938 / 256;
            const double k = 0.0818191908426;

            double z = Math.Tan(MathHelper.PID4 + rLat / 2) / Math.Pow(Math.Tan(MathHelper.PID4 + Math.Asin(k * Math.Sin(rLat)) / 2), k);
            double z1 = Math.Pow(2, 23 - zoom);


            tx = (20037508.342789 + a * rLon) * d / z1;
            ty = (20037508.342789 - a * Math.Log(z)) * d / z1;
        }

        public static double Distance(double lng1, double lat1, double lng2, double lat2)
        {
            double dx, dy;
            Distance(lng1, lat1, lng2, lat2, out dx, out dy);
            return Math.Sqrt(dx * dx + dy * dy);
        }

        public static void Distance(double lng1, double lat1, double lng2, double lat2, out double dx, out double dy)
        {
            double scfY = Math.Sin(lat1 * MathHelper.Deg2Rad);
            double sctY = Math.Sin(lat2 * MathHelper.Deg2Rad);
            double ccfY = Math.Cos(lat1 * MathHelper.Deg2Rad);
            double cctY = Math.Cos(lat2 * MathHelper.Deg2Rad);
            double cX = Math.Cos((lng1 - lng2) * MathHelper.Deg2Rad);
            double sizeX1 = Math.Abs(R * Math.Acos(scfY * scfY + ccfY * ccfY * cX));
            double sizeX2 = Math.Abs(R * Math.Acos(sctY * sctY + cctY * cctY * cX));
            dx = (sizeX1 + sizeX2) / 2.0;
            dy = R * Math.Acos(scfY * sctY + ccfY * cctY);
            if (double.IsNaN(dx)) dx = 0;
            if (double.IsNaN(dy)) dy = 0;
        }

        public static double Repeat(double n, double minValue, double maxValue)
        {
            if (double.IsInfinity(n) || double.IsInfinity(minValue) || double.IsInfinity(maxValue) || double.IsNaN(n) || double.IsNaN(minValue) || double.IsNaN(maxValue)) return n;

            double range = maxValue - minValue;
            while (n < minValue || n > maxValue)
            {
                if (n < minValue) n += range;
                else if (n > maxValue) n -= range;
            }
            return n;
        }
    }
}