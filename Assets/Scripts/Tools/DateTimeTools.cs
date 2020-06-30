using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SthGame
{
    public class DateTimeTools
    {
        const string LOG_DATE_FORMAT = "yyyy-MM-dd HH:mm:ss fff";
        static DateTime utcStartDataTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);

        public static string FormatNowTimeStamp()
        {
            return DateTime.Now.ToString(LOG_DATE_FORMAT);
        }

        public static uint GetCurrentTimeStamp()
        {
            return ConvertDateTimeToTimeStamp(DateTime.Now);
        }

        public static uint ConvertDateTimeToTimeStamp(DateTime dateTime)
        {
            DateTime utcTime = dateTime.ToUniversalTime();
            return (uint)(utcTime.Subtract(utcStartDataTime)).TotalSeconds;
        }
    }
}
