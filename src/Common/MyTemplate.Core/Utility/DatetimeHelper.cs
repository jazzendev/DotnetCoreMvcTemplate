using System;
namespace MyTemplate.Core.Utility
{
    public enum TimeFormat
    {
        /// <summary>
        /// yyyy年MM月dd日 HH:mm:ss
        /// </summary>
        Long = 1,
        /// <summary>
        /// yyyy年MM月dd日
        /// </summary>
        Short = 2,
        /// <summary>
        /// yyyy-MM-dd HH:mm:ss
        /// </summary>
        SimpleLong = 3,
        /// <summary>
        /// yyyy-MM-dd
        /// </summary>
        SimpleShort = 4,
        /// <summary>
        /// yyyyMMddHHmmss
        /// </summary>
        StickyLong = 5,
        /// <summary>
        /// yyyyMMdd
        /// </summary>
        StickyShort = 6
    }
    public static class DatetimeHelper
    {
        public static string ChineseLocalTime(this DateTime time, TimeFormat format = TimeFormat.Long)
        {
            time = time.AddHours(8);
            switch (format)
            {
                case TimeFormat.StickyLong:
                    return time.ToString("yyyyMMddHHmmss");
                case TimeFormat.StickyShort:
                    return time.ToString("yyyyMMdd");
                case TimeFormat.Short:
                    return time.ToString("yyyy年MM月dd日");
                case TimeFormat.SimpleLong:
                    return time.ToString("yyyy-MM-dd HH:mm:ss");
                case TimeFormat.SimpleShort:
                    return time.ToString("yyyy-MM-dd");
                case TimeFormat.Long:
                default:
                    return time.ToString("yyyy年MM月dd日 HH:mm:ss");
            }
        }

        public static string ChinaToUTCTime(this DateTime time, TimeFormat format = TimeFormat.Long)
        {
            time = time.AddHours(-8);
            switch (format)
            {
                case TimeFormat.StickyLong:
                    return time.ToString("yyyyMMddHHmmss");
                case TimeFormat.StickyShort:
                    return time.ToString("yyyyMMdd");
                case TimeFormat.Short:
                    return time.ToString("yyyy年MM月dd日");
                case TimeFormat.SimpleLong:
                    return time.ToString("yyyy-MM-dd HH:mm:ss");
                case TimeFormat.SimpleShort:
                    return time.ToString("yyyy-MM-dd");
                case TimeFormat.Long:
                default:
                    return time.ToString("yyyy年MM月dd日 HH:mm:ss");
            }
        }
    }
}
