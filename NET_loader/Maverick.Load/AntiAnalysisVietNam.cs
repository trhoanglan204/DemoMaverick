using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maverick.Agent
{
    public static class AntiAnalysisVietNam
    {
        private static bool IsInVietNam()
        {
            int num = 0;
            if (AntiAnalysisVietNam.IsValidVietNamTimeZone())
            {
                num++;
            }
            if (AntiAnalysisVietNam.IsVietNamLocale())
            {
                num++;
            }
            if (AntiAnalysisVietNam.IsVietNamRegion())
            {
                num++;
            }
            if (AntiAnalysisVietNam.IsVietNameDateFormat())
            {
                num++;
            }
            return num >= 2;
        }

        private static bool IsValidVietNamTimeZone()
        {
            try
            {
                var timeZone = TimeZoneInfo.Local;
                return timeZone != null && (timeZone.Id.Equals("SE Asia Standard Time", StringComparison.OrdinalIgnoreCase) || timeZone.Id.Equals("Asia/Ho_Chi_Minh", StringComparison.OrdinalIgnoreCase));
            }
            catch
            {
                return false;
            }
        }

        private static bool IsVietNamLocale()
        {
            try
            {
                var culture = System.Globalization.CultureInfo.CurrentCulture;
                return culture != null && culture.Name.StartsWith("vi", StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        private static bool IsVietNamRegion()
        {
            try
            {
                var region = System.Globalization.RegionInfo.CurrentRegion;
                return region != null && region.TwoLetterISORegionName.Equals("VN", StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        private static bool IsVietNameDateFormat()
        {
            try
            {
                var culture = System.Globalization.CultureInfo.CurrentCulture;
                var datePattern = culture.DateTimeFormat.ShortDatePattern;
                return datePattern != null && datePattern.Contains("dd/MM/yyyy");
            }
            catch
            {
                return false;
            }
        }

        public static bool IsSuspiciousEnvironment()
        {
            return AntiAnalysisVietNam.IsInVietNam();
        }

        public static bool ShouldContinueExecution()
        {
            return !AntiAnalysisVietNam.IsSuspiciousEnvironment();
        }
    }
}
