using System;

namespace MyTemplate.Core.Utility
{
    public static class IdHelper
    {
        const string TIME_FORMAT = "yyyyMMddHHmmssfff";

        /// <summary>
        /// by default create a 20 length id
        /// </summary>
        /// <param name="length"></param>
        /// <param name="padding"></param>
        /// <returns></returns>
        public static string NewId(int length = 17, int padding = 3)
        {
            string root = DateTime.UtcNow.ToString(TIME_FORMAT);
            string paddingRight = "";

            if (length <= root.Length)
            {
                root = root.Substring(0, length);
            }

            if (padding > 0)
            {
                paddingRight = RandomHelper.Instance.Next(1, (int)Math.Pow(10, padding)).ToString("D" + padding);
            }

            return root + paddingRight;
        }

        /// <summary>
        /// by default create a 20 length id
        /// </summary>
        /// <param name="length"></param>
        /// <param name="padding"></param>
        /// <returns></returns>
        public static string NewUTC8Id(int length = 17, int padding = 3)
        {
            string root = DateTime.UtcNow.AddHours(8).ToString(TIME_FORMAT);
            string paddingRight = "";

            if (length <= root.Length)
            {
                root = root.Substring(0, length);
            }

            if (padding > 0)
            {
                paddingRight = RandomHelper.Instance.Next(1, (int)Math.Pow(10, padding)).ToString("D" + padding);
            }

            return root + paddingRight;
        }
    }
}
